using Newtonsoft.Json;
namespace Shopping_Web.Repository
{
    public static class SessionExtensions
    {
        // dk vao program
       public static void SetJson(this ISession session, String key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetJson<T> (this ISession session, String key)
        {
            var sessionData = session.GetString(key);
            return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
        }
    }
}
