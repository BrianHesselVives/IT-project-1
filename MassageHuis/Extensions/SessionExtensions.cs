using Newtonsoft.Json;

namespace MassageHuis.Extensions
{
    public static class SessionExtensions
    {
        // Extension methods, as the name suggests, are additional methods.
        // Extension methods allow you to inject additional methods
        // without modifying, deriving or 
        // recompiling the original class, struct or interface.
 

            public static void SetObject(this ISession session, string key, object? value)
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }



            public static T? GetObject<T>(this ISession session, string key)
            {
                var value = session.GetString(key); // Json-Object
                return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }
    }
}

