using boqtakeoff.core;

namespace tester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getProjectNameForm form1 = new getProjectNameForm();
            form1.ShowDialog();
        }
    }
}