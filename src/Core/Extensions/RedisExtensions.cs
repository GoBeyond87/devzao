using System.Text.Json;

namespace Core.Extensions
{
    public static class RedisExtensions
    {
        public static string Serialize<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static T? Deserialize<T>(this string? str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(str);
        }
    }
}
