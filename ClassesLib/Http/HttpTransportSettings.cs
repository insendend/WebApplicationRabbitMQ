using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace ClassesLib.Http
{
    public class HttpTransportSettings
    {
        public string ContentType { get; set; } = "";

        public MediaTypeWithQualityHeaderValue MediaTypeWithQualityHeader { get; set; }

        public Uri Url { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public AuthenticationHeaderValue AuthenticationHeader { get; set; }
    }
}
