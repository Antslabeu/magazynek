using Microsoft.EntityFrameworkCore;
using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Configuration;

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

        public static readonly List<string> adminNeededPages = new List<string>()
        {
            "system-settings"
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
        Task PrepareUserTables(User user);
        Task<User> DeactivateUser(User user);
        Task UpdatePassword(User user, string password);
        Task<User?> GetLoggedUser(ProtectedSessionStorage sessionStorage);

        List<Session> GetSessions();
        Task<List<User>> GetAllUsers();

        void Admin_DeleteSession(Session s);
        Task Admin_DeleteUser(User user);
    }

    public class SessionService : ISessionService
    {
        private List<Session> sessions;
        private readonly IServiceProvider serviceProvider;
        private readonly INeededSetting neededSetting;

        public SessionService(IServiceProvider serviceProvider, INeededSetting neededSetting)
        {
            sessions = new List<Session>();
            this.serviceProvider = serviceProvider;
            this.neededSetting = neededSetting;
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
        public async Task PrepareUserTables(User user)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            // settings:
            List<NeededSettingObject> systemSettings = neededSetting.GetNeddedSettings();
            foreach(NeededSettingObject neededSetting in systemSettings)
            {
                database.SystemSettings.Add(new SystemSetting(
                    name: neededSetting.Name, 
                    type: (SystemSetting.SettingType)neededSetting.Type,
                    Value: "", 
                    sName: (SystemSetting.SettingName)neededSetting.SName,
                    userID: user.id
                ));
            }
            await database.SaveChangesAsync();

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
        public async Task UpdatePassword(User user, string password)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            User? dbUser = await database.Users.FirstOrDefaultAsync(u => u.id == user.id);
            if (dbUser == null) return;

            dbUser.SetPassword(password);
            dbUser.SetActive(dbUser.activatorGuid);
            await database.SaveChangesAsync();
        }
        public async Task<User?> GetLoggedUser(ProtectedSessionStorage sessionStorage)
        {
            ProtectedBrowserStorageResult<Guid> result = await sessionStorage.GetAsync<Guid>(ISessionService.sessionGuidKey);
            if (!result.Success || result.Value == Guid.Empty) return null;

            Session? session = sessions.FirstOrDefault(s => s.guid == result.Value);
            if(session == null) return null;

            return session.user;
        }
        public List<Session> GetSessions() => sessions;

        public void Admin_DeleteSession(Session s)
        {
            sessions.Remove(s);
        }
        public async Task<List<User>> GetAllUsers()
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            return await database.Users.ToListAsync();
        }
        public async Task Admin_DeleteUser(User user)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            database.Users.Remove(user);
            await database.SaveChangesAsync();
        }
    }
}