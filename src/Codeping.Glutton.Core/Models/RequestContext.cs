using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    public class RequestContext
    {
        public RequestContext(string url, string rootPath, bool isPack)
        {
            this.Url = new UriNode(url, NodeType.Html, rootPath, null);

            this.IsPack = isPack;
        }

        public IUriNode Url { get; set; }
        public bool IsPack { get; set; }
        public bool IsOnlySubdirectory { get; set; }
        public Action<NotifyInfo> OnChange
        {
            get => this.Url.Notify;
            set => this.Url.Notify = value;
        }
        public string Cookies
        {
            get => this.Url.Cookies;
            set => this.Url.Cookies = value;
        }
    }
}
