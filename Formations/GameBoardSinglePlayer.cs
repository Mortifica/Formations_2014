using Formations.Connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using TomShane.Neoforce.Controls;

namespace Formations
{
    
    public class GameBoardSinglePlayer : IGame
    {
        
        private Formations formation;
        private ConnectionManager connectionManager = ConnectionManager.getInstance();
        private Player[] players = new Player[2];
        private Manager uiManager;
        private string gameName;
        private Vector2 gameNameLocation = new Vector2(500, 10);
        private int movesLeftInPhase = 6;
        private bool isHost;
        private bool isHostsTurn = true;
        private bool isFirstPhase = true;
        private bool attackInProgress = false;
        private bool moveInProgress = false;
        private bool magicInProgress = false;
        private bool isSmallBoard = false;
        private bool attPlacementInProgress = false;
        private bool defPlacementInProgress = false;
        private bool magPlacementInProgress = false;
        private bool endTurnIsVisible = false;
        private MouseState currentMouseState;
        private Label hexInfo;
        private Label gameInfo;
        private Label gameNameLabel;
        private Label phaseLabel;
        private Hexagon turnSignal;
        private Hexagon attUnit;
        private Hexagon defUnit;
        private Hexagon magUnit;
        private Hexagon attHex;
        private Hexagon defHex;
        private Hexagon magHex;
        private Hexagon attAction;
        private Hexagon magicAction;
        private Hexagon moveAction;
        private TileBasic currentTile;
        private int unitSideLength;
        private const int boardHeight = 10;
        private const int boardWidth = 19;
        private int largeTileSideLength = 30;
        private int smallTileSideLength = 15;
        private float largeBoardOffsetX = 130;
        private float largeBoardOffsetY = 130;
        private float smallBoardOffsetX = 660;
        private float smallBoardOffsetY = 357;
        private float xTileOffset = 27.5F;
        private float xAdjustment = 55;
        private float yAdjustment = 47;
        private float changeInX;
        private float changeInY;
        private TileBasic[,] tiles = new TileBasic[boardWidth, boardHeight];
        private VertexPositionColor[] vertices = new VertexPositionColor[6];
        private VertexPositionColor[] borderLines = new VertexPositionColor[8];
        private BasicEffect basicEffect;

        public List<Texture2D> unitTextures;
        
        // Various buttons
        private Button resizeButton;
        private Button endTurn;
        private Window endTurnWindow;
        private Button endYesButton;
        private Button endNoButton;

        private Button endGameButton;
        private Window endingPanel;
        private Label endGameLabel;
        

        /// <summary>
        /// Default Constructor
        /// creates TilesArray for the Board
        /// </summary>
        public GameBoardSinglePlayer()
        {
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
               {
                   tiles[i, j] = new TileBasic(largeTileSideLength);
               }
            }
            unitSideLength = largeTileSideLength / 2;
        }
        /// <summary>
        /// Initalizer for this object
        /// </summary>
        /// <param name="uiManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="font"></param>
        /// <param name="gameName"></param>
        public override void init(Manager uiManager, GraphicsDevice graphicsDevice, Formations formation, string gameName, bool isHost, List<Texture2D> unitTextures)
        {
            this.gameName = gameName;
            this.isHost = isHost;
            this.uiManager = uiManager;
            this.formation = formation;
            this.unitTextures = unitTextures;
            players[0] = new Player(true);
            players[1] = new Player(false);
            /*
             * need to pass in the player from the connection here to create it maybe
             */
           // if (isHost)
           // {
            players[0].init("Player 1", createUnitArray(10, 5, 5), graphicsDevice, uiManager, unitTextures);
            players[1].init("Player 2", createUnitArray(10, 5, 5), graphicsDevice, uiManager, unitTextures);
                players[0].isPlayersTurn = true;
                players[1].isPlayersTurn = false;
                
           // }
           // else
           // {
           //     players[0].init("<GuestNameHere>", createUnitArray(10, 5, 5), graphicsDevice, uiManager);
           //     players[1].init("<PlayerNameHere>", createUnitArray(10, 5, 5), graphicsDevice, uiManager);
           // }
            /*
             * setting up the graphics here
             */ 
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, graphicsDevice.Viewport.Width,       // left, right
                graphicsDevice.Viewport.Height, 0,      // bottom, top
                0, 1);                                  // near, far plane
            
            /*
             *setting up the board area
             */ 
            createBoardArea();
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (j % 2 == 0)
                    {
                        tiles[i, j].init(largeBoardOffsetX + (i * xAdjustment), largeBoardOffsetY + (j * yAdjustment), graphicsDevice);
                    }
                    else
                    {
                        tiles[i, j].init(largeBoardOffsetX + xTileOffset + (i * xAdjustment), largeBoardOffsetY + (j * yAdjustment), graphicsDevice);
                    }
                }
            }
            /*
             * runs through the board setting the surrounding tiles for each tile
             */ 
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    tiles[i, j].initTileArray(tiles, boardWidth, boardHeight, i, j);
                }
            }
            /*
             * finding the hrizontal distance change of a tile
             */ 
            changeInX = (float)Math.Sqrt((float)(largeTileSideLength * largeTileSideLength) - (float)(largeTileSideLength / 2) * (float)(largeTileSideLength / 2));
            changeInY = (float)(largeTileSideLength / 2);
            /*
             * setting up buttons and shapes
             */ 
            turnSignal = new Hexagon(20);
            attUnit = new Hexagon(unitSideLength);
            defUnit = new Hexagon(unitSideLength);
            magUnit = new Hexagon(unitSideLength);
            attAction = new Hexagon(unitSideLength);
            moveAction = new Hexagon(unitSideLength);
            magicAction = new Hexagon(unitSideLength);
            /*
             * initiallizing the Hexagons
             */
            Color tempColor;

            //System.Console.WriteLine("is this even working");

            if (isHostsTurn)
            {
                tempColor = GameColors.HostControlOutsideColor;
                System.Console.WriteLine("Is hosts turn");
            }
            else
            {
                tempColor = GameColors.guestControlOutsideColor;
                System.Console.WriteLine("Is guests turn");
            }
            
            turnSignal.init(600,50, graphicsDevice, GameColors.turnButtonInsideColor, GameColors.turnButtonOutsideColor);
            attUnit.init(0, 0, graphicsDevice, tempColor, tempColor);
            magUnit.init(0, 0, graphicsDevice, tempColor, tempColor);
            defUnit.init(0, 0, graphicsDevice, tempColor, tempColor);
            attAction.init(0, 0, graphicsDevice, tempColor, tempColor);
            moveAction.init(0, 0, graphicsDevice, tempColor, tempColor);
            magicAction.init(0, 0, graphicsDevice, tempColor, tempColor);

            attHex = new Hexagon(20);
            defHex = new Hexagon(20);
            magHex = new Hexagon(20);

            attHex.init(40, 200, graphicsDevice, tempColor, tempColor);
            defHex.init(40, 245, graphicsDevice, tempColor, tempColor);
            magHex.init(40, 290, graphicsDevice, tempColor, tempColor);
            /*
             * Resize Button
             */ 
            resizeButton = new Button(uiManager);
            resizeButton.SetPosition(10, 120);
            resizeButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.resizeBoard);
            resizeButton.Text = "Small Map";
            /*
             * End Turn Button/Window
             */ 
            endTurn = new Button(uiManager);
            endTurn.SetPosition(10, 150);
            endTurn.Click += new TomShane.Neoforce.Controls.EventHandler(this.toggleEndTurn);
            endTurn.Text = "EndTurn";
            endYesButton = new Button(uiManager);
            endYesButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.newTurn);
            endYesButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.toggleEndTurn);
            endYesButton.Text = "Yes";
            endYesButton.SetPosition(0, 0);
            endYesButton.SetSize(100, 100);
            endNoButton = new Button(uiManager);
            endNoButton.Click += new TomShane.Neoforce.Controls.EventHandler(this.toggleEndTurn);
            endNoButton.Text = "No";
            endNoButton.SetPosition(0, 100);
            endNoButton.SetSize(100, 100);
            endTurnWindow = new Window(uiManager);
            endTurnWindow.SetSize(114,235);
            endTurnWindow.SetPosition(500, 250);
            endTurnWindow.Text = "  End Turn?";
            endTurnWindow.Shadow = true;
            endTurnWindow.CloseButtonVisible = false;
            endTurnWindow.Add(endYesButton);
            endTurnWindow.Add(endNoButton);
            /*
             * Game Name Label
             */ 
            gameNameLabel = new Label(uiManager);
            gameNameLabel.SetPosition(550,10);
            gameNameLabel.Text = gameName;
            gameNameLabel.SetSize(200,10);
            /*
             * phase label
             */
            phaseLabel = new Label(uiManager);
            phaseLabel.SetPosition(550, 70);
            phaseLabel.SetSize(200,20);
            phaseLabel.Text = "Phase 1 - Land Grab";
            /*
             * ending box
             */ 
            endingPanel = new Window(uiManager);
            endingPanel.BackColor = Color.Aqua;
            endingPanel.SetPosition(300, 200);
            endingPanel.SetSize(400, 300);
            endingPanel.Text = "Game Over";

            endGameButton = new Button(uiManager);
            endGameButton.SetPosition(100, 200);
            endGameButton.SetSize(200, 100);
            endGameButton.Text = "Lobby";
            endGameButton.Click += endGameButton_Click;
            endGameLabel = new Label(uiManager);
            endGameLabel.SetPosition(100, 50);
            endGameLabel.SetSize(200, 100);
            endingPanel.Init();
            endGameLabel.Init();
            endNoButton.Init();
            endingPanel.Add(endGameLabel);
            endingPanel.Add(endGameButton);

            uiManager.Add(endTurn);
            uiManager.Add(resizeButton);
            uiManager.Add(gameNameLabel);
            uiManager.Add(phaseLabel);
            //uiManager.Add(endingPanel);
        }

        private void endGameButton_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {   
            uiManager.Remove(endingPanel);
            uiManager.Remove(endTurn);
            uiManager.Remove(resizeButton);
            uiManager.Remove(gameNameLabel);
            uiManager.Remove(phaseLabel);
            uiManager.Remove(endingPanel);
            players[0].removeUI();
            players[1].removeUI();
            formation.endGame();
            gameDone = true;


        }
        /// <summary>
        /// Creates the UnitAbstract Array with the correct number of attack units defense units and Manipulation units
        /// </summary>
        /// <param name="numberAtt">The correct number of Attack units to add to the UnitAbstract array</param> 
        /// <param name="numberDef">The correct number of Defensive units to add to the UnitAbstract array</param>
        /// <param name="numberMag">The correct number of Manipulation units to add to the UnitAbstract array</param>
        /// <returns></returns>
        private UnitAbstract[,] createUnitArray(int numberAtt, int numberDef, int numberMag)
        {
            UnitAbstract[,] tempArray = new UnitAbstract[3, 20];

            for (int i = 0; i < numberAtt; i++)
            {
                tempArray[0, i] = new UnitAtt();
            }
            for (int i = 0; i < numberDef; i++)
            {
                tempArray[1, i] = new UnitDef();
            }
            for (int i = 0; i < numberMag; i++)
            {
                tempArray[2, i] = new UnitMag();
            }
            return tempArray;
        }
            

        #region - Mouse Methods
        public override void mousePressed(MouseState mouseState)
        {
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                System.Console.WriteLine("Button right");
                resetBools();
            }
            //selecting the correct player  
            Player self;
            self = selectPlayer();

                if (self.AttUnitsNotPlaced > 0 && attHex.IsPointInPolygon(mouseState.X, mouseState.Y))
                {
                    resetBools();
                    attPlacementInProgress = true;
                }
                if (self.DefUnitsNotPlaced > 0 && defHex.IsPointInPolygon(mouseState.X, mouseState.Y))
                {
                    resetBools();
                    defPlacementInProgress = true;
                }
                if (self.MagUnitsNotPlaced > 0 && magHex.IsPointInPolygon(mouseState.X, mouseState.Y))
                {
                    resetBools();
                    magPlacementInProgress = true;
                }
                foreach (TileBasic tile in tiles)
	            {
		 
                    if (tile.isHovered())
                    {
                        tile.mousePressed(mouseState);
                        if (attPlacementInProgress)
                        {
                            if (playerCanSetUnit(tile, self) && self.Stamina >= UnitAtt.STAMINA_PLACE_COST)
                            {
                                tile.setUnit(self.getAttUnit());
                                self.useStamina(UnitAtt.STAMINA_PLACE_COST);
                                makeMove();
                                resetBools();
                            }
                        }
                        if (defPlacementInProgress)
                        {
                            if (playerCanSetUnit(tile, self) && self.Stamina >= UnitDef.STAMINA_PLACE_COST)
                            {
                                tile.setUnit(self.getDefUnit());
                                self.useStamina(UnitDef.STAMINA_PLACE_COST);
                                makeMove();
                                resetBools();
                            }
                        }
                        if (magPlacementInProgress)
                        {
                            if (playerCanSetUnit(tile, self) && self.Stamina >= UnitMag.STAMINA_PLACE_COST)
                            {
                                tile.setUnit(self.getMagUnit());
                                self.useStamina(UnitMag.STAMINA_PLACE_COST);
                                makeMove();
                                resetBools();
                            }
                        }
                        if (attackInProgress)
                        {
                            unitAttackUnit(mouseState, self);
                        }
                        if (moveInProgress)
                        {
                            moveUnit(mouseState, self);
                        }
                        if (magicInProgress)
                        {
                            healUnit(mouseState, self);
                        }
                    }
                
            }
        }
        public override void mouseReleased(MouseState mouseState)
        {
            //selecting the correct player  
            Player self;
            self = selectPlayer();
            //checking board for where the mouse was released

            foreach (TileBasic tile in tiles)
            {
                if (tile.isSelected())
                {
                    tile.mouseReleased(mouseState);
                    //Attack happens here
                    if (tile.hasUnit() && ((isHostsTurn && tile.getUnit().IsHostsUnit)
                        || (!isHostsTurn && !tile.getUnit().IsHostsUnit))
                        && attAction.IsPointInPolygon(mouseState.X, mouseState.Y))
                    {
                        self.SelectedTile = tile;
                        attackInProgress = true;
                        TileBasic[] attackableTiles = tile.getUnit().getAttackableTiles();
                        foreach (TileBasic currentTile in attackableTiles)
                        {
                            if (currentTile != null) { currentTile.setAsAttackArea(); }
                        }
                        return;
                    }
                    //Move happens here
                    if (tile.hasUnit() && ((isHostsTurn && tile.getUnit().IsHostsUnit)
                        || (!isHostsTurn && !tile.getUnit().IsHostsUnit))
                        && moveAction.IsPointInPolygon(mouseState.X, mouseState.Y))
                    {
                        self.SelectedTile = tile;
                        moveInProgress = true;
                        return;
                    }
                    //Magic happens here
                    if (tile.hasUnit() && ((isHostsTurn && tile.getUnit().IsHostsUnit)
                        || (!isHostsTurn && !tile.getUnit().IsHostsUnit))
                        && magicAction.IsPointInPolygon(mouseState.X, mouseState.Y))
                    {
                        self.SelectedTile = tile;
                        magicInProgress = true;
                        return;
                    }
                }

            }

            recalculateControlArea();
        }
        public override void mouseDragged(MouseState mouseState)
        {
            currentMouseState = mouseState;

            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    tiles[i, j].mouseDragged(mouseState);
                }
            }
        }
        public override void mouseMoved(MouseState mouseState)
        {
            currentMouseState = mouseState;


            // if (hexInfo == null)
            // {
            //    hexInfo = new Label(uiManager);
            //    uiManager.Add(hexInfo);
            //    hexInfo.SetSize(150, 25);
            //    hexInfo.TextColor = Color.Black;

            //}

            //hexInfo.SetPosition(mouseState.X, mouseState.Y - 15);
            //setHoverLabel(mouseState);

            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    tiles[i, j].mouseMoved(mouseState);
                }
            }
            
        }
        /// <summary>
        /// Used to determine which players turn it is
        /// </summary>
        /// <returns></returns>
        private Player selectPlayer()
        {
            Player self;
            if ((isHost && isHostsTurn))
            {
                self = players[0];
            }
            else
            {
                self = players[1];
            }
            return self;
        }
        /// <summary>
        /// Resets the current booleans to false
        /// </summary>
        private void resetBools()
        {
            attPlacementInProgress = false;
            defPlacementInProgress = false;
            magPlacementInProgress = false;
            moveInProgress = false;
            attackInProgress = false;
        }

        #endregion - Mouse Methods

        public void showEndTurn()
        {
            uiManager.Add(endTurnWindow);
            endTurnIsVisible = true;
        }

        public void hideEndTurn()
        {
            uiManager.Remove(endTurnWindow);
            endTurnIsVisible = false;
        }
        public void toggleEndTurn(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (endTurnIsVisible)
            {
                hideEndTurn();
            }
            else
            {
                showEndTurn();
            }
        }

        //TODO: change or remove
        private void setHoverLabel(MouseState mouseState)
        {
            if (attAction.IsPointInPolygon(mouseState.X, mouseState.Y))
            {
                hexInfo.Text = "Attack";
            }
            else if (moveAction.IsPointInPolygon(mouseState.X, mouseState.Y))
            {
                hexInfo.Text = "Move";
            }
            else if (magicAction.IsPointInPolygon(mouseState.X, mouseState.Y))
            {
                hexInfo.Text = "Magic";
            }
            else if (attHex.IsPointInPolygon(mouseState.X, mouseState.Y))
            {
                hexInfo.Text = "Set Attack Unit";
            }
            else if (defHex.IsPointInPolygon(mouseState.X, mouseState.Y))
            {
                hexInfo.Text = "Set Defense Unit";
            }
            else if (magHex.IsPointInPolygon(mouseState.X, mouseState.Y))
            {
                hexInfo.Text = "Set Magic Unit";
            }
            else
            {
                hexInfo.Text = "";
                foreach (var tile in tiles)
                {
                    if (tile.isHovered() && tile.hasUnit())
                    {
                        hexInfo.Text = tile.getUnit().getUnitType();
                    }   
                }
            }
        }
        private void healUnit(MouseState mouseState, Player player)
        { 
            Player self = player;
            TileBasic[] surroundingTiles = self.SelectedTile.getUnit().getAttackableTiles();
            UnitMag currentMag = (UnitMag)self.SelectedTile.getUnit();
            for (int i = 1; i < surroundingTiles.Length; i++)
            {//starts on 1 because 0 is the attacker
                if (surroundingTiles[i] != null && surroundingTiles[i].isPointInTile(mouseState) && surroundingTiles[i].hasUnit() && (surroundingTiles[i].getUnit().Player.Equals(self)))
                {
                    if (self.Stamina >= currentMag.calculateAttackCost())
                    {
                        Point attackerPosition = new Point((int)surroundingTiles[0].getX(), (int)surroundingTiles[0].getY());
                        Point defendersPosition = new Point((int)surroundingTiles[i].getX(), (int)surroundingTiles[i].getY());

                        self.useStamina(currentMag.calculateAttackCost());
                        currentMag.heal(surroundingTiles[i].getUnit());
                    }
                }
            }
            magicInProgress = false;
        }
        private void moveUnit(MouseState mouseState, Player player)
        {
            Player self = player;
            TileBasic[] currentSurroundingTiles = self.SelectedTile.getSurroundingTiles();
            for (int i = 1; i < currentSurroundingTiles.Length; i++)
            {//starts on 1 because 0 is the attacker
                if (currentSurroundingTiles[i] != null && currentSurroundingTiles[i].isPointInTile(mouseState))
                {
                    //Console.WriteLine("moveUnit");
                    if (!currentSurroundingTiles[i].hasUnit())
                    {
                        if (self.Stamina >= currentSurroundingTiles[0].getUnit().StaminaMoveCost)
                        {
                            currentSurroundingTiles[i].setUnit(currentSurroundingTiles[0].getUnit());
                            self.useStamina(currentSurroundingTiles[0].getUnit().StaminaMoveCost);
                            currentSurroundingTiles[0].setUnit(null);
                        }
                        //Console.WriteLine("moveUnit Move");
                    }
                }
            }
            self.SelectedTile = null;
            moveInProgress = false;
        }
        private void unitAttackUnit(MouseState mouseState, Player player)
        {  
            Player self = player;
            TileBasic[] currentAttackableTiles = self.SelectedTile.getUnit().getAttackableTiles();
            for (int i = 1; i < currentAttackableTiles.Length; i++)
            {//starts on 1 because 0 is the attacker
                if (currentAttackableTiles[i] != null && currentAttackableTiles[i].isPointInTile(mouseState) && currentAttackableTiles[i].hasUnit() && !(currentAttackableTiles[i].getUnit().Player.Equals(self)))
                {
                    if (self.Stamina >= self.SelectedTile.getUnit().calculateAttackCost())
                    {
                        Point attackerPosition = new Point((int)currentAttackableTiles[0].getX(), (int)currentAttackableTiles[0].getY());
                        Point defendersPosition = new Point((int)currentAttackableTiles[i].getX(), (int)currentAttackableTiles[i].getY());

                        int preAttackHealth = currentAttackableTiles[i].getUnit().Life;

                        self.useStamina(self.SelectedTile.getUnit().calculateAttackCost());
                        currentAttackableTiles[0].getUnit().attack(currentAttackableTiles[i].getUnit());

                        if (currentAttackableTiles[i].getUnit().isDead)
                        {
                            currentAttackableTiles[i].setUnit(null);
                            //maybe some death effects here i.e blood and gore
                        }
                    }
                }
            }
            self.SelectedTile = null;
            attackInProgress = false;
            checkForWin();
        }
        private void makeMove()
        {
            if (isFirstPhase)
            { 
                movesLeftInPhase--;
                showEndTurn();
                //update phase info here
            }
        }

        private bool playerCanSetUnit(TileBasic tile, Player player)
        {
            bool result = false; 
            Player self = player;

            if (isFirstPhase && (((tile.isHostControlled() == true) && (tile.isGuestControlled() == false) && self.IsHost)
                || ((tile.isHostControlled() == false) && (tile.isGuestControlled() == true) && !self.IsHost)
                || ((tile.isHostControlled() == false) && (tile.isGuestControlled() == false))))
            {
                result = true; 
            }
            else if (((tile.isHostControlled() == true) && (tile.isGuestControlled() == false) && self.IsHost)
                || ((tile.isHostControlled() == false) && (tile.isGuestControlled() == true) && !self.IsHost))
            {
                result = true; 
            }
            return result;
        }
        private void newTurn(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
 
            Player self;
            self = selectPlayer();
            //need to reset units to 0 attacks

            self.resetUnits();
            //switching turns here
            if (isHostsTurn)
            {
                turnSignal.setInsideColor(GameColors.turnButtonInsideColorGuest);
                isHostsTurn = false;
                players[0].isPlayersTurn = false;
                players[1].isPlayersTurn = true;
            }
            else
            {
                turnSignal.setInsideColor(GameColors.turnButtonInsideColor);
                isHostsTurn = true;
                players[0].isPlayersTurn = true;
                players[1].isPlayersTurn = false;
            }
            self.newTurn();
            resetBools();
            //checking if phase 1 has ended
            if (movesLeftInPhase == 0)
            {
                isFirstPhase = false;
                phaseLabel.Text = "Phase 2 - Game Phase";

            }
        }
        private void recalculateControlArea()
        {
            //resets the board to have no control
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    tiles[i, j].updateHostControl(false);
                    tiles[i, j].updateGuestControl(false);
                }
            }
            //finds units and set the area of their control
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (tiles[i, j].getUnit() == null) { continue; }
                    if (tiles[i, j].getUnit().IsHostsUnit) { tiles[i, j].updateSurroundingTilesControl(true, true); }
                    if (!tiles[i, j].getUnit().IsHostsUnit) { tiles[i, j].updateSurroundingTilesControl(true, false); }
                }
            }
        }
        private void checkForWin()
        {
            int playerZeroTotal = 0;
            int playerOneTotal = 0;
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (tiles[i, j].hasUnit())
                    {
                        if (players[0].Equals(tiles[i, j].getUnit().Player))
                        {
                            playerZeroTotal++;
                        }
                        else
                        {
                            playerOneTotal++;
                        }
                    }
                }
            }
            if (playerZeroTotal == 0)
            {
                gameDone = true;
                endGameLabel.Text = players[0].playerName + " has Won!!";
                uiManager.Add(endingPanel);
                //host wins
                turnSignal.setInsideColor(Color.Red);
                turnSignal.setOutsideColor(Color.Red);
            }
            else if (playerOneTotal == 0)
            {
                gameDone = true;
                endGameLabel.Text = players[0].playerName + " has Won!!";
                uiManager.Add(endingPanel);
                //guest wins
                turnSignal.setInsideColor(Color.White);
                turnSignal.setOutsideColor(Color.White);
            }
        }
        public override void update()
        {
            foreach (Player player in players)
            {
                player.update();
            }
            foreach (TileBasic tile in tiles)
            {
                if (tile.isHovered())
                {
                    tile.updateHexHover();
                }
            }

        }

        public override void draw(SpriteBatch spriteBatch)
        {
            resetButtons();
            basicEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, 2);
            spriteBatch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, borderLines, 0, 4);
            bool found = false;
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    tiles[i,j].draw(spriteBatch);
                    if (tiles[i, j].isHovered())
                    {
                       // Console.WriteLine("selected: " + i + ", " + j);
                        currentTile = tiles[i, j];
                        found = true;
                    }
                    else if(!found){
                        currentTile = null;
                    }
                }
            }
            foreach (Player player in players)
            {
                player.draw(spriteBatch);
            }

            drawUnitButtons(currentTile, spriteBatch);
            // drawUnitInfo(spriteBatch);

            
            

            turnSignal.draw(spriteBatch);


            
        }
        private void drawUnitButtons(TileBasic currentTile, SpriteBatch spriteBatch)
        {
            Color tempColor;

            if (isHostsTurn)
            {
                tempColor = GameColors.HostControlOutsideColor;
            }
            else
            {
                tempColor = GameColors.guestControlOutsideColor;
            }

            if (attPlacementInProgress)
            {

                //attUnit.moveHex(currentMouseState.X, currentMouseState.Y, GameColors.noUnitInsideColor, GameColors.noControlOutsideColor);
                //attUnit.draw(spriteBatch);
                spriteBatch.Draw(Formations.attackSprite, new Rectangle((int)currentMouseState.X - 20, (int)currentMouseState.Y - 20, 40, 40), Color.White);
            }
            if (defPlacementInProgress)
            {
                //defUnit.moveHex(currentMouseState.X, currentMouseState.Y, GameColors.noUnitInsideColor, GameColors.noControlOutsideColor);
                //defUnit.draw(spriteBatch);
                spriteBatch.Draw(Formations.defenderSprite, new Rectangle((int)currentMouseState.X - 20, (int)currentMouseState.Y - 20, 40, 40), Color.White);
            }
            if (magPlacementInProgress)
            {
                //magUnit.moveHex(currentMouseState.X, currentMouseState.Y, GameColors.noUnitInsideColor, GameColors.noControlOutsideColor);
                //magUnit.draw(spriteBatch);
                spriteBatch.Draw(Formations.healerSprite, new Rectangle((int)currentMouseState.X - 15, (int)currentMouseState.Y - 20, 30, 40), Color.White);
            }
            if (currentTile == null)
            {
                return;
            }
            drawUnitInfo(spriteBatch);
            float x = currentTile.getX();
            float y = currentTile.getY();
            UnitAbstract currentUnit = currentTile.getUnit();
            if (moveInProgress)
            {
                moveAction.moveHex(currentMouseState.X, currentMouseState.Y, GameColors.moveButton, GameColors.moveButton);
                moveAction.draw(spriteBatch);
            }
            else if(attackInProgress)
            {
                attAction.moveHex(currentMouseState.X, currentMouseState.Y, tempColor, tempColor);
                attAction.draw(spriteBatch);
            }
            else if(magicInProgress)
            {
                magicAction.moveHex(currentMouseState.X, currentMouseState.Y, GameColors.magicButton, GameColors.magicButton);
                magicAction.draw(spriteBatch);
            }
            else
            {
                if (!isFirstPhase && currentTile.hasUnit() && currentTile.isSelected() && !isSmallBoard)
                {

                    if ((currentUnit.IsHostsUnit && isHostsTurn) || (!currentUnit.IsHostsUnit && !isHostsTurn))
                    {
                        attAction.moveHex(x + changeInX, y - changeInY, tempColor, tempColor);
                        moveAction.moveHex(x - changeInX, y - changeInY, GameColors.moveButton, GameColors.moveButton);
                        attAction.draw(spriteBatch);
                        moveAction.draw(spriteBatch);
                        if (currentUnit.GetType() == typeof(UnitMag))
                        {
                            magicAction.moveHex(x, y - largeTileSideLength, GameColors.magicButton, GameColors.magicButton);
                            magicAction.draw(spriteBatch);
                        }
                    }
                }

            }
        }
        private void resetButtons()
        {
            Color tempColor;

            if (isHostsTurn)
            {
                tempColor = GameColors.HostControlOutsideColor;
                System.Console.WriteLine("Is hosts turn");
            }
            else
            {
                tempColor = GameColors.guestControlOutsideColor;
                System.Console.WriteLine("Is guests turn");
            }

            createButtonArea();
            attAction.moveHex(-100, -100, tempColor, tempColor);
            moveAction.moveHex(-100, -100, tempColor, tempColor);
            magicAction.moveHex(-100, -100, tempColor, tempColor);
            attUnit.moveHex(-100, -100, tempColor, tempColor);
            defUnit.moveHex(-100, -100, tempColor, tempColor);
            magUnit.moveHex(-100, -100, tempColor, tempColor);

        }
        private void drawUnitInfo(SpriteBatch spriteBatch)
        {
            
        }
        private void createButtonArea()
        {

            float width = 85;
            float height = 150;
            if (gameInfo == null)
            {
                gameInfo = new Label(uiManager);
                gameInfo.Init();
                gameInfo.Height = (int)height;
                gameInfo.Width = (int)width;
                //gameInfo.DrawBorders = true;
                gameInfo.MaximumWidth = (int)width;
                gameInfo.SetPosition(4, (int)largeBoardOffsetY+210);
                gameInfo.TextColor = Color.White;
                gameInfo.BackColor = Color.Indigo;
                //gameInfo.WordWrap = true;
                uiManager.Add(gameInfo);
            }
            gameInfo.Text = "";
            UnitAbstract tempUnit;
            foreach (var tile in tiles)
            {   tempUnit = tile.getUnit();
                if (tile.isHovered() && tile.hasUnit())
                {
                    gameInfo.Text += "\n";
                    gameInfo.Text += tempUnit.getUnitType() + "\n";
                    gameInfo.Text += "Life: " + tempUnit.Life + "\n";
                    gameInfo.Text += "Damage: " + tempUnit.calculateAtt() + "\n";
                    gameInfo.Text += "Range: " + tempUnit.calculateRange() + "\n";
                    gameInfo.Text += "__________\n";
                    gameInfo.Text += "Stamina Cost\n";
                    gameInfo.Text += "Attack $" + tempUnit.calculateAttackCost() + "\n";
                    gameInfo.Text += "Move $" + tempUnit.StaminaMoveCost + "\n";
                    gameInfo.Text += "Place $" + tempUnit.StaminaPlaceCost + "\n";

                }
            }
        }
        private void resizeTiles(int multiplyer, float xOffset, float yOffset, int tileLength)
        {
            float currentBoardOffsetX = xOffset;
            float currentBoardOffsetY = yOffset;
            int currentTileSideLength = tileLength;

            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (j % 2 == 0)
                    {
                        tiles[i, j].resizeHex(currentBoardOffsetX + (i * xAdjustment / multiplyer), currentBoardOffsetY + (j * yAdjustment / multiplyer), currentTileSideLength);
                    }
                    else
                    {
                        tiles[i, j].resizeHex(currentBoardOffsetX + xTileOffset / multiplyer + (i * xAdjustment / multiplyer), currentBoardOffsetY + (j * yAdjustment / multiplyer), currentTileSideLength);
                    }
                }
            }
        }
        private void resizeBoard(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            
            float currentBoardOffsetX;
            float currentBoardOffsetY;
            int currenttileSideLength;
            int multiplyer;
            if (!isSmallBoard)
            {
                currentBoardOffsetX = smallBoardOffsetX;
                currentBoardOffsetY = smallBoardOffsetY;
                currenttileSideLength = smallTileSideLength;
                multiplyer = 2;
                resizeButton.Text = "Large Map";
                isSmallBoard = true;
            }
            else
            {
                currentBoardOffsetX = largeBoardOffsetX;
                currentBoardOffsetY = largeBoardOffsetY;
                currenttileSideLength = largeTileSideLength;
                multiplyer = 1;
                resizeButton.Text = "Small Map";
                isSmallBoard = false;
            }
            resizeTiles(multiplyer, currentBoardOffsetX, currentBoardOffsetY, currenttileSideLength);
            float border = 10/multiplyer;
            float halfHexWidth = (float)Math.Sqrt(currenttileSideLength * currenttileSideLength - (currenttileSideLength / 2) * (currenttileSideLength / 2));
            float widthOfBoard = (xAdjustment * boardWidth + border)/multiplyer;
            float heightOfBoard = (yAdjustment * boardHeight - currenttileSideLength + 2 * border) / multiplyer;
            vertices[0] = new VertexPositionColor(new Vector3(currentBoardOffsetX - halfHexWidth - border, currentBoardOffsetY - currenttileSideLength - border, 0), GameColors.boardAreaBackground);
            vertices[1] = new VertexPositionColor(new Vector3(currentBoardOffsetX + widthOfBoard, currentBoardOffsetY - currenttileSideLength - border, 0), GameColors.boardAreaBackground);
            vertices[2] = new VertexPositionColor(new Vector3(currentBoardOffsetX + widthOfBoard, currentBoardOffsetY + heightOfBoard, 0), GameColors.boardAreaBackground);
            vertices[3] = new VertexPositionColor(new Vector3(currentBoardOffsetX + widthOfBoard, currentBoardOffsetY + heightOfBoard, 0), GameColors.boardAreaBackground);
            vertices[4] = new VertexPositionColor(new Vector3(currentBoardOffsetX - halfHexWidth - border, currentBoardOffsetY + heightOfBoard, 0), GameColors.boardAreaBackground);
            vertices[5] = new VertexPositionColor(new Vector3(currentBoardOffsetX - halfHexWidth - border, currentBoardOffsetY - currenttileSideLength - border, 0), GameColors.boardAreaBackground);

            borderLines[0] = new VertexPositionColor(new Vector3(currentBoardOffsetX - halfHexWidth - border, currentBoardOffsetY - currenttileSideLength - border, 0), Color.Blue);
            borderLines[1] = new VertexPositionColor(new Vector3(currentBoardOffsetX + widthOfBoard, currentBoardOffsetY - currenttileSideLength - border, 0), Color.Blue);
            borderLines[2] = new VertexPositionColor(new Vector3(currentBoardOffsetX + widthOfBoard, currentBoardOffsetY - currenttileSideLength - border, 0), Color.Blue);
            borderLines[3] = new VertexPositionColor(new Vector3(currentBoardOffsetX + widthOfBoard, currentBoardOffsetY + heightOfBoard, 0), Color.Blue);
            borderLines[4] = new VertexPositionColor(new Vector3(currentBoardOffsetX + widthOfBoard, currentBoardOffsetY + heightOfBoard, 0), Color.Blue);
            borderLines[5] = new VertexPositionColor(new Vector3(currentBoardOffsetX - halfHexWidth - border, currentBoardOffsetY + heightOfBoard, 0), Color.Blue);
            borderLines[6] = new VertexPositionColor(new Vector3(currentBoardOffsetX - halfHexWidth - border, currentBoardOffsetY + heightOfBoard, 0), Color.Blue);
            borderLines[7] = new VertexPositionColor(new Vector3(currentBoardOffsetX - halfHexWidth - border, currentBoardOffsetY - currenttileSideLength - border, 0), Color.Blue);
        }
        private void createBoardArea()
        {
            float border = 10;
            float halfHexWidth = (float)Math.Sqrt(largeTileSideLength * largeTileSideLength - (largeTileSideLength / 2) * (largeTileSideLength / 2));
            float widthOfBoard = (xAdjustment * boardWidth + border);
            float heightOfBoard = (yAdjustment * boardHeight - largeTileSideLength + 2 * border);
            vertices[0] = new VertexPositionColor(new Vector3(largeBoardOffsetX - halfHexWidth - border, largeBoardOffsetY - largeTileSideLength - border, 0), GameColors.boardAreaBackground);
            vertices[1] = new VertexPositionColor(new Vector3(largeBoardOffsetX + widthOfBoard, largeBoardOffsetY - largeTileSideLength - border, 0), GameColors.boardAreaBackground);
            vertices[2] = new VertexPositionColor(new Vector3(largeBoardOffsetX + widthOfBoard, largeBoardOffsetY + heightOfBoard, 0), GameColors.boardAreaBackground);
            vertices[3] = new VertexPositionColor(new Vector3(largeBoardOffsetX + widthOfBoard, largeBoardOffsetY + heightOfBoard, 0), GameColors.boardAreaBackground);
            vertices[4] = new VertexPositionColor(new Vector3(largeBoardOffsetX - halfHexWidth - border, largeBoardOffsetY + heightOfBoard, 0), GameColors.boardAreaBackground);
            vertices[5] = new VertexPositionColor(new Vector3(largeBoardOffsetX - halfHexWidth - border, largeBoardOffsetY - largeTileSideLength - border, 0), GameColors.boardAreaBackground);

            borderLines[0] = new VertexPositionColor(new Vector3(largeBoardOffsetX - halfHexWidth - border, largeBoardOffsetY - largeTileSideLength - border, 0), Color.Blue);
            borderLines[1] = new VertexPositionColor(new Vector3(largeBoardOffsetX + widthOfBoard, largeBoardOffsetY - largeTileSideLength - border, 0), Color.Blue);
            borderLines[2] = new VertexPositionColor(new Vector3(largeBoardOffsetX + widthOfBoard, largeBoardOffsetY - largeTileSideLength - border, 0), Color.Blue);
            borderLines[3] = new VertexPositionColor(new Vector3(largeBoardOffsetX + widthOfBoard, largeBoardOffsetY + heightOfBoard, 0), Color.Blue);
            borderLines[4] = new VertexPositionColor(new Vector3(largeBoardOffsetX + widthOfBoard, largeBoardOffsetY + heightOfBoard, 0), Color.Blue);
            borderLines[5] = new VertexPositionColor(new Vector3(largeBoardOffsetX - halfHexWidth - border, largeBoardOffsetY + heightOfBoard, 0), Color.Blue);
            borderLines[6] = new VertexPositionColor(new Vector3(largeBoardOffsetX - halfHexWidth - border, largeBoardOffsetY + heightOfBoard, 0), Color.Blue);
            borderLines[7] = new VertexPositionColor(new Vector3(largeBoardOffsetX - halfHexWidth - border, largeBoardOffsetY - largeTileSideLength - border, 0), Color.Blue);
        }

    }
}
