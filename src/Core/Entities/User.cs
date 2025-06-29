using System;

namespace Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public string? PasswordHash { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Serialize() => System.Text.Json.JsonSerializer.Serialize(this);
        public static User? Deserialize(string value) => System.Text.Json.JsonSerializer.Deserialize<User>(value);
    }
}
