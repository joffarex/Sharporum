namespace Violetum.ApplicationCore.Dtos.Profile
{
    public class UpdateProfileDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Picture { get; set; }
        public string Gender { get; set; }
        public string Birthdate { get; set; }
        public string Website { get; set; }
    }
}