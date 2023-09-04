using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VxnLauncher
{
    public partial class SettingsForm : Form
    {
        public static bool isSettingsFormOpen {  get; set; }
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            isSettingsFormOpen = false;
            this.Visible = false;
        }

        private void button1_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible == true)
            {
                isSettingsFormOpen = true;
            }
            if(this.Visible == false)
            {
                isSettingsFormOpen = false;
            }
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            isSettingsFormOpen = false;
        }
    }
}
