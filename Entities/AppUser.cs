using Microsoft.AspNetCore.Identity;

namespace magazynek.Entities
{
    public class AppUser : IdentityUser
    {
        public enum UserType
        {
            Admin,
            User,
            PremiumUser,
            DemoUser
        }
        public UserType userType { get; set; }  // opcjonalnie własne pola
    }
}