using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using Shared.DataTransferObjects.Requests;
using Shared.Extensions.Mappers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Services;
public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private User? _user;

    public AuthenticationService(UserManager<User> userManager,
                                 IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegistrationRequest request)
    {
        var user = request.ToEntity();

        var result = await _userManager.CreateAsync(user, request.Password);

        return result;
    }

    public async Task<bool> Authenticate(UserAuthenticationRequest userAuthenticationRequest)
    {
        _user = await _userManager.FindByNameAsync(userAuthenticationRequest.UserName);

        var result = (_user != null) && 
                     await _userManager.CheckPasswordAsync(_user, userAuthenticationRequest.Password);
        
        return result;
    }

    public async Task<bool> AuthenticateWithGoogle(Payload payload)
    {
        // Obtén el ID de Google del payload
        var googleId = payload.Subject;

        // Busca al usuario en la base de datos utilizando el ID de Google
        _user = await _userManager.Users
                                     .FirstOrDefaultAsync(u => u.GoogleId == googleId);

        // Si el usuario no existe, puedes optar por registrarlo
        if (_user == null)
        {
            _user = new User
            {
                GoogleId = googleId,
                Email = payload.Email,
                UserName = payload.Email, // Identity requiere un UserName
                                          // Aquí puedes agregar más campos si los necesitas
            };

            var result = await _userManager.CreateAsync(_user);
            if (!result.Succeeded)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<string> CreateToken()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _user.Id),
            new Claim(ClaimTypes.Name, _user.UserName)
        };

        var roles = await _userManager.GetRolesAsync(_user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken
        (
            issuer: jwtSettings.GetSection("validIssuer").Value,
            audience: jwtSettings.GetSection("validAudience").Value,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
         );

        return tokenOptions;
    }

    public async Task<User> GetUserByIdAsync(string userIdentifier)
    {
        return await _userManager.FindByIdAsync(userIdentifier);
    }

    public async Task<User> GetUserByNameAsync(string name)
    {
        return await _userManager.FindByNameAsync(name);
    }
}
