using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Formations
{
    public partial class Login : Form
    {
        private Lobby lobby;
        public Login()
        {
            InitializeComponent();
            this.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(nameText.Text == "" || passwordText.Text == "")
            {
                Error.Text = "You need to enter both\n your name and password!";
            }
            else
            {
                lobby = new Lobby(nameText.Text, passwordText.Text);
                this.Visible = false;
            }
        }
    }
}
