using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaks.Restclient.Models
{
    public sealed class NoBodyType
    {
        private NoBodyType() { }
        private readonly static NoBodyType noBodyType = new NoBodyType();
        public static NoBodyType Body { get { return noBodyType; } }
        public string Info = "Response has no body";
    }
}
