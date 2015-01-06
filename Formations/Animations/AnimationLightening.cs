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
    class AnimationLightening : IUpdateDraw
    {

        
        private Color lighteningColor;
        
        private VertexPositionColor[] lightening;
        
        private List<Point> points = new List<Point>();
        
        private static Random rand = new Random();
        private bool isAnimating = false;
        private const int DEPTH = 3;
        private const int CHANGE = 20;
        private int depth;
        private int change;
        private int lighteningPosition;
        private int count = 0;
        public AnimationLightening()
        {

        }
        public void createLightening(Color color, Point startingPoint, Point endingPoint)
        {
            count = 0;
            depth = DEPTH;
            change = CHANGE;
            lighteningColor = color;
            points.Add(startingPoint);
            points.Add(endingPoint);
            stylizeLine();
        }
        private void stylizeLine()
        {
            if (depth == 0) 
            {
                lighteningPosition = points.Count;
                copy();
                isAnimating = true;
                return;
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                double tempRandom = rand.NextDouble() * change;
                int boolRand = rand.Next(10);
                if (boolRand % 2 == 0) { tempRandom = 0 + tempRandom; }
                else { tempRandom = 0 - tempRandom; }

                double x = (points.ElementAt<Point>(i).X + points.ElementAt<Point>(i + 1).X) / 2;
                double y = tempRandom + (points.ElementAt<Point>(i).Y + points.ElementAt<Point>(i + 1).Y) / 2;
                points.Insert(++i, new Point((int)x, (int)y));

            }
            depth--;
            change /= 2;
            stylizeLine();
        }
        private void copy()
        {
            lightening = new VertexPositionColor[points.Count * 2];
            float x = points.ElementAt<Point>(0).X;
            float y = points.ElementAt<Point>(0).Y;
            Console.WriteLine("X,Y: " + x + "," + y);
            lightening[0] = new VertexPositionColor(new Vector3(x, y, 0), lighteningColor);

            for (int i = 1; i < points.Count; i++)
            {
                x = points.ElementAt<Point>(i).X;
                y = points.ElementAt<Point>(i).Y;
                Console.WriteLine("X,Y: " + x + "," + y);
                lightening[i] = new VertexPositionColor(new Vector3(x, y, 0), lighteningColor);
                lightening[++i] = new VertexPositionColor(new Vector3(x, y, 0), lighteningColor);
            }
        }
        public void update()
        {
            if (isAnimating)
            {
                if (count == 60)
                {
                    isAnimating = false;
                    lightening = new VertexPositionColor[1];
                    points.Clear();
                }
                count++;
            }            
        }

        public void draw(SpriteBatch spriteBatch)
        {
            if (isAnimating)
            {
                int random = rand.Next(15) + 1;
                if (count % random == 0)
                {
                    spriteBatch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, lightening, 0, 4);
                    
                }
            }
            
        }
    }
}
