using AutoMapper;
using CrudUsuario.Business;
using CrudUsuario.Entity;
using CrudUsuario.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudUsuario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IEncrypter _encrypter;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IEncrypter encrypter, IMapper mapper)
        {
            _mapper = mapper;
            _encrypter = encrypter;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<UserResponse>> Get()
        {
            return _mapper.Map<IEnumerable<UserResponse>>(await _userService.List());
        }


        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var user = _userService.GetUser(id);
            if (user != null)
                return Ok(_mapper.Map<UserResponse>(user));

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }
            await _userService.Remove(user);
            return NoContent();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] UserModel userModel)
        {
            var user = new User
            {
                Email = userModel.Email,
                Name = userModel.Name
            };

            user.SetPassword(userModel.Password, _encrypter);

            await _userService.Insert(user);
            return new CreatedResult($"api/user/{user.Id}", _mapper.Map<UserResponse>(user));

        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel resetPassword)
        {

            var userLogged = ControllerContext.HttpContext.User.Identity.Name;

            var user = await _userService.GetUser(userLogged);

            if (user == null)
                return NotFound();

            await _userService.ChangePassword(user, resetPassword.Password, resetPassword.NewPassword);

            return Ok(new
            {
                message = "Password changed successfully"
            });
        }

    }
}
