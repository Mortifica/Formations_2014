using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formations
{
    public interface IMouseListener
    {
        void mousePressed(MouseState mouseState);

        void mouseReleased(MouseState mouseState);

        void mouseMoved(MouseState mouseState);

        void mouseDragged(MouseState mouseState);
    }
}
