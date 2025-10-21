using AU2013;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cbb.core
{
    public partial class Form1 : Form
    {
        public string selected_project_name;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.DialogResult = DialogResult.OK;
            Debug.WriteLine("Close button was clicked");
            Close();
            return;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            selected_project_name = comboBox1.SelectedItem.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Get the Project Unique names
            string access_token = AU2013.CreatorAPIRequest.get_access_token();
            List<getProjectDetails> project_uid_list = AU2013.CreatorAPIRequest.getProjectNameList(access_token);
            // Add the list to the combo box
            foreach (getProjectDetails curr_project_data in project_uid_list)
            {
                comboBox1.Items.Add(curr_project_data.project_name);
            }

        }
    }
}
