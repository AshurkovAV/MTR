using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Core;
namespace Medical.AppLayer.Scripts { public class ElmedicineConnectionString{ [Category("Основные настройки")][DisplayNameAttribute("Название")]public string Name { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Источник данных")]public string DataSource { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("База данных")]public string Database { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Провайдер")]public string Provider { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Аутентификация Windows")]public bool IsWindowsAuth { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Пользователь")]public string UserId { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("Пароль")]public string Password { get; set; }
[Category("Расширенные настройки")][DisplayNameAttribute("Таймаут")]public int Timeout { get; set; }
public void Test() { ScriptHelpers.TestDatabase(Dynamic); }
[BrowsableAttribute(false)][JsonIgnore] public object Dynamic { get { return this; } }
 }
 }
