using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class ProgConsole : Form
    {
        public ProgConsole()
        {
            InitializeComponent();
        }

        public ProgConsole(string s)
        {
            this.Text = s;
            InitializeComponent();
        }



        private void buttonSendCommand_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
