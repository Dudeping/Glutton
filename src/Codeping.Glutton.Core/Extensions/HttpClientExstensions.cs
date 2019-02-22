using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Codeping.Glutton.Core
{
    public static class HttpClientExstensions
    {
        public const string CLIENT_NAME = "codeping.glutton";

        public static void GeneralInitialize(this HttpClient client)
        {
            if (client == null)
            {
                return;
            }

            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36");

            client.DefaultRequestHeaders.Accept.TryParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");

            //_client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip, deflate, br");

            client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd("zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");

            client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");

            client.DefaultRequestHeaders.Upgrade.TryParseAdd("1");

            client.DefaultRequestHeaders.Pragma.TryParseAdd("no-cache");
        }
    }
}
