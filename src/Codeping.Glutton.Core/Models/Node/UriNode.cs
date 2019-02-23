using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    public class UriNode : UriBuilder
    {
        public UriNode(string uri, NodeType type, string rootPath, UriNode parents) : base(uri)
        {
            this.Type = type;
            this.RootPath = rootPath;
            this.Parents = parents;

            if (parents == null)
            {
                this.LaunchUrl = this;
            }
        }

        public string RootPath { get; }
        public UriNode Parents { get; }
        public NodeType Type { get; }
        public string Cookies { get; set; }
        public Action<NotifyInfo> Notify { get; set; }
        public string OriginalString => this.Uri.OriginalString;
        public string PathAndQuery => this.Uri.PathAndQuery;
        public string FullPath => this.GetFullPath();
        public bool IsLocalUrl => this.LaunchUrl.AbsoluteHost == this.AbsoluteHost;
        public string AbsoluteHost => this.GetAbsoluteHost(this);
        public string RelativeFilePath => this.Parents.SaveDir.GetRelativeFilePath(this.FullPath);
        public string SaveDir => System.IO.Path.GetDirectoryName(this.FullPath);
        public UriNode LaunchUrl { get; private set; }

        public UriNode Create(string uri, NodeType type)
        {
            return new UriNode(uri, type, this.RootPath, this)
            {
                Cookies = this.Cookies,
                Notify = this.Notify,
                LaunchUrl = this.LaunchUrl,
            };
        }

        public bool IsSameOrigin(string fullPath)
        {
            var builder = new UriBuilder(fullPath);

            return this.LaunchUrl.AbsoluteHost == this.GetAbsoluteHost(builder);
        }

        private string GetAbsoluteHost(UriBuilder builder)
        {
            var url = builder.Scheme + Uri.SchemeDelimiter + builder.Host;

            return builder.Uri.IsDefaultPort ? url : $"{url}:{builder.Port}";
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
