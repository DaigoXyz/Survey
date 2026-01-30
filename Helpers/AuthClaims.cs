using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Survey.Helpers
{
    public static class AuthClaims
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }

        public static int GetPositionId(ClaimsPrincipal user)
        {
            var posStr = user.FindFirstValue("PositionId");
            return int.TryParse(posStr, out var posId) ? posId : 0;
        }
    }
}