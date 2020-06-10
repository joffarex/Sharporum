namespace Violetum.ApplicationCore.ViewModels.User
{
    public class ProfileViewModel : UserViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public string Gender { get; set; }
        public string Birthdate { get; set; }
    }
}