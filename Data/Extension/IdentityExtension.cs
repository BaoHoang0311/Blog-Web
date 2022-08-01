using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace blog_web.Extension
{
    public static class IdentityExtension
    {
        public static string GetAccountID(this ClaimsPrincipal identity)
        {
            var claim = identity.FindFirst("Account_Id");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetRoleID(this ClaimsPrincipal identity)
        {
            var claim = identity.FindFirst("Role_Id");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetRoleName(this ClaimsPrincipal identity)
        {
            var claim = identity.FindFirst(ClaimTypes.Role);
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetSpecificClaim(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
