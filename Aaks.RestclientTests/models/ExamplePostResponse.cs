using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaks.RestclientTests.models
{
    
    public class Data
    {
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class Warning
    {
        public string id { get; set; }
        public string message { get; set; }
        public string url { get; set; }
    }

    public class ExamplePostResponse
    {
        public Data data { get; set; }
        public List<Warning> warnings { get; set; }
    }
}
