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
    public class UnitMag: UnitAbstract
    {
        public const int DAMAGE = 1;
        public const int LIFE = 1;
        public const int MAX_LIFE = 1;
        public const int RANGE = 1;
        public const int STAMINA_MOVE_COST = 1;
        public const int STAMINA_ATT_COST = 5;
        public const int STAMINA_PLACE_COST = 5;
        private int healAmount = 1;
        private List<AnimationLightening> lightening = new List<AnimationLightening>();

        public static Texture2D sprite = Formations.healerSprite;


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

            for (int i = 0; i < 5; i++)
            {
                lightening.Add(new AnimationLightening());
            }
            bloodParticles = new BloodParticleEngine(Formations.bloodTextures, new Vector2(400, 240));
            healingParticles = new HealingParticleEngine(Formations.healingTextures, new Vector2(400, 240));
            floatingText = new FloatingText(this);
        }
        public override string getUnitType()
        {
            return "Magic Unit";
        }
        public override void attack(UnitAbstract unit)
        {
            Point attackerPosition = new Point((int)ContainingTile.getX(), (int)ContainingTile.getY());
            Point defendersPosition = new Point((int)unit.ContainingTile.getX(), (int)unit.ContainingTile.getY());
            foreach (AnimationLightening strike in lightening)
            {
                strike.createLightening(Color.Silver, attackerPosition, defendersPosition);

            }
            unit.defend(this);
            incrementAttack();
        }
        public override void defend(UnitAbstract unit)
        {
            bloodParticles.particlesOn = true;
            bloodParticles.EmitterLocation = new Vector2(ContainingTile.getX(), ContainingTile.getY());
            // Scrolling damage text
            int damage = calculateDamage(unit.calculateAtt());
            floatingText.displayDamageTaken(damage, true);
            Life -= (damage);
            if (Life <= 0)
            {
                isDead = true;
            }
        }
        public void heal(UnitAbstract unit)
        {
            Point attackerPosition = new Point((int)unit.ContainingTile.getX(), (int)unit.ContainingTile.getY());
            Point defendersPosition = new Point((int)unit.ContainingTile.getX(), (int)unit.ContainingTile.getY());
            int healingAmount = this.calculateHeal();
            unit.getHealed(healingAmount);
            foreach (AnimationLightening strike in lightening)
            {
                strike.createLightening(Color.LawnGreen, attackerPosition, defendersPosition);

            }
            
             
        }
        public override int calculateAtt()
        {
            return Damage;
        }
        public int calculateHeal()
        {
           
            int result = healAmount;
            TileBasic[] surroundingTiles = ContainingTile.getSurroundingTiles();
            for (int i = 1; i < surroundingTiles.Length; i++)//starts on 1 because 0 is its self
            {

                UnitAbstract unit = surroundingTiles[i].getUnit();
                if (unit == null) { continue; }
                if (unit.Player.Equals(Player) && unit.GetType() == typeof(UnitMag))
                {
                    result++;
                }
            }
            return result;
        }
        public override int calculateRange()
        {
            int calculatedRange = Range;
            TileBasic[] surroundingTiles = ContainingTile.getSurroundingTiles();

            for (int i = 1; i < surroundingTiles.Length; i++)
            {
                if (surroundingTiles[i] != null)
                {
                    UnitAbstract unit = surroundingTiles[i].getUnit();
                    if (unit == null) { continue; }
                    if (unit.Player.Equals(Player) && unit.GetType() == typeof(UnitMag))
                    {
                        calculatedRange++;
                    }
                }
            }

            return calculatedRange;
        }

        public override void update()
        {
            foreach (AnimationLightening strike in lightening)
            {
                strike.update();
            }

            bloodParticles.Update();
        }

        public override void draw(SpriteBatch spriteBatch)
        {

            if (ContainingTile != null && ContainingTile.getUnit() != null)
            {
                spriteBatch.Draw(sprite, new Rectangle((int)ContainingTile.getX() - 15, (int)ContainingTile.getY() - 20, 30, 40), Color.White);
            }

            foreach (AnimationLightening strike in lightening)
            {
                strike.draw(spriteBatch);
            }

            // Particles
            bloodParticles.Draw(spriteBatch);
            // Damage text
            floatingText.draw(spriteBatch);
        }
    }
}
