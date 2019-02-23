using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    internal interface IRequestFilter
    {
        void Init(RequestContext context);
        bool IsFilter(UriNode node);
    }
}
