using Aaks.Restclient.Oauth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaks.RestclientTests.Oauth
{
    [TestClass]
    public class OauthAuthenticationManagerTests
    {
        private const string BaseUrl = "https://open-ic.epic.com/Argonaut/api/FHIR/Argonaut/metadata";
        private const string ClientId = "230ebfda-20dc-438a-9935-b85d42ddeb11";
        private const string CallbackUrl = "https://alburt.us/epicredirect";

        [TestMethod]
        public void FetchAccessToken_GetsOauthResponseForAccessTokenByUrl_ShouldSucceed()
        {
            var manager = new OauthAuthenticationManager();
            manager.FetchAccessToken(BaseUrl);
            //manager.FetchAccessToken(
              //  String.Format("https://open-ic.epic.com/argonaut/oauth2/authorize?response_type=code&client_id={0}&redirect_uri={1}",
                //ClientId, CallbackUrl));
        }
    }
}
