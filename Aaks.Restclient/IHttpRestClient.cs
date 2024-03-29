﻿using Aaks.Restclient.Enums;
using Aaks.Restclient.Models;
using Aaks.RestclientTests.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aaks.Restclient
{
    public interface IHttpRestClient
    {
        void SetIpAddress(string ipAddress);
        HttpResponse<T> Post<T, K>(string url, K body, Dictionary<string, string> headers = null, string contentType = null);
        Task<HttpResponse<T>> PostAsync<T, K>(string url, K body, Dictionary<string, string> headers = null);
        Task<HttpResponse<T>> PostAsync<T, K>(HttpPostRequest<K> request);
        HttpResponse<T> Delete<T>(string url, Dictionary<string, string> headers = null);
        Task<HttpResponse<T>> DeleteAsync<T>(HttpDeleteRequest request);
        Task<HttpResponse<T>> DeleteAsync<T>(string url, Dictionary<string, string> headers = null);
        HttpResponse<T> Get<T>(string url, Dictionary<string, string> headers = null);
        Task<HttpResponse<T>> GetAsync<T>(HttpRequest request);
        Task<HttpResponse<T>> GetAsync<T>(string url, Dictionary<string, string> headers = null);
    }
}