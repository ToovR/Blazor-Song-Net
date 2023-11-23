using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System.IO;

namespace Blazor.Song.Indexer
{
    public class FileAzureBlobAbstraction : TagLib.File.IFileAbstraction
    {
        public FileAzureBlobAbstraction(ShareFileClient file)
        {
            ShareFileDownloadInfo download = file.Download();

            MemoryStream stream = new MemoryStream();
            download.Content.CopyTo(stream);
            ReadStream = stream;
            WriteStream = stream;
        }

        public void CloseStream(Stream stream)
        {
            stream.Dispose();
        }

        public string Name { get; private set; }

        public Stream ReadStream { get; private set; }

        public Stream WriteStream { get; private set; }
    }
}