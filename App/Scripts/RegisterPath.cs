using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Core;
namespace Medical.AppLayer.Scripts { public class RegisterPath{ [Category("Основные настройки")][DisplayNameAttribute("Путь к месту хранения необработанных R файлов OMS")]public string UnprocessedPathR { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Путь к месту хранения необработанных D файлов OMS")]public string UnprocessedPathD { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Путь к месту хранения необработанных A файлов OMS")]public string UnprocessedPathA { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Паттерн для названия папки с обработанными файлами OMS")]public string ProcessedPattern { get; set; }
[BrowsableAttribute(false)][JsonIgnore] public object Dynamic { get { return this; } }
 }
 }
