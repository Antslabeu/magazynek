using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Magazynek.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Magazynek.Entities
{
    [Table("users")]
    public class User
    {
        public enum UserRole
        {
            [Display(Name = "User")] User = 0,
            [Display(Name = "System Admin")] Admin = 100
        };

        [Key] public Guid id { get; private set; }
        [Required] public string name { get; private set; }
        [Required] public string login { get; private set; }
        [Required] public string password { get; private set; }
        [Required] public bool active { get; private set; }
        [Required] public DateTime created { get; private set; }
        [Required][Column("user_role")] public UserRole userRole { get; private set; }
        [Required][Column("activator_guid")] public Guid activatorGuid { get; private set; }

        protected User()
        {
            id = Guid.NewGuid();
            name = string.Empty;
            password = string.Empty;
            login = string.Empty;
            userRole = UserRole.User;
            active = false;
            activatorGuid = Guid.NewGuid();
        }
        public User(Guid id, string name, string login, string password, UserRole userRole, Guid activatorGuid)
        {
            this.id = id;
            this.name = name;
            this.password = BCrypt.Net.BCrypt.HashPassword(password);
            this.login = login;
            this.userRole = userRole;
            active = false;
            this.activatorGuid = activatorGuid;
        }
        public void Update(string name, UserRole userRole)
        {
            this.name = name;
            this.userRole = userRole;
        }
        public void SetPassword(string password)
        {
            this.password = BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool SetActive(Guid activatorGuid)
        {
            if (activatorGuid == this.activatorGuid)
            {
                this.active = true;
                return true;
            }
            return false;
        }
        public void SetInactive()
        {
            this.activatorGuid = Guid.NewGuid();
            this.active = false;
        }

        public override string ToString() => $@"{this.name}: {this.id}";
        public static async Task<Session?> GetUserSession(
            ProtectedSessionStorage sessionStorage,
            ISessionService sessionService,
            UserRole userRole = UserRole.User
        )
        {
            Session currentSession = await sessionService.GetOrCreateSession(sessionStorage);

            if (currentSession == null) return null;
            if (currentSession.user == null || currentSession.user.userRole < userRole) return null;

            currentSession.RefreshSessionIfLogged();
            return currentSession;
        }
    }
}