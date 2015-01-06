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
    class UnitsComponent : IUpdateDraw
    {
        private Hexagon[] attackUnits;
        private Hexagon[] defenseUnits;
        private Hexagon[] magicUnits;
        private int tileSideLength = 5;
        private float boardOffsetX;
        private float boardOffsetY;
        private float xAdjustment = 10;
        private float yAdjustment = 10;
        public UnitsComponent(int x, int y, int attSize, int defSize, int magSize)
        {
            boardOffsetX = x;
            boardOffsetY = y;
            attackUnits = new Hexagon[attSize];
            defenseUnits = new Hexagon[defSize];
            magicUnits = new Hexagon[magSize];
            for (int i = 0; i < attackUnits.Length; i++)
            {
                attackUnits[i] = new Hexagon(tileSideLength);
            }
            for (int i = 0; i < defenseUnits.Length; i++)
            {
                defenseUnits[i] = new Hexagon(tileSideLength);
            }
            for (int i = 0; i < magicUnits.Length; i++)
            {
                magicUnits[i] = new Hexagon(tileSideLength);
            }
        }

        public void init(GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < attackUnits.Length; i++)
            {
                attackUnits[i].init(boardOffsetX + (i * xAdjustment), boardOffsetY, graphicsDevice, Color.Red, Color.Red);
            }
            for (int i = 0; i < defenseUnits.Length; i++)
            {
                defenseUnits[i].init(boardOffsetX + (i * xAdjustment), boardOffsetY + yAdjustment, graphicsDevice, Color.Gray, Color.Gray);
            }
            for (int i = 0; i < magicUnits.Length; i++)
            {
                magicUnits[i].init(boardOffsetX + (i * xAdjustment), boardOffsetY + (2 * yAdjustment), graphicsDevice, Color.Purple, Color.Purple);
            }
        }
        public void updateUnitHex(int attAmount, int defAmount, int magAmount)
        {
            Color tempColor;
            for (int i = 0; i < attackUnits.Length; i++)
            {
                if (attAmount > 0)
                {
                    tempColor = Color.Red;
                    attAmount--;
                }
                else { tempColor = Color.Black; }

                attackUnits[i].setInsideColor(tempColor);
            }
            for (int i = 0; i < defenseUnits.Length; i++)
            {
                if (defAmount > 0)
                {
                    tempColor = Color.Gray;
                    defAmount--;
                }
                else { tempColor = Color.Black; }

                defenseUnits[i].setInsideColor(tempColor);
            }
            for (int i = 0; i < magicUnits.Length; i++)
            {
                if (magAmount > 0)
                {
                    tempColor = Color.Purple;
                    magAmount--;
                }
                else { tempColor = Color.Black; }

                magicUnits[i].setInsideColor(tempColor);
            }
        }
        public void update()
        {
            
        }

        public void draw(SpriteBatch spriteBatch)
        {
            foreach (Hexagon hex in attackUnits)
            {
                hex.draw(spriteBatch);
            }
            foreach (Hexagon hex in defenseUnits)
            {
                hex.draw(spriteBatch);
            }
            foreach (Hexagon hex in magicUnits)
            {
                hex.draw(spriteBatch);
            }
        }
    }
}
