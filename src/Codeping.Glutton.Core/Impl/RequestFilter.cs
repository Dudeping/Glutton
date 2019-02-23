using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    internal class RequestFilter : IRequestFilter
    {
        private RequestContext _context;

        public void Init(RequestContext context)
        {
            _context = context;
        }

        public bool IsFilter(UriNode node)
        {
            if (node.Type != NodeType.Html)
            {
                return false;
            }

            var isSubdirectory = node.OriginalString.StartsWith(
                _context.Url.OriginalString.TrimEnd('/'), StringComparison.OrdinalIgnoreCase);

            if (_context.IsOnlySubdirectory && !isSubdirectory)
            {
                return true;
            }

            return false;
        }
    }
}
