using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Formations
{
    [Serializable]
    public abstract class TileAbstract : IUpdateDraw, IMouseListener
    {
        public abstract void init(float x, float y, GraphicsDevice graphicsDevice);
        public abstract void mousePressed(MouseState mouseState);
        public abstract void mouseReleased(MouseState mouseState);
        public abstract void mouseDragged(MouseState mouseState);
        public abstract void mouseMoved(MouseState mouseState);
        public abstract void update();
        public abstract void draw(SpriteBatch spriteBatch);
    }
}
