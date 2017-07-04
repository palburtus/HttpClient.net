using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aaks.Restclient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaks.RestclientTests.models;

namespace Aaks.Restclient.Tests
{
    [TestClass()]
    public class HttpRestClientTests
    {
        [TestMethod()]
        public void Get_makesNonAuthenicatedApiCallWithNoHeaders_returnsString()
        {
            HttpRestClient client = new HttpRestClient();
            var result = client.Get<string>("https://api.bitfinex.com/v2/ticker/tETHUSD");
            Assert.IsTrue(result.Body.StartsWith("["));
        }

        [TestMethod()]
        public void Get_makesNonAuthenicatedApiCallWithNoHeaders_returnsHttpResponse()
        {
            HttpRestClient client = new HttpRestClient();
            var result = client.Get<ExamplePostResponse>("https://api.coinbase.com/v2/prices/spot?currency=USD");
            Assert.AreEqual("USD", result.Body.data.currency);
            Assert.AreEqual("missing_version", result.Body.warnings[0].id);
            Assert.AreEqual("Please supply API version (YYYY-MM-DD) as CB-VERSION header", result.Body.warnings[0].message);
        }

        [TestMethod()]
        public async void Get_makeAsyncsNonAuthenicatedApiCallWitOuthHeaders_returnsTask()
        {
            HttpRestClient client = new HttpRestClient();
            var result = await client.GetAsync<ExamplePostResponse>("https://api.coinbase.com/v2/prices/spot?currency=USD");
            Assert.AreEqual("USD", result.Body.data.currency);
            Assert.AreEqual("missing_version", result.Body.warnings[0].id);
            Assert.AreEqual("Please supply API version (YYYY-MM-DD) as CB-VERSION header", result.Body.warnings[0].message);
        }

        [TestMethod()]
        public async void Get_makeAsyncsNonAuthenicatedApiCallWithHeaders_returnsTask()
        {
            HttpRestClient client = new HttpRestClient();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("CB-VERSION", "2016-08-10");
            var result = await client.GetAsync<ExamplePostResponse>("https://api.coinbase.com/v2/prices/spot?currency=USD", headers);
            Assert.AreEqual("USD", result.Body.data.currency);
            Assert.AreEqual("missing_version", result.Body.warnings[0].id);
            Assert.AreEqual("Please supply API version (YYYY-MM-DD) as CB-VERSION header", result.Body.warnings[0].message);
        }

        [TestMethod()]
        public void Get_makesNonAuthenicatedApiCallWithHeaderSet_returnsHttpResponse()
        {
            HttpRestClient client = new HttpRestClient();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("CB-VERSION", "2016-08-10");
            var result = client.Get<ExamplePostResponse>("https://api.coinbase.com/v2/prices/ETH-USD/buy", headers);
            Assert.AreEqual("USD", result.Body.data.currency);
            Assert.AreEqual("missing_version", result.Body.warnings[0].id);
            Assert.AreEqual("Please supply API version (YYYY-MM-DD) as CB-VERSION header", result.Body.warnings[0].message);
        }
    }
}