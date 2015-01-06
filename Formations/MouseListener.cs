using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Formations
{
    [Serializable]
    class MouseListener
    {
        
        private MouseState oldState;
        
        private Formations game;
        
        private Task mouseThread;
        public MouseListener(MouseState mouseState, Formations game)
        {
            oldState = mouseState;
            this.game = game;
            
        }
        public void startListener()
        {
            mouseThread = Task.Factory.StartNew(() => checkMouse());
        }
        public void checkMouse()
        {
            while (true)
            { 
                var newState = Microsoft.Xna.Framework.Input.Mouse.GetState();
                if(oldState.LeftButton == ButtonState.Released && newState.LeftButton == ButtonState.Pressed)
                {
                    game.mousePressed(newState);
                }
                else if (oldState.RightButton == ButtonState.Released && newState.RightButton == ButtonState.Pressed)
                {
                    game.mousePressed(newState);
                }
                else if (oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Released)
                {
                    game.mouseReleased(newState);
                }
                else if (oldState.Position.X != newState.Position.X || oldState.Position.Y != newState.Position.Y)
                {
                    if (oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Pressed)
                    {
                        game.mouseDragged(newState);
                    }
                    else
                    {
                        game.mouseMoved(newState);
                    }
                }
 
                oldState = newState; // this reassigns the old state so that it is ready for next time
            }
        }


    }
}
