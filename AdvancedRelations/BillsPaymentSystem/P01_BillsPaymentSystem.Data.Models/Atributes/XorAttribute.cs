using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace P01_BillsPaymentSystem.Data.Models.Atributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class XorAttribute : ValidationAttribute
    {
        private string xorTargetAttribute;

        public XorAttribute(string xorTargetAttribute)
        {
            this.xorTargetAttribute = xorTargetAttribute;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var targetAttribute = validationContext.ObjectType
                .GetProperty(xorTargetAttribute)
                .GetValue(validationContext.ObjectInstance);

            if ((targetAttribute == null && value != null) || (targetAttribute != null && value == null))
            {
                return ValidationResult.Success;
            }

            string errorMsg = "Not valid!";

            return new ValidationResult(errorMsg);
        }
    }
}
