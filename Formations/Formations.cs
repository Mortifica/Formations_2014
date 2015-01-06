using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Importers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using TomShane.Neoforce.Controls;

namespace Formations
{
    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Formations : Game, IMouseListener, IKeyboardListener
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        IGame gb;
        private Person _person;
        private bool isGameStarted = false;
        private GameLogin login;
        private GameLobby gameLobby;
        public static List<Texture2D> attackTextures;
        public static List<Texture2D> bloodTextures;
        public static List<Texture2D> healingTextures;

        public static Texture2D attackSprite;
        public static Texture2D defenderSprite;
        public static Texture2D healerSprite;

        public static List<Texture2D> unitTextures;
        //MouseState mouseState;
        MouseListener mouseListener;

        private SpriteFont font;
        public static SpriteFont damageFont;

        // Neoforce GUI manager
        private Manager theManager{ get; set; }
        public Person person 
        { 
            get 
            {
                return _person; 
            } 
            set 
            { 
                _person = value; 
            } 
        }

        public Formations()
            : base()
        {
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            theManager = new Manager(this, graphics, "Default");
            theManager.AutoCreateRenderTarget = true;
            theManager.TargetFrames = 60;
            theManager.LogUnhandledExceptions = false;
            theManager.ShowSoftwareCursor = true;
            

            graphics.PreferredBackBufferWidth = 1200;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 600;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
            var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            mouseListener = new MouseListener(mouseState, this);
            theManager.Initialize();
            theManager.SetSkin(new Skin(theManager, "Blue"));

            login = new GameLogin(theManager);
            login.init(this);
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("spriteFont");
            damageFont = Content.Load<SpriteFont>("DamageFont");

            attackTextures = new List<Texture2D>();
            attackTextures.Add(Content.Load<Texture2D>("axe"));
            attackTextures.Add(Content.Load<Texture2D>("sword2"));
            attackTextures.Add(Content.Load<Texture2D>("sword3"));

            bloodTextures = new List<Texture2D>();
            bloodTextures.Add(Content.Load<Texture2D>("bloodparticle"));

            healingTextures = new List<Texture2D>();
            healingTextures.Add(Content.Load<Texture2D>("heart"));
            healingTextures.Add(Content.Load<Texture2D>("heart2"));
            healingTextures.Add(Content.Load<Texture2D>("star2"));

            unitTextures = new List<Texture2D>();
            attackSprite = Content.Load<Texture2D>("attackunit");
            defenderSprite = Content.Load<Texture2D>("defendunit");
            healerSprite = Content.Load<Texture2D>("magicunit");

            unitTextures.Add(attackSprite);
            unitTextures.Add(defenderSprite);
            unitTextures.Add(healerSprite);



            // TODO: use this.Content to load your game content here
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        internal void setPerson(Person person)
        {
            this.person = person;
            createLobby();
            //newGame();
        }
       public void challengePerson(IGame game)
        {
            newGame(game);
        }
       public void endGame()
       {
           isGameStarted = false;
           Thread.Sleep(50);
           gb = null;
           gameLobby.lobbyReconnect();
       }
        private void createLobby(){
            gameLobby = GameLobby.getInstance();
            gameLobby.init(this, theManager, person);
            
        }
        private void newGame(IGame game)
        {
            gb = game;
            gb.init(theManager, GraphicsDevice, this , "Formations", true, unitTextures);
            mouseListener.startListener();
            isGameStarted = true;
            
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (login.isLoggedIn && isGameStarted)
            {
                gb.update();
                //mouseListener.update(mouseState);
            }
            else if(gameLobby != null)
            {
                gameLobby.update();
            }
            theManager.Update(gameTime);
            base.Update(gameTime);

            
        }
        public void mousePressed(MouseState mouseState)
        {
            if (gb != null)
            {
                gb.mousePressed(mouseState);
            }
            
        }
        public void mouseReleased(MouseState mouseState)
        {
            if (gb != null)
            {
                gb.mouseReleased(mouseState);
            }
        }
        public void mouseDragged(MouseState mouseState)
        {
            if (gb != null)
            {
                gb.mouseDragged(mouseState);
            }
        }
        public void mouseMoved(MouseState mouseState)
        {
            if (gb != null)
            {
                gb.mouseMoved(mouseState);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            
            // TODO: Add your drawing code here
            
            theManager.BeginDraw(gameTime);
            GraphicsDevice.Clear(Color.DarkSlateGray);
            spriteBatch.Begin();
            if (login.isLoggedIn && isGameStarted)
            {
                gb.draw(spriteBatch);
            }
            else if (gameLobby != null)
            {
                gameLobby.draw(spriteBatch);
            }
            spriteBatch.End();
            theManager.EndDraw();

            base.Draw(gameTime);

        }


    }
}
