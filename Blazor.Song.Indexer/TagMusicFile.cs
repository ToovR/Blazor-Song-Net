using System.IO;

namespace Blazor.Song.Indexer
{
    public class TagMusicFile : TagLib.File.IFileAbstraction
    {
        public TagMusicFile(string path)
        {
            Name = Path.GetFileName(path);
            var fileStream = File.OpenRead(path);
            ReadStream = WriteStream = fileStream;
        }

        public string Name { get; private set; }

        public Stream ReadStream { get; private set; }

        public Stream WriteStream { get; private set; }

        public void CloseStream(Stream stream)
        {
            if (stream != null)
                stream.Close();
        }
    }
}
