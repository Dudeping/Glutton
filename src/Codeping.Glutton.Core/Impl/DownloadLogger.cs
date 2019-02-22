using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    internal class DownloadLogger : IDownloadLogger
    {
        private readonly object _lock = new object();
        private readonly ISet<string> _logs;
        private readonly IDictionary<UriNode, Exception> _errors;

        public DownloadLogger()
        {
            _logs = new HashSet<string>();
            _errors = new Dictionary<UriNode, Exception>();
        }

        public bool IsDownload(string url)
        {
            lock (_lock)
            {
                if (!_logs.Contains(url))
                {
                    _logs.Add(url);

                    return false;
                }

                return true;
            }
        }

        public void SetError(UriNode url, Exception exception)
        {
            lock (_lock)
            {
                if (!_logs.Contains(url.OriginalString))
                {
                    _logs.Add(url.OriginalString);
                }

                if (!_errors.ContainsKey(url))
                {
                    _errors.Add(url, exception);
                }
            }
        }
    }
}
