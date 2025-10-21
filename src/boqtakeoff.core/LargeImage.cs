using System;
using System.Windows.Forms;

namespace boqtakeoff.core
{
    public partial class LargeImage : Form
    {
        public string ImageURL{ get; set; }
        public LargeImage(string imageURL)
        {
            InitializeComponent();
            ImageURL = imageURL;
        }

        private void LargeImage_Load(object sender, EventArgs e)
        {
            BackgroundImage = System.Drawing.Image.FromFile(ImageURL);
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        }
    }
}
