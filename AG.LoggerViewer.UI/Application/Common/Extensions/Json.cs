using Newtonsoft.Json;

namespace AG.LoggerViewer.UI.Application.Common.Extensions
{
    internal static class Json
    {
        public static T FromJson<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                TypeNameHandling =TypeNameHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
            });
        }

        public static T Clone<T>(this object value)
        {
            var val = value.ToJson();
            return val.FromJson<T>();
        }
    }
}