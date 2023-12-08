using Blazor.Song.Net.Shared;
using System.ServiceModel.Syndication;
using System.Text;

namespace Blazor.Song.Net.Helpers
{
    public static class SyndicationItemExtensions
    {
        public static FeedItem ToFeedItem(this SyndicationItem syndicationItem)
        {
            string url = syndicationItem.Links.FirstOrDefault(l => l.MediaType != null && l.MediaType.StartsWith("audio")).Uri.AbsoluteUri;
            return new FeedItem
            {
                Id = ToLong(syndicationItem.Id),
                Title = syndicationItem.Title.Text,
                Uri = url
            };
        }

        public static long ToLong(string stringValue)
        {
            long returnValue = 0;
            var byteAsciiTable = Encoding.ASCII.GetBytes(stringValue).Reverse().Take(8).ToArray();
            for (int index = 0; index < byteAsciiTable.Length; index++)
            {
                returnValue += byteAsciiTable[index] << (index * 2);
            }
            return returnValue;
        }
    }
}