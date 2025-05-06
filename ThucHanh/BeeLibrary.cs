using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace ThucHanh
{
    class BeeLibrary
    {
        public static async Task<List<RssItem>> GetRssFeedAsync(string url)
        {
            return await Task.Run(() =>
            {
                var rssItems = new List<RssItem>();

                using (var reader = XmlReader.Create(url))
                {
                    var feed = SyndicationFeed.Load(reader);
                    foreach (var item in feed.Items)
                    {
                        rssItems.Add(new RssItem
                        {
                            Title = item.Title.Text,
                            Description = item.Summary?.Text ?? "",
                            PubDate = item.PublishDate.ToString("yyyy-MM-dd HH:mm")
                        });
                    }
                }

                return rssItems;
            });
        }
    }
}
