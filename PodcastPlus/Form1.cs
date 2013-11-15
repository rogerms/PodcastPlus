using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace PodcastPlus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            PodcastClient pc = new PodcastClient();
            List<PodcastFeed> feed = pc.RetrieveFeedAsync(@"C:\Users\Powerspeak\SkyDrive\Programming\Podcast.xml");

            Debug.WriteLine("Title :" + feed.Count);
        }
    }
}
