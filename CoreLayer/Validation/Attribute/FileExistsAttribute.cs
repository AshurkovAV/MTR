using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Medical.CoreLayer.Validation.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class FileExistsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return File.Exists(value as string);
        }
    }
}
