using System.ComponentModel.DataAnnotations;

public class AllowedEmailDomainAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var email = value as string;

        if (string.IsNullOrEmpty(email))
        {
            return ValidationResult.Success;
        }

        var configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
        var allowedDomains = configuration.GetSection("AppSettings:AllowedEmailDomains").Get<string[]>();

        // Se não houver domínios especificados, permite todos os domínios de email.
        if (allowedDomains == null || allowedDomains.Length == 0)
        {
            return ValidationResult.Success;
        }

        // Verifica se o domínio do email está na lista de domínios permitidos.
        var domain = email.Split('@')[1];

        if (!allowedDomains.Contains(domain))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}