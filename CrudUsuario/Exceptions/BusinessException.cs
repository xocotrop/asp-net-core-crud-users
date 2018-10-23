using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudUsuario.Exceptions
{
    public class BusinessException : Exception
    {
        public string Code { get; }

        public BusinessException(string message, params object[] args) : this(string.Empty, message, args)
        {

        }

        public BusinessException(string code, string message, params object[] args) : this(null, code, message, args)
        {

        }

        public BusinessException(Exception innerException, string code, string message, params object[] args) : base(string.Format(message, args), innerException)
        {
            Code = code;
        }

        public BusinessException(string code)
        {

        }
    }
}
