using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TomShane.Neoforce.Controls;

namespace Formations
{
    [Serializable]
    public class UnitDef : UnitAbstract
    {
        public const int DAMAGE = 1;
        public const int LIFE = 5;
        public const int MAX_LIFE = 5;
        public const int RANGE = 1;
        public const int STAMINA_MOVE_COST = 1;
        public const int STAMINA_ATT_COST = 3;
        public const int STAMINA_PLACE_COST = 5;
        private int absorbAmount = 1;

        public static Texture2D sprite = Formations.defenderSprite;

        public override void init(bool isHostsUnit, Player player, Texture2D sprite)
        {
            this.IsHostsUnit = isHostsUnit;
            this.Damage = DAMAGE;
            this.Life = LIFE;
            this.MaxLife = MAX_LIFE;
            this.Range = RANGE;
            this.StaminaAttCost = STAMINA_ATT_COST;
            this.StaminaMoveCost = STAMINA_MOVE_COST;
            this.StaminaPlaceCost = STAMINA_PLACE_COST;
            this.Player = player;
            //this.sprite = sprite;
            // Particle engine stuff
            bloodParticles = new BloodParticleEngine(Formations.bloodTextures, new Vector2(400, 240));
            attackParticles = new AttackParticleEngine(Formations.attackTextures, new Vector2(400, 240));
            healingParticles = new HealingParticleEngine(Formations.healingTextures, new Vector2(400, 240));
            floatingText = new FloatingText(this);
        }
        public void loadData()
        {

        }
        public override string getUnitType()
        {
            return "Defense Unit";
        }
        public override void attack(UnitAbstract unit)
        {
            // Start particle effect
            attackParticles.particlesOn = true;
            attackParticles.EmitterLocation = new Vector2(ContainingTile.getX(), ContainingTile.getY());
            unit.defend(this);
            incrementAttack();
        }
        public override void defend(UnitAbstract unit)
        {
            bloodParticles.particlesOn = true;
            bloodParticles.EmitterLocation = new Vector2(ContainingTile.getX(), ContainingTile.getY());
            int damage = calculateDamage(unit.calculateAtt());
            floatingText.displayDamageTaken(damage, true);
            Life -= (damage);
            if (Life <= 0)
            {
                isDead = true;
            }
        }
        public override int calculateAtt()
        {
            return Damage;
        }

        public override int calculateRange()
        {
            return Range;
        }
        public int absorbDamage(int damage)
        {
            Life -= absorbAmount;
            damage -= absorbAmount;
            if (Life <= 0)
            {
                isDead = true;
            }
            return damage;
        }
        public override void update()
        {
            attackParticles.Update();
            bloodParticles.Update();
            healingParticles.Update();
        }

        public override void draw(SpriteBatch spriteBatch)
        {

            if (ContainingTile != null && ContainingTile.getUnit() != null)
            {
                spriteBatch.Draw(sprite, new Rectangle((int)ContainingTile.getX() - 20, (int)ContainingTile.getY() - 20, 40, 40), Color.White);
            }

            // Particles
            attackParticles.Draw(spriteBatch);
            bloodParticles.Draw(spriteBatch);
            healingParticles.Draw(spriteBatch);
            // Damage text

            floatingText.draw(spriteBatch);
        }
    }
}
