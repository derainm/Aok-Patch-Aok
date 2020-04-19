using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Aok_Patch.patcher_
{
    public partial class SlpViewer : Form
    {
        public List<string> lstbitemap;
        public SlpViewer()
        {
            InitializeComponent();
        }
        public string ImagNameFile;
        int counter = 1;
        FolderBrowserDialog fbd = new FolderBrowserDialog();
        bool playing = true;
        public SlpViewer(List<string> lst)
        {
            InitializeComponent();
            lstbitemap = new List<string>();
            lstbitemap = lst;
            playing = true;

            lstbitemap = lst;
        }
        public void Play()
        {
            timer1.Start();
            playing = true;

        }
        public void  stop()
        {
            timer1.Stop();
            playing = false;
        }
        public Image byteArrayToImage(byte[] bytesArr)
        {
            using (MemoryStream memstr = new MemoryStream(bytesArr))
            {
                Image img = Image.FromStream(memstr);
                return img;
            }
        }
        private void SlpViewer_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            counter++;
            string[] images = lstbitemap.ToArray();// Directory.GetFiles(fbd.SelectedPath, "*.*");
            if (counter > images.Length - 1)
            {
                counter = 0;
            }
            //we do that because  Image.FromFile acces to image to write and cause a bug.
            byte[] imgByte = File.ReadAllBytes(images[counter]);
            Image img = byteArrayToImage(imgByte);
            ((Bitmap)img).MakeTransparent(Color.FromArgb(180, 255, 180));
            pictureBox1.Image = img;
            //filename.Text = images[counter]; //show name of the image
        }
    }
}
