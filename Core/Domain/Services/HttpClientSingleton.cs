using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services
{
    public static class HttpClientSingleton
    {
        private static HttpClient _httpClient;

        public static HttpClient Instance
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }

                return _httpClient;
            }
        }
    }
}
