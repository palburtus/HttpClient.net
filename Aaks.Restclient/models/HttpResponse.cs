using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aaks.RestclientTests.models
{
    public class HttpResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T Body { get; set; }
        public string ErrorMessage { get; set; }
    }
}
