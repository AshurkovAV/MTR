using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Newtonsoft.Json;

namespace Medical.DatabaseCore.Services.Database
{
    /// <summary>
    /// Хранение настроек в БД
    /// </summary>
    public class AppRemoteSettings : IAppRemoteSettings
    {
        public static string DefaultOmsVersion = "DefaultOmsVersion";
        public static string ElmedicineConnectionString = "ElmedicineConnectionString";
        public static string TortillaConnectionString = "TortillaConnectionString";
        public static string SrzServiceConnectionString = "SrzServiceConnectionString";
        public static string MedicineLogConnectionString = "MedicineLogConnectionString";
        public static string RegisterPath = "RegisterPath";
        public static string ActIds = "ActIds";
        public static string FFomsReportConnectionString = "FFomsReportConnectionString";
        public static string TerritoriCode = "TerritoriCode";
        public static string FileXmlOms = "FileXmlOms";
        public static string ClientMo = "ClientMo";


        private readonly IMedicineRepository _repository;

        private Dictionary<string, object> _storage;
        private Dictionary<string, object> _metadata;

        

        #region DefaultConfig

        private readonly Dictionary<string, Tuple<string, string>> _defaultSettings = new Dictionary
            <string, Tuple<string, string>>
        {
            {
                DefaultOmsVersion,
                Tuple.Create(
                    @"{""version"":9}",
                    @"{
                        ""Id"" : ""DefaultOmsVersion"",
                        ""Title"" : ""Версия формата OMS по умолчанию"",
                        ""Descriptions"" : [
                            {
                                ""Id"" : ""version"",
                                ""DisplayName"" : ""Версия"",
                                ""DefaultValue"" : 2,
                                ""Typo"" : ""int"",
                                ""DisplayCategory"" : ""Основные настройки""
                            }
                        ] 
                    }"
                    )
            },
            {
                ElmedicineConnectionString,
                Tuple.Create(
                    @"{
                        ""Name"": ""elmedicine"",
                        ""DataSource"": "".\\SQLEXPRESS"",
                        ""Database"": ""elmedicineNewfond"",
                        ""Timeout"": 15,
                        ""Provider"": ""Sql"",
                        ""IsWindowsAuth"": true
                    }",
                    @"{
                    ""Id"" : ""ElmedicineConnectionString"",
                    ""Title"" : ""Соединение с БД Elmedicine"",
                    ""Descriptions"" : [
                        {
                            ""Id"" : ""Name"",
                            ""DisplayName"" : ""Название"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""DataSource"",
                            ""DisplayName"" : ""Источник данных"",
                            ""DefaultValue"" : "".\\SQLEXPRESS"",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Database"",
                            ""DisplayName"" : ""База данных"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Provider"",
                            ""DisplayName"" : ""Провайдер"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""IsWindowsAuth"",
                            ""DisplayName"" : ""Аутентификация Windows"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""bool"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""UserId"",
                            ""DisplayName"" : ""Пользователь"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Password"",
                            ""DisplayName"" : ""Пароль"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Timeout"",
                            ""DisplayName"" : ""Таймаут"",
                            ""DefaultValue"" : ""15"",
                            ""Typo"" : ""int"",
                            ""DisplayCategory"" : ""Расширенные настройки""
                        }
                    ],
                    ""Methods"" : [
                        {
                            ""Name"" : ""Test"",
                            ""Code"" : ""ScriptHelpers.TestDatabase(Dynamic);"",
                        }
                    ]
                }")
            },
            {
                TortillaConnectionString, Tuple.Create(
                    @"{
                        ""Name"": ""tortila"",
                        ""DataSource"": "".\\SQLEXPRESS"",
                        ""Database"": ""tortila"",
                        ""Timeout"": 15,
                        ""Provider"": ""Sql"",
                        ""IsWindowsAuth"": true
                    }",
                    @"{
                    ""Id"" : ""TortillaConnectionString"",
                    ""Title"" : ""Соединение с БД Tortilla"",
                    ""Descriptions"" : [
                        {
                            ""Id"" : ""Name"",
                            ""DisplayName"" : ""Название"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""DataSource"",
                            ""DisplayName"" : ""Источник данных"",
                            ""DefaultValue"" : "".\\SQLEXPRESS"",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Database"",
                            ""DisplayName"" : ""База данных"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Provider"",
                            ""DisplayName"" : ""Провайдер"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""IsWindowsAuth"",
                            ""DisplayName"" : ""Аутентификация Windows"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""bool"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""UserId"",
                            ""DisplayName"" : ""Пользователь"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Password"",
                            ""DisplayName"" : ""Пароль"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Timeout"",
                            ""DisplayName"" : ""Таймаут"",
                            ""DefaultValue"" : ""15"",
                            ""Typo"" : ""int"",
                            ""DisplayCategory"" : ""Расширенные настройки""
                        }
                    ],
                    ""Methods"" : [
                        {
                            ""Name"" : ""Test"",
                            ""Code"" : ""ScriptHelpers.TestDatabase(Dynamic);"",
                        }
                    ] 
                }")
            },
            {
                SrzServiceConnectionString, Tuple.Create(
                    @"{
                        ""Name"": ""srzservice"",
                        ""RemoteAddress"": ""localhost"",
                        ""Port"": 8001,
                        ""ServiceProtocol"": ""NetTcpBinding""
                    }",
                    @"{
                    ""Id"" : ""SrzServiceConnectionString"",
                    ""Title"" : ""Соединение с сервисом данных Srz"",
                    ""Descriptions"" : [
                        {
                            ""Id"" : ""Name"",
                            ""DisplayName"" : ""Название"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""RemoteAddress"",
                            ""DisplayName"" : ""Источник данных"",
                            ""DefaultValue"" : ""localhost"",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Port"",
                            ""DisplayName"" : ""Порт"",
                            ""DefaultValue"" : 8001,
                            ""Typo"" : ""int"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""ServiceProtocol"",
                            ""DisplayName"" : ""Протокол"",
                            ""DefaultValue"" : ""NetTcpBinding"",
                            ""Typo"" : ""BindingType"",
                            ""DisplayCategory"" : ""Основные настройки""
                        }
                    ],
                    ""Methods"" : [
                        {
                            ""Name"" : ""Test"",
                            ""Code"" : ""ScriptHelpers.TestDataService(Dynamic);"",
                        }
                    ] 
                }")
            },
            {
                MedicineLogConnectionString, Tuple.Create(
                    @"{
                    ""Name"": ""medicineLog"",
                    ""DataSource"": "".\\SQLEXPRESS"",
                    ""Database"": ""medicine_log"",
                    ""Timeout"": 15,
                    ""Provider"": ""Sql"",
                    ""IsWindowsAuth"": true
                }",
                    @"{
                ""Id"" : ""MedicineLogConnectionString"",
                ""Title"" : ""Соединение с БД логирования"",
                ""Descriptions"" : [
                    {
                        ""Id"" : ""Name"",
                        ""DisplayName"" : ""Название"",
                        ""DefaultValue"" : """",
                        ""Typo"" : ""string"",
                        ""DisplayCategory"" : ""Основные настройки""
                    },
                    {
                        ""Id"" : ""DataSource"",
                        ""DisplayName"" : ""Источник данных"",
                        ""DefaultValue"" : "".\\SQLEXPRESS"",
                        ""Typo"" : ""string"",
                        ""DisplayCategory"" : ""Основные настройки""
                    },
                    {
                        ""Id"" : ""Database"",
                        ""DisplayName"" : ""База данных"",
                        ""DefaultValue"" : """",
                        ""Typo"" : ""string"",
                        ""DisplayCategory"" : ""Основные настройки""
                    },
                    {
                        ""Id"" : ""Provider"",
                        ""DisplayName"" : ""Провайдер"",
                        ""DefaultValue"" : """",
                        ""Typo"" : ""string"",
                        ""DisplayCategory"" : ""Основные настройки""
                    },
                    {
                        ""Id"" : ""IsWindowsAuth"",
                        ""DisplayName"" : ""Аутентификация Windows"",
                        ""DefaultValue"" : """",
                        ""Typo"" : ""bool"",
                        ""DisplayCategory"" : ""Основные настройки""
                    },
                    {
                        ""Id"" : ""UserId"",
                        ""DisplayName"" : ""Пользователь"",
                        ""DefaultValue"" : """",
                        ""Typo"" : ""string"",
                        ""DisplayCategory"" : ""Основные настройки""
                    },
                    {
                        ""Id"" : ""Password"",
                        ""DisplayName"" : ""Пароль"",
                        ""DefaultValue"" : """",
                        ""Typo"" : ""string"",
                        ""DisplayCategory"" : ""Основные настройки""
                    },
                    {
                        ""Id"" : ""Timeout"",
                        ""DisplayName"" : ""Таймаут"",
                        ""DefaultValue"" : ""15"",
                        ""Typo"" : ""int"",
                        ""DisplayCategory"" : ""Расширенные настройки""
                    }
                ],
                ""Methods"" : [
                    {
                        ""Name"" : ""Test"",
                        ""Code"" : ""ScriptHelpers.TestDatabase(Dynamic);"",
                    }
                ]}")
            },
            {
                RegisterPath, Tuple.Create(
                    @"{
                    ""UnprocessedPathR"": ""d:\\!Projects!\\MedicineRefactoring\\Bin\\Debug\\RegistersR\\"",
                    ""UnprocessedPathD"": ""d:\\!Projects!\\MedicineRefactoring\\Bin\\Debug\\RegistersD\\"",
                    ""UnprocessedPathA"": ""d:\\!Projects!\\MedicineRefactoring\\Bin\\Debug\\RegistersA\\"",
                    ""ProcessedPattern"": ""Processed_{0}""
                }",
                    @"{
                    ""Id"" : ""RegisterPath"",
                    ""Title"" : ""Путь к месту хранения файлов OMS"",
                    ""Descriptions"" : [
                        {
                            ""Id"" : ""UnprocessedPathR"",
                            ""DisplayName"" : ""Путь к месту хранения необработанных R файлов OMS"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""UnprocessedPathD"",
                            ""DisplayName"" : ""Путь к месту хранения необработанных D файлов OMS"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""UnprocessedPathA"",
                            ""DisplayName"" : ""Путь к месту хранения необработанных A файлов OMS"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""ProcessedPattern"",
                            ""DisplayName"" : ""Паттерн для названия папки с обработанными файлами OMS"",
                            ""DefaultValue"" : ""Processed_{0}"",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        }
                    ],
                    ""Methods"" : [
                            
                    ] 
                }")
            },
            {
                ActIds, Tuple.Create(
                    @"{
                    ""ActMedicalMee"": 14,
                    ""ActMee"": 13,
                    ""ActMedicalEqma"": 16,
                    ""ActEqma"": 15
                }",
                    @"{
                    ""Id"" : ""ActIds"",
                    ""Title"" : ""ID актов МЭЭ/ЭКМП для МО и территорий"",
                    ""Descriptions"" : [
                        {
                            ""Id"" : ""ActMedicalMee"",
                            ""DisplayName"" : ""ID акта МЭЭ для МО"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""int"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""ActMee"",
                            ""DisplayName"" : ""ID акта МЭЭ для территорий"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""int"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""ActMedicalEqma"",
                            ""DisplayName"" : ""ID акта ЭКМП для МО"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""int"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""ActEqma"",
                            ""DisplayName"" : ""ID акта ЭКМП для территорий"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""int"",
                            ""DisplayCategory"" : ""Основные настройки""
                        }
                    ],
                    ""Methods"" : [
                            
                    ] 
                }")
            },
            {
                FFomsReportConnectionString,
                Tuple.Create(
                    @"{
                        ""Name"": ""ffomsreports"",
                        ""DataSource"": ""tfoms-2012"",
                        ""Database"": ""elmedicineNewfond"",
                        ""Timeout"": 15,
                        ""Provider"": ""Sql"",
                        ""IsWindowsAuth"": true
                    }",
                    @"{
                    ""Id"" : ""FFomsReportConnectionString"",
                    ""Title"" : ""Соединение с БД отчетов в фед. фонд"",
                    ""Descriptions"" : [
                        {
                            ""Id"" : ""Name"",
                            ""DisplayName"" : ""Название"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""DataSource"",
                            ""DisplayName"" : ""Источник данных"",
                            ""DefaultValue"" : "".\\SQLEXPRESS"",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Database"",
                            ""DisplayName"" : ""База данных"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Provider"",
                            ""DisplayName"" : ""Провайдер"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""IsWindowsAuth"",
                            ""DisplayName"" : ""Аутентификация Windows"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""bool"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""UserId"",
                            ""DisplayName"" : ""Пользователь"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Password"",
                            ""DisplayName"" : ""Пароль"",
                            ""DefaultValue"" : """",
                            ""Typo"" : ""string"",
                            ""DisplayCategory"" : ""Основные настройки""
                        },
                        {
                            ""Id"" : ""Timeout"",
                            ""DisplayName"" : ""Таймаут"",
                            ""DefaultValue"" : ""15"",
                            ""Typo"" : ""int"",
                            ""DisplayCategory"" : ""Расширенные настройки""
                        }
                    ],
                    ""Methods"" : [
                        {
                            ""Name"" : ""Test"",
                            ""Code"" : ""ScriptHelpers.TestDatabase(Dynamic);"",
                        }
                    ]
                }")
            },
            {
                TerritoriCode,
                Tuple.Create(
                    @"{""tf_code"":46,""tf_okato"":""38000""}",
                    @"{
                        ""Id"" : ""TerritoriCode"",
                        ""Title"" : ""Код территории страхования"",
                        ""Descriptions"" : [
                            {
                                ""Id"" : ""tf_code"",
                                ""DisplayName"" : ""Код тер"",
                                ""DefaultValue"" : 46,
                                ""Typo"" : ""int"",
                                ""DisplayCategory"" : ""Основные настройки""
                            },
							{
                                ""Id"" : ""tf_okato"",
                                ""DisplayName"" : ""Код ОКАТО"",
                                ""DefaultValue"" : ""38000"",
                                ""Typo"" : ""string"",
                                ""DisplayCategory"" : ""Основные настройки""
                            }
                        ] 
                    }"
                    )
            },
            {
                FileXmlOms,
                Tuple.Create(
                    @"{""delXml"": false,
                            ""delOms"": false,
                            ""fileA"": ""D:\\"",
                            ""fileR"": ""D:\\"",
                            ""fileD"": ""D:\\""
                           }",
                    @"{
                        ""Id"" : ""FileXmlOms"",
                        ""Title"" : ""Работа с файлами xml, oms"",
                        ""Descriptions"" : [
                            {
                                ""Id"" : ""delXml"",
                                ""DisplayName"" : ""Удаление xml файлов при загрузке счета(вх.счетов)"",
                                ""DefaultValue"" : ""false"",
                                ""Typo"" : ""bool"",
                                ""DisplayCategory"" : ""Основные настройки""
                            },
							{
                                ""Id"" : ""delOms"",
                                ""DisplayName"" : ""Удаление oms файлов при загрузке счета(вх.счетов)"",
                                ""DefaultValue"" : ""false"",
                                ""Typo"" : ""bool"",
                                ""DisplayCategory"" : ""Основные настройки""
                            },
							{
                                ""Id"" : ""fileA"",
                                ""DisplayName"" : ""Путь к месту выгрузки А файлов OMS"",
                                ""DefaultValue"" : ""D:\\"",
                                ""Typo"" : ""string"",
                                ""DisplayCategory"" : ""Основные настройки""
                            },
							{
                                ""Id"" : ""fileR"",
                                ""DisplayName"" : ""Путь к месту выгрузки R файлов OMS"",
                                ""DefaultValue"" : ""D:\\"",
                                ""Typo"" : ""string"",
                                ""DisplayCategory"" : ""Основные настройки""
                            },
							{
                                ""Id"" : ""fileD"",
                                ""DisplayName"" : ""Путь к месту выгрузки D файлов OMS"",
                                ""DefaultValue"" : ""D:\\"",
                                ""Typo"" : ""string"",
                                ""DisplayCategory"" : ""Основные настройки""
                            }
                        ] ,
                         ""Methods"" : [
                  
                    ] 
                    }")
            },
            {
                ClientMo,
                Tuple.Create(
                    @"{""MO"": false,  ""Mcod"": ""460026"", }",
                    @"{
                        ""Id"" : ""ClientMo"",
                        ""Title"" : ""Работа с данными в мед организаци"",
                        ""Descriptions"" : [
                            {
                                ""Id"" : ""MO"",
                                ""DisplayName"" : ""Добавляет функции для работы в МО"",
                                ""DefaultValue"" : ""false"",
                                ""Typo"" : ""bool"",
                                ""DisplayCategory"" : ""Основные настройки""
                            },
							{
                                ""Id"" : ""Mcod"",
                                ""DisplayName"" : ""Код медицинской организации из справочника F003"",
                                ""DefaultValue"" : ""460026"",
                                ""Typo"" : ""string"",
                                ""DisplayCategory"" : ""Основные настройки""
                            }
                        ] ,
                         ""Methods"" : [                  
                    ] 
                    }")
            }

        };
    
        #endregion 

        private readonly IEnumerable<string> _settingsKeys = new List<string>
        {
            DefaultOmsVersion,
            ElmedicineConnectionString,
            TortillaConnectionString,
            SrzServiceConnectionString,
            MedicineLogConnectionString,
            RegisterPath,
            ActIds,
            FFomsReportConnectionString,
            TerritoriCode,
            FileXmlOms,
            ClientMo
        };

        public AppRemoteSettings(IMedicineRepository repository)
        {
            _repository = repository;
            Load();
        }

        public bool SaveAll()
        {
            var success = true;
            try
            {
                foreach (var entry in _storage)
                {
                    var result = _repository.UpdateLocalSettings(entry.Key, JsonConvert.SerializeObject(entry.Value));
                    if (!result.Success)
                    {
                        success = false;
                    }
                }

                return success;
            }
            catch (Exception exception)
            {
                //TODO add error
            }
            return success;
        }

        public bool Save(string key, object value)
        {
            var success = true;
            try
            {
                var result = _repository.UpdateLocalSettings(key, JsonConvert.SerializeObject(value));
                if (!result.Success)
                {
                    success = false;
                }

                return success;
            }
            catch (Exception exception)
            {
                //TODO add error
            }
            return success;
        }

        public bool SaveMetadata(string key, object metadata)
        {
            var success = true;
            try
            {
                var result = _repository.UpdateLocalSettingsMetadata(key, JsonConvert.SerializeObject(metadata));
                if (!result.Success)
                {
                    success = false;
                }

                return success;
            }
            catch (Exception exception)
            {
                //TODO add error
            }
            return success;
        }

        public bool Load()
        {
            try
            {
                var result = _repository.GetLocalSettings();
                if (result.Success)
                {
                    _storage = new Dictionary<string, object>();
                    result.Data.Select(p=>new
                    {
                        key = p.Key,
                        value = p.Value
                    }).ForEachAction(p => _storage.Add(p.key, JsonConvert.DeserializeObject<object>(p.value)));

                    _metadata = new Dictionary<string, object>();
                    result.Data.Select(p => new
                    {
                        key = p.Key,
                        value = p.Metadata
                    }).ForEachAction(p => _metadata.Add(p.key, JsonConvert.DeserializeObject<object>(p.value)));
                }
                return true;
            }
            catch (Exception exception)
            {
                //TODO add error
                Console.WriteLine();
            }
            return false;
        }

        public object Get(string section)
        {
            if (_storage.ContainsKey(section))
            {
                return _storage[section];
            }
            {
                if (_defaultSettings.ContainsKey(section))
                {
                    Put(section,JsonConvert.DeserializeObject<object>(_defaultSettings[section].Item1));
                    return _storage[section];
                }
            }
            return null;
        }

        public object GetMetadata(string section)
        {
            if (_metadata.ContainsKey(section))
            {
                return _metadata[section];
            }
            else
            {
                if (_defaultSettings.ContainsKey(section))
                {
                    PutMetadata(section,JsonConvert.DeserializeObject<object>(_defaultSettings[section].Item2));
                    return _metadata[section];
                }
            }
            return null;
        }

        public bool Has(string section)
        {
            return _storage.ContainsKey(section);
        }

        public Dictionary<string, object> GetAll()
        {
            return _storage;
        }

        public Dictionary<string, object> GetAllMetadata()
        {
            return _metadata;
        }

        public IEnumerable<string> GetAllKeys()
        {
            return _settingsKeys;
        }

        public void Put(string section, object cfg)
        {
            if (_storage.ContainsKey(section))
            {
                _storage.Remove(section);
            }

            _storage.Add(section,cfg);
            Save(section, _storage[section]);
        }

        public void PutMetadata(string section, object cfg)
        {
            if (_metadata.ContainsKey(section))
            {
                _metadata.Remove(section);
            }

            _metadata.Add(section, cfg);
            SaveMetadata(section, _metadata[section]);
        }

    }
}
