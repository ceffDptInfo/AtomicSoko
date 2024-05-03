using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace atomicSoko
{
    public class Repository
    {
        static HttpClient client = new HttpClient();
        static string url = "https://atomicsokodb-default-rtdb.europe-west1.firebasedatabase.app/";

        static public Repository Instance = new Repository(); //singleton

        public async Task<Uri>? PostUser(UserModel user)
        {
            if (await GetUser(user)! == null)
            {
                HttpResponseMessage response = await client.PatchAsJsonAsync<UserModel>($"{url}users/{user.UserName}.json", user);
                return response.Headers.Location!;
            }
            else
            {
                return null!;
            }

        }

        public async Task<string>? GetUrlForSkin(string id)
        {
            if(id != null)
            {
                HttpResponseMessage response = await client.GetAsync($"{url}skins/{id}.json");
                if(response.IsSuccessStatusCode)
                {
                    string newUrl = await response.Content.ReadAsStringAsync();
                    if(newUrl != null)
                    {
                        return newUrl;
                    }
                }
            }
            return null!;
        }

        public async Task<UserModel>? GetUser(UserModel user)
        {
            UserModel? newUser = null;
            HttpResponseMessage response = await client.GetAsync($"{url}users/{user.UserName}.json");
            if (response.IsSuccessStatusCode)
            {
                newUser = await response.Content.ReadFromJsonAsync<UserModel>();

                if (newUser != null && TestIfMyUser(newUser!, user))
                {
                    return newUser;
                }
            }
            return null!;
        }

        private bool TestIfMyUser(UserModel newUser, UserModel user)
        {
            if (newUser.Password == user.Password)
            {
                return true;
            }
            return false;
        }

        public async Task<Int64> UpdateMoney(User user)
        {
            HttpResponseMessage response = await client.GetAsync($"{url}users/{user.UserName}/cash.json");

            Int64 number = await response.Content.ReadFromJsonAsync<Int64>();
            return number;
        }
    }
}
