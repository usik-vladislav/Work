using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CountriesLibrary.Managers
{
    public static class WebManager
    {
        public static async Task<string> Request(string uri)
        {
            var request = WebRequest.Create(uri);
            var response = await request.GetResponseAsync().ConfigureAwait(false);
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream ?? throw new NullReferenceException("Response stream is null.")))
                {
                    return await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
