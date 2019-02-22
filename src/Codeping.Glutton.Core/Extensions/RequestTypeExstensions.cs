using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Codeping.Glutton.Core
{
    public static class RequestTypeExstensions
    {
        public static string GetSaveDirName(this NodeType type)
        {
            switch (type)
            {
                case NodeType.Html: return "html";
                case NodeType.Css: return "css";
                case NodeType.Font: return "fonts";
                case NodeType.Script: return "scripts";
                case NodeType.Image: return "images";
                case NodeType.BG_Image: return "images";
                default: throw new NotImplementedException();
            }
        }

        public static string GetPattern(this NodeType type)
        {
            switch (type)
            {
                case NodeType.Html: return @"<a[^>]*href\s*=\s*[""'](?<href>[^'"">]*)[^>]*>";
                case NodeType.Css: return @"<link[^>]*href\s*=\s*[""'](?<href>[^'"">]*)[^>]*>";
                case NodeType.Font: return @"url\s*\(\s*(?<url>[^\)]*)\s*\)";
                case NodeType.Script: return @"<script[^>]*src\s*=\s*[""'](?<src>[^'"">]*)[^>]*>";
                case NodeType.Image: return @"<img[^>]*src\s*=\s*[""'](?<src>[^'"">]*)[^>]*>";
                case NodeType.BG_Image: return @"background-image\s*:\s*url\s*\(\s*(?<url>[^\)]*)\s*\)";
                default: throw new NotImplementedException();
            }
        }

        public static bool IsBinary(this NodeType type)
        {
            switch (type)
            {
                case NodeType.Html:
                case NodeType.Css:
                case NodeType.Script:
                    return false;

                case NodeType.Font:
                case NodeType.Image:
                case NodeType.BG_Image:
                    return true;

                default: throw new NotImplementedException();
            }
        }
    }
}
