using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Codeping.Glutton.Core
{
    internal class Requestor : IRequestor
    {
        private readonly HttpClient _client;
        private readonly IRecorder _recorder;
        private readonly IRequestFilter _filter;

        public Requestor(IServiceProvider provider)
        {
            _recorder = provider.GetService<IRecorder>();
            _filter = provider.GetService<IRequestFilter>();

            _client = provider.GetService<IHttpClientFactory>()
                .CreateClient(HttpClientExstensions.CLIENT_NAME);
        }

        public async Task<string> RequestAsync(RequestContext context)
        {
            _filter.Init(context);

            await this.RequestContentAsync(context.Url);

            if (context.IsPack)
            {
                // TODO:
                // pack and delete files
                // return package;
            }

            return context.Url.RootPath;
        }

        public async Task<string> RequestContentAsync(UriNode node)
        {
            if (_recorder.IsDownloaded(node.OriginalString))
            {
                return node.FullPath;
            }

            if (_filter.IsFilter(node))
            {
                return node.OriginalString;
            }

            HttpResponseMessage response = null;

            node.Downloading();

            try
            {
                var message = new HttpRequestMessage(HttpMethod.Get, node.OriginalString);

                if (!string.IsNullOrWhiteSpace(node.Cookies))
                {
                    message.Headers.Add("Cookie", node.Cookies);
                }

                message.Headers.Referrer = node.Parents?.Uri ?? new Uri("https://www.baidu.com:443/");

                response = (await _client.SendAsync(message))?.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                node.Notify(new NotifyInfo(node).Error(ex));

                return node.OriginalString;
            }

            var fullPath = node.FullPath;

            var dir = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (node.Type.IsBinary())
            {
                using (var fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
            else
            {
                var content = new StringBuilder(await response.Content.ReadAsStringAsync());

                await this.HandleNestNodeAsync(node, Path.GetDirectoryName(fullPath), content);

                File.WriteAllText(fullPath, content.ToString());
            }

            node.Downloaded();

            return fullPath;
        }

        public async Task HandleNestNodeAsync(UriNode node, string directory, StringBuilder content)
        {
            await this.UnityHandleAsync(node, directory, content, NodeType.Css);

            await this.UnityHandleAsync(node, directory, content, NodeType.Script);

            await this.UnityHandleAsync(node, directory, content, NodeType.Html);

            await this.UnityHandleAsync(node, directory, content, NodeType.BG_Image);

            await this.UnityHandleAsync(node, directory, content, NodeType.Image);

            await this.UnityHandleAsync(node, directory, content, NodeType.Font);
        }

        private async Task UnityHandleAsync(UriNode node, string directory, StringBuilder content, NodeType type)
        {
            var regex = new Regex(type.GetPattern(), RegexOptions.IgnoreCase);

            var result = regex.Matches(content.ToString());

            var information = new NotifyInfo();

            foreach (Match item in result)
            {
                var rawText = item.Groups[0].Value;

                var hyperlink = item.Groups[1].Value;

                var lowerLink = hyperlink.ToLower();

                if (string.IsNullOrWhiteSpace(lowerLink) ||
                    lowerLink.StartsWith("data:") ||
                    lowerLink.StartsWith("data：") ||
                    lowerLink.StartsWith("javascript:") ||
                    lowerLink.StartsWith("javascript："))
                {
                    continue;
                }

                var fullUrl = node.OriginalString.BuildUrlPath(hyperlink);

                var child = node.Create(fullUrl, type);

                if (_filter.IsFilter(child))
                {
                    continue;
                }

                child.Downloading();

                if (!(type == NodeType.Html && !child.IsLocalUrl))
                {
                    var itemFullPath = await this.RequestContentAsync(child);

                    if (itemFullPath != fullUrl)
                    {
                        fullUrl = directory.GetRelativeFilePath(itemFullPath);
                    }
                }

                var text = rawText.Replace(hyperlink, fullUrl);

                content.Replace(rawText, text);

                child.Downloaded();
            }
        }
    }
}
