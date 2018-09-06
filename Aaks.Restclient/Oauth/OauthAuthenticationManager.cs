using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaks.Restclient.Oauth
{
    public class OauthAuthenticationManager
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public void FetchAccessToken(string url)
        {
            var client = new HttpRestClient();
            var response = client.Get<string>(url);

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
        }

        public void FetchRefreshToken()
        {

        }
    }
}
