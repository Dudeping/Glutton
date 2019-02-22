using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    public interface IUriNode
    {
        string RootPath { get; }
        IUriNode LaunchUrl { get; }
        IUriNode Parents { get; }
        NodeType Type { get; }
        string Cookies { get; set; }
        Action<NotifyInfo> Notify { get; set; }
        string OriginalString { get; }
        string PathAndQuery { get; }
        string FullPath { get; }
        bool IsLocalUrl { get; }
        bool IsFilter { get; }
        string AbsoluteHost { get; }
        string RelativeFilePath { get; }
        string SaveDir { get; }
    }
}
