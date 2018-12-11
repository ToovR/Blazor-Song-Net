using System.IO;

namespace Blazor.Song.Net.Server.Bll
{
    public class TagMusicFile : TagLib.File.IFileAbstraction
    {
        public string Name { get; private set; }

        public Stream ReadStream { get; private set; }

        public Stream WriteStream { get; private set; }

        public TagMusicFile(string path)
        {
            Name = Path.GetFileName(path);
            var fileStream = File.OpenRead(path);
            ReadStream = WriteStream = fileStream;
        }

        public void CloseStream(Stream stream)
        {
            if (stream != null)
                stream.Close();
        }
    }
}