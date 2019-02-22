using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    internal class SkipedUriNode : IUriNode
    {
        public SkipedUriNode(string url)
        {

        }

        public string RootPath => throw new NotImplementedException();

        public UriNode LaunchUrl => throw new NotImplementedException();

        public UriNode Parents => throw new NotImplementedException();

        public NodeType Type => throw new NotImplementedException();

        public string Cookies { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<NotifyInfo> Notify { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string OriginalString => throw new NotImplementedException();

        public string PathAndQuery => throw new NotImplementedException();

        public string FullPath => throw new NotImplementedException();

        public bool IsLocalUrl => throw new NotImplementedException();

        public bool IsFilter => throw new NotImplementedException();

        public string AbsoluteHost => throw new NotImplementedException();

        public string RelativeFilePath => throw new NotImplementedException();

        public string SaveDir => throw new NotImplementedException();
    }
}
