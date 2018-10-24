using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudUsuario.Exceptions
{
    public class BusinessException : Exception
    {
        public string Code { get; }

       

        public BusinessException(string code, string message, params object[] args) : this(null, code, message, args)
        {

        }

        public BusinessException(Exception innerException, string code, string message, params object[] args) : base(string.Format(message, args), innerException)
        {
            Code = code;
        }

        
    }
}
