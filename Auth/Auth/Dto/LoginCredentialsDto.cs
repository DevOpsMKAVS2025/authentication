using System.Text.Json;

namespace Auth.Dto
{
    public class LoginCredentialsDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginCredentialsDto() { }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
