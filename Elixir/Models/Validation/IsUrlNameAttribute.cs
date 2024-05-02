using System.ComponentModel.DataAnnotations;

namespace Elixir.Models.Validation
{
    public class IsUrlNameAttribute : ValidationAttribute
    {
        public bool PublisherUrlCheck { get; set; }

        public IsUrlNameAttribute() : base() { }
        
        public IsUrlNameAttribute(string errorMessage, bool publisherUrlCheck = false)
        {
            ErrorMessage = errorMessage;
            PublisherUrlCheck = publisherUrlCheck;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var url = value as string;
            if (url == null)
                return null;
            
            if(PublisherUrlCheck)
            {
                url = url.Trim();
                if (url.Contains(".") && !url.Contains(" "))
                    return ValidationResult.Success;
                else
                    return new ValidationResult(ErrorMessage);
            }
            else
            {
                foreach (var c in url)
                {
                    if (!(char.IsLetter(c) && char.IsLower(c) || char.IsDigit(c) || c.Equals('(') || c.Equals(')') || c.Equals('-')))
                        return new ValidationResult($"Symbol '{c}' is not valid. URL name should be lowercase and contain only letters, digits, parenthesis and dashes.");
                }

                return ValidationResult.Success;
            }
        }
    }
}