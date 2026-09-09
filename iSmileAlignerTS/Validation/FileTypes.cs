using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace iSmileAlignerTS.Validation
{
    public class FileTypesAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly List<string> _types;

        public FileTypesAttribute(string types)
        {
            _types = types.Split(',').ToList();
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            var fileExt = System.IO.Path.GetExtension((value as HttpPostedFileBase).FileName).Substring(1);
            if (_types.Contains("*", StringComparer.OrdinalIgnoreCase)) return true;
            return _types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(String.Join(", ", _types));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(String.Join(", ", _types)),
                ValidationType = "filetypes"
            };
            rule.ValidationParameters["types"] = String.Join(", ", _types);
            yield return rule;
        }
    }
}