using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TomShane.Neoforce.Controls;

namespace Formations
{

    class GameLogin
    {
        
        private Formations formation;
        
        private Window loginWindow;
        
        private Button submitButton;
        
        private Label title;
        
        private Label error;
        
        private Label nameLabel;
        
        private Label passwordLabel;
        
        private TextBox nameText;
        
        private TextBox passwordText;
        
        private Manager uiManager;
        
        private bool _loggedIn = false;
        public bool isLoggedIn 
        { 
            get 
            { 
                return _loggedIn; 
            } 
            private set 
            { 
                _loggedIn = value; 
            } 
        }
        public GameLogin(Manager uiManager)
        {
            this.uiManager = uiManager;
        }
        public void init(Formations formation)
        {
            this.formation = formation;
            loginWindow = new Window(uiManager);
            loginWindow.SetSize(200, 200);
            loginWindow.SetPosition(550, 200);
            loginWindow.Visible = true;
            loginWindow.Movable = false;
            loginWindow.Resizable = false;
            loginWindow.CloseButtonVisible = false;
            loginWindow.Text = "Login";
            loginWindow.Init();

            title = new Label(uiManager);
            title.SetPosition(60, 0);
            title.SetSize(100,40);
            title.Text = "FORMATIONS" +
                         "\n    LOGIN  ";
            title.Init();

            error = new Label(uiManager);
            error.SetPosition(35, 30);
            error.SetSize(150, 40);
            error.Text = " Please enter both\n" +
                         "Name and Password";
            error.TextColor = Color.Gray;
            error.Init();

            nameLabel = new Label(uiManager);
            nameLabel.SetPosition(20, 80);
            nameLabel.SetSize(60, 20);
            nameLabel.Text = "Name";
            nameLabel.Init();

            passwordLabel = new Label(uiManager);
            passwordLabel.SetPosition(20, 105);
            passwordLabel.SetSize(60, 20);
            passwordLabel.Text = "Password";
            passwordLabel.Init();
            
            submitButton = new Button(uiManager);
            submitButton.SetSize(50, 20);
            submitButton.SetPosition(70, 140);
            submitButton.Text = "Submit";
            submitButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.submitClicked);
            submitButton.Init();
            

            nameText = new TextBox(uiManager);
            nameText.SetPosition(80, 80);
            nameText.SetSize(100, 20);
            nameText.KeyDown += new KeyEventHandler(this.handleTextInput);
            nameText.Init();
            

            passwordText = new TextBox(uiManager);
            passwordText.SetPosition(80, 105);
            passwordText.SetSize(100, 20);
            passwordText.KeyDown += new KeyEventHandler(this.handleTextInput);
            passwordText.Init();
            

            loginWindow.Add(title);
            loginWindow.Add(error);
            loginWindow.Add(nameLabel);
            loginWindow.Add(passwordLabel);
            loginWindow.Add(nameText);
            loginWindow.Add(passwordText);
            loginWindow.Add(submitButton);

            uiManager.Add(loginWindow);
        }

        private void handleTextInput(object sender, KeyEventArgs e)
        {
            error.TextColor = Color.Gray;
            if (e.Key == Keys.Enter && sender.Equals(passwordText))
            {
                System.Console.WriteLine("password");
                this.submitClicked(sender, e);
            }
            else if (e.Key == Keys.Enter && sender.Equals(nameText))
            {
                System.Console.WriteLine("name");
                this.submitClicked(sender, e);
            }
            else if (e.Key == Keys.Tab)
            {
                
                if(nameText.Focused)
                {
                    passwordText.SelectAll();
                }
                else
                {
                    nameText.SelectAll();
                }
            }
        }
        private void submitClicked(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (nameText.Text == "" || passwordText.Text == "")
            {
                error.TextColor = Color.Red;
            }
            else
            {
                Person person = new Person(nameText.Text, passwordText.Text);
                formation.setPerson(person);
                uiManager.Remove(loginWindow);
                isLoggedIn = true;
            }
        }
    }
}
