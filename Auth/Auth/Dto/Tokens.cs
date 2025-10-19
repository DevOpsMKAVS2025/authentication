using System.Text.Json;

namespace Auth.Dto
{
    public class Tokens
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
