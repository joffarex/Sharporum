namespace Violetum.ApplicationCore.ViewModels.User
{
    public class ProfileViewModel : UserViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Picture { get; set; }
        public string Gender { get; set; }
        public string Birthdate { get; set; }
        public string Website { get; set; }
    }
}