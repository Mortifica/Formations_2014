using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Formations
{
    [Serializable]
    class StaminaComponent:IUpdateDraw
    {
        private Hexagon[,] bar;
        private const int boardHeight = 3;
        private const int boardWidth = 10;
        private int tileSideLength = 7;
        private float boardOffsetX;
        private float boardOffsetY;
        private float xTileOffsetMiddleRow = 6;
        private float xTileOffsetTopRow = 12;
        private float xAdjustment = 13;
        private float yAdjustment = 11;
        public StaminaComponent(int x, int y)
        {
            boardOffsetX = x;
            boardOffsetY = y;
            bar = new Hexagon[boardWidth, boardHeight];
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    bar[i, j] = new Hexagon(tileSideLength);;
                }
            }
        }
        public void init(GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (j % 3 == 0)
                    {
                        bar[i, j].init(boardOffsetX + (i * xAdjustment), boardOffsetY - (j * yAdjustment), graphicsDevice, Color.Blue, Color.Aqua);
                    }
                    else if (j % 2 == 0)
                    {
                        bar[i, j].init(boardOffsetX + xTileOffsetTopRow + (i * xAdjustment), boardOffsetY - (j * yAdjustment), graphicsDevice, Color.Blue, Color.Aqua);
                    }
                    else
                    {
                        bar[i, j].init(boardOffsetX + xTileOffsetMiddleRow + (i * xAdjustment), boardOffsetY - (j * yAdjustment), graphicsDevice, Color.Blue, Color.Aqua);
                    }
                }
            }
        }
        public void updateBar(int staminaAmount)
        {
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (staminaAmount > 0) { bar[i, j].setInsideColor(Color.White); }
                    else { bar[i, j].setInsideColor(Color.Black); }
                    staminaAmount--;
                }
            }
        }
        public void update()
        {

        }
        public void draw(SpriteBatch spriteBatch)
        {
            foreach (Hexagon hex in bar)
            {
                hex.draw(spriteBatch);
            }

        }
    


    }
}
