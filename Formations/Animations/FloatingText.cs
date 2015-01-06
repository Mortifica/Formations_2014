using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Timers;
using TomShane.Neoforce.Controls;

namespace Formations
{
    [Serializable]
    public class FloatingText : IUpdateDraw
    {
        // Damage text
        private UnitAbstract unit;
        
        protected SpriteFont damageTextFont;
        
        protected Vector2 damageTextVector;
        protected int damageGiven;
        protected float damageTextAlpha = 1.0f;

        
        protected Timer damageTextTimer;
        private bool evenText = true;
        private bool isDamage;

        public FloatingText(UnitAbstract unit)
        {
            this.unit = unit;
            damageTextFont = Formations.damageFont;
        }
        public void displayDamageTaken(int damage, bool isDamage)
        {
            this.isDamage = isDamage;
            this.damageGiven = damage;
            this.damageTextVector = new Vector2(unit.ContainingTile.getX() - 20, unit.ContainingTile.getY());

            damageTextTimer = new System.Timers.Timer(10);
            damageTextTimer.Elapsed += (sender, e) => slideDamageTextUp(sender, e, damageTextTimer);
            damageTextTimer.Start();
        }

        // Called by the Timer in a separate thread
        protected void slideDamageTextUp(object sender, ElapsedEventArgs e, System.Timers.Timer timer)
        {
            int counter = 0;
            float changeInX = .1f;
            if (evenText) { changeInX *= -1; }// weather the text goes left or right
            while (counter < 25)
            {
                System.Threading.Thread.Sleep(60);

                this.damageTextVector.Y--;
                this.damageTextVector.X += changeInX;
                if (counter > 5 && counter % 20 == 0)
                {
                    this.damageTextAlpha -= .1f;
                }

                //System.Console.WriteLine(this.damageTextAlpha);
                changeInX *= 1.1f;//changes how fast the text soes in the left or right
                counter++;
            }

            timer.Stop();
            //swaps the direction the text trails off in
            if (evenText) { evenText = false; }
            else { evenText = true; }
        }
        public void update()
        {
            throw new NotImplementedException();
        }

        public void draw(SpriteBatch spriteBatch)
        {
            if (damageTextTimer != null && damageTextTimer.Enabled)
            {
                if (isDamage)
                {
                    spriteBatch.DrawString(this.damageTextFont, "-" + damageGiven, this.damageTextVector, new Color(255, 0, 0, this.damageTextAlpha));

                }
                else
                {
                    spriteBatch.DrawString(this.damageTextFont, "+" + damageGiven, this.damageTextVector, new Color(0, 255, 0, this.damageTextAlpha));

                }
            }
        }
    }
}
