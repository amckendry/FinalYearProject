using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    public class PlayerBattleCharacter : BattleCharacter
    {
        public PlayerBattleCharacter()
        {
        }
        public void LoadContent(int battleOrder, PartyCharacter partyCharacter)
        {
           PlayerBattleCharacterData playerData = GameClass.ContentManager.Load<PlayerBattleCharacterData>("BattleCharacters/" + partyCharacter.CharacterID.ToString() + "/" + partyCharacter.CharacterID.ToString() + "Battle");
          
           name = playerData.name;
           runSpeed = playerData.runSpeed;
           this.battleOrder = battleOrder;
           this.battlePortrait = GameClass.LoadTextureData(playerData.battlePortraitFileName);
           Sprites = new Dictionary<string, AnimatedSprite>();
           Sprites.Add("BattleStart", AnimatedSprite.CreateFromData(playerData.battleStartAnimation));
           Sprites.Add("Idle", AnimatedSprite.CreateFromData(playerData.idleAnimation));
           Sprites.Add("Run", AnimatedSprite.CreateFromData(playerData.runAnimation));
           Sprites.Add("Damage", AnimatedSprite.CreateFromData(playerData.damageAnimation));
           Sprites["Damage"].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
           Sprites.Add("Death", AnimatedSprite.CreateFromData(playerData.deathAnimation));
           Sprites["Death"].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
           Sprites.Add("Revived", AnimatedSprite.CreateFromData(playerData.revivedAnimation));
           Sprites["Revived"].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
           Sprites.Add("Victory", AnimatedSprite.CreateFromData(playerData.victoryAnimation));
           Sprites["Victory"].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
           Sprites.Add("Item", AnimatedSprite.CreateFromData(playerData.itemAnimation));
           Sprites["Item"].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
           yDrawOffset = playerData.drawOffset;


           switch (battleOrder)
           {
               case 0:
                   position = new Vector2(260, 275 - playerData.drawOffset);
                   battlePortraitPosition = new Vector2(10, 338);
                   break;
               case 1:
                   position = new Vector2(210, 295 - playerData.drawOffset);
                   battlePortraitPosition = new Vector2(10, 358);
                   break;
               case 2:
                   position = new Vector2(160, 315 - playerData.drawOffset);
                   battlePortraitPosition = new Vector2(10, 378);
                   break;
           }

           CurrentAction = "BattleStart";
           Facing = CharacterFacing.Right;
           maxHP = partyCharacter.HP;
           currentHP = maxHP;
           displayHP = maxHP;
           maxMP = partyCharacter.MP;
           currentMP = maxMP;
           displayMP = maxMP;
           baseATK = partyCharacter.ATK;
           currentATK = baseATK;
           baseDEF = partyCharacter.DEF;
           currentDEF = baseDEF;
           baseMAG_ATK = partyCharacter.MAG_ATK;
           currentMAG_ATK= baseMAG_ATK;
           baseMAG_DEF = partyCharacter.MAG_DEF;
           currentMAG_DEF = baseMAG_DEF;
           baseSPD = partyCharacter.SPD;
           currentSPD = baseSPD;

            techniques = new List<BattleTechnique>();
            foreach (BattleTechniqueData techData in playerData.techniques)
            {
                int techLevelRestriction = 0;
                foreach(BattleTechniqueLevelData techLevelData in partyCharacter.TechniqueLevelData)
                {
                    if (techLevelData.techniqueName == techData.techniqueName)
                    {
                        techLevelRestriction = techLevelData.levelLearnt;
                    }
                }

                if (!(partyCharacter.Level < techLevelRestriction))
                {
                    BattleTechnique newTechnique = new BattleTechnique();
                    newTechnique.LoadContent(this, techData);
                    techniques.Add(newTechnique);
                }
            }

        }
    }
}
