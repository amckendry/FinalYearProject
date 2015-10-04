using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    public class BattleCharacter : MultipleSpriteAnimatingObject<string>
    {//This class is a base for all characters that participate in battle. Player and Enemy Battle Character classes extend this class.

        #region Fields
        CharacterFacing facing;
        public CharacterFacing Facing
        {
            get
            {
              return facing;
            }
            set
            {
                facing = value;
            }
        }

        //These length factors determine how much of each status bar is drawn in the Draw() Method
        float HPBarLengthFactor = 1.0f;
        float HPDamageBarLengthFactor = 0.0f;
        float HPHealingBarLengthFactor = 0.0f;
        float MPBarLengthFactor = 1.0f;
        float MPDamageBarLengthFactor = 0.0f;
        float MPHealingBarLengthFactor = 0.0f;

        //Display HP and MP are used to create the illusion of gradually decreasing HP and MP bars.
        protected int displayHP;
        protected int displayMP;

        //The Battle Order determines where the character and its assets are drawn. Player Characters are 0-2, Enemies are 3-5.
        protected int battleOrder;
        public int BattleOrder
        {
            get
            {
                return battleOrder;
            }
            set
            {
                battleOrder = value;
            }
        }

        protected Texture2D battlePortrait;
        public Texture2D BattlePortrait
            {
                get
                {
                    return battlePortrait;
                }
            }

        protected string name;
        public string Name
         {
             get
             {
                 return name;
             }
         }

        protected float runSpeed;
        public float RunSpeed
        {
            get
            {
                return runSpeed;
            }
        }

        protected int currentHP;
        public int CurrentHP
         {
             get
             {
                 return currentHP;
             }
             set
             {
                 currentHP = value;
             }
         }


        protected int maxHP;
        public int MaxHP
         {
             get
             {
                 return maxHP;
             }
             set
             {
                 maxHP = value;
             }
         }

        protected int currentMP;
        public int CurrentMP
         {
             get
             {
                 return currentMP;
             }
             set
             {
                 currentMP = value;
             }
         }

         protected int maxMP;
         public int MaxMP
         {
             get
             {
                 return maxMP;
             }
             set
             {
                 maxMP = value;
             }
         }

         protected int baseATK;
         public int BaseATK
         {
             get
             {
                 return baseATK;
             }
         }

         protected int currentATK;
         public int CurrentATK
         {
             get
             {
                 return currentATK;
             }
             set
             {
                 currentATK = value;
             }
         }

         protected int baseDEF;
         public int BaseDEF
         {
             get
             {
                 return baseDEF;
             }
         }

         protected int currentDEF;
         public int CurrentDEF
         {
             get
             {
                 return currentDEF;
             }
             set
             {
                 currentDEF = value;
             }
         }

         protected int baseMAG_ATK;
         public int BaseMAG_ATK
         {
             get
             {
                 return baseMAG_ATK;
             }
         }

         protected int currentMAG_ATK;
         public int CurrentMAG_ATK
         {
             get
             {
                 return currentMAG_ATK;
             }
             set
             {
                 currentMAG_ATK = value;
             }
         }

         protected int baseMAG_DEF;
         public int BaseMAG_DEF
         {
             get
             {
                 return baseMAG_DEF;
             }
         }

         protected int currentMAG_DEF;
         public int CurrentMAG_DEF
         {
             get
             {
                 return currentMAG_DEF;
             }
             set
             {
                 currentMAG_DEF = value;
             }
         }

         protected int baseSPD;
         public int BaseSPD
         {
             get
             {
                 return baseSPD;
             }
         }

         protected int currentSPD;
         public int CurrentSPD
         {
             get
             {
                 return currentSPD;
             }
             set
             {
                 currentSPD = value;
             }
         }

         //Since some characters are taller than others, an offset in their Y position must be introduced to make it appear that all characters are standing on the same plain.
         protected int yDrawOffset;
         public int YDrawOffset
         {
             get
             {
                 return yDrawOffset;
             }
         }

         protected List<BattleTechnique> techniques;
         public List<BattleTechnique> Techniques
         {
             get
             {
                 return techniques;
             }
         }

         protected Vector2 battlePortraitPosition;
         public Vector2 BattlePortraitPosition
         {
             get
             {
                 return battlePortraitPosition;
             }
         }
         #endregion
        #region Methods

         public void ChangeHP(int HPChange, BattleTechniqueActionType HPChangeType)
         {//This method manages the change of HP as dictated by a damage action from the BattleScreen instance.
             switch (HPChangeType)
             {
                 case BattleTechniqueActionType.Damage:
                     CurrentAction = "Damage";
                     currentHP -= HPChange;
                     break;

                 case BattleTechniqueActionType.Healing:
                     currentHP += HPChange;
                     break;
                 case BattleTechniqueActionType.Revive:
                     CurrentAction = "Revived";
                     currentHP += HPChange;
                     break;
             }
             MathHelper.Clamp(currentHP, 0, maxHP);
             MathHelper.Clamp(displayHP, 0, maxHP);

             //HP bar is set to immediately show the players current health.
             if (HPChangeType == BattleTechniqueActionType.Damage)
             {
                 HPBarLengthFactor = (float)currentHP / (float)maxHP;
             }
             else
             {
                 HPBarLengthFactor = (float)displayHP / (float)maxHP;
                 HPHealingBarLengthFactor = (float)currentHP / (float)maxHP;
             }
         }
         public void ChangeMP(int MPChange, BattleTechniqueActionType MPChangeType)
         {//This method manages the change of MP as dictated by a damage action from the BattleScreen instance.
             switch (MPChangeType)
             {
                 case BattleTechniqueActionType.Damage:
                     currentMP -= MPChange;
                     break;

                 case BattleTechniqueActionType.Healing:
                     currentMP += MPChange;
                     break;
             }

             MathHelper.Clamp(currentMP, 0, maxMP);
             MathHelper.Clamp(displayMP, 0, maxMP);

             //MP bar is set to immediately show the players current MP.
             MPBarLengthFactor = (float)currentMP / (float)maxMP;
         }

         public override void Update(GameTime gameTime)
         {

             //Clamp HP and MP between 0 and the max HP and MP.
             if (currentHP < 0)
             {
                 currentHP = 0;
             }
             else if (currentHP > maxHP)
             {
                 currentHP = maxHP;
             }

             if (currentMP < 0)
             {
                 currentMP = 0;
             }
             else if (currentMP > maxMP)
             {
                 currentMP = maxMP;
             }

             //Handle changes in facing.
             switch (facing)
             {
                 case CharacterFacing.Left:
                     SpriteEffect = SpriteEffects.FlipHorizontally;
                     break;

                 case CharacterFacing.Right:
                     SpriteEffect = SpriteEffects.None;
                     break;
             }



             switch (CurrentAction)
             {
                 case "BattleStart":
                     //If the character is doing an introductory sequence and its sprite has reached looping point, switch to the idle animation.
                     if (!(CurrentSprite.CurrentFrame < CurrentSprite.TotalFrames - 1))
                     {
                         CurrentAction = "Idle";
                     }
                     break;

                 case "Damage":
                     //If the character is damaged and its sprite has reached looping point....
                     if (CurrentSprite.CurrentFrame > CurrentSprite.TotalFrames - 2)
                     {   //If the damage it took dropped its HP to 0 or less, then perform death animation.
                         if (currentHP == 0 || currentHP < 0)
                         {
                             currentHP = 0;
                             displayHP = 0;
                             CurrentAction = "Death";
                         }
                         //Otherwise return to the idle animation.
                         else
                         {
                             CurrentAction = "Idle";
                         }
                     }
                     break;
                 case "Revived":
                     //if the character is performing the Revival sequence, and its sprite has reached looping point, change to idle animation.
                     if (CurrentSprite.CurrentFrame > CurrentSprite.TotalFrames - 2)
                     {
                         CurrentAction = "Idle";
                     }
                     break;
             }


             //if the display HP bar is shorter than what it should be, then increase the display HP gradually.
             if (displayHP < currentHP)
             {
                 if (displayHP < maxHP)
                 {
                     displayHP += 1;
                 }
                 else
                 {
                     displayHP = maxHP;
                 }
                 HPDamageBarLengthFactor = 0.0f;
                 HPBarLengthFactor = (float)currentHP / (float)maxHP;
                 HPHealingBarLengthFactor = (float)displayHP / (float)maxHP;
             }
             //if the display HP bar is longer than what it should be, then decrease the display HP gradually.
             else if (displayHP > currentHP)
             {
                 if (displayHP > 0)
                 {
                     displayHP -= 1;
                 }
                 else
                 {
                     displayHP = 0;
                 }
                 HPDamageBarLengthFactor = (float)displayHP / (float)maxHP;
                 HPHealingBarLengthFactor = 0.0f;
             }
             else
             {
                 HPDamageBarLengthFactor = 0.0f;
                 HPHealingBarLengthFactor = 0.0f;
             }

             //if the display MP bar is shorter than what it should be, then increase the display MP gradually.
             if (displayMP < currentMP)
             {
                 if (displayMP < maxMP)
                 {
                     displayMP += 1;
                 }
                 else
                 {
                     displayMP = maxMP;
                 }
                 MPDamageBarLengthFactor = 0.0f;
                 MPHealingBarLengthFactor = (float)displayMP / (float)maxMP;
             }
             //if the display MP bar is longer than what it should be, then decrease the display MP gradually.
             else if (displayMP > currentMP)
             {
                 if (displayMP > 0)
                 {
                     displayMP -= 1;
                 }
                 else
                 {
                     displayMP = 0;
                 }
                 MPDamageBarLengthFactor = (float)displayMP / (float)maxMP;
                 MPHealingBarLengthFactor = 0.0f;
             }
             else
             {
                 MPDamageBarLengthFactor = 0.0f;
                 MPHealingBarLengthFactor = 0.0f;
             }

             base.Update(gameTime);
         }
         public override void Draw()
         {
             //A draw offset for the drawing of the display HP and MP is introduced here, so that whenever they reach 100 they still appear centred.
             int HPDrawOffset = 0;
             int MPDrawOffset = 0;
             bool drawMPBar = true;

             //Clamp the display HP and MP here so that they never appear as less than zero or greater than the characters max.
             if (displayHP < 0)
             {
                 displayHP = 0;
             }
             else if (displayHP > maxHP)
             {
                 displayHP = maxHP;
             }

             if (displayMP < 0)
             {
                 displayMP = 0;
             }
             else if (displayMP > maxMP)
             {
                 displayMP = maxMP;
             }


             if (displayHP > 99)
             {
                 HPDrawOffset = -3;
             }
             if (maxMP == 0)
             {
                drawMPBar = false;
             }
             
             if (displayMP > 99)
             {
                 MPDrawOffset = -3;
             }

             //Draw the appropraite battle portrait in the appropraite place calculated from the battle order of the derived class instance.
             GameClass.SpriteBatch.Draw(battlePortrait, battlePortraitPosition, Color.White);


             //Draw all other assets of the character(i.e. Status bars and text) in the appropraite position depending on the battle order.
             switch (battleOrder)
             {

                 case 0:
                     GameClass.SpriteBatch.DrawString(GameClass.Size8Font, name, new Vector2(66, 335), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.StatusBarTexture, new Vector2(115, 338), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayHP.ToString(), new Vector2(211 + HPDrawOffset, 336), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayMP.ToString(), new Vector2(277 + MPDrawOffset, 336), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(141, 342), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPDamageBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Red);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(141, 342), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPHealingBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Blue);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(141, 342), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Lime);

                     if (drawMPBar)
                     {
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(250, 342), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPDamageBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Red);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(250, 342), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPHealingBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Blue);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(250, 342), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Magenta);
                     }
                     break;
                 case 1:
                     GameClass.SpriteBatch.DrawString(GameClass.Size8Font, name, new Vector2(66, 355), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.StatusBarTexture, new Vector2(115, 358), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayHP.ToString(), new Vector2(211 + HPDrawOffset, 356), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayMP.ToString(), new Vector2(277 + MPDrawOffset, 356), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(141, 362), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPDamageBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Red);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(141, 362), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPHealingBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Blue);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(141, 362), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Lime);
                     if (drawMPBar)
                     {
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(250, 362), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPDamageBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Red);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(250, 362), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPHealingBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Blue);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(250, 362), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Magenta);
                     }
                     break;
                 case 2:
                     GameClass.SpriteBatch.DrawString(GameClass.Size8Font, name, new Vector2(66, 375), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.StatusBarTexture, new Vector2(115, 378), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayHP.ToString(), new Vector2(211 + HPDrawOffset, 376), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayMP.ToString(), new Vector2(277 + MPDrawOffset, 376), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(141, 382), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPDamageBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Red);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(141, 382), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPHealingBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Blue);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(141, 382), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Lime);
                     if (drawMPBar)
                     {
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(250, 382), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPDamageBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Red);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(250, 382), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPHealingBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Blue);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(250, 382), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Magenta);
                     }
                     break;

                 case 3:
                     GameClass.SpriteBatch.DrawString(GameClass.Size8Font, name, new Vector2(386, 335), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.StatusBarTexture, new Vector2(435, 338), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayHP.ToString(), new Vector2(531 + HPDrawOffset, 336), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayMP.ToString(), new Vector2(597 + MPDrawOffset, 336), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(461, 342), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPDamageBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Red);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(461, 342), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPHealingBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Blue);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(461, 342), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Lime);
                     if (drawMPBar)
                     {
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(570, 342), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPDamageBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Red);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(570, 342), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPHealingBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Blue);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(570, 342), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Magenta);
                     }
                     break;
                 case 4:
                     GameClass.SpriteBatch.DrawString(GameClass.Size8Font, name, new Vector2(386, 355), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.StatusBarTexture, new Vector2(435, 358), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayHP.ToString(), new Vector2(531 + HPDrawOffset, 356), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayMP.ToString(), new Vector2(597 + MPDrawOffset, 356), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(461, 362), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPDamageBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Red);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(461, 362), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPHealingBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Blue);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(461, 362), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Lime);
                     if (drawMPBar)
                     {
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(570, 362), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPDamageBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Red);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(570, 362), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPHealingBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Blue);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(570, 362), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Magenta);
                     }
                     break;
                 case 5:
                     GameClass.SpriteBatch.DrawString(GameClass.Size8Font, name, new Vector2(386, 375), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.StatusBarTexture, new Vector2(435, 378), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayHP.ToString(), new Vector2(531 + HPDrawOffset, 376), Color.White);
                     GameClass.SpriteBatch.DrawString(GameClass.Size6Font, displayMP.ToString(), new Vector2(597 + MPDrawOffset, 376), Color.White);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(461, 382), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPDamageBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Red);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(461, 382), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPHealingBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Blue);
                     GameClass.SpriteBatch.Draw(BattleScreen.HPBarTexture, new Vector2(461, 382), new Rectangle(0, 0, (int)(BattleScreen.HPBarTexture.Width * HPBarLengthFactor), BattleScreen.HPBarTexture.Height), Color.Lime);
                     if (drawMPBar)
                     {
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(570, 382), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPDamageBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Red);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(570, 382), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPHealingBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Blue);
                         GameClass.SpriteBatch.Draw(BattleScreen.MPBarTexture, new Vector2(570, 382), new Rectangle(0, 0, (int)(BattleScreen.MPBarTexture.Width * MPBarLengthFactor), BattleScreen.MPBarTexture.Height), Color.Magenta);
                     }
                     break;
             }

             base.Draw();
         }
         #endregion
    }
}
