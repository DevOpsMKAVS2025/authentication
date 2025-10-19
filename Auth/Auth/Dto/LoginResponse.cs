using System.Text.Json;

namespace Auth.Dto
{
    public class LoginResponse
    {
        public required string AccessToken { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
