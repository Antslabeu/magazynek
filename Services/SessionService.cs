using Microsoft.EntityFrameworkCore;
using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Magazynek.Services
{
    public interface ISessionService
    {
        const string sessionGuidKey = "RemoteGUID";
        public static readonly List<string> unprotectedPages = new List<string>()
        {
            "",
            "new-account",
            "new-account/activate",
            "new-account/revert-password",
            "login",
            "activate"
        };
        public static readonly TimeSpan logoutTime = TimeSpan.FromMinutes(30);

        Task<Session> GetOrCreateSession(ProtectedSessionStorage sessionStorage);
        Task<User?> GetUser(User.UserRole role);
        Task<User?> GetUser(string login);
        Task<bool> LoginUser(Session session, string userLogin, string plainPass);
        bool LogoutUser(Session? session);
        bool UserShouldBeLoggedOut(Session session);

        Task AddNewUser(User user);
        Task<bool> TryActivateUser(Guid activateGuid, User user);
        Task<User> DeactivateUser(User user);
    }

    public class SessionService : ISessionService
    {
        private List<Session> sessions;
        private readonly IServiceProvider serviceProvider;

        public SessionService(IServiceProvider serviceProvider)
        {
            sessions = new List<Session>();
            this.serviceProvider = serviceProvider;
        }

        public async Task<Session> GetOrCreateSession(ProtectedSessionStorage sessionStorage)
        {
            Guid currentGuid = Guid.Empty;
            ProtectedBrowserStorageResult<Guid> result = await sessionStorage.GetAsync<Guid>(ISessionService.sessionGuidKey);
            if (!result.Success || result.Value == Guid.Empty)
            {
                currentGuid = Guid.NewGuid();
                await sessionStorage.SetAsync(ISessionService.sessionGuidKey, currentGuid);
                Session newSession = new Session(currentGuid);
                sessions.Add(newSession);
                return newSession;
            }
            else
            {
                Session? session = sessions.FirstOrDefault(s => s.guid == result.Value);
                if (session == null)
                {
                    session = new Session(result.Value);
                    sessions.Add(session);
                }

                session.RefreshSessionIfLogged();
                return session;
            }

            throw new Exception("Unable to find session!!");
        }
        public async Task<User?> GetUser(User.UserRole role)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await database.Users.FirstOrDefaultAsync(u => u.userRole == role);
        }
        public async Task<User?> GetUser(string login)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await database.Users.FirstOrDefaultAsync(u => u.login == login);
        }
        public async Task<bool> LoginUser(Session session, string userlogin, string plainPass)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            User? dbUser = await database.Users.FirstOrDefaultAsync(u => u.login == userlogin && u.active);

            if (dbUser == null) return false;
            bool loggedIn = BCrypt.Net.BCrypt.Verify(plainPass, dbUser.password);
            if (!loggedIn) return false;

            sessions.FirstOrDefault(s => s.guid == session.guid)?.UpdateUser(dbUser);
            return true;
        }
        public bool LogoutUser(Session? session)
        {
            if (session == null) return false;
            sessions.FirstOrDefault(s => s.guid == session.guid)?.UpdateUser(null);
            return true;
        }
        public bool UserShouldBeLoggedOut(Session session)
        {
            if (session.user == null) return false;
            return session.modifiedAt + ISessionService.logoutTime <= DateTime.Now;
        }

        public async Task AddNewUser(User user)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            await database.Users.AddAsync(user);
            await database.SaveChangesAsync();
        }
        public async Task<bool> TryActivateUser(Guid activateGuid, User user)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            if (!user.SetActive(activateGuid)) return false;
            User? dbUser = await database.Users.FirstOrDefaultAsync(u => u.id == user.id);
            if (dbUser == null) return false;

            if (!dbUser.SetActive(activateGuid))
            {
                user.SetInactive();
                return false;
            }

            await database.SaveChangesAsync();
            return true;
        }
        public async Task<User> DeactivateUser(User user)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            User? dbUser = await database.Users.FirstOrDefaultAsync(u => u.id == user.id);
            if (dbUser == null) return user;

            dbUser.SetInactive();
            await database.SaveChangesAsync();
            return dbUser;
        }

    }
}