using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    internal interface IDownloadLogger
    {
        bool IsDownload(string url);
        void SetError(IUriNode url, Exception exception);
    }
}
