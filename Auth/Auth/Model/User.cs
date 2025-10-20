
using Auth.Model;

namespace Auth.Model
{
    public class User : BaseEntity
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public required string Username { get; set; }
        public required string Address { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required UserType UserType { get; set; }
	}
}
