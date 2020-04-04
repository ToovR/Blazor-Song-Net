using System.IO;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Server.Services
{
    public class FileHelper : IFileHelper
    {
        public async Task<byte[]> ReadFile(string path)
        {
            using (FileStream s = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[s.Length];
                await s.ReadAsync(buffer, 0, (int)s.Length);
                return buffer;
            }
        }
    }
}