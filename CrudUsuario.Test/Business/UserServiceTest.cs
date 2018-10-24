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
            _ctx.Dispose();
            _ctx = null;
        }

        [Fact]
        public async void InsertUser_Success()
        {
            var u = new Entity.User()
            {
                Name = "Igor Teste",
                Email = "igorteste@teste.com.br",

            };
            u.SetPassword("teste", _encrypter);
            await _userService.Insert(u);
            Assert.NotEqual(Guid.Empty, u.Id);
        }

        [Fact]
        public void InsertUser_Error()
        {
            var u = new Entity.User()
            {
                Name = "Igor Teste",
                Email = "igorteste@teste.com.br",

            };
            var ex = Assert.Throws<BusinessException>(() => u.SetPassword("", _encrypter));
            Assert.Equal("password can not be empty", ex.Message);
        }

        [Fact]
        public async Task RemoveUser()
        {
            var u = await _userService.GetUser("igorteste3@teste.com");

            await _userService.Remove(u);

            var u2 = await _userService.GetUser("igorteste3@teste.com");

            
            Assert.Equal((Entity.User) null, u2);
        }

        [Fact]
        public async void GetUser_Email_Success()
        {
            var u = await _userService.GetUser("igorteste@teste.com");
            Assert.True(u != null);
            Assert.Equal("Igor Teste", u.Name);
        }

        [Fact]
        public void GetUser_Guid_Success()
        {
            var u = _userService.GetUser(_guidTest);
            Assert.True(u != null);
            Assert.Equal("Igor Teste", u.Name);
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

            Assert.Equal("Old password is invalid", ex.Message);
            Assert.Equal("error_password", ex.Code);
        }

        [Fact]
        public async void GetUser_Email_Password_Success()
        {
            var u = await _userService.GetUserPassword("igorteste@teste.com", "teste");
            Assert.True(u != null);
            Assert.Equal("Igor Teste", u.Name);
        }

        [Fact]
        public void GetUser_Email_Password_Error_Password()
        {
            var ex = Assert.ThrowsAsync<BusinessException>(async () => await _userService.GetUserPassword("igorteste@teste.com", "teste2"));

            Assert.True(ex != null);
            Assert.Equal("E-mail or password is incorrect", ex.Result.Message);
            Assert.Equal("invalid_credentials", ex.Result.Code);
        }

        [Fact]
        public void GetUser_Email_Password_Error_Email()
        {
            var ex = Assert.ThrowsAsync<BusinessException>(async () => await _userService.GetUserPassword("igorteste44@teste.com", "teste"));

            Assert.True(ex != null);
            Assert.Equal("E-mail or password is incorrect", ex.Result.Message);
            Assert.Equal("invalid_credentials", ex.Result.Code);
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
            b.UseInMemoryDatabase("DemoMemory" + new string[] { }.GetHashCode().ToString(), null);
            options = b.Options;
            return new ApplicationDbContext(options);
        }
    }
}