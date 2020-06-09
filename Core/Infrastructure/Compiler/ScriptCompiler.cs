using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Core.Utils;
using Microsoft.CSharp;

namespace Core.Infrastructure.Compiler
{
    public class ScriptCompiler : IScriptCompiler
    {
        private string _cfgPath = Path.Combine(GlobalConfig.BaseDirectory, "Data/Assemblies_4_0.cfg");
        private string _scriptCsDllPath = Path.Combine(GlobalConfig.BaseDirectory, "Scripts/Output/Scripts.CS.dll");
        private string _scriptCsHashPath = Path.Combine(GlobalConfig.BaseDirectory, "Scripts/Output/Scripts.CS.hash");
        private string _scriptCsDllDeleteMask = "Scripts.CS*.dll";
        
        private Assembly[] _assemblies;

        public Assembly[] Assemblies
        {
            get { return _assemblies; }
            set { _assemblies = value; }
        }

        private List<string> _additionalReferences = new List<string>();

        public string[] GetReferenceAssemblies()
        {
            List<string> list = new List<string>();


            if (File.Exists(_cfgPath))
            {
                using (var ip = new StreamReader(_cfgPath))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        if (line.Length > 0 && !line.StartsWith("#"))
                            list.Add(line);
                    }
                }
            }

            list.Add(GlobalConfig.ExePath);

            list.AddRange(_additionalReferences);

