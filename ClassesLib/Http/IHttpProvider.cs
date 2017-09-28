using System.Net.Http;

namespace ClassesLib.Http
{
    public interface IHttpProvider
    {
        CustomHttpResult ProcessRequest(HttpRequestMessage requestMessage);
        CustomHttpResult GetString(HttpTransportSettings settings);
        CustomHttpResult PostEncodedParam(HttpTransportSettings settings, FormUrlEncodedContent content);
        CustomHttpResult PostRawBytes(HttpTransportSettings settings, byte[] bytes);
    }
}
