using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{//This class creates an instance of an enemy character to oppose the player team in battle.
    class EnemyBattleCharacter : BattleCharacter
    {
        bool deathSoundPlayed = false;
        int expWorth;
        public int EXPWorth
        {
            get
            {
                return expWorth;
            }
        }

        public EnemyBattleCharacter()
        {
        }
        public void LoadContent(int battleOrder, CharacterIdentity enemyID)
        {
           EnemyBattleCharacterData enemyData = GameClass.ContentManager.Load<EnemyBattleCharacterData>("BattleCharacters/" + enemyID.ToString() + "/" + enemyID.ToString() + "Battle");
           
           name = enemyData.name;
           runSpeed = enemyData.runSpeed;
           this.battleOrder = battleOrder;
           Sprites = new Dictionary<string, AnimatedSprite>();
           Sprites.Add("Idle", AnimatedSprite.CreateFromData(enemyData.idleAnimation));
           Sprites.Add("Run", AnimatedSprite.CreateFromData(enemyData.runAnimation));
           Sprites.Add("Damage", AnimatedSprite.CreateFromData(enemyData.damageAnimation));
           Sprites.Add("Death", AnimatedSprite.CreateFromData(enemyData.deathAnimation));
           Sprites["Death"].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
           yDrawOffset = enemyData.drawOffset;

           switch (battleOrder)
           {
               case 3:
                   position = new Vector2(390, 275 - enemyData.drawOffset);
                   battlePortrait = GameClass.LoadTextureData("General/EnemyBattlePortrait1");
                   battlePortraitPosition = new Vector2(330, 338);
                   break;
               case 4:
                   position = new Vector2(440, 295 - enemyData.drawOffset);
                   battlePortrait = GameClass.LoadTextureData("General/EnemyBattlePortrait2");
                   battlePortraitPosition = new Vector2(330, 358);
                   break;
               case 5:
                   position = new Vector2(490, 315 - enemyData.drawOffset);
                   battlePortrait = GameClass.LoadTextureData("General/EnemyBattlePortrait3");
                   battlePortraitPosition = new Vector2(330, 378);
                   break;
           }


           CurrentAction = "Idle";
           CurrentSprite.CurrentFrame = GameClass.Random.Next(0, CurrentSprite.TotalFrames);

           Facing = CharacterFacing.Left;
           maxHP = enemyData.HP;
           currentHP = maxHP;
           displayHP = maxHP;
           maxMP = enemyData.MP;
           currentMP = maxMP;
           displayMP = maxMP;
           baseATK = enemyData.ATK;
           currentATK = baseATK;
           baseDEF = enemyData.DEF;
           currentDEF = baseDEF;
           baseMAG_ATK = enemyData.MAG_ATK;
           currentMAG_ATK= baseMAG_ATK;
           baseMAG_DEF = enemyData.MAG_DEF;
           currentMAG_DEF = baseMAG_DEF;
           baseSPD = enemyData.SPD;
           currentSPD = baseSPD;
           expWorth = enemyData.EXPWorth;

            techniques = new List<BattleTechnique>();
            foreach (BattleTechniqueData techData in enemyData.techniques)
            {
                BattleTechnique newTechnique = new BattleTechnique();
                newTechnique.LoadContent(this, techData);
                techniques.Add(newTechnique);
            }

        }
        public override void Update(GameTime gameTime)
        {
            if (CurrentAction == "Death" && deathSoundPlayed == false)
            {
                deathSoundPlayed = true;
                GameClass.SoundManager.PlaySoundEffect("Audio/enemyDeath");
            }
            base.Update(gameTime);
        }
    }
}
