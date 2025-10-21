using System.ComponentModel.DataAnnotations;

namespace Shopping_Web.Repository.ValidationImage
{
    public class FileExtensionsAttibute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if( value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                String[] extensions = { "png", "jpg", "jpeg" };
                
                bool result = extensions.Any(x => extension.EndsWith(x));
                if(!result)
                {
                    return new ValidationResult("Allowed extensions are png , jpg or jpeg");
                }
            }
            return ValidationResult.Success;
        }
    }
}
