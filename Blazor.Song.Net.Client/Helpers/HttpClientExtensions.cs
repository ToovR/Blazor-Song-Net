using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string address, object parameter)
        {
            var jsonParameter = JsonSerializer.Serialize(parameter);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonParameter);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return await client.PostAsync(address, byteContent);
        }
    }
}