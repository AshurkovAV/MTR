using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Infrastructure.Compiler
{
    public interface IScriptCompiler
    {
        Assembly[] Assemblies { get; set; }
        string[] GetReferenceAssemblies();
        string GetCompilerOptions(bool debug);
        bool CompileCSScripts(out Assembly assembly);
        bool CompileCSScripts(bool debug, out Assembly assembly);
        bool CompileCSScripts(bool debug, bool cache, out Assembly assembly);
        void Display(CompilerResults results);
        string GetUnusedPath(string name);
        bool Compile();
        bool Compile(bool debug);
        bool Compile(bool debug, bool cache);
        void Invoke(string method);
        TypeCache GetTypeCache(Assembly asm);
        Type FindTypeByFullName(string fullName);
        Type FindTypeByFullName(string fullName, bool ignoreCase);
        Type FindTypeByName(string name);
        Type FindTypeByName(string name, bool ignoreCase);
        void EnsureDirectory(string dir);
        string[] GetScripts(string filter);
        void GetScripts(List<string> list, string path, string filter);
    }
}