using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Requests;
public class GoogleUserAuthenticationRequest
{
    [Required(ErrorMessage = "Credential is required")]
    public string Credential { get; init; }
}
