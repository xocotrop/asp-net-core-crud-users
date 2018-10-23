using CrudUsuario.Business;
using CrudUsuario.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CrudUsuario.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly TokenConfig _tokenConfigurations;
        private readonly JwtConfiguration _jwtConfiguration;

        public LoginController(IUserService userService, [FromServices] TokenConfig tokenConfigurations, [FromServices] JwtConfiguration jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration;
            _userService = userService;
            _tokenConfigurations = tokenConfigurations;
        }

        [HttpPost("")]
        public async Task< ActionResult<AuthOk>> Login([FromBody]UserModel userModel)
        {
            var user = await _userService.GetUserPassword(userModel.Email, userModel.Password);

            if(user != null)
            {
                var identity = new ClaimsIdentity(new GenericIdentity(user.Email, "Login"),
                    new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString("N"))
                    }
                    );
                var created = DateTime.Now;
                var expires = created + TimeSpan.FromSeconds(_tokenConfigurations.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
                {
                    Issuer = _tokenConfigurations.Issuer,
                    Audience = _tokenConfigurations.Audience,
                    SigningCredentials = _jwtConfiguration.Credentials,
                    Subject = identity,
                    NotBefore = created,
                    Expires = expires
                });

                var token = handler.WriteToken(securityToken);
                return Ok(new AuthOk(true, created, expires, token, "OK"));
            }

            return NotFound();
        }
    }
}
