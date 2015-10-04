using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    public enum BattleEffectState
    {
        Active,
        Inactive
    }
    //This class holds the information for aura and projectile effects that occur when a Battle Character performs a technique
    public class BattleTechniqueEffect : SingleSpriteAnimatingObject
    {
        BattleEffectState effectState;
        public BattleEffectState EffectState
        {
            get
            {
               return effectState;
            }
            set
            {
                if (value == BattleEffectState.Inactive)
                {
                    Sprite.CurrentFrame = 0;
                    velocity = Vector2.Zero;
                }
                effectState = value;
            }
        }

        EffectType effectType;
        public EffectType EffectType
        {
            get
            {
                return effectType;
            }
        }

        int startTime;
        public int StartTime
        {
            get
            {
                return startTime;
            }
        }

        int lifeTime;
        public int LifeTime
        {
            get
            {
                return lifeTime;
            }
            set
            {
                lifeTime = value;
            }
        }

        float currentTimer;
        public float CurrentTimer
        {
            get
            {
                return currentTimer;
            }
        }

        float speed;
        public float Speed
        {
            get
            {
                return speed;
            }

        }

        Vector2 relativePosition;
        public Vector2 RelativePosition
        {
            get
            {
                return relativePosition;
            }

        }

        List<BattleTechniqueAction> effectActions;
        public List<BattleTechniqueAction> EffectActions
        {
            get
            {
                return effectActions;
            }
        }

        List<BattleTechniqueBuffData> effectBuffs;
        public List<BattleTechniqueBuffData> EffectBuffs
        {
            get
            {
                return effectBuffs;
            }
        }

        List<BattleTechniqueSoundData> effectSounds;
        public List<BattleTechniqueSoundData> EffectSounds
        {
            get
            {
                return effectSounds;
            }
        }

        public BattleTechniqueEffect()
        {
        }

        public void Initialise(Vector2 characterPosition)
        {
            position = new Vector2(characterPosition.X + relativePosition.X, characterPosition.Y + relativePosition.Y);
            velocity = Vector2.Zero;
            effectState = BattleEffectState.Inactive;
            currentTimer = 0;
        }

        public void LoadContent(BattleTechniqueEffectData effectData)
        {
            effectType = effectData.effectType;
            relativePosition = effectData.relativePosition;
            sprite = AnimatedSprite.CreateFromData(effectData.effectAnimation);
            startTime = effectData.startTime;
            lifeTime = effectData.lifeTime;
            speed = effectData.speed;
            effectActions = new List<BattleTechniqueAction>();
            foreach (BattleTechniqueActionData effectActionData in effectData.effectActions)
            {
                BattleTechniqueAction newEffectAction = new BattleTechniqueAction();
                newEffectAction.LoadContent(effectActionData);
                effectActions.Add(newEffectAction);
            }

            effectBuffs = effectData.effectBuffs;
            effectSounds = effectData.effectSounds;
            foreach (BattleTechniqueSoundData sound in effectSounds)
            {
                GameClass.SoundManager.LoadSound(sound.soundFileName);
            }
        }
        public void LoadContent(BattleTechniqueEffect effectToCopy)
        {
            effectType = effectToCopy.effectType;
            relativePosition = effectToCopy.relativePosition;
            sprite = new AnimatedSprite(effectToCopy.Sprite.TotalFrames, effectToCopy.Sprite.TextureFileName,  effectToCopy.Sprite.FPS);
            startTime = effectToCopy.StartTime;
            lifeTime = effectToCopy.LifeTime;
            speed = effectToCopy.Speed;
            position = effectToCopy.Position;
            effectActions = effectToCopy.effectActions;
            effectBuffs = effectToCopy.effectBuffs;
            effectSounds = effectToCopy.effectSounds;
        }

        public override void Update(GameTime gameTime)
        {
            if (effectState == BattleEffectState.Active)
            {
                if (currentTimer > lifeTime || currentTimer == lifeTime)
                {
                    currentTimer = 0;
                    effectState = BattleEffectState.Inactive;
                }
                else
                {
                    currentTimer += 1 / Sprite.FPS;
                }
                base.Update(gameTime);
            }
        }
        public override void Draw()
        {
            if (effectState == BattleEffectState.Active)
            {
                if (Velocity.X > 0)
                {
                    SpriteEffect = SpriteEffects.None;
                }
                else if (Velocity.X < 0)
                {
                    SpriteEffect = SpriteEffects.FlipHorizontally;
                }
                base.Draw();
            }
        }
        public static BattleTechniqueEffect DuplicateEffect(BattleTechniqueEffect effectToCopy)
        {
            BattleTechniqueEffect effectCopy = new BattleTechniqueEffect();
            effectCopy.LoadContent(effectToCopy);
            return effectCopy;
        }
    }
}
