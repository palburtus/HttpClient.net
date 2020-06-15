using Aaks.Restclient.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaks.Restclient.Models
{
    public class HttpPostRequest<T> : HttpRequest
    {
        public RequestType RequestType { get; set; } = RequestType.JSON;
        public T Body { get; set; }
    }
}
