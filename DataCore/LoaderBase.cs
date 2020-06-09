using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Core.Extensions;
using Core.Helpers;
using Medical.DataCore.Validate;

namespace Medical.DataCore
{
    public abstract class LoaderBase
    {
        private IList<string> _errors;
        private IList<string> _warnings;
        public IQueryable<string> ErrorsQuery
        {
            get { return Errors.AsQueryable(); }
        }
        public IList<string> Errors
        {
            get { return _errors ?? (_errors = new List<string>()); }
            set { _errors = value; }
        }

        public IList<string> Warnings
        {
            get { return _warnings ?? (_warnings = new List<string>()); }
            set { _warnings = value; }
        }

        public string ErrorsAggregate
        {
            get
            {
                return ErrorCount == 0 ? string.Empty : Errors.Aggregate((current, next) => current + ", " + next);
            }
        }

        public string WarningsAggregate
        {
            get
            {
                return WarningCount == 0 ? string.Empty : Warnings.Aggregate((current, next) => current + ", " + next);
            }
        }
        

        public int ErrorCount
        {
            get { return ErrorsQuery.Count(); }
        }
        public int WarningCount
        {
            get { return _warnings.Count(); }
        }
        public abstract string SchemePath { get; }
        public abstract int FileCount { get; }
        public abstract string FileExtension { get; }
        public RegisterInfo Info { get; set; }

        public virtual List<string> Unpack<T>(string omsFileName) where T : RegisterInfo, new()
        {
            try
            {
                string xmlFileName = string.Format("{0}.{1}",Path.GetFileNameWithoutExtension(omsFileName), FileExtension);
                Info = new T();
                Info.SetInfo(Path.GetFileNameWithoutExtension(omsFileName));
                if (!string.IsNullOrWhiteSpace(Info.Errors))
                {
                    Errors.Add(string.Format("Неправильное имя файла {0}", Path.GetFileNameWithoutExtension(omsFileName)));
                    return null;
                }

                var zipHelper = new ZipHelpers();
                List<string> fileNames = zipHelper.UnpackFiles(omsFileName).Where(p => p.ToLowerInvariant().EndsWith(".xml")).ToList();
                string[] xml = new[] {".xml"};
                if (fileNames.Count == 0 || fileNames.Count() > FileCount)
                {
                    Errors.Add(
                        string.Format("Не найдены файлы в архиве или неверное количество файлов {0} (0 или >{1} )",
                                      fileNames.Count, FileCount));
                    return null;
                }

                foreach (string name in fileNames)
                {
                    string unpackFileName = Path.GetFileName(name);
                    if (string.IsNullOrWhiteSpace(unpackFileName))
                    {
                        Errors.Add(string.Format("Не найден распакованный файл {0}", name));
                        return null;
                    }

                    if (!unpackFileName.Equals(xmlFileName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Errors.Add(string.Format("Имя файла не совпадает с распакованным {0} != {1}", unpackFileName,
                                                 xmlFileName));
                        return null;
                    }
                }

                return fileNames;
            }
            catch (Exception exception)
            {
                Errors.Add(string.Format("При загрузке xml поизошло исключение {0}", exception));
            }
            return null;
        }

        public bool LoadInfo<T>(string omsFileName) where T : RegisterInfo, new()
        {
            try
            {
                string xmlFileName = string.Format("{0}.{1}", Path.GetFileNameWithoutExtension(omsFileName), FileExtension);
                Info = new T();
                Info.SetInfo(Path.GetFileNameWithoutExtension(omsFileName));
                if (!string.IsNullOrWhiteSpace(Info.Errors))
                {
                    Errors.Add(string.Format("Неправильное имя файла {0}", Path.GetFileNameWithoutExtension(omsFileName)));
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                Errors.Add(string.Format("При загрузке xml поизошло исключение {0}", exception));
            }
            return false;
        }

        public T Load<T>(string xmlFileName) where T : class
        {
            try
            {
                using (var stream = new FileStream(xmlFileName, FileMode.Open))
                {
                    return Load<T>(stream);
                }
            }
            catch (Exception exception)
            {
                Errors.Add(string.Format("При загрузке xml поизошло исключение {0}", exception));
            }
            return null;
        }

        public bool Validate(Stream reader, Stream scheme)
        {
            ISchemaValidate validate = new SchemaValidateHandler();
            validate.Validate(reader, scheme);
            if (validate.Errors.Count != 0)
            {
                Errors.AddRange(validate.Errors);
                return false;
            }
            return true;
        }

        public bool Validate<T>(Stream reader,string schemePath) where T : class
        {
            return Validate(reader, typeof(T).Assembly.GetManifestResourceStream(schemePath));
        }

        public bool Validate<T>(Stream reader) where T : class
        {
            return Validate(reader, typeof (T).Assembly.GetManifestResourceStream(SchemePath));
        }

        public bool Validate<T>(string file) where T : class
        {
            using (var stream = new FileStream(file, FileMode.Open))
            {
                return Validate(stream, typeof(T).Assembly.GetManifestResourceStream(SchemePath));
            }
        }

        public bool Validate<T>(string file,string schemePath) where T : class
        {
            using (var stream = new FileStream(file, FileMode.Open))
            {
                return Validate(stream, typeof(T).Assembly.GetManifestResourceStream(schemePath));
            }
        }

        public bool Validate<T>(XmlTextReader reader, string schemePath) where T : class
        {
            Errors.Clear();
            ISchemaValidate validate = new SchemaValidateHandler();
            validate.Validate(reader, typeof(T).Assembly.GetManifestResourceStream(schemePath));
            if (validate.Errors.Count != 0)
            {
                Errors.AddRange(validate.Errors);
                return false;
            }
            return true;
        }

        public bool Validate2<T>(string file) where T : class
        {
            Errors.Clear();
            using (var reader = new XmlTextReader(file))
            {
                ISchemaValidate validate = new SchemaValidateHandler();
                validate.Validate(reader, typeof(T).Assembly.GetManifestResourceStream(SchemePath));
                if (validate.Errors.Count != 0)
                {
                    Errors.AddRange(validate.Errors);
                    return false;
                }
                return true;
            }
        }

        public bool Validate2<T>(string file, string schemePath) where T : class
        {
            Errors.Clear();
            using (var reader = new XmlTextReader(file))
            {
                ISchemaValidate validate = new SchemaValidateHandler();
                validate.Validate(reader, typeof(T).Assembly.GetManifestResourceStream(schemePath));
                if (validate.Errors.Count != 0)
                {
                    Errors.AddRange(validate.Errors);
                    return false;
                }
                return true;
            }
        }

        public T LoadAndValidate<T>(Stream reader) where T : class
        {
            try
            {
                if (Validate<T>(reader))
                {
                    var serializer = new XmlSerializer(typeof (T));
                    reader.Position = 0;
                    var register = (T) serializer.Deserialize(reader);
                    reader.Close();
                    return register; 
                }
            }
            catch (Exception exception)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                Errors.Add(string.Format("При загрузке произошло исключение {0}", exception));
            }
            return null;
        }

        public T Load<T>(Stream reader) where T : class
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader.Position = 0;
                var register = (T)serializer.Deserialize(reader);
                reader.Close();
                return register;
            }
            catch (Exception exception)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                Errors.Add(string.Format("При загрузке произошло исключение {0}", exception));
            }
            return null;
        }
    }
}