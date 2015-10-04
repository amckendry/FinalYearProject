using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    public class BattleTechniqueBuff : TexturedGameObject
    {
        BattleCharacter target;
        public BattleCharacter Target
        {
            get
            {
                return target;
            }
        }
        int buffTime;
        public int BuffTime
        {
            get
            {
                return buffTime;
            }
        }
        BattleTechniqueBuffType buffType;
        public BattleTechniqueBuffType BuffType
        {
            get
            {
                return buffType;
            }
        }
        int buffStrength;
        public int BuffStrength
        {
            get
            {
                return buffStrength;
            }
        }
        int duration;
        public int Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }
        AlternatingFadeState arrowFadeState;
        float arrowAlpha;
        int drawOffsetX = 0;

        public BattleTechniqueBuff()
        {
        }
        public void LoadContent(BattleCharacter target, BattleTechniqueBuffData buffData)
        {
            this.target = target;
            buffTime = buffData.buffTime;
            buffType = buffData.buffType;
            buffStrength = buffData.buffStrength;
            duration = buffData.duration;
            texture = BattleScreen.BuffArrowTexture;
            arrowFadeState = AlternatingFadeState.FadingOut;
            arrowAlpha = 0.0f;

            if (buffStrength == 0)
            {
                throw new Exception("A buff with base strength 0 is not allowed.");
            }
            else if(buffStrength < 0)
            {
                SpriteEffect = SpriteEffects.FlipVertically;
            }

            switch (buffType)
            {
                case BattleTechniqueBuffType.ATK:
                    Tint = Color.Red;
                    drawOffsetX = 2;
                    break;
                case BattleTechniqueBuffType.DEF:
                    Tint = Color.Green;
                    drawOffsetX = 12;
                    break;
                case BattleTechniqueBuffType.MAG_ATK:
                    Tint = Color.Orange;
                    drawOffsetX = 22;
                    break;
                case BattleTechniqueBuffType.MAG_DEF:
                    Tint = Color.Purple;
                    drawOffsetX = 32;
                    break;
                case BattleTechniqueBuffType.SPD:
                    Tint = Color.Yellow;
                    drawOffsetX = 42;
                    break;
            }

            Position = new Vector2(target.BattlePortraitPosition.X + drawOffsetX, target.BattlePortraitPosition.Y + 1);
        }

        public void ApplyStatChange()
        {
            switch(buffType)
            {
                case BattleTechniqueBuffType.ATK:
                    target.CurrentATK += (buffStrength * target.BaseATK) / 100;
                    break;
                case BattleTechniqueBuffType.DEF:
                    target.CurrentDEF += (buffStrength * target.BaseDEF) / 100;
                    break;
                case BattleTechniqueBuffType.MAG_ATK:
                    target.CurrentMAG_ATK += (buffStrength * target.BaseMAG_ATK) / 100;
                    break;
                case BattleTechniqueBuffType.MAG_DEF:
                    target.CurrentMAG_DEF += (buffStrength * target.BaseMAG_DEF) / 100;
                    break;
                case BattleTechniqueBuffType.SPD:
                    target.CurrentSPD += (buffStrength *target.BaseSPD) / 100;
                    break;
            }
        }
        public void RemoveStatChange()
        {
            switch (buffType)
            {
                case BattleTechniqueBuffType.ATK:
                    target.CurrentATK = target.BaseATK;
                    break;
                case BattleTechniqueBuffType.DEF:
                    target.CurrentDEF = target.BaseDEF;
                    break;
                case BattleTechniqueBuffType.MAG_ATK:
                    target.CurrentMAG_ATK = target.BaseMAG_ATK;
                    break;
                case BattleTechniqueBuffType.MAG_DEF:
                    target.CurrentMAG_DEF = target.BaseMAG_DEF;
                    break;
                case BattleTechniqueBuffType.SPD:
                    target.CurrentSPD = target.BaseSPD;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            switch (arrowFadeState)
            {
                case AlternatingFadeState.FadingIn:
                    if (arrowAlpha > 0.0f)
                    {
                        arrowAlpha -= 0.05f;
                    }
                    else
                    {
                        arrowAlpha = 0.0f;
                        arrowFadeState = AlternatingFadeState.FadingOut;
                    }
                    break;

                case AlternatingFadeState.FadingOut:
                    if (arrowAlpha < 1.0f)
                    {
                        arrowAlpha += 0.05f;
                    }
                    else
                    {
                        arrowAlpha = 1.0f;
                        arrowFadeState = AlternatingFadeState.FadingIn;
                    }
                    break;
            }
            base.Update(gameTime);
        }

        public override void Draw()
        {
            Tint = new Color(Tint.R, Tint.G, Tint.B, arrowAlpha);
            base.Draw();
        }
    }
}
