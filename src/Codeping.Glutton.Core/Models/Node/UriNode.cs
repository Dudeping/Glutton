using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    public class UriNode : UriBuilder, IUriNode
    {
        public UriNode(string uri, NodeType type, string rootPath, UriNode parents) : base(uri)
        {
            this.Type = type;
            this.RootPath = rootPath;
            this.Parents = parents;
        }

        public string RootPath { get; }
        public IUriNode Parents { get; }
        public NodeType Type { get; }
        public string Cookies { get; set; }
        public Action<NotifyInfo> Notify { get; set; }
        public string OriginalString => this.Uri.OriginalString;
        public string PathAndQuery => this.Uri.PathAndQuery;
        public string FullPath => this.GetFullPath();
        public bool IsLocalUrl => this.LaunchUrl.AbsoluteHost == this.AbsoluteHost;
        public bool IsFilter => this.Filter.IsFilter(this);
        public string AbsoluteHost => this.GetAbsoluteHost();
        public string RelativeFilePath => this.Parents.SaveDir.GetRelativeFilePath(this.FullPath);
        public string SaveDir => System.IO.Path.GetDirectoryName(this.FullPath);
        internal IRequestFilter Filter { get; set; }
        public IUriNode LaunchUrl { get; private set; }

        public IUriNode Create(string uri, NodeType type)
        {
            return new UriNode(uri, type, this.RootPath, this)
            {
                Cookies = this.Cookies,
                Notify = this.Notify
            };
        }

        private string GetAbsoluteHost()
        {
            var url = this.Scheme + Uri.SchemeDelimiter + this.Host;

            return this.Uri.IsDefaultPort ? url : $"{url}:{this.Port}";
        }

        private string GetFullPath()
        {
            var typeDir = this.Type.GetSaveDirName().Trim('\\');
            var savePath = this.OriginalString.GetSavePathByPathAndQuery(this.Type == NodeType.Html).TrimStart('\\');

            var fullPath = this.LaunchUrl != null && this.Host != this.LaunchUrl.Host
                ? System.IO.Path.Combine(this.RootPath, typeDir, this.Host, savePath)
                : System.IO.Path.Combine(this.RootPath, typeDir, savePath);

            return fullPath;
        }
    }
}
