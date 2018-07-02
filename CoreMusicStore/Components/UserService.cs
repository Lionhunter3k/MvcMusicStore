using CoreMusicStore.Services;
using Microsoft.AspNetCore.Http;
using MvcMusicStore.Models;
using MvcMusicStore.Models.Enums;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreMusicStore.Components
{
    public class UserService : IUserService
    {
        private readonly NHibernate.ISession _session;
        private readonly HttpContext _httpContext;

        public UserService(NHibernate.ISession session, IHttpContextAccessor httpContext)
        {
            this._session = session;
            this._httpContext = httpContext.HttpContext;
        }

        public async Task<AnonymousUser> GetOrCreateAnonymousUserAsync()
        {
            var currentIpAddress = _httpContext.Connection.RemoteIpAddress.ToString();
            var anonymousUser = await _session.Query<AnonymousUser>().Where(r => r.Role == Roles.Anonymous && r.LatestAddress == currentIpAddress).FirstOrDefaultAsync();
            if (anonymousUser == null)
            {
                anonymousUser = new AnonymousUser { Role = Roles.Anonymous, LatestAddress = currentIpAddress };
                await _session.SaveAsync(anonymousUser);
            }
            return anonymousUser;
        }

        public async Task<RegisteredUser> GetCurrentRegisteredUserAsync()
        {
            var rawId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(rawId, out int id))
            {
                return await _session.GetAsync<RegisteredUser>(id);
            }
            else
                return null;
        }

        public async Task<AnonymousUser> GetCurrentAnonymousUserAsync()
        {
            var rawId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(rawId, out int id))
            {
                return await _session.GetAsync<AnonymousUser>(id);
            }
            else
                return null;
        }

        public async Task CopyCartItemsFromAnonymousUserAsync(RegisteredUser user)
        {
            var anonymousUser = await GetCurrentAnonymousUserAsync();
            if (anonymousUser != null)
            {
                anonymousUser.Items.Where(p => user.Items.Add(p)).ForEach(p => anonymousUser.Items.Remove(p));
                await _session.DeleteAsync(anonymousUser);
            }
        }
    }
}
