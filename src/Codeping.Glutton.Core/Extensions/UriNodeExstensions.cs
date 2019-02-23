using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Codeping.Glutton.Core
{
    public static class UriNodeExstensions
    {
        public static void Downloading(this UriNode node)
        {
            node.Notify(new NotifyInfo(node));
        }

        public static void Downloaded(this UriNode node)
        {
            node.Notify(new NotifyInfo(node).Downloaded());
        }

        public static void Skiped(this UriNode node)
        {
            node.Notify(new NotifyInfo(node).Skiped());
        }
    }
}
