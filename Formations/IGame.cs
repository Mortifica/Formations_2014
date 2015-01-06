using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;

namespace Formations
{
    public abstract class IGame : IMouseListener
    {
        public bool gameDone = false;
        public abstract void init(Manager uiManager, GraphicsDevice graphicsDevice, Formations formation, string gameName, bool isHost, List<Texture2D> unitTextures);
        public abstract void update();
        public abstract void draw(SpriteBatch spriteBatch);


        public abstract void mousePressed(Microsoft.Xna.Framework.Input.MouseState mouseState);

        public abstract void mouseReleased(Microsoft.Xna.Framework.Input.MouseState mouseState);

        public abstract void mouseMoved(Microsoft.Xna.Framework.Input.MouseState mouseState);

        public abstract void mouseDragged(Microsoft.Xna.Framework.Input.MouseState mouseState);
    }
}
