using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrudUsuario.Model
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Password can not be empty")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
        [Required(ErrorMessage = "NewPassword can not be empty")]
        [MinLength(6, ErrorMessage = "NewPassword must be at least 6 characters")]
        public string NewPassword { get; set; }
    }
}
