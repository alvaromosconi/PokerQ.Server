using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Requests;

public record UserRegistrationRequest
{
    public string? Name { get; init; }
    [Required(ErrorMessage = "Username is required.")]
    public string? UserName { get; init; }
    [Required(ErrorMessage = "Password is required.")]
    public string? Password { get; init; }
    [Required(ErrorMessage = "Email is required.")]
    public string? Email { get; init; }
}