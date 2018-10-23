using Xunit;
using CrudUsuario.Business;
using Moq;
using CrudUsuario.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using CrudUsuario.Exceptions;

namespace CrudUsuario.Test.Business
{
    public class UserServiceTest : IDisposable
    {
        private UserService _userService;
        private Encrypter _encrypter;
        private ApplicationDbContext _ctx;

        private Guid _guidTest;

        public UserServiceTest()
        {
            _ctx = GetInMemoryDB();
            _encrypter = new Encrypter();
            _userService = new UserService(_ctx, _encrypter);
            Task.Run(async () =>
            {
                await PopulateDB();
            }).Wait();
        }

        public void Dispose()
        {
            _userService = null;
            _encrypter = null;
            _ctx = null;
        }

        [Fact]
        public async void InsertUser_Success()
        {
            var u = new Entity.User()
            {
                Name = "Igor Teste",
                Email = "igorteste@teste.com",

            };
            u.SetPassword("teste", _encrypter);
            await _userService.Insert(u);
            Assert.NotEqual(Guid.Empty, u.Id);
        }

        [Fact]
        public async void GetUser_Email_Success()
        {
            var u = await _userService.GetUser("igorteste@teste.com");
            Assert.True(u != null);
            Assert.Equal(u.Name, "Igor Teste");
        }

        [Fact]
        public void GetUser_Guid_Success()
        {
            var u = _userService.GetUser(_guidTest);
            Assert.True(u != null);
            Assert.Equal(u.Name, "Igor Teste");
        }

        [Fact]
        public async void Change_Password()
        {
            var u = new Entity.User()
            {
                Name = "Igor Teste2",
                Email = "igorteste2@teste.com",

            };
            u.SetPassword("teste", _encrypter);

            await _userService.ChangePassword(u, "teste", "teste2");

            Assert.True(u.ValidatePassword("teste2", _encrypter));
        }

        [Fact]
        public async void Change_Password_error()
        {
            var u = new Entity.User()
            {
                Name = "Igor Teste2",
                Email = "igorteste2@teste.com",

            };
            u.SetPassword("teste", _encrypter);

            var ex = await Assert.ThrowsAsync<BusinessException>(async () => await _userService.ChangePassword(u, "teste2", "teste3"));

            Assert.Equal(ex.Message, "Old password is invalid");
            Assert.Equal(ex.Code, "error_password");
        }

        [Fact]
        public async void GetUser_Email_Password_Success()
        {
            var u = await _userService.GetUserPassword("igorteste@teste.com", "teste");
            Assert.True(u != null);
            Assert.Equal(u.Name, "Igor Teste");
        }

        [Fact]
        public void GetUser_Email_Password_Error_Password()
        {
            var ex = Assert.ThrowsAsync<BusinessException>(async () => await _userService.GetUserPassword("igorteste@teste.com", "teste2"));

            Assert.True(ex != null);
            Assert.Equal(ex.Result.Message, "E-mail or password is incorrect");
            Assert.Equal(ex.Result.Code, "invalid_credentials");
        }

        [Fact]
        public void GetUser_Email_Password_Error_Email()
        {
            var ex = Assert.ThrowsAsync<BusinessException>(async () => await _userService.GetUserPassword("igorteste44@teste.com", "teste"));

            Assert.True(ex != null);
            Assert.Equal(ex.Result.Message, "E-mail or password is incorrect");
            Assert.Equal(ex.Result.Code, "invalid_credentials");
        }

        private async Task PopulateDB()
        {
            var u1 = new Entity.User()
            {
                Name = "Igor Teste",
                Email = "igorteste@teste.com",

            };
            u1.SetPassword("teste", _encrypter);

            var u2 = new Entity.User()
            {
                Name = "Igor Teste2",
                Email = "igorteste2@teste.com",

            };
            u2.SetPassword("teste", _encrypter);

            var u3 = new Entity.User()
            {
                Name = "Igor Teste3",
                Email = "igorteste3@teste.com",

            };
            u3.SetPassword("teste", _encrypter);
            await _userService.Insert(u1);
            _guidTest = u1.Id;
            await _userService.Insert(u2);
            await _userService.Insert(u3);
        }

        private ApplicationDbContext GetInMemoryDB()
        {
            DbContextOptions<ApplicationDbContext> options;
            var b = new DbContextOptionsBuilder<ApplicationDbContext>();
            b.UseInMemoryDatabase("DemoMemory", null);
            options = b.Options;
            return new ApplicationDbContext(options);
        }
    }
}