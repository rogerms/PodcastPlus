using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace PodcastPlus
{
    class PodcastClient
    {
        public PodcastClient()
        {

        }

        public PodcastFeedList RetrieveFeedAsync(Uri feedUriString)
        {
            PodcastFeed pfeed = new PodcastFeed();
            PodcastFeedList feedList = new PodcastFeedList();
            try
            {
                XmlDocument doc = new XmlDocument();
                string xmlString = webRequest(feedUriString);
                doc.Load(new StringReader(xmlString));
                XmlNodeList podcasts = doc.GetElementsByTagName("channel");

                foreach (XmlNode podcast in podcasts)
                {
                    pfeed = new PodcastFeed();
                    pfeed.Title = getValue(podcast["title"]);
                    pfeed.Author = getValue(podcast["author"]);
                    pfeed.Description = getValue(podcast["description"]);
                    //pfeed.ImageUrl = getAttribute(podcast["image"], "href"); //image href
                    pfeed.ImageUrl = getValue(podcast["imageurl"]);
                    pfeed.PubDate = getDateValue(podcast["pubDate"]); 
                    pfeed.Link = getValue(podcast["link"]);
                    pfeed.Language = getValue(podcast["language"]);
                    pfeed.Keywords = getValue(podcast["keywords"]);
                    pfeed.Category = getValue(podcast["category"]);

                    foreach (XmlNode item in (podcast as XmlElement).GetElementsByTagName("item"))
                    {
                        PodcastEpisode pItem = new PodcastEpisode();

                        pItem.Title = getValue(item["title"]);
                        pItem.Duration = getIntValue(item["duration"]); //if not removing itunes NS, could be item[itunes:duration]
                        pItem.Url = getAttribute(item["enclosure"], "url");
                        pItem.Length = strToLong(getAttribute(item["enclosure"], "length")); //bytes
                        pItem.Type = getAttribute(item["enclosure"], "type");
                        pItem.PubDate = getDateValue(item["pubDate"]);
                        pItem.Description = getValue(item["description"]);
                        pItem.Guid = getValue(item["guid"]);

                        pfeed.Items.Add(pItem);
                    }
                    feedList.Add(pfeed);
                }
                doc = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

            return feedList;
        }

        private string webRequest(Uri feedUriString)
        {
            String xmlString = "";

            try
            {
                WebRequest wr = WebRequest.Create(feedUriString);
                Stream oStream = wr.GetResponse().GetResponseStream();
                StreamReader oSReader = new StreamReader(oStream);
                xmlString = oSReader.ReadToEnd();
            }
            catch (WebException e)
            {
                Debug.WriteLine(e.Message + "\nurl:" + feedUriString);
                return null;
            }

            //Debug.WriteLine(xmlString);

            xmlString = xmlString.Replace("itunes:", "");
            return xmlString;
        }

        public PodcastFeedList RetrieveFeedAsync(string feedUriString)
        {
            Uri feedUri = new Uri(feedUriString);

            return RetrieveFeedAsync(feedUri);
        }

        private string getValue(XmlElement node)
        {
            if (node != null)
                return node.InnerText;
            return "";
        }

        private DateTime getDateValue(XmlElement node)
        {
            string date = getValue(node);
            //Sun, 26 May 2013 17:09:29 PST
            DateTime result = new DateTime();
            if (date.Equals(String.Empty)) return new DateTime();
            try
            {
                result = DateTime.Parse(date);
            }
            catch (Exception e)
            {
                Debug.WriteLine("get date error: " + e + "\ndate: " + date);
            }
            return result;
        }

        private Int64 getInt64Value(XmlElement node)
        {
            string num = getValue(node);
            Int64 result;
            Int64.TryParse(num, out result);
            return result;
        }

        //private string getImageUrl(XmlNode node)
        //{
        //    try
        //    {
        //        return node["image"].GetElementsByTagName("url")[0].Value;
        //    }
        //    catch (Exception)
        //    {
        //        Debug.WriteLine("Problem getting Image");
        //        return "";
        //    }
        //}

        private Int64 strToLong(string num)
        {
            Int64 result;
            Int64.TryParse(num, out result);
            return result;
        }

        private int getIntValue(XmlElement node)
        {
            string num = getValue(node);
            Int32 result;
            Int32.TryParse(num, out result);
            return result;
        }

        private string getAttribute(XmlElement node, string attr)
        {
            if (node != null)
                return node.GetAttribute(attr);
            return "";
        }
    }


    public class PodcastFeed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Language { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PubDate { get; set; }
        public string Copyright { get; set; }
        public string Author { get; set; }
        public string Keywords { get; set; }
        public string Category { get; set; }
        public string Explicit { get; set; }
        public List<PodcastEpisode> Items {get; set;}
    }

    public class PodcastEpisode
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PubDate { get; set; }
        public string Guid { get; set; }
        /** Media length in seconds**/
        public int Duration { get; set; }
        public string Url { get; set; }
        /** File size in bytes **/
        public long Length { get; set; }
        public string Type { get; set; }
    }

    public class PodcastFeedList : List<PodcastFeed>
    {
      //  public PodcastFeed this[int index] { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }

        public bool RemoveFeed(string title)
        {
            PodcastFeed item = this.Find(p => p.Title.Equals(title));
            if (item == null) return false;

            return this.Remove(item);
        }

        public bool RemoveEpisode(PodcastFeed feed, string title)
        {
            PodcastEpisode item = feed.Items.Find(p => p.Title.Equals(title));
            if (item == null) return false;

            return feed.Items.Remove(item);
        }

        public bool RemoveEpisode(string feedTitle, string episodeTitle)
        {
            PodcastFeed feed = this.Find(p => p.Title.Equals(feedTitle));
            if (feed == null) return false;
            PodcastEpisode item = feed.Items.Find(p => p.Title.Equals(episodeTitle));
            if (item == null) return false;
            return feed.Items.Remove(item);
        }
    }
}