            return list.ToArray();
        }

        public string GetCompilerOptions(bool debug)
        {
            StringBuilder sb = null;

            if (!debug)
            {
                 AppendCompilerOption(ref sb, "/optimize");
            }
            
            AppendCompilerOption(ref sb, "/d:Framework_4_0");

            return (sb == null ? null : sb.ToString());
        }

        private void AppendCompilerOption(ref StringBuilder sb, string define)
        {
            if (sb == null)
                sb = new StringBuilder();
            else
                sb.Append(' ');

            sb.Append(define);
        }

        private byte[] GetHashCode(string compiledFile, string[] scriptFiles, bool debug)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bin = new BinaryWriter(ms))
                {
                    FileInfo fileInfo = new FileInfo(compiledFile);

                    bin.Write(fileInfo.LastWriteTimeUtc.Ticks);

                    foreach (string scriptFile in scriptFiles)
                    {
                        fileInfo = new FileInfo(scriptFile);

                        bin.Write(fileInfo.LastWriteTimeUtc.Ticks);
                    }

                    bin.Write(debug);
                    bin.Write(GlobalConfig.Version.ToString());

                    ms.Position = 0;

                    using (SHA1 sha1 = SHA1.Create())
                    {
                        return sha1.ComputeHash(ms);
                    }
                }
            }
        }

        public bool CompileCSScripts(out Assembly assembly)
        {
            return CompileCSScripts(false, true, out assembly);
        }

        public bool CompileCSScripts(bool debug, out Assembly assembly)
        {
            return CompileCSScripts(debug, true, out assembly);
        }

        public bool CompileCSScripts(bool debug, bool cache, out Assembly assembly)
        {
            Console.Write("Scripts: Compiling C# scripts...");
            string[] files = GetScripts("*.cs");

            if (files.Length == 0)
            {
                Console.WriteLine("no files found.");
                assembly = null;
                return true;
            }

            if (File.Exists(_scriptCsDllPath))
            {
                if (cache && File.Exists(_scriptCsHashPath))
                {
                    try
                    {
                        byte[] hashCode = GetHashCode(_scriptCsDllPath, files, debug);

                        using (
                            FileStream fs = new FileStream(_scriptCsHashPath, FileMode.Open,
                                FileAccess.Read, FileShare.Read))
                        {
                            using (BinaryReader bin = new BinaryReader(fs))
                            {
                                byte[] bytes = bin.ReadBytes(hashCode.Length);

                                if (bytes.Length == hashCode.Length)
                                {
                                    bool valid = true;

                                    for (int i = 0; i < bytes.Length; ++i)
                                    {
                                        if (bytes[i] != hashCode[i])
                                        {
                                            valid = false;
                                            break;
                                        }
                                    }

                                    if (valid)
                                    {
                                        assembly = Assembly.LoadFrom(_scriptCsDllPath);

                                        if (!_additionalReferences.Contains(assembly.Location))
                                        {
                                            _additionalReferences.Add(assembly.Location);
                                        }

                                        Console.WriteLine("done (cached)");

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    catch( Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }

            DeleteFiles(_scriptCsDllDeleteMask);

            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                string path = GetUnusedPath("Scripts.CS");

                CompilerParameters parms = new CompilerParameters(GetReferenceAssemblies(), path, debug);

                string options = GetCompilerOptions(debug);

                if (options != null)
                    parms.CompilerOptions = options;

                CompilerResults results = provider.CompileAssemblyFromFile(parms, files);
                _additionalReferences.Add(path);

                Display(results);

                if (results.Errors.Count > 0)
                {
                    throw new Exception("Compiler error!");
                    /*foreach (CompilerError err in results.Errors)
                    {
                        if (!err.IsWarning)
                        {
                            assembly = null;
                            return false;
                        }
                    }*/
                }


                if (cache && Path.GetFileName(path) == "Scripts.CS.dll")
                {
                    try
                    {
                        byte[] hashCode = GetHashCode(path, files, debug);

                        using ( var fs = new FileStream(_scriptCsHashPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            using (var bin = new BinaryWriter(fs))
                            {
                                bin.Write(hashCode, 0, hashCode.Length);
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                assembly = results.CompiledAssembly;
                return true;
            }
        }


        public void Display(CompilerResults results)
        {
            if (results.Errors.Count > 0)
            {
                var errors = new Dictionary<string, List<CompilerError>>(results.Errors.Count, StringComparer.OrdinalIgnoreCase);
                var warnings = new Dictionary<string, List<CompilerError>>(results.Errors.Count, StringComparer.OrdinalIgnoreCase);

                foreach (CompilerError e in results.Errors)
                {
                    string file = e.FileName;

                    // Ridiculous. FileName is null if the warning/error is internally generated in csc.
                    if (string.IsNullOrEmpty(file))
                    {
                        Console.WriteLine("ScriptCompiler: {0}: {1}", e.ErrorNumber, e.ErrorText);
                        continue;
                    }

                    Dictionary<string, List<CompilerError>> table = (e.IsWarning ? warnings : errors);

                    List<CompilerError> list = null;
                    table.TryGetValue(file, out list);

                    if (list == null)
                        table[file] = list = new List<CompilerError>();

                    list.Add(e);
                }

                if (errors.Count > 0)
                    Console.WriteLine("failed ({0} errors, {1} warnings)", errors.Count, warnings.Count);
                else
                    Console.WriteLine("done ({0} errors, {1} warnings)", errors.Count, warnings.Count);

                string scriptRoot = Path.GetFullPath(Path.Combine(GlobalConfig.BaseDirectory, "Scripts" + Path.DirectorySeparatorChar));
                Uri scriptRootUri = new Uri(scriptRoot);

                CommonUtils.PushColor(ConsoleColor.Yellow);

                if (warnings.Count > 0)
                    Console.WriteLine("Warnings:");

                foreach (KeyValuePair<string, List<CompilerError>> kvp in warnings)
                {
                    string fileName = kvp.Key;
                    List<CompilerError> list = kvp.Value;

                    string fullPath = Path.GetFullPath(fileName);
                    string usedPath =
                        Uri.UnescapeDataString(scriptRootUri.MakeRelativeUri(new Uri(fullPath)).OriginalString);

                    Console.WriteLine(" + {0}:", usedPath);

                    CommonUtils.PushColor(ConsoleColor.DarkYellow);

                    foreach (CompilerError e in list)
                        Console.WriteLine("    {0}: Line {1}: {3}", e.ErrorNumber, e.Line, e.Column, e.ErrorText);

                    CommonUtils.PopColor();
                }

                CommonUtils.PopColor();

                CommonUtils.PushColor(ConsoleColor.Red);

                if (errors.Count > 0)
                    Console.WriteLine("Errors:");

                foreach (KeyValuePair<string, List<CompilerError>> kvp in errors)
                {
                    string fileName = kvp.Key;
                    List<CompilerError> list = kvp.Value;

                    string fullPath = Path.GetFullPath(fileName);
                    string usedPath =
                        Uri.UnescapeDataString(scriptRootUri.MakeRelativeUri(new Uri(fullPath)).OriginalString);

                    Console.WriteLine(" + {0}:", usedPath);

                    CommonUtils.PushColor(ConsoleColor.DarkRed);

                    foreach (CompilerError e in list)
                        Console.WriteLine("    {0}: Line {1}: {3}", e.ErrorNumber, e.Line, e.Column, e.ErrorText);

                    CommonUtils.PopColor();
                }

                CommonUtils.PopColor();
            }
            else
            {
                Console.WriteLine("done (0 errors, 0 warnings)");
            }
        }

        public string GetUnusedPath(string name)
        {
            string path = Path.Combine(GlobalConfig.BaseDirectory, String.Format("Scripts/Output/{0}.dll", name));

            for (int i = 2; File.Exists(path) && i <= 1000; ++i)
                path = Path.Combine(GlobalConfig.BaseDirectory, String.Format("Scripts/Output/{0}.{1}.dll", name, i));

            return path;
        }

        public static void DeleteFiles(string mask)
        {
            try
            {
                string[] files = Directory.GetFiles(Path.Combine(GlobalConfig.BaseDirectory, "Scripts/Output"), mask);

                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        private delegate CompilerResults Compiler(bool debug);

        public bool Compile()
        {
            return Compile(false);
        }

        public bool Compile(bool debug)
        {
            return Compile(debug, true);
        }

        public bool Compile(bool debug, bool cache)
        {
            try
            {
                EnsureDirectory("Scripts/");
                EnsureDirectory("Scripts/Output/");

                if (_additionalReferences.Count > 0)
                    _additionalReferences.Clear();

                List<Assembly> assemblies = new List<Assembly>();

                Assembly assembly;

                if (CompileCSScripts(debug, cache, out assembly))
                {
                    if (assembly != null)
                    {
                        assemblies.Add(assembly);
                    }
                }
                else
                {
                    return false;
                }

                if (assemblies.Count == 0)
                {
                    return false;
                }

                _assemblies = assemblies.ToArray();

                Console.Write("Scripts: Verifying...");

                Stopwatch watch = Stopwatch.StartNew();

                Verify(Assembly.GetCallingAssembly());

                watch.Stop();

                Console.WriteLine("done ({0:F2} seconds)", watch.Elapsed.TotalSeconds);

                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public void Invoke(string method)
        {
            List<MethodInfo> invoke = new List<MethodInfo>();

            for (int a = 0; a < _assemblies.Length; ++a)
            {
                Type[] types = _assemblies[a].GetTypes();

                for (int i = 0; i < types.Length; ++i)
                {
                    MethodInfo m = types[i].GetMethod(method, BindingFlags.Static | BindingFlags.Public);

                    if (m != null)
                        invoke.Add(m);
                }
            }

            invoke.Sort(new CallPriorityComparer());

            for (int i = 0; i < invoke.Count; ++i)
                invoke[i].Invoke(null, null);
        }

        private Dictionary<Assembly, TypeCache> m_TypeCaches = new Dictionary<Assembly, TypeCache>();
        private TypeCache m_NullCache;

        public TypeCache GetTypeCache(Assembly asm)
        {
            if (asm == null)
            {
                if (m_NullCache == null)
                    m_NullCache = new TypeCache(null);

                return m_NullCache;
            }

            TypeCache c = null;
            m_TypeCaches.TryGetValue(asm, out c);

            if (c == null)
                m_TypeCaches[asm] = c = new TypeCache(asm);

            return c;
        }

        public Type FindTypeByFullName(string fullName)
        {
            return FindTypeByFullName(fullName, true);
        }

        public Type FindTypeByFullName(string fullName, bool ignoreCase)
        {
            Type type = null;

            for (int i = 0; type == null && i < _assemblies.Length; ++i)
                type = GetTypeCache(_assemblies[i]).GetTypeByFullName(fullName, ignoreCase);

            if (type == null)
                type = GetTypeCache(GlobalConfig.EntryAssembly).GetTypeByFullName(fullName, ignoreCase);

            return type;
        }

        public Type FindTypeByName(string name)
        {
            return FindTypeByName(name, true);
        }

        public Type FindTypeByName(string name, bool ignoreCase)
        {
            Type type = null;

            for (int i = 0; type == null && i < _assemblies.Length; ++i)
                type = GetTypeCache(_assemblies[i]).GetTypeByName(name, ignoreCase);

            if (type == null)
                type = GetTypeCache(GlobalConfig.EntryAssembly).GetTypeByName(name, ignoreCase);

            return type;
        }

        public void EnsureDirectory(string dir)
        {
            string path = Path.Combine(GlobalConfig.BaseDirectory, dir);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public string[] GetScripts(string filter)
        {
            List<string> list = new List<string>();

            GetScripts(list, Path.Combine(GlobalConfig.BaseDirectory, "Scripts"), filter);

            return list.ToArray();
        }

        public void GetScripts(List<string> list, string path, string filter)
        {
            foreach (string dir in Directory.GetDirectories(path))
                GetScripts(list, dir, filter);

            list.AddRange(Directory.GetFiles(path, filter));
        }

        private void VerifyType(Type t)
        {
            //TODO
            /*if (t.IsSubclassOf(typeof(Script)))
            {
                Interlocked.Increment(ref ScriptCount);

                StringBuilder warningSb = null;

                try
                {
                    if (t.GetConstructor(...) == null)
                    {
                        if (warningSb == null)
                            warningSb = new StringBuilder();

                        warningSb.AppendLine("       - No serialization constructor");
                    }

                    if (t.GetMethod("Serialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
                    {
                        if (warningSb == null)
                            warningSb = new StringBuilder();

                        warningSb.AppendLine("       - No Serialize() method");
                    }

                    if (t.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
                    {
                        if (warningSb == null)
                            warningSb = new StringBuilder();

                        warningSb.AppendLine("       - No Deserialize() method");
                    }

                    if (warningSb != null && warningSb.Length > 0)
                    {
                        Console.WriteLine("Warning: {0}\n{1}", t, warningSb.ToString());
                    }
                }
                catch
                {
                    Console.WriteLine("Warning: Exception in serialization verification of type {0}", t);
                }
            }*/
        }

        private void Verify(Assembly a)
        {
            if (a == null)
                return;

            Parallel.ForEach(a.GetTypes(), VerifyType);
        }
    }

    public class TypeCache
    {
        private Type[] m_Types;
        private TypeTable m_Names, m_FullNames;

        public Type[] Types
        {
            get { return m_Types; }
        }

        public TypeTable Names
        {
            get { return m_Names; }
        }

        public TypeTable FullNames
        {
            get { return m_FullNames; }
        }

        public Type GetTypeByName(string name, bool ignoreCase)
        {
            return m_Names.Get(name, ignoreCase);
        }

        public Type GetTypeByFullName(string fullName, bool ignoreCase)
        {
            return m_FullNames.Get(fullName, ignoreCase);
        }

        public TypeCache(Assembly asm)
        {
            if (asm == null)
                m_Types = Type.EmptyTypes;
            else
                m_Types = asm.GetTypes();

            m_Names = new TypeTable(m_Types.Length);
            m_FullNames = new TypeTable(m_Types.Length);

            Type typeofTypeAliasAttribute = typeof(TypeAliasAttribute);

            for (int i = 0; i < m_Types.Length; ++i)
            {
                Type type = m_Types[i];

                m_Names.Add(type.Name, type);
                m_FullNames.Add(type.FullName, type);

                if (type.IsDefined(typeofTypeAliasAttribute, false))
                {
                    object[] attrs = type.GetCustomAttributes(typeofTypeAliasAttribute, false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        TypeAliasAttribute attr = attrs[0] as TypeAliasAttribute;

                        if (attr != null)
                        {
                            for (int j = 0; j < attr.Aliases.Length; ++j)
                                m_FullNames.Add(attr.Aliases[j], type);
                        }
                    }
                }
            }
        }
    }

    public class TypeTable
    {
        private Dictionary<string, Type> m_Sensitive, m_Insensitive;

        public void Add(string key, Type type)
        {
            m_Sensitive[key] = type;
            m_Insensitive[key] = type;
        }

        public Type Get(string key, bool ignoreCase)
        {
            Type t = null;

            if (ignoreCase)
                m_Insensitive.TryGetValue(key, out t);
            else
                m_Sensitive.TryGetValue(key, out t);

            return t;
        }

        public TypeTable(int capacity)
        {
            m_Sensitive = new Dictionary<string, Type>(capacity);
            m_Insensitive = new Dictionary<string, Type>(capacity, StringComparer.OrdinalIgnoreCase);
        }
    }
}


