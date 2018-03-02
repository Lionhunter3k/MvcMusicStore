using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreMusicStore.Services
{
    public interface IUserService
    {
        Task CopyCartItemsFromAnonymousUserAsync(RegisteredUser user);

        Task<AnonymousUser> GetOrCreateAnonymousUserAsync();

        Task<RegisteredUser> GetCurrentRegisteredUserAsync();

        Task<AnonymousUser> GetCurrentAnonymousUserAsync();
    }
}
