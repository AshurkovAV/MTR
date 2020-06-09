using System;
using System.Diagnostics.CodeAnalysis;

namespace Core.Attributes
{
    /// <summary>
    /// Приоритет загрузки/инициализации модуля
    /// </summary>
    public enum ModulePriority
    {
        Ignore = 0,
        Low = 1,
        Normal = 2,
        High = 3,
        Citical = 4
    }

    /// <summary>
    /// Атрибут для работы модульной структуры приложения
    /// </summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// Название модуля
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Приоритет загрузки/инициализации
        /// </summary>
        public ModulePriority Priority { get; set; }
        /// <summary>
        /// Тип класса, к которому относится атрибут
        /// </summary>
        public Type ClassType { get; set; }
        /// <summary>
        /// Порядок загрузки/инициализации
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Зависимости модуля
        /// </summary>
        public string[] Dependencies { get; set; }
    }

}
