using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using System.Timers;
using System.Threading;
using Formations.Connection;
using System.Runtime.Serialization;

namespace Formations
{
    
    public class Chat
    {
        public ConnectionManager connectionManager;
        private static readonly int WINDOW_WIDTH = 300;
        private static readonly int WINDOW_HEIGHT = 430;
        private Manager uiManager;

        private bool isVisible;

        // GUI Stuff
        private Panel chatPanel;
        private ScrollBar chatScrollbar;
        public TextBox chatHistoryTextbox;
        private TextBox inputTextBox;
        private Button chatSendButton;
        private System.Timers.Timer timer;

        private Boolean sliding;

        public Chat() { }

        public void init(Manager manager)
        {
            uiManager = manager;
            manager.SetSkin(new Skin(uiManager, "Blue"));

            chatPanel = new Panel(uiManager);
            chatScrollbar = new ScrollBar(uiManager, Orientation.Vertical);
            chatHistoryTextbox = new TextBox(uiManager);
            inputTextBox = new TextBox(uiManager);
            chatSendButton = new Button(uiManager);

            // Main chat panel
            chatPanel.SetSize(WINDOW_WIDTH, WINDOW_HEIGHT);

            // Scrollbar
            chatScrollbar.SetSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            chatScrollbar.SetPosition(0, 0);

            // History text area
            chatHistoryTextbox.SetSize(300, 400);
            chatHistoryTextbox.SetPosition(0, 0);
            chatHistoryTextbox.WordWrap = true;
            chatHistoryTextbox.Mode = TextBoxMode.Multiline;
            chatHistoryTextbox.ReadOnly = true;
            chatHistoryTextbox.CaretVisible = false;
            chatHistoryTextbox.TextColor = Color.Azure;

            // Input box
            inputTextBox.SetSize(370, 30);
            inputTextBox.SetPosition(0, 400);
            inputTextBox.KeyDown += new KeyEventHandler(this.handleTextInput);

            // Send button
            chatSendButton.SetSize(80, 30);
            chatSendButton.SetPosition(220, 400);
            chatSendButton.Text = "Send";
            chatSendButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.sendMessage);

            // Now initialize them all
            chatPanel.Init();
            chatScrollbar.Init();
            chatHistoryTextbox.Init();
            inputTextBox.Init();
            chatSendButton.Init();

            // Add all the components to the gui manager
            chatScrollbar.Add(chatHistoryTextbox);
            chatPanel.Add(chatScrollbar);
            chatPanel.Add(inputTextBox);
            chatPanel.Add(chatSendButton);

            connectionManager = ConnectionManager.getInstance();
        }

        public void toggle(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (this.isVisible)
            {
                hide();
            }
            else
            {
                show();
            }
        }
        public Chat getChat()
        {
            return this;
        }
        public bool chatIsVisible()
        {
            return isVisible;
        }


        private void show()
        {
            if (!sliding)
            {
                chatPanel.SetPosition(-WINDOW_WIDTH, 120);
                timer = new System.Timers.Timer(50);
                timer.Elapsed += windowSlideRight;
                timer.Start();

            }
        }

        private void hide()
        {
            if (!sliding)
            {
                timer = new System.Timers.Timer(50);
                timer.Elapsed += windowSlideLeft;
                timer.Start();

            }
        }

        private void windowSlideLeft(object sender, ElapsedEventArgs e)
        {

            sliding = true;

            while (chatPanel.Left > -300)
            {
                Thread.Sleep(2);
                chatPanel.Left--;
            }

            sliding = false;
            isVisible = false;
            timer.Stop();
        }

        private void windowSlideRight(object sender, ElapsedEventArgs e)
        {
            uiManager.Add(chatPanel);
            isVisible = true;
            sliding = true;

            while (chatPanel.Left < 8)
            {
                Thread.Sleep(2);
                chatPanel.Left++;
            }

            sliding = false;
            timer.Stop();
        }

        private void handleTextInput(object sender, TomShane.Neoforce.Controls.KeyEventArgs e)
        {
            if (e.Key == Keys.Enter)
            {
                this.sendMessage(sender, e);
            }
        }

        private void sendMessage(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            connectionManager.sendMessagePlayer(inputTextBox.Text);

            inputTextBox.Text = "";
        }
    }
}
