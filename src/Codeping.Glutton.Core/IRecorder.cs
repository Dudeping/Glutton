using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    internal interface IRecorder
    {
        bool IsDownloaded(string url);
        void SetError(UriNode url, Exception exception);
    }
}
