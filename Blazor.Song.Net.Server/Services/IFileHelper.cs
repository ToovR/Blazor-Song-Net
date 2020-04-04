using System.Threading.Tasks;

namespace Blazor.Song.Net.Server.Services
{
    public interface IFileHelper
    {
        Task<byte[]> ReadFile(string path);
    }
}