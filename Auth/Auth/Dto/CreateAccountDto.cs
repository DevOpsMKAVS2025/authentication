using Auth.Model;

namespace Auth.Dto
{
    public class CreateAccountDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Address { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; }
        public required string Type { get; set; }
    }
}
