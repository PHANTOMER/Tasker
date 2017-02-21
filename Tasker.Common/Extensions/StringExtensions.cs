using Newtonsoft.Json;

namespace Tasker.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool ToBoolean(this string source, bool def = default(bool))
        {
            bool result;
            return bool.TryParse(source, out result) ? result : def;
        }

        public static int ToInt(this string source, int def = default(int))
        {
            int result;
            return int.TryParse(source, out result) ? result : def;
        }

        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        public static T DeserializeJson<T>(this string source)
        {
            if (source.IsNullOrEmpty())
                return default(T);

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.MissingMemberHandling = MissingMemberHandling.Ignore;

            return JsonConvert.DeserializeObject<T>(source, jss);
        }

        public static string SerializeToJson<T>(this T source)
        {
            if (source == null)
                return null;

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.MissingMemberHandling = MissingMemberHandling.Ignore;

            return JsonConvert.SerializeObject(source, jss);
        }
    }
}
