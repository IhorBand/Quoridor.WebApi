using Microsoft.AspNetCore.SignalR;
using Quoridor.Shared.DTO.Configuration;

namespace Quoridor.WebApi.Hubs
{
    public class UserHubBase : Hub
    {
        public Guid UserId
        {
            get
            {
                var idStr = this.GetClaimValue(JwtCustomClaimNames.UserId);
                return new Guid(idStr);
            }
        }

        public string Email { get => this.GetClaimValue(JwtCustomClaimNames.Email); }
        public string UserName { get => this.GetClaimValue(JwtCustomClaimNames.UserName); }
        public string DisplayName { get => this.GetClaimValue(JwtCustomClaimNames.DisplayName); }

        private string GetClaimValue(string claimName)
        {
            if (this.Context.User != null)
            {
                var claim = this.Context.User.Claims.FirstOrDefault(c => c.Type == claimName);
                if (claim != null)
                {
                    return claim.Value;
                }
            }

            return string.Empty;
        }
    }
}
