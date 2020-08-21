namespace Sharporum.Core.Dtos.User
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Birthdate { get; set; }
    }
}