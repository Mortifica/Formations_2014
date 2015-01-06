using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Formations
{
    [Serializable]
    public class Particle
    {
        
        private Texture2D _texture;
        
        private Vector2 _position;
        
        private Vector2 _velocity;
        
        private Color _color;
        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        public Vector2 Position { get { return _position; } set { _position = value; } }
        public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public Color Color { get { return _color; } set { _color = value; } }
        public float Size { get; set; }
        public int TTL { get; set; }

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            TTL = ttl;
        }

        public void Update()
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }
}
