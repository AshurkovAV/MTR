using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Core;
namespace Medical.AppLayer.Scripts { public class ActIds{ [Category("Основные настройки")][DisplayNameAttribute("ID акта МЭЭ для МО")]public int ActMedicalMee { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("ID акта МЭЭ для территорий")]public int ActMee { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("ID акта ЭКМП для МО")]public int ActMedicalEqma { get; set; }
[Category("Основные настройки")][DisplayNameAttribute("ID акта ЭКМП для территорий")]public int ActEqma { get; set; }
[BrowsableAttribute(false)][JsonIgnore] public object Dynamic { get { return this; } }
 }
 }
