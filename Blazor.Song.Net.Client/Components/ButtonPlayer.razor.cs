using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor.Song.Net.Client.Components
{
    public partial class ButtonPlayer
    {
        [Parameter]
        public bool IsEnabled { get; set; }

        //[Parameter]
        //public EventCallback<bool> IsEnabledChanged { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public RenderFragment Template { get; set; }
    }
}