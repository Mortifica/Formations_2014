using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formations
{
    [Serializable]
    class Hexagon
    {
        
        private VertexPositionColor[] vectors;
        
        private VertexPositionColor[] vertices;
        
        private VertexPositionColor[] borders;
        
        private BasicEffect basicEffect;
        
        private int tileSideLength;
        
        private Color insideColor;
        
        private Color outsideColor;
        
        private int hoverPosition = 0;
        
        private int count = 0;
       
        public Hexagon(int tileSideLength)
        {
            this.tileSideLength = tileSideLength;
        }
        public void init(float x, float y, GraphicsDevice graphicsDevice, Color insideColor, Color outsideColor)
        {
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, graphicsDevice.Viewport.Width,     // left, right
                graphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);                                // near, far plane
            vertices = new VertexPositionColor[18];
            moveHex(x, y, insideColor, outsideColor);

        }
        public void changeResizeAndLocation(float x, float y, int sideLength)
        {
            tileSideLength = sideLength;
            moveHex(x,y,insideColor,outsideColor);
        }
        public static VertexPositionColor[] CalculateVertices(float sideLength, Vector3 center)
        {
            VertexPositionColor[] tempVectors = new VertexPositionColor[6];
            double angle;
            VertexPositionColor temp = new VertexPositionColor();

            for (int i = 0; i < tempVectors.Length; i++)
            {
                angle = 2 * Math.PI / 6.0 * (i + 0.05) - 10;
                temp.Position.X = (float)(center.X + sideLength * Math.Cos(angle));
                temp.Position.Y = (float)(center.Y + sideLength * Math.Sin(angle));
                temp.Position.Z = 0f;
                tempVectors[i] = temp;
            }
            return tempVectors;
        }
        public void moveHex(float x, float y, Color insideColor, Color outsideColor)
        {
            this.insideColor = insideColor;
            this.outsideColor = outsideColor;

            vectors = CalculateVertices(tileSideLength, new Vector3(x, y, 0));
            createBorder();
            vertices[0] = new VertexPositionColor(new Vector3(x, y, 0), insideColor);
            vertices[1] = new VertexPositionColor(vectors[0].Position, outsideColor);
            vertices[2] = new VertexPositionColor(vectors[1].Position, outsideColor);
            vertices[3] = new VertexPositionColor(new Vector3(x, y, 0), insideColor);
            vertices[4] = new VertexPositionColor(vectors[1].Position, outsideColor);
            vertices[5] = new VertexPositionColor(vectors[2].Position, outsideColor);

            vertices[6] = new VertexPositionColor(new Vector3(x, y, 0), insideColor);
            vertices[7] = new VertexPositionColor(vectors[2].Position, outsideColor);
            vertices[8] = new VertexPositionColor(vectors[3].Position, outsideColor);
            vertices[9] = new VertexPositionColor(new Vector3(x, y, 0), insideColor);
            vertices[10] = new VertexPositionColor(vectors[3].Position, outsideColor);
            vertices[11] = new VertexPositionColor(vectors[4].Position, outsideColor);

            vertices[12] = new VertexPositionColor(new Vector3(x, y, 0), insideColor);
            vertices[13] = new VertexPositionColor(vectors[4].Position, outsideColor);
            vertices[14] = new VertexPositionColor(vectors[5].Position, outsideColor);
            vertices[15] = new VertexPositionColor(new Vector3(x, y, 0), insideColor);
            vertices[16] = new VertexPositionColor(vectors[5].Position, outsideColor);
            vertices[17] = new VertexPositionColor(vectors[0].Position, outsideColor);
        }
        private void createBorder()
        {
            borders = new VertexPositionColor[12];

            borders[0] = vectors[0];
            
            borders[1] = vectors[1];
            borders[2] = vectors[1];
            borders[3] = vectors[2];
            borders[4] = vectors[2];
            borders[5] = vectors[3];
            borders[6] = vectors[3];
            borders[7] = vectors[4];
            borders[8] = vectors[4];
            borders[9] = vectors[5];
            borders[10] = vectors[5];
            borders[11] = vectors[0];
            borders[0] = vectors[0];

            setBorderColor(GameColors.normalBorderColor);
        }
        public void setBorderColor(Color color)
        {
            for (int i = 0; i < borders.Length; i++)
            {
                borders[i].Color = color;
            }
        }
        public void setInsideColor(Color color)
        {
            insideColor = color;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (i % 3 == 0) { vertices[i].Color = color; }
            }
        }
        public void setOutsideColor(Color color)
        {
            outsideColor = color;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (i % 3 != 0) { vertices[i].Color = color; }
            }
        }
        public void updateHover()
        {
            if (count % 5 == 0)
            {
                hoverPosition = (hoverPosition + 1) % 12;
                setBorderColor(Color.Red);
                int positionOne = hoverPosition;
                int positionTwo = (hoverPosition + 11) % 12;
                borders[positionOne].Color = Color.Black;
                borders[positionTwo].Color = Color.Black;
            }
            count++;
        }
        public bool IsPointInPolygon(int pointX, int pointY)
        {
          
            bool isInside = false;
            VertexPositionColor temp1;
            VertexPositionColor temp2;
            for (int i = 0, j = vectors.Length - 1; i < vectors.Length; j = i++)
            {
                temp1 = vectors[i];
                temp2 = vectors[j];
                if (((temp1.Position.Y > pointY) != (temp2.Position.Y > pointY)) &&
                    (pointX < (temp2.Position.X - temp1.Position.X) * (pointY - temp1.Position.Y) / (temp2.Position.Y - temp1.Position.Y) + temp1.Position.X))
                 {
                    isInside = !isInside;
                 }
            }
            return isInside;
        }

        public void draw(SpriteBatch spriteBatch)
        {

                basicEffect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, 6);
                spriteBatch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, borders, 0, 6);
            
        }
    }
}
