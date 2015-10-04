using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RPGProjectLibrary;

namespace RPGProject
{
    public class BattleTechnique
    {//This class holds the parameters of a battle characters technique, including attacking, defending, and special skills.

        public string techniqueName;
        public string techniqueDescription;
        public BattleTechniqueRange techniqueRange;
        public BattleTechniqueTargetType techniqueTargeting;
        public int techniqueCost;
        public List<BattleTechniqueEffect> effects;
        public List<BattleTechniqueAction> actions;
        public List<BattleTechniqueBuffData> buffs;
        public List<BattleTechniqueSoundData> sounds;
        public List<BattleTechniqueAICategory> AICategories;


        public BattleTechnique()
        {
        }
        public void LoadContent(BattleCharacter character, BattleTechniqueData techniqueData)
        {
            character.Sprites.Add(techniqueData.techniqueName, AnimatedSprite.CreateFromData(techniqueData.characterAnimation));
            character.Sprites[techniqueData.techniqueName].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
            techniqueName = techniqueData.techniqueName;
            techniqueDescription = techniqueData.techniqueDescription;
            techniqueRange = techniqueData.techniqueRange;
            techniqueTargeting = techniqueData.techniqueTargeting;
            techniqueCost = techniqueData.techniqueCost;

            effects = new List<BattleTechniqueEffect>();
            foreach(BattleTechniqueEffectData effectData in techniqueData.effects)
            {
                BattleTechniqueEffect newEffect = new BattleTechniqueEffect();
                newEffect.LoadContent(effectData);
                effects.Add(newEffect);
            }

            actions = new List<BattleTechniqueAction>();
            foreach (BattleTechniqueActionData actionData in techniqueData.actions)
            {
                BattleTechniqueAction newAction = new BattleTechniqueAction();
                newAction.LoadContent(actionData);
                actions.Add(newAction);
            }

            buffs = techniqueData.buffs;
            sounds = techniqueData.sounds;
            foreach (BattleTechniqueSoundData sound in sounds)
            {
                GameClass.SoundManager.LoadSound(sound.soundFileName);
            }
            AICategories = techniqueData.AICategories;
        }
        public static BattleTechnique GenerateItemTechnique(BattleCharacter characterUsingItem, BattleItem itemBeingUsed)
        {
            BattleTechnique newTechnique = new BattleTechnique();
            newTechnique.techniqueName = itemBeingUsed.ItemID.ToString();
            newTechnique.techniqueDescription = "";
            newTechnique.techniqueRange = BattleTechniqueRange.Ranged;
            newTechnique.techniqueTargeting = itemBeingUsed.BattleEffectTargeting;
            newTechnique.techniqueCost = 0;
            newTechnique.effects = new List<BattleTechniqueEffect>();
            newTechnique.effects.Add(itemBeingUsed.BattleEffect);
            newTechnique.actions = new List<BattleTechniqueAction>();
            newTechnique.buffs = new List<BattleTechniqueBuffData>();
            newTechnique.sounds = new List<BattleTechniqueSoundData>();
            return newTechnique;
        }
    }
}
