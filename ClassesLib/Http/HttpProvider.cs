using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;

namespace ClassesLib.Http
{
    public class HttpProvider : IHttpProvider
    {
        public CustomHttpResult ProcessRequest(HttpRequestMessage requestMessage)
        {
            using (var client = BuildHttpClient())
            {
                var res = client.SendAsync(requestMessage).Result;
                res.EnsureSuccessStatusCode();
                var retVal = new CustomHttpResult
                {
                    IsSuccessStatusCode = res.IsSuccessStatusCode,
                    HttpStatusCode = res.StatusCode,
                    Content = res.Content.ReadAsStringAsync().Result
                };
                return retVal;
            }
        }

        public CustomHttpResult GetString(HttpTransportSettings settings)
        {
            using (var client = BuildHttpClient(settings))
            {
                var res = client.GetAsync(settings.Url).Result;
                var retVal = new CustomHttpResult
                {
                    IsSuccessStatusCode = res.IsSuccessStatusCode,
                    HttpStatusCode = res.StatusCode,
                    Content = res.Content.ReadAsStringAsync().Result
                };
                return retVal;
            }

        }

        public CustomHttpResult PostEncodedParam(HttpTransportSettings settings, FormUrlEncodedContent content)
        {
            using (var client = BuildHttpClient(settings))
            {
                var res = client.PostAsync(settings.Url, content).Result;

                var retVal = new CustomHttpResult
                {
                    IsSuccessStatusCode = res.IsSuccessStatusCode,
                    HttpStatusCode = res.StatusCode,
                    Content = res.Content.ReadAsStringAsync().Result
                };
                return retVal;
            }
        }

        public CustomHttpResult PostRawBytes(HttpTransportSettings settings, byte[] bytes)
        {
            using (var client = BuildHttpClient(settings))
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = settings.Url,
                    Method = HttpMethod.Post,
                    Content = new ByteArrayContent(bytes),
                };

                request.Content.Headers.ContentType = new MediaTypeHeaderValue(settings.ContentType);

                var res = client.PostAsync(settings.Url, new ByteArrayContent(bytes)).Result;

                var retVal = new CustomHttpResult
                {
                    IsSuccessStatusCode = res.IsSuccessStatusCode,
                    HttpStatusCode = res.StatusCode,
                    Content = res.Content.ReadAsStringAsync().Result
                };
                return retVal;
            }
        }

        private HttpClient BuildHttpClient(HttpTransportSettings settings = null)
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
                SslProtocols = SslProtocols.Tls12,
                Proxy = null
            };

            var client = new HttpClient(httpClientHandler);

            if (settings is null)
                return client;

            if (settings.AuthenticationHeader != null)
                client.DefaultRequestHeaders.Authorization = settings.AuthenticationHeader;

            foreach (var header in settings.Headers)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);

            if (settings.MediaTypeWithQualityHeader != null)
                client.DefaultRequestHeaders
                    .Accept
                    .Add(settings.MediaTypeWithQualityHeader);

            return client;
        }
    }
}
