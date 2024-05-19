using Entities;
using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects.Requests;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Services.Contracts;
public interface IAuthenticationService
{
    Task<string> CreateToken();
    Task<User> GetUserByNameAsync(string name);
    Task<User> GetUserByIdAsync(string connectionId);
    Task<IdentityResult> RegisterUserAsync(UserRegistrationRequest request);
    Task<bool> Authenticate(UserAuthenticationRequest userAuthenticationRequest);
    Task<bool> AuthenticateWithGoogle(Payload payload);
}
