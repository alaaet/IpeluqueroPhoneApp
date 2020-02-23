using OAuthNativeFlow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Essentials;

namespace IpeluqueroPhoneApp.Services
{
    class ApiService
    {
        HttpClient _nativeClient = new HttpClient();
        HttpClient _fbClient = new HttpClient();
        Constants c = new Constants();
        public ApiService() {
            _nativeClient.BaseAddress = new Uri(c.APIBaseUrl);
            _fbClient.BaseAddress = new Uri(c.FBBaseUrl);
        }
        //------------------------ CONSUMABLE FUNCTIONS ------------------------ 
        public async Task<bool> CheckIfNativeLoginExistsAndUpadteUser(string email, string password)
        {
            Dictionary<string, string> bodyData = new Dictionary<string, string>();
            bodyData.Add("username", email);
            bodyData.Add("password", password);
            var responseBody = await PostData(_nativeClient, c.APINativeLoginRealtivePath, bodyData, c.DataTypeJson);
            if (!responseBody.Equals("Error"))
            {
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
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);                    
                }
            }
            return false;
        }

        public async Task<User> GetFacebookUser(string access_token) {
            User user = new User();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("fields", "name,picture.type(large),email,birthday");
            parameters.Add("access_token",access_token);
            string response = await GetData(_fbClient, "", parameters);
            if (!response.Equals("Error"))
            {
                user = JsonConvert.DeserializeObject<User>(response);            
                Dictionary<string, string> userData = new Dictionary<string, string>();
                userData.Add("username", user.Name!=null? user.Name:"n/a");
                userData.Add("email",user.Email);
                userData.Add("given_name", user.GivenName != null ? user.GivenName : "n/a");
                userData.Add("family_name", user.FamilyName != null ? user.FamilyName : "n/a");
                userData.Add("user_id", user.Id);
                userData.Add("social_token", access_token);
                userData.Add("provider", "Facebook");
                userData.Add("image_url", user.Picture != null ? user.Picture : "n/a");
                string responseBody = await PostData(_nativeClient, c.APISocialLoginRealtivePath, userData, c.DataTypeJson);
                Console.WriteLine(responseBody);
                //"{
                    //"user":{
                        //"id":1,
                        //"username":"Alaa",
                        //"email":"alaaet@gmail.com"
                    //},
                    //"token":"5c844e548a84605b865d51bd4cf233b30c4c2ceca0d9780bfe965e667c6985b8"
                //}
            }
            return user;
        }

        // ------------------------ SUPPORT FUNCTIONS ------------------------ 
        private async Task<string> PostData(HttpClient client, string relativeUrl, Dictionary<string,string> content, string dataType)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,relativeUrl);//ex: "auth/login"
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(dataType));//ex: "application/json"
            request.Content = new FormUrlEncodedContent(content);
            var response = await client.SendAsync(request).ConfigureAwait(false);
            try
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }
        }
        private async Task<string> GetData(HttpClient client, string relativeUrl, Dictionary<string, string> parameters)
        {
            string composedUri = client.BaseAddress + relativeUrl;
            bool isFirstParam = true;
            foreach (KeyValuePair<string,string> pair in parameters)
            {
                if (isFirstParam)
                {
                    composedUri += "?" + pair.Key + "=" + pair.Value;
                    isFirstParam = false;
                }
                else 
                {
                    composedUri += "&" + pair.Key + "=" + pair.Value;
                }                
            }
            try
            {
                string response = await client.GetStringAsync(composedUri).ConfigureAwait(false);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }
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
