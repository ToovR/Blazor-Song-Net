using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Element
    {
        public Element(string id, IJSRuntime jsRuntime)
        {
            _id = id;
            JsRuntime = jsRuntime;
        }

        protected readonly string _id;

        public IJSRuntime JsRuntime { get; }

        public async Task<int> GetOffsetWidth()
        {
            return await JsRuntime.InvokeAsync<int>("element.get_offsetWidth", _id);
        }
    }
}