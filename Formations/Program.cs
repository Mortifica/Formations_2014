#region Using Statements
using Formations.Connection;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
#endregion

namespace Formations
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main()
        {
            string message = "Run the server?";
            string caption = "Server";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            // The person wants to run the server
            if (result == DialogResult.Yes)
            {
                // Eventually, build this into the Web.config file so the game will grab the Server IP from that, letting the server move around if needed...
                ServerManager sm = new ServerManager();

                // TODO: Make sure that you have the Formations Output Type: set to Console to see debug info... Will fix this with different server code.
                sm.Run();
            }
            else
            {
                using (var game = new Formations())
                {
                    game.Run();
                }
            }
        }
    }
#endif
}
