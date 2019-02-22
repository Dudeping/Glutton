using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Codeping.Glutton.Core
{
    internal class Requestor : IRequestor
    {
        private readonly HttpClient _client;
        private readonly IDownloadLogger _logger;
        private readonly IRequestFilter _filter;

        public Requestor(IServiceProvider provider)
        {
            _filter = provider.GetService<IRequestFilter>();
            _logger = provider.GetService<IDownloadLogger>();
            _client = provider.GetService<IHttpClientFactory>().CreateClient(HttpClientExstensions.CLIENT_NAME);
        }

        public string Request(RequestContext context)
        {
            _filter.Init(context);

            this.RequestContent(context.Url);

            if (context.IsPack)
            {
                // TODO:
                // pack and delete files
                // return package;
            }

            return context.Url.RootPath;
        }

        public string RequestContent(UriNode node)
        {
            var isLoaded = _logger.IsDownload(node.OriginalString);

            var fullPath = node.FullPath;

            if (isLoaded)
            {
                return fullPath;
            }

            HttpResponseMessage response = null;

            try
            {
                var message = new HttpRequestMessage(HttpMethod.Get, node.OriginalString);

                if (!string.IsNullOrWhiteSpace(node.Cookies))
                {
                    message.Headers.Add("Cookie", node.Cookies);
                }

                message.Headers.Referrer = node.Parents?.Uri ?? new Uri("www.baidu.com");

                response = _client.SendAsync(message).Result?.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                node.Notify(new NotifyInfo().Error(node, ex));

                return node.OriginalString;
            }

            var dir = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (node.Type.IsBinary())
            {
                using (var fw = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    response.Content.CopyToAsync(fw).Wait();
                }
            }
            else
            {
                var content = new StringBuilder(response.Content.ReadAsStringAsync().Result);

                this.HandleNestNode(node, Path.GetDirectoryName(fullPath), content);

                File.WriteAllText(fullPath, content.ToString());
            }

            return fullPath;
        }

        public void HandleNestNode(UriNode node, string directory, StringBuilder content)
        {
            this.UnityHandle(node, directory, content, NodeType.Css);

            this.UnityHandle(node, directory, content, NodeType.Script);

            this.UnityHandle(node, directory, content, NodeType.Html);

            this.UnityHandle(node, directory, content, NodeType.BG_Image);

            this.UnityHandle(node, directory, content, NodeType.Image);

            this.UnityHandle(node, directory, content, NodeType.Font);
        }

        private void UnityHandle(UriNode node, string directory, StringBuilder content, NodeType type)
        {
            var regex = new Regex(type.GetPattern(), RegexOptions.IgnoreCase);

            var result = regex.Matches(content.ToString());

            var information = new NotifyInfo();

            node.Downloading();

            foreach (Match item in result)
            {
                var hyperlink = item.Groups[1].Value;

                var lowerLink = hyperlink.ToLower();

                if (_filter.IsFilter(node))
                {
                    continue;
                }

                var fullUrl = node.OriginalString.BuildUrlPath(hyperlink);

                if (type == NodeType.Html && !this.IsLocalUrl(fullUrl))
                {
                    var aurl = item.Groups[0].Value.Replace(hyperlink, fullUrl);

                    content.Replace(item.Groups[0].Value, aurl);

                    continue;
                }

                var child = node.Create(fullUrl, type);

                var itemFullPath = this.RequestContent(child);

                var replacePath = fullUrl != itemFullPath
                    ? directory.GetRelativeFilePath(itemFullPath)
                    : fullUrl;

                var text = item.Groups[0].Value.Replace(hyperlink, replacePath);

                content.Replace(item.Groups[0].Value, text);

                node.Downloaded();
            }
        }
    }
}
