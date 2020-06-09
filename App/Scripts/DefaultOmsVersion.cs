using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Core;
namespace Medical.AppLayer.Scripts { public class DefaultOmsVersion{ [Category("Основные настройки")][DisplayNameAttribute("Версия")]public int version { get; set; }
[BrowsableAttribute(false)][JsonIgnore] public object Dynamic { get { return this; } }
 }
 }
