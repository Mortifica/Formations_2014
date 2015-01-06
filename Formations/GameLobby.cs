using Formations.Connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TomShane.Neoforce.Controls;

namespace Formations
{

    class GameLobby : IUpdateDraw
    {

        private static readonly int WINDOW_WIDTH = 300;
        private static readonly int WINDOW_HEIGHT = 430;
        private ConnectionManager connectionManager;
        private static GameLobby gameLobbyInstance;
        private Person _person;
        private Formations formation;
        private Manager uiManager;
        private bool endTurnIsVisible = false;
        private ChallengeRequest _currentRequest;
        // GUI Stuff
        private Panel chatPanel;
        private ScrollBar chatScrollbar;
        public TextBox chatHistoryTextbox;
        private TextBox inputTextBox;
        public ListBox playerlist;
        //private Button endTurn;
        private Window acceptWindow;
        private Button acceptButton;
        private Button unacceptButton;
        private Button chatSendButton;
        private Button challengeButton;
        private Button singlePlayerGameButton;
        private int count = 1;

        private AttackParticleEngine attackParticleEngine;
        private HealingParticleEngine healingParticleEngine;

        public Person person 
        { 
            get 
            { 
                return _person; 
            }
            private set 
            {
                _person = value; 
            }
        }
        public ChallengeRequest CurrentRequest
        {
            get { return _currentRequest; }
            set { _currentRequest = value; }
        }
        private GameLobby()
        {
            
        }
        public static GameLobby getInstance()
        {
            if (gameLobbyInstance == null)
            {
                gameLobbyInstance = new GameLobby();
            }
            return gameLobbyInstance;
        }
        public void init(Formations formation, Manager uiManager, Person person)
        {
            this.formation = formation;
            this.uiManager = uiManager;
            this.person = person;

            playerlist = new ListBox(uiManager);
            playerlist.SetPosition(900, 8);
            playerlist.SetSize(300, 200);
            
            playerlist.Init();

            challengeButton = new Button(uiManager);
            challengeButton.SetPosition(900, 220);
            challengeButton.SetSize(100, 28);
            challengeButton.Text = "Challenge";
            challengeButton.BackColor = Color.Indigo; 
            challengeButton.Click += challengeButton_Click;



            chatPanel = new Panel(uiManager);
            chatScrollbar = new ScrollBar(uiManager, Orientation.Vertical);
            chatHistoryTextbox = new TextBox(uiManager);
            inputTextBox = new TextBox(uiManager);
            chatSendButton = new Button(uiManager);



            // Main chat panel
            chatPanel.SetSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            //chatPanel.SetPosition(800, 10);

            // Scrollbar
            chatScrollbar.SetSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            chatScrollbar.SetPosition(0, 0);

            // History text area
            chatHistoryTextbox.SetSize(600, 400);
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
            /*
            * End Turn Button/Window
             */
            //endTurn = new Button(uiManager);
           // endTurn.SetPosition(10, 150);
           // endTurn.Click += new TomShane.Neoforce.Controls.EventHandler(this.toggleChallengeAccept);
            //endTurn.Text = "EndTurn";
            acceptButton = new Button(uiManager);
            //endYesButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.newTurn);
            acceptButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.toggleChallengeAccept);
            acceptButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.challengeAccept);
            acceptButton.Text = "Accept";
            acceptButton.SetPosition(0, 0);
            acceptButton.SetSize(100, 100);
            unacceptButton = new Button(uiManager);
            unacceptButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.toggleChallengeAccept);
            unacceptButton.Text = "Unaccept";
            unacceptButton.SetPosition(0, 100);
            unacceptButton.SetSize(100, 100);
            acceptWindow = new Window(uiManager);
            acceptWindow.SetSize(200, 235);
            acceptWindow.SetPosition(500, 250);
            acceptWindow.Text = "";
            acceptWindow.Shadow = true;
            acceptWindow.CloseButtonVisible = false;
            acceptWindow.Add(acceptButton);
            acceptWindow.Add(unacceptButton);

            singlePlayerGameButton = new Button(uiManager);
            singlePlayerGameButton.Click += singlePlayerGameButton_Click;
            singlePlayerGameButton.Text = "Single Player Game";
            singlePlayerGameButton.SetPosition(400, 300);
            singlePlayerGameButton.SetSize(200, 100);


            // Add all the components to the gui manager
            chatScrollbar.Add(chatHistoryTextbox);
            chatPanel.Add(chatScrollbar);
            chatPanel.Add(inputTextBox);
            chatPanel.Add(chatSendButton);

            //uiManager.Add(endTurn);
            uiManager.Add(singlePlayerGameButton);
            uiManager.Add(chatPanel);
            uiManager.Add(challengeButton);
            uiManager.Add(playerlist);

            connectionManager = ConnectionManager.getInstance();

            //attackParticleEngine = new AttackParticleEngine();
            healingParticleEngine = new HealingParticleEngine(Formations.healingTextures, new Vector2(0, 200));

        }

        void singlePlayerGameButton_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            formation.challengePerson(new GameBoardSinglePlayer());
            //uiManager.Remove(endTurn);
            uiManager.Remove(singlePlayerGameButton);
            uiManager.Remove(chatPanel);
            uiManager.Remove(challengeButton);
            uiManager.Remove(playerlist);
        }
        public bool isGameStarted()
        {
            bool result = false;
            if (connectionManager != null)
            {
               result = connectionManager.isConnectedToPlayer();
            }
            return result;
        }
        public void lobbyReconnect()
        {
            uiManager.Add(singlePlayerGameButton);
            uiManager.Add(chatPanel);
            uiManager.Add(challengeButton);
            uiManager.Add(playerlist);
        }
        public void showChallengeAccept()
        {
            uiManager.Add(acceptWindow);
            endTurnIsVisible = true;
        }

        public void hideChallengeAccept()
        {
            uiManager.Remove(acceptWindow);
            endTurnIsVisible = false;
        }
        private void toggleChallengeAccept(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (endTurnIsVisible)
            {
                hideChallengeAccept();
            }
            else
            {
                showChallengeAccept();
            }
        }
        private void challengeAccept(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CurrentRequest.IsAccepted = true;
            connectionManager.sendChallengeRequect(CurrentRequest);

        }
        void challengeButton_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (playerlist.ItemIndex != -1)
            {
                Person temp = (Person)playerlist.Items.ElementAt<object>(playerlist.ItemIndex);//grabs the person that was selected
                connectionManager.sendChallengeRequect(new ChallengeRequest(person, temp));
            }
        }
        public void sendChallengeRequest(Person person)
        {
            uiManager.Remove(playerlist);
            uiManager.Remove(challengeButton);
            formation.challengePerson(new GameBoard());
        }
        public void AcceptChallengeWindowOpen(ChallengeRequest request)
        {
                acceptWindow.Text = "Challenge from " + request.Sender.Name;
                showChallengeAccept();
                CurrentRequest = request;
        }
        public void updatePlayersList(Person newPerson)
        {
            if (!playerlist.Items.Contains(newPerson))
            {
                playerlist.Items.Add(newPerson);
            }
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
            connectionManager.sendMessageServer(inputTextBox.Text);
            inputTextBox.Text = "";
        }

        public void connectToServer()
        {
            //connects back to the server with already logged in Person when game is over
        }
        public void disconnectFromServer()
        {
            //disConnects from server when challenge is accepted
        }


        public void update()
        {
            //healingParticleEngine = new HealingParticleEngine(Formations.healingTextures, new Vector2(500, 200));

            if (CurrentRequest != null && CurrentRequest.IsAccepted)
            {
                formation.challengePerson(new GameBoard());
                //uiManager.Remove(endTurn);
                uiManager.Remove(singlePlayerGameButton);
                uiManager.Remove(chatPanel);
                uiManager.Remove(challengeButton);
                uiManager.Remove(playerlist);
            }
            if (count >= 10000)
            {
                playerlist.Items.Clear();
                count = 1;
            }
            if (count % 250 == 0)
            {
                connectionManager.sendPerson(person);
            }
            count++;

            healingParticleEngine.Update();
        }

        public void draw(SpriteBatch spriteBatch)
        {
            //throw new NotImplementedException();

            healingParticleEngine.Draw(spriteBatch);
        }
    }
}
