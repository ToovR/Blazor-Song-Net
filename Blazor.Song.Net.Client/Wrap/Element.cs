using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Element
    {
        protected readonly string _id;

        public Element(string id, IJSRuntime jsRuntime)
        {
            _id = id;
            JsRuntime = jsRuntime;
        }

        public IJSRuntime JsRuntime { get; }

        public async Task<int> GetOffsetWidth()
        {
            return await JsRuntime.InvokeAsync<int>("element.get_offsetWidth", _id);
        }
    }
}