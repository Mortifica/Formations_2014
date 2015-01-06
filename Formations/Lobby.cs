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
    public partial class Lobby : Form
    {
        private Person person;
        public Lobby(String name, String password)
        {
            person = new Person(name, password);
            InitializeComponent();
            this.Visible = true;
        }

        private void Lobby_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            using (var game = new Formations())
            //using (var db = new PlayerContext()) 
            {
                game.Run();
                Console.Write("New Player Database has been created.");
                //var player1 = new Player { playerName = "missmagdalene" };

                //db.Players.Add(player1);
                //db.SaveChanges();
                Console.Write("Player 1 has been added to the database.");
            }
        }
    }
}
