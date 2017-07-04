using Aaks.RestclientTests.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Aaks.Restclient
{
    public class HttpRestClient
    {
        public T Post<T, K>(string url, K body)
        {
            return (T)Convert.ChangeType(1, typeof(T));
        }

        public HttpResponse<T> Get<T>(string url)
        {
            return Get<T>(url, null);
        }

        public async Task<HttpResponse<T>> GetAsync<T>(string url)
        {
            return await GetAsync<T>(url, null);
        }

        public HttpResponse<T> Get<T>(string url, Dictionary<string, string> headers)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                if (headers != null)
                {
                    foreach(string key in headers.Keys)
                    {
                        httpWebRequest.Headers.Add(key, headers[key]);
                    }
                }
               
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {

                    StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    string stream = streamReader.ReadToEnd();
                    HttpResponse<T> response = new HttpResponse<T>();
                    Type type = typeof(T);
                    if (type != typeof(string))
                    {
                        response.Body = Deserialize<T>(stream);
                    }
                    else
                    {
                        response.Body = (T)Convert.ChangeType(stream, typeof(T));
                    }
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
            }
            catch (WebException e)
            {
                using (WebResponse webResponse = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)webResponse;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);

                    using (Stream data = webResponse.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        HttpResponse<T> response = new HttpResponse<T>();
                        response.ErrorMessage = reader.ReadToEnd();
                        response.StatusCode = httpResponse.StatusCode;
                        return response;

                    }
                }
            }
        }

        public async Task<HttpResponse<T>> GetAsync<T>(string url, Dictionary<string, string> headers)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        httpWebRequest.Headers.Add(key, headers[key]);
                    }
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                    string stream = await streamReader.ReadToEndAsync();
                    Type type = typeof(T);
                    HttpResponse<T> response = new HttpResponse<T>();
                    if (type != typeof(string))
                    {
                        response.Body = Deserialize<T>(stream);
                    }
                    else
                    {
                        response.Body = (T)Convert.ChangeType(stream, typeof(T));
                    }
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
            }
            catch (WebException e)
            {
                using (WebResponse webResponse = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)webResponse;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);

                    using (Stream data = webResponse.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        HttpResponse<T> response = new HttpResponse<T>();
                        response.ErrorMessage = reader.ReadToEnd();
                        response.StatusCode = httpResponse.StatusCode;
                        return response;

                    }
                }
            }
        }

        private static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            return retVal;
        }

        private static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}
