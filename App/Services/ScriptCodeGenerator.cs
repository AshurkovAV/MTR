using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Medical.DatabaseCore.Services.Database;
using Newtonsoft.Json;
using Medical.CoreLayer.Service;

namespace Medical.AppLayer.Services
{
    public class FieldDescription
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string DisplayCategory { get; set; }
        public string DefaultValue { get; set; }
        public string Typo { get; set; }
    }

    public class MethodDescription
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class CodeDescription
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<FieldDescription> Descriptions { get; set; }
        public List<MethodDescription> Methods { get; set; }
    }

    public interface IScriptCodeGenerator
    {
        OperationResult GenerateSettingsScriptsCode();
        OperationResult<Tuple<string, string>> GenerateCodeByDescription(string description);
    }

    public class ScriptCodeGenerator : IScriptCodeGenerator
    {
        private readonly IMedicineRepository _repository;
        private readonly IFileService _fileService;
        private readonly IMessageService _messageService;

        private const string DefaultPath = "Scripts";

        private readonly List<string> _usingList = new List<string>
        {
            "System",
            "System.Collections.Generic",
            "System.ComponentModel",
            "System.Linq",
            "System.Text",
            "Newtonsoft.Json",
            "Core"
        };

        private const string DefaultFileExt = ".cs";

        private const string DefaultNamespace = "Medical.AppLayer.Scripts";

        private const string EOL = "\r\n";
        private const string UsingTemplate = "using {0};" + EOL;
        private const string NameSpaceTemplate = "namespace {0} {{ {1} }}" + EOL;
        private const string ClassTemplate = "public class {0}{{ {1} }}" + EOL;
        private const string PropertyTemplate = "[Category(\"{0}\")][DisplayNameAttribute(\"{1}\")]public {2} {3} {{ get; set; }}" + EOL;
        private const string MethodTemplate = "public void {0}() {{ {1} }}" + EOL;
        private const string PropertyDynamicTemplate = "[BrowsableAttribute(false)][JsonIgnore] public object Dynamic {{ get {{ return this; }} }}" + EOL;

        public ScriptCodeGenerator(
            IMedicineRepository repository, 
            IFileService fileService, 
            IMessageService messageService)
        {
            _repository = repository;
            _fileService = fileService;
            _messageService = messageService;
        }
        public OperationResult<Tuple<string, string>> GenerateCodeByDescription(string description)
        {
            var result = new OperationResult<Tuple<string, string>>();
            try
            {
                var propNameList = new List<string>();
                var fieldDescription = JsonConvert.DeserializeObject<CodeDescription>(description);

                var sb = new StringBuilder();
                foreach (var v in _usingList)
                {
                    sb.AppendFormat(UsingTemplate, v);
                }

                var prop = new StringBuilder();
                foreach (var d in fieldDescription.Descriptions)
                {
                    propNameList.Add(d.Id);
                    prop.AppendFormat(PropertyTemplate, d.DisplayCategory, d.DisplayName, d.Typo, d.Id);
                }

                if (fieldDescription.Methods.IsNotNull())
                {
                    foreach (var d in fieldDescription.Methods)
                    {
                        prop.AppendFormat(MethodTemplate, d.Name, d.Code);
                    }
                }
                

                if (propNameList.Any())
                {
                    prop.AppendFormat(PropertyDynamicTemplate);
                }

                var cls = new StringBuilder();
                cls.AppendFormat(ClassTemplate, fieldDescription.Id, prop);

                sb.AppendFormat(NameSpaceTemplate, DefaultNamespace, cls);

                result.Data = Tuple.Create(fieldDescription.Id + DefaultFileExt, sb.ToString());
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult GenerateSettingsScriptsCode()
        {
            var result = new OperationResult();
            try
            {
                var settingsResult = _repository.GetLocalSettings();
                if (settingsResult.Success)
                {
                    foreach (var settings in settingsResult.Data)
                    {
                        if (settings.Metadata.IsNotNullOrWhiteSpace())
                        {
                            var generateResult = GenerateCodeByDescription(settings.Metadata);
                            if (generateResult.Success)
                            {
                                var fileResult = _fileService.SaveTextFileWithEncoding(
                                    Path.Combine(GlobalConfig.BaseDirectory, DefaultPath, generateResult.Data.Item1), 
                                    generateResult.Data.Item2,
                                    Encoding.UTF8);

                                if (fileResult.HasError)
                                {
                                    _messageService.ShowException(fileResult.LastError, "Исключение при записи сгенерированного файла скрипта", typeof(ScriptCodeGenerator));
                                    //TODO logging
                                }
                            }
                            else
                            {
                                _messageService.ShowException(generateResult.LastError, "Исключение при генерации файла скрипта", typeof(ScriptCodeGenerator));
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
           
            return result;
        }
    }
}
