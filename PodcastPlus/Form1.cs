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
            List<PodcastFeed> feed = pc.RetrieveFeedAsync(@"C:\Users\Powerspeak\SkyDrive\Programming\Podcast.xml");

            List<PodcastFeed> feedCopy = new List<PodcastFeed>();
            Random r = new Random();
            int total = feed.Count;
            for (int i = 0; i < 5; i++)
            {
                int num = r.Next(0, total);
                feedCopy.Add(feed[num]);
            }
            string output = JsonConvert.SerializeObject(feedCopy);

            List<PodcastFeed> deserializedProduct = JsonConvert.DeserializeObject<List<PodcastFeed>>(output);

            Debug.WriteLine("Output :" + output);

            for (int i = 0; i < 5; i++)
            {
                Debug.WriteLine("deserialized :" + deserializedProduct[i].Title);
            }
        }
    }
}
