using CrudUsuario.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudUsuario.Business
{
    public interface IUserService
    {
        Task<User> GetUser(string email);
        User GetUser(Guid id);
        Task<User> GetUserPassword(string email, string password);
        Task ChangePassword(User user, string password, string newPassword);
        Task Insert(User user);
        Task<IEnumerable<User>> List();
        Task Remove(User user);
    }
}