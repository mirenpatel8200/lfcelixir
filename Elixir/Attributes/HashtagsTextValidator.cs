using System.ComponentModel.DataAnnotations;
using System;
using System.Text.RegularExpressions;

namespace Elixir.Attributes
{
    public class HashtagsTextValidator : ValidationAttribute
    {
        public HashtagsTextValidator(string errorMessage) : base(errorMessage) {}

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string fieldValue = (string)value;
            //Field is not required
            if (fieldValue == null) return ValidationResult.Success;
            string[] tokens = fieldValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            bool anyInvalidItem = false;

            //Regex took from here: https://stackoverflow.com/questions/8133669/c-sharp-regex-to-allow-only-alpha-numeric
            //Hashtags - any combination of letters (uppercase or lowercase) and digits, but at least one character
            Regex alphanumericRegex = new Regex(@"^[a-zA-Z0-9]+$");
            foreach (string token in tokens)
            {
                string strippedText = token.Substring(1);
                if (!token.StartsWith("#") || !alphanumericRegex.IsMatch(strippedText))
                {
                    anyInvalidItem = true;
                    break;
                }
            }
            if (!anyInvalidItem)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }
    }
}