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
    public abstract class UnitAbstract : IUpdateDraw
    {
        private bool _isDead = false;
        private bool _isHostsUnit;
        private int _life;
        public int MaxLife;
        private int _range;
        private int _damage;
        private int _staminaAttCost;
        private int _staminaMoveCost;
        private int _staminaPlaceCost;
        private int _attacksThisRound;
        private TileBasic _containingTile;
        private Player _player;
        protected AttackParticleEngine attackParticles;
        protected BloodParticleEngine bloodParticles;
        protected HealingParticleEngine healingParticles;
        protected FloatingText floatingText;
        public bool isDead
        { 
            get { return _isDead; } 
            protected set { _isDead = value; } 
        }
        public int Life
        {
            get { return _life; }
            set { _life = value; }
        }
        public int Damage
        {
            get { return _damage; }
            protected set { _damage = value; }
        }
        public bool IsHostsUnit
        {
            get { return _isHostsUnit; }
            protected set { _isHostsUnit = value; }
        }
        public int Range 
        {
            get { return _range;}
            protected set { _range = value; } 
        }
        public int StaminaMoveCost
        {
            get { return _staminaMoveCost;}
            protected set { _staminaMoveCost = value; }
        }
        public int StaminaAttCost
        {
            get { return _staminaAttCost;}
            protected set { _staminaAttCost = value; }
        }
        public int StaminaPlaceCost
        {
            get { return _staminaPlaceCost;}
            protected set { _staminaPlaceCost = value; }
        }
        public TileBasic ContainingTile
        {
            get { return _containingTile; }
            set { _containingTile = value; }
        }
        public Player Player
        {
            get { return _player; }
            protected set { _player = value; }
        }
        public int AttacksThisRound
        {
            get { return _attacksThisRound; }
            private set { _attacksThisRound = value; }
        }
        public abstract void init(bool isOwnedByPlayer, Player player, Texture2D sprite);
        public abstract string  getUnitType();
        public abstract void attack(UnitAbstract unit);
        public abstract void defend(UnitAbstract unit);
        public abstract int calculateAtt();
        public int calculateDamage(int attackDamage)
        {
            int result = attackDamage;
            TileBasic[] surroundingTiles = ContainingTile.getSurroundingTiles();
            for (int i = 1; i < surroundingTiles.Length; i++)//starts on 1 because 0 is its self
            {
                if (surroundingTiles[i] == null) { continue; }
                UnitAbstract unit = surroundingTiles[i].getUnit();
                UnitDef defUnit;
                if (unit == null) { continue; }
                if (unit.Player.Equals(Player) && unit.GetType() == typeof(UnitDef))
                {
                    defUnit = (UnitDef)unit;
                    result = defUnit.absorbDamage(result);
                }
            }
            return result;
        }
        public abstract int calculateRange();
        public void getHealed(int healingAmount)
        {
            healingParticles.particlesOn = true;
            healingParticles.EmitterLocation = new Vector2(ContainingTile.getX(), ContainingTile.getY());
            floatingText.displayDamageTaken(healingAmount, false);
            Life += healingAmount;
        }
        public TileBasic[] getAttackableTiles()
        {
            int calculatedRange = calculateRange();
            List<TileBasic> currentAttackableTiles = ContainingTile.getSurroundingTiles().ToList<TileBasic>();
            List<TileBasic> tempTiles = new List<TileBasic>();
            for (int i = 0; i < calculatedRange - 1; i++)
            {
                foreach (TileBasic tile in currentAttackableTiles)
                {
                    if (tile != null)
                    {
                        foreach (TileBasic newTile in tile.getSurroundingTiles())
                        {
                            if (!currentAttackableTiles.Contains(newTile))
                            {
                                tempTiles.Add(newTile);
                            }
                        }
                    }
                }
                foreach (TileBasic tile in tempTiles)
                {
                    currentAttackableTiles.Add(tile);
                }

            }
            return currentAttackableTiles.ToArray<TileBasic>();
        }
        protected void incrementAttack()
        {
            AttacksThisRound++;
        }
        public void resetAttacks()
        {
            AttacksThisRound = 0;
        }
        public int calculateAttackCost()
        {
            return (StaminaAttCost * (AttacksThisRound + 1));
        }
        public abstract void update();
        public abstract void draw(SpriteBatch spriteBatch);
    }
}
