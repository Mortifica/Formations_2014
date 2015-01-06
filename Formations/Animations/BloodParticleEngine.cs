using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace Formations
{
    [Serializable]
    public class BloodParticleEngine
    {
        private Random random;

        private Vector2 _emitterLocation;
        public Vector2 EmitterLocation { get { return _emitterLocation; } set { _emitterLocation = value; } }
        private List<Particle> particles;

        private List<Texture2D> textures;

        public bool particlesOn { get; set; }
        public int count;

        public BloodParticleEngine(List<Texture2D> textures, Vector2 location)
        {
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
        }

        public void Update()
        {
            if (count > 100)
            {
                particlesOn = false;

                count = 0;
            }

            if (particlesOn)
            {
                int total = 5;

                for (int i = 0; i < total; i++)
                {
                    particles.Add(GenerateNewParticle());
                }

                count++;
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        private Particle GenerateNewParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                                    .5f * (float)(random.NextDouble() - .5f),
                                    .5f * (float)(random.NextDouble() - .5f));
            float angle = 0;
            float angularVelocity = 0;
            Color color = Color.Red;
            float size = 0.07f;
            int ttl = 60 + random.Next(40);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
        }
    }
}