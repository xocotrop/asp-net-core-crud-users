using CrudUsuario.Business;
using CrudUsuario.Controllers;
using CrudUsuario.Model;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CrudUsuario.Test.Controllers
{
    public class LoginControllerTest : IDisposable
    {
        private Mock<IUserService> _userService;
        private TokenConfig _tokenConfig;
        private JwtConfiguration _jwtConfig;

        public LoginControllerTest()
        {
            _userService = new Mock<IUserService>();
        }
        public void Dispose()
        {
            _userService = null;
        }

        [Fact]
        public async Task LoginController_Success()
        {
            var controller = GetController();
            var user = Builder<Entity.User>.CreateNew().Build();
            _userService.Setup(u => u.GetUserPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);

            var response = await controller.Login(new Model.UserModel
            {
                Email = "igorteste@teste.com",
                Password = "teste"
            });
            var result = response.Result as OkObjectResult;

            Assert.IsType<ActionResult<AuthOk>>(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var authOk = result.Value as AuthOk;
            Assert.True(authOk != null);

        }

        [Fact]
        public async Task LoginController_Notfound()
        {
            var controller = GetController();
            
            _userService.Setup(u => u.GetUserPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((Entity.User)null);

            var response = await controller.Login(new Model.UserModel
            {
                Email = "igorteste@teste.com",
                Password = "teste"
            });
            var result = response.Result as OkObjectResult;

            Assert.IsType<ActionResult<AuthOk>>(response);
            Assert.IsType<NotFoundResult>(response.Result);


        }

        private LoginController GetController()
        {
            _tokenConfig = FizzWare.NBuilder.Builder<TokenConfig>.CreateNew().Build();
            _jwtConfig = FizzWare.NBuilder.Builder<JwtConfiguration>.CreateNew().Build();
            return new LoginController(_userService.Object, _tokenConfig, _jwtConfig);
        }
    }
}
