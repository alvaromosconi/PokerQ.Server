using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Requests;
public class UserAuthenticationRequest
{
    [Required(ErrorMessage = "Username is required.")]
    public string? UserName { get; init; }
    [Required(ErrorMessage = "Password is required.")]
    public string? Password { get; init; }
}
