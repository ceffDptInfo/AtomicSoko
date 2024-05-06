namespace AtomicSokoHub
{
    public class Repository
    {
        private HttpClient client = new HttpClient();
        private string url = "https://atomicsokodb-default-rtdb.europe-west1.firebasedatabase.app/";

        static public Repository Instance = new Repository();
        private Repository() {}

        public async void UpdateUserCash(string userName, Int64 cash = 0)
        {
            UserModel? userModel = await GetUser(userName);

            if (userModel != null)
            {
                userModel.Cash += cash;
                await client.PutAsJsonAsync($"{url}users/{userName}/cash.json", userModel.Cash);
            }
        }

        public async void UpdateUserSkins(string userName, string skinName)
        {
            UserModel? userModel = await GetUser(userName);
            if (userModel != null)
            {
                bool hasSkin = false;
                if (userModel.Skins != null)
                {
                    foreach (string skin in userModel.Skins.Values)
                    {
                        if (skin == skinName)
                        {
                            hasSkin = true;
                            break;
                        }
                    }
                }

                if (!hasSkin)
                {
                    await client.PostAsJsonAsync($"{url}users/{userName}/skins.json", skinName);
                }

            }
        }

        public async Task<UserModel?> GetUser(string userName)
        {
            HttpResponseMessage response = await client.GetAsync($"{url}users/{userName}.json");

            if(response.Content != null)
            {
                return await response.Content.ReadFromJsonAsync<UserModel>();
            }

            return null;
        }

    }
}
