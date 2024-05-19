using Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.DataTransferObjects.Requests;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Server.Api.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : Controller
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
        => _authenticationService = authenticationService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
    {
        var result = await _authenticationService.RegisterUserAsync(request);
        bool succeeded = result.Succeeded;

        if (succeeded)
        {
            result.Errors
                    .ToList()
                    .ForEach(error => ModelState
                                        .AddModelError(error.Code,
                                                       error.Description));
        }
        
        return succeeded ? StatusCode(201) 
                         : BadRequest(ModelState);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserAuthenticationRequest request)
    {
        var result = await _authenticationService.Authenticate(request);

        return result == false ? Unauthorized() 
                               : Ok( new { Token = await _authenticationService.CreateToken() });
    }

    [AllowAnonymous]
    [HttpPost("google-response")]
    public async Task<IActionResult> GoogleResponse([FromBody] GoogleUserAuthenticationRequest request)
    {
        Payload payload= await ValidateAsync(request.Credential, new ValidationSettings()).ConfigureAwait(false);

        var result = await _authenticationService.AuthenticateWithGoogle(payload);
  
        return result == false ? Unauthorized() 
                               : Ok(new { Token = await _authenticationService.CreateToken() });
    }

}