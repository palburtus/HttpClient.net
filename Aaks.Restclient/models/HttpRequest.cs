using Aaks.Restclient.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaks.Restclient.Models
{
    public class HttpRequest
    {
        public string Url { get; set; }

        public Dictionary<string, string> Headers { get; set; }
        public ResponseType ResponseType { get; set; } = ResponseType.JSON;
        public AuthorizationTypes AuthType { get; set; } = AuthorizationTypes.NONE;
        public HttpAuthBasic BasicAuthCredentials { get; set; }
    }

    public class HttpAuthBasic
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
