using AutoMapper;
using CrudUsuario.Business;
using CrudUsuario.Controllers;
using CrudUsuario.Mappers;
using CrudUsuario.Model;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CrudUsuario.Test.Controllers
{
    public class UserControllerTest : IDisposable
    {
        private Mock<IUserService> _userService;
        private IEncrypter _encrypter;
        private IMapper _mapper;

        public UserControllerTest()
        {
            _mapper = new Mapper(new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<ConfigureMapper>();
                }
                ));
            _encrypter = new Encrypter();
            _userService = new Mock<IUserService>();
        }
        public void Dispose()
        {
            _userService = null;
        }

        [Fact]
        public void UserController_GetUser_GUID()
        {
            var controller = GetController();
            var user = Builder<Entity.User>.CreateNew().Build();
            _userService.Setup(u => u.GetUser(It.IsAny<Guid>())).Returns(user);
            var guid = Guid.NewGuid();
            var response = controller.Get(guid);

            var result = response as OkObjectResult;

            var userResponse = result.Value as UserResponse;
            Assert.True(userResponse != null);
            Assert.Equal(user.Name, userResponse.Name);
            Assert.Equal(user.Email, userResponse.Email);
            Assert.Equal(user.Id, userResponse.Id);

        }


        [Fact]
        public async Task UserController_Delete()
        {
            var controller = GetController();
            var user = Builder<Entity.User>.CreateNew().Build();
            _userService.Setup(u => u.GetUser(It.IsAny<Guid>())).Returns(user);
            var guid = Guid.NewGuid();
            var response = await controller.Delete(guid);

            _userService.Verify(u => u.Remove(It.IsAny<Entity.User>()), Times.Once);

            var result = response as NoContentResult;


            Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);

        }

        [Fact]
        public async Task UserController_Delete_NotFound()
        {
            var controller = GetController();
            _userService.Setup(u => u.GetUser(It.IsAny<Guid>())).Returns((Entity.User)null);
            var guid = Guid.NewGuid();
            var response = await controller.Delete(guid);

            _userService.Verify(u => u.Remove(It.IsAny<Entity.User>()), Times.Never);

            var result = response as NotFoundResult;


            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);

        }

        [Fact]
        public void UserController_GetUser_GUID_NotFound()
        {
            var controller = GetController();
            _userService.Setup(u => u.GetUser(It.IsAny<Guid>())).Returns((Entity.User)null);
            var guid = Guid.NewGuid();
            var response = controller.Get(guid);

            var result = response as NotFoundResult;

            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);

        }

        [Fact]
        public async Task UserController_GetUser_All()
        {
            var controller = GetController();
            var users = Builder<Entity.User>.CreateListOfSize(4).Build();

            _userService.Setup(u => u.List()).ReturnsAsync(users);

            var response = (await controller.Get()).ToList();


            Assert.True(response != null);

            for (var i = 0; i < users.Count; i++)
            {
                Assert.Equal(users[i].Name, response[i].Name);
                Assert.Equal(users[i].Email, response[i].Email);
                Assert.Equal(users[i].Id, response[i].Id);
            }

        }

        [Fact]
        public async Task UserController_Post()
        {
            var controller = GetController();

            var response = await controller.Post(new UserModel
            {
                Email = "igorteste@teste.com",
                Name = "Igor Teste",
                Password = "teste"
            });

            var result = response as CreatedResult;

            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            _userService.Verify(u => u.Insert(It.IsAny<Entity.User>()), Times.Once);

        }

        [Fact]
        public async Task UserController_ResetPassword_Success()
        {
            var controller = GetController();
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "teste"),
                new Claim(ClaimTypes.NameIdentifier, "123123"),
                new Claim("name", "teste@teste.com")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new System.Security.Claims.ClaimsPrincipal(identity)
                }
            };
            _userService.Setup(u => u.GetUser(It.IsAny<string>())).ReturnsAsync(Builder<Entity.User>.CreateNew().Build());
            var response = await controller.ResetPassword(new ResetPasswordModel
            {
                Password = "teste",
                NewPassword = "teste2"
            });

            var result = response as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            _userService.Verify(u => u.ChangePassword(It.IsAny<Entity.User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);


        }

        private UserController GetController()
        {
            return new UserController(_userService.Object, _encrypter, _mapper);
        }
    }
}
