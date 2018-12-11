using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Element
    {
        protected readonly string _id;

        public Element(string id)
        {
            _id = id;
        }

        public async Task<int> GetOffsetWidth()
        {
            return await JSRuntime.Current.InvokeAsync<int>("element.get_offsetWidth", _id);
        }
    }
}
