using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuthNativeFlow;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IpeluqueroPhoneApp.Helpers
{
    class UserDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Dictionary<string,object>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                string url = token.ToObject<string>();

                return url;
            }
            else if(token.Type == JTokenType.Object)
            {
                try
                {                   
                    return token.SelectToken("$.data.url").ToString();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
