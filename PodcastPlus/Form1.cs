using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json;

namespace PodcastPlus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            PodcastClient pc = new PodcastClient();
            PodcastFeedList feed = (PodcastFeedList)pc.RetrieveFeedAsync(@"C:\Users\Powerspeak\SkyDrive\Programming\ac360.xml");
            //PodcastFeedList feed = pc.RetrieveFeedAsync(@"http://feeds.foxnews.com/podcasts/TalkingPoints");
        

            PodcastFeedList feedCopy = new PodcastFeedList();
            //Random r = new Random();
            int total = feed.Count;
            for (int i = 0; i < total; i++)
            {
                //int num = r.Next(0, total);
                feedCopy.Add(feed[i]);
            }

            bool res = feedCopy.RemoveFeed("The Listening Room");
            res &= feedCopy.RemoveFeed("");
            res &= feedCopy.RemoveFeed("KCRW's Le Show (Harry Shearer)");

            Debug.WriteLine("Remove Results!!" + res);

            bool res = feedCopy.RemoveEpisode("Bill O'Reilly", "Some straight talk about the ObamaCare mess");
            res &= feedCopy.RemoveEpisode("Bill O'Reilly", "The people vs. the establishment");
            Debug.WriteLine("Remove Results!!" + res + " " + feedCopy.FindFeed("Bill O'Reilly").Items.Count);


            string output = JsonConvert.SerializeObject(feedCopy);
            //AC360 Daily Podcast 11/19/2013
            PodcastFeedList deserializedProduct = JsonConvert.DeserializeObject<PodcastFeedList>(output);

            // "KCRW's Le Show (Harry Shearer)"

            Debug.WriteLine("Output :" + output);


            for (int i = 0; i < deserializedProduct.Count; i++)
            {
                Debug.WriteLine("deserialized :" + deserializedProduct[i].Title);
            }
        }
    }
}
