using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudUsuario.Model
{
    public class AuthOk
    {
        public bool Authenticated { get; private set; }
        public string Created { get; private set; }
        public string Expiration { get; private set; }
        public string AccessToken { get; private set; }
        public string Message { get; private set; }

        public AuthOk(bool auth, DateTime created, DateTime expires, string token, string message)
        {
            Authenticated = auth;
            Created = created.ToString("yyyy-MM-dd HH:mm:ss");
            Expiration = expires.ToString("yyyy-MM-dd HH:mm:ss");
            AccessToken = token;
            Message = message;
        }
    }


}
