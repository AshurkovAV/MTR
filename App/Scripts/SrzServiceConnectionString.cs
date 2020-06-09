using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Core;
namespace Medical.AppLayer.Scripts { public class SrzServiceConnectionString{ [Category("Основные настройки")][DisplayNameAttribute("Название")]public string Name { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Источник данных")]public string RemoteAddress { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Порт")]public int Port { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Протокол")]public BindingType ServiceProtocol { get; set; }
public void Test() { ScriptHelpers.TestDataService(Dynamic); }
[BrowsableAttribute(false)][JsonIgnore] public object Dynamic { get { return this; } }
 }
 }
