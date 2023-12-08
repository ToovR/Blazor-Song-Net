using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System;
using System.IO;

namespace Blazor.Song.Indexer
{
    public class FileAzureBlobAbstraction : TagLib.File.IFileAbstraction, IDisposable
    {
        public FileAzureBlobAbstraction(ShareFileClient file)
        {
            ShareFileDownloadInfo download = file.Download();

            this.Name = file.Name;
            MemoryStream stream = new();
            download.Content.CopyTo(stream);
            ReadStream = stream;
            WriteStream = stream;
        }

        public string Name { get; private set; }

        public Stream ReadStream { get; private set; }

        public Stream WriteStream { get; private set; }

        public void CloseStream(Stream stream)
        {
            stream.Close();
        }

        public void Dispose()
        {
            this.ReadStream.Dispose();
        }
    }
}