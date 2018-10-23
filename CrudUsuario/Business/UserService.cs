using CrudUsuario.Data;
using CrudUsuario.Entity;
using CrudUsuario.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudUsuario.Business
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncrypter _encrypter;

        public UserService(ApplicationDbContext context, IEncrypter encrypter)
        {
            _encrypter = encrypter;
            _context = context;
        }

        public User GetUser(Guid id) => _context.Users.FirstOrDefault(u => u.Id == id);
        public async Task<User> GetUser(string email) => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        public async Task<User> GetUserPassword(string email, string password)
        {
            var user = await GetUser(email);
            if (user == null)
                throw new BusinessException("invalid_credentials", "E-mail or password is incorrect");

            if (!user.ValidatePassword(password, _encrypter))
                throw new BusinessException("invalid_credentials", "E-mail or password is incorrect");

            return user;
        }

        public async Task ChangePassword(User user, string password, string newPassword)
        {
            if (!user.ValidatePassword(password, _encrypter))
            {
                throw new BusinessException("error_password", "Old password is invalid");
            }

            user.SetPassword(newPassword, _encrypter);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Insert(User user)
        {
            var userBd = await GetUser(user.Email);
            if(userBd != null)
            {
                throw new BusinessException("email_in_use", $"{user.Email} is already in use");
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> List() => await _context.Users.ToListAsync();
    }
}
