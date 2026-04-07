using System.ComponentModel.DataAnnotations;

namespace Dima.Core.Requests.Account;

public class LoginRequest : BaseRequest
{
    [Required(ErrorMessage = "Informe o email")]
    [EmailAddress(ErrorMessage = "Informe o e-mail")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Informe a senha")]
    public string Password { get; set; } = string.Empty;
}