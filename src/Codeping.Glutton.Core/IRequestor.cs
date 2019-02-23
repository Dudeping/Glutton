using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Codeping.Glutton.Core
{
    public interface IRequestor
    {
        Task<string> RequestAsync(RequestContext context);
    }
}
