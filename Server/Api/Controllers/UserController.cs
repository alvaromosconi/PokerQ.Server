using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.DataTransferObjects.Resources;
using Shared.Extensions.Mappers;

namespace Server.Api.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public UserController(IAuthenticationService authenticationService)
        => _authenticationService = authenticationService;

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUser()
    {
        var user = await _authenticationService.GetUserByNameAsync(User.Identity.Name);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user.ToDTO());
    }
}
