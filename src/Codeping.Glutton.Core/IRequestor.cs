using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    public interface IRequestor
    {
        string Request(RequestContext context);
    }
}
