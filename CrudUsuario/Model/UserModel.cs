using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrudUsuario.Model
{
    public class UserModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name can not be empty")]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email can not be empty")]
        [MaxLength(60)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password can not be empty")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }
}
