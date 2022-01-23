using Aaks.Restclient.Enums;
using Aaks.Restclient.Models;
using Aaks.RestclientTests.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Aaks.Restclient
{
    public class HttpRestClient : IHttpRestClient
    {
        private string IpAddress;

        public void SetIpAddress(string ipAddress)
        {
            IpAddress = ipAddress;
        }

  
        public HttpResponse<T> Post<T, K>(string url, K body, Dictionary<string, string> headers = null, string contentType = null)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.ContentType = contentType ?? "application/json";

                httpWebRequest.Method = "POST";

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        httpWebRequest.Headers.Add(key, headers[key]);
                    }
                }

                var serializedResult = SerializeJson<K>(body);
                byte[] requestBody = Encoding.UTF8.GetBytes(serializedResult);

                using (var postStream = httpWebRequest.GetRequestStream())
                {
                    postStream.Write(requestBody, 0, requestBody.Length);
                }

                var response = (HttpWebResponse) httpWebRequest.GetResponse();

                string charSet = response.CharacterSet;
                Encoding encoding = Encoding.GetEncoding(charSet);
                
                var reader = new StreamReader(response.GetResponseStream(), encoding);

                string stream = reader.ReadToEnd();
                HttpResponse<T> httpResposne = new HttpResponse<T>();
                Type type = typeof(T);

                if (type != typeof(string))
                {
                    httpResposne.Body = DeserializeJson<T>(stream);
                }
                else
                {
                    httpResposne.Body = (T)Convert.ChangeType(stream, typeof(T));
                }

                httpResposne.StatusCode = HttpStatusCode.OK;
                return httpResposne;


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

        public async Task<HttpResponse<T>> PostAsync<T, K>(HttpPostRequest<K> request)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(request.Url);
                switch (request.RequestType)
                {
                    case RequestType.XML:
                        httpWebRequest.ContentType = "application/xml; charset=utf-8";
                        break;
                    case RequestType.APPLICATION:
                        httpWebRequest.ContentType = "application /x-www-form-urlencoded";
                        break;
                    default:
                        httpWebRequest.ContentType = "application/json";
                        break;
                }

                httpWebRequest.Method = "POST";
                
                if (request.Headers != null)
                {
                    foreach (string key in request.Headers.Keys)
                    {
                        httpWebRequest.Headers.Add(key, request.Headers[key]);
                    }
                }

                if (request.AuthType == AuthorizationTypes.BASIC)
                {
                    string encodedBasicCredentials =
                        Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(request.BasicAuthCredentials.UserName + ":" + request.BasicAuthCredentials.Password));

                    httpWebRequest.Headers.Add("Authorization", "Basic " + encodedBasicCredentials);
                }

                Type requestType = typeof(K);
                if (requestType != typeof(string))
                {
                    var serializedResult = request.RequestType == RequestType.XML ?
                        SerializeXml<K>(request.Body) : SerializeJson<K>(request.Body);

                    byte[] requestBody = Encoding.UTF8.GetBytes(serializedResult);

                    using (var postStream = await httpWebRequest.GetRequestStreamAsync())
                    {
                        await postStream.WriteAsync(requestBody, 0, requestBody.Length);
                    }
                }
                else
                {
                    byte[] requestBytes = new ASCIIEncoding().GetBytes(request.Body.ToString());
                    using (Stream requestStream = httpWebRequest.GetRequestStream())
                    {
                        await requestStream.WriteAsync(requestBytes, 0, requestBytes.Length);
                    }
                }

                var response = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                                
                Encoding encoding = null;

                if (request.AuthType != AuthorizationTypes.BASIC)
                {
                    string charSet = response.CharacterSet;
                    encoding = Encoding.GetEncoding(charSet);
                }

                using (var reader = encoding != null ? new StreamReader(response.GetResponseStream(), encoding) : new StreamReader(response.GetResponseStream()))
                {
                    string stream = await reader.ReadToEndAsync();
                    HttpResponse<T> httpResponse = new HttpResponse<T>();
                    Type type = typeof(T);

                    if (type != typeof(string))
                    {
                        if (request.ResponseType == ResponseType.XML)
                        {
                            httpResponse.Body = DeserializeXml<T>(stream);
                        }
                        else
                        {
                             httpResponse.Body = DeserializeJson<T>(stream);
                        }

                    }
                    else
                    {
                        httpResponse.Body = (T)Convert.ChangeType(stream, typeof(T));
                    }

                    httpResponse.StatusCode = HttpStatusCode.OK;

                    return httpResponse;
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

        public async Task<HttpResponse<T>> PostAsync<T,K>(string url, K body, Dictionary<string, string> headers = null)
        {
            var request = new HttpPostRequest<K>();
            request.Url = url;
            request.Body = body;
            request.Headers = headers;

            return await PostAsync<T, K>(request);
        }

        public HttpResponse<T> Get<T>(string url, Dictionary<string, string> headers = null)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";

                if (IpAddress != null)
                {

                }

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        httpWebRequest.Headers.Add(key, headers[key]);
                    }
                }

                HttpWebResponse httpWebResponse =
                    (HttpWebResponse)httpWebRequest.GetResponse();

                string charSet = httpWebResponse.CharacterSet;
                Encoding encoding = Encoding.GetEncoding(charSet);

                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {

                    StreamReader streamReader = new StreamReader(responseStream, encoding);

                    string stream = streamReader.ReadToEnd();
                    HttpResponse<T> response = new HttpResponse<T>();
                    Type type = typeof(T);
                    if (type != typeof(string))
                    {
                        response.Body = DeserializeJson<T>(stream);
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
        public async Task<HttpResponse<T>> GetAsync<T>(string url, Dictionary<string, string> headers = null)
        {
            return await GetAsync<T>(new HttpRequest { Url = url, Headers = headers });
        }

        public async Task<HttpResponse<T>> GetAsync<T>(HttpRequest request)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(request.Url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";

                if (request.Headers != null)
                {
                    foreach (string key in request.Headers.Keys)
                    {
                        httpWebRequest.Headers.Add(key, request.Headers[key]);
                    }
                }

                if (request.AuthType == AuthorizationTypes.BASIC)
                {
                    string encodedBasicCredentials =
                        Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(request.BasicAuthCredentials.UserName + ":" + request.BasicAuthCredentials.Password));

                    httpWebRequest.Headers.Add("Authorization", "Basic " + encodedBasicCredentials);
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                Encoding encoding = null;

                if (request.AuthType != AuthorizationTypes.BASIC)
                {
                    string charSet = httpWebResponse.CharacterSet;
                    encoding = Encoding.GetEncoding(charSet);
                }

                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    using (StreamReader streamReader = encoding != null ? new StreamReader(responseStream, encoding) : new StreamReader(responseStream))
                    {
                        string stream = await streamReader.ReadToEndAsync();
                        Type type = typeof(T);

                        if (type == typeof(string[]))
                        {
                            throw new InvalidCastException("Cannot deserialize array of T use List<T> instead");
                        }

                        HttpResponse<T> response = new HttpResponse<T>();

                        if (request.ResponseType == ResponseType.XML)
                        {
                            response.Body = DeserializeXml<T>(stream);
                        }
                        else
                        {
                            response.Body = type != typeof(string) ?
                                DeserializeJson<T>(stream) : (T)Convert.ChangeType(stream, typeof(T));
                        }

                        response.StatusCode = HttpStatusCode.OK;

                        return response;
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse webResponse = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)webResponse;

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

        public HttpResponse<T> Delete<T>(string url, Dictionary<string, string> headers = null)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "DELETE";

                if (IpAddress != null)
                {

                }

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        httpWebRequest.Headers.Add(key, headers[key]);
                    }
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                string charSet = httpWebResponse.CharacterSet;
                Encoding encoding = Encoding.GetEncoding(charSet);


                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {

                    StreamReader streamReader = new StreamReader(responseStream, encoding);

                    string stream = streamReader.ReadToEnd();
                    HttpResponse<T> response = new HttpResponse<T>();
                    Type type = typeof(T);
                    if (type != typeof(string))
                    {
                        response.Body = DeserializeJson<T>(stream);
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

        public async Task<HttpResponse<T>> DeleteAsync<T>(HttpDeleteRequest request)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(request.Url);
                switch (request.RequestType)
                {
                    case RequestType.XML:
                        httpWebRequest.ContentType = "application/xml; charset=utf-8";
                        break;
                    case RequestType.APPLICATION:
                        httpWebRequest.ContentType = "application /x-www-form-urlencoded";
                        break;
                    default:
                        httpWebRequest.ContentType = "application/json";
                        break;
                }
                httpWebRequest.Method = "DELETE";

                if (IpAddress != null)
                {

                }

                if (request.Headers != null)
                {
                    foreach (string key in request.Headers.Keys)
                    {
                        httpWebRequest.Headers.Add(key, request.Headers[key]);
                    }
                }

                if (request.AuthType == AuthorizationTypes.BASIC)
                {
                    string encodedBasicCredentials =
                        Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(request.BasicAuthCredentials.UserName + ":" + request.BasicAuthCredentials.Password));

                    httpWebRequest.Headers.Add("Authorization", "Basic " + encodedBasicCredentials);
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
               
                Encoding encoding = null;

                if (request.AuthType != AuthorizationTypes.BASIC)
                {
                    string charSet = httpWebResponse.CharacterSet;
                    encoding = Encoding.GetEncoding(charSet);
                }

                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    HttpResponse<T> response = new HttpResponse<T>();

                    Type type = typeof(T);
                    if(type == typeof(NoBodyType))
                    {
                        response.Body = (T)Convert.ChangeType(NoBodyType.Body, typeof(T));
                        response.StatusCode = HttpStatusCode.OK;
                        return response; 
                    }

                    StreamReader streamReader = new StreamReader(responseStream, encoding);

                    string stream = await streamReader.ReadToEndAsync();
                    
                  
                    if (type != typeof(string))
                    {
                        response.Body = DeserializeJson<T>(stream);
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

        public async Task<HttpResponse<T>> DeleteAsync<T>(string url, Dictionary<string, string> headers = null)
        {
            return await DeleteAsync<T>(new HttpDeleteRequest { Url = url, Headers = headers });
        }

        private string SerializeJson<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                string retVal = Encoding.UTF8.GetString(ms.ToArray());
                return retVal;
            }   
        }

        private T DeserializeJson<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
                return obj;
            }
        }

        private string SerializeXml<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                xmlSerializer.Serialize(ms, obj);
                string retVal = Encoding.UTF8.GetString(ms.ToArray());
                return retVal;
            }
        }

        private T DeserializeXml<T>(string xml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            var obj = (T)xmlSerializer.Deserialize(new StringReader(xml));

            return obj;
        }

        
    }
}
