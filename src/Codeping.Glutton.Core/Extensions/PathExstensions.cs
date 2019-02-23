using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Codeping.Glutton.Core
{
    public static class PathExstensions
    {
        public static string GetRelativeFilePath(this string rootDir, string fullPath)
        {
            var roots = rootDir.Trim().Trim('\\').Split('\\').SelectMany(x => x.Split('/')).ToList();
            var paths = fullPath.Trim().Trim('\\').Split('\\').SelectMany(x => x.Split('/')).ToList();

            int index = 0;

            for (index = 0; index < roots.Count; index++)
            {
                if (roots.Count > index && paths.Count > index)
                {
                    if (roots[index].Equals(paths[index], StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                break;
            }

            var result = string.Join("\\", paths.GetRange(index, paths.Count - index));

            if (roots.Count == index)
            {
                return result;
            }

            var builder = new StringBuilder(result);

            for (; index < roots.Count; index++)
            {
                builder.Insert(0, "..\\");
            }

            return builder.ToString();
        }

        public static string GetSavePathByPathAndQuery(this string url, bool isHtml = false)
        {
            var builder = new UriBuilder(url);

            var path = builder.Path?.TrimEnd('/');

            if (string.IsNullOrWhiteSpace(path))
            {
                path = "index.html";
            }

            var name = Path.GetFileNameWithoutExtension(path);

            var extension = Path.GetExtension(path);

            if (isHtml)
            {
                extension = ".html";
            }

            string query = null;

            if (!string.IsNullOrWhiteSpace(builder.Query))
            {
                query = builder.Query
                    .Replace('\\', '_')
                    .Replace('/', '_')
                    .Replace(':', '_')
                    .Replace('?', '_')
                    .Replace('"', '_')
                    .Replace('<', '_')
                    .Replace('>', '_')
                    .Replace('|', '_');
            }

            return $"{Path.GetDirectoryName(path).TrimStart('\\')}\\{name}{query}{extension}";
        }

        public static string BuildUrlPath(this string parentUrl, string currentUrl)
        {
            currentUrl = currentUrl.Trim('\'');

            var parent = new UriBuilder(parentUrl);
            var domain = $"{parent.Scheme}://{parent.Host}{(parent.Uri.IsDefaultPort ? "" : ":" + parent.Port)}";

            if (currentUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                currentUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return currentUrl;
            }
            else if (currentUrl.StartsWith("//"))
            {
                return "https:" + currentUrl;
            }
            else if (currentUrl.StartsWith("/"))
            {
                return domain + (currentUrl == "/" ? "/" : currentUrl);
            }
            else if (currentUrl.StartsWith("?"))
            {
                var index = parentUrl.IndexOf('?');

                if (index != -1)
                {
                    parentUrl = parentUrl.Remove(index);
                }

                return parentUrl + currentUrl;
            }
            else if (currentUrl.StartsWith("#"))
            {
                var index = parentUrl.IndexOf('#');

                if (index != -1)
                {
                    parentUrl = parentUrl.Remove(index);
                }

                return parentUrl + currentUrl;
            }
            else if (currentUrl.StartsWith("../"))
            {
                var path_query = parent.Uri.PathAndQuery;

                do
                {
                    var index = path_query.LastIndexOf('/');

                    if (index != -1)
                    {
                        path_query = path_query.Remove(index);
                    }

                    currentUrl = currentUrl.Substring(3, currentUrl.Length - "../".Length);

                } while (currentUrl.StartsWith("../"));

                return BuildUrlPath($"{domain}{path_query}", currentUrl);
            }
            else
            {
                var path_query = parent.Uri.PathAndQuery;

                var index = path_query.LastIndexOf('/');

                if (index != -1)
                {
                    path_query = path_query.Remove(index);
                }

                return $"{domain}{path_query}/{currentUrl}";
            }
        }
    }
}
