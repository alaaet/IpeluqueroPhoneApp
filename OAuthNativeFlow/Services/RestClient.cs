using Newtonsoft.Json;
using OAuthNativeFlow;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;

namespace IpeluqueroPhoneApp.Services
{
    class RestClient<T>
    {
        private const string MainWebServiceUrl = "https://todo-demo-alaa.herokuapp.com/api/";

        public async Task<bool> checkLogin(string email, string password)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWebServiceUrl);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "auth/login");
            request.Content = new StringContent("{\"username\":\""+ email + "\",\"password\":\""+password+"\"}",Encoding.UTF8, "application/json");//CONTENT-TYPE header
            var response = await client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
                NativeUser tempUser = JsonConvert.DeserializeObject<NativeUser>(data["user"].ToString());                
                User newUser = new User();
                newUser.Id = tempUser.Id;
                newUser.Name = tempUser.Name;
                newUser.Email = tempUser.Email;
                newUser.AccessToken = (string)data["token"];
                App.User = newUser;
                // Clean previously saved values
                if (App.User != null) { SecureStorage.Remove(App.Const.AppName); }
                await SecureStorage.SetAsync(App.Const.AppName, JsonConvert.SerializeObject(App.User));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            return response.IsSuccessStatusCode;
        }
    }

    [JsonObject]
    public class NativeUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("token")]
        public string AccessToken { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("username")]
        public string Name { get; set; }       
    }
}