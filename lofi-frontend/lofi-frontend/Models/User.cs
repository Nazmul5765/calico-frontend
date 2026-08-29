using System.ComponentModel.DataAnnotations;
using lofi_frontend.Models.Enum;

namespace lofi_frontend.Models
{
    public class UserData
    {
        public UserData() 
        {
            Id = null;
            Username = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Dob = new DateTime(DateTime.Now.Year - 20, 1, 1);
            Gender = Gender.PreferNotToSay;
            Playlists = [];
        }

        public string? Id { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime Dob { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();
    }

    public class UserWithPassword
    {
        public UserWithPassword() 
        {
            UserData = new UserData();
            Password = string.Empty;
        }

        public UserWithPassword(UserData user, string password)
        {
            UserData = user;
            Password = password;
        }

        public UserData UserData { get; set; }
        public string Password { get; set; }
    }


    public class UserSettingsForm
    {
        [Required(ErrorMessage = "Please enter your New Password")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Please confirm your New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = "";
    }

}
