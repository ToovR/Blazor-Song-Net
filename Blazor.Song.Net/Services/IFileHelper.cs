using System.Threading.Tasks;

namespace Blazor.Song.Net.Services
{
    public interface IFileHelper
    {
        Task<byte[]> ReadFile(string path);
    }
}