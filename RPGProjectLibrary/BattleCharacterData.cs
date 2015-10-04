using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace RPGProjectLibrary
{
    public enum BattleTechniqueRange
    {
        Direct,
        Ranged
    }
    public enum BattleTechniqueTargetType
    {
        Self,
        SingleEnemy,
        AllEnemies,
        SingleActiveAlly,
        SingleKOdAlly,
        AllActiveAllies,
        Everyone
    }
    public enum BattleTechniqueActionType
    {
        Damage,
        Healing,
        Revive
    }
    public enum ActionStatType
    {
        Physical,
        Magical
    }
    public enum EffectType
    {
        SpellTargeted,
        SpellIndirect,
        ProjectileTargeted,
        ProjectileIndirect
    }
    public enum BattleTechniqueBuffType
    {
        ATK,
        DEF,
        MAG_ATK,
        MAG_DEF,
        SPD
    }
    public enum BattleTechniqueAICategory
    {
        Offensive,
        Damaging,
        Buff,
        Healing
    }

    public struct PlayerBattleCharacterData
    {
        public string name;
        public string battlePortraitFileName;
        public int drawOffset;
        public float runSpeed;
        public AnimatedSpriteData battleStartAnimation;
        public AnimatedSpriteData idleAnimation;
        public AnimatedSpriteData runAnimation;
        public AnimatedSpriteData damageAnimation;
        public AnimatedSpriteData deathAnimation;
        public AnimatedSpriteData revivedAnimation;
        public AnimatedSpriteData victoryAnimation;
        public AnimatedSpriteData itemAnimation;
        public List<BattleTechniqueData> techniques;
    }
    public class PlayerBattleCharacterDataReader : ContentTypeReader<PlayerBattleCharacterData>
    {
        protected override PlayerBattleCharacterData Read(ContentReader input, PlayerBattleCharacterData existingInstance)
        {
            PlayerBattleCharacterData data = new PlayerBattleCharacterData();
            data.name = input.ReadString();
            data.battlePortraitFileName = input.ReadString();
            data.drawOffset = input.ReadInt32();
            data.runSpeed = (float)input.ReadDouble();
            data.battleStartAnimation = input.ReadObject<AnimatedSpriteData>();
            data.idleAnimation = input.ReadObject<AnimatedSpriteData>();
            data.runAnimation = input.ReadObject<AnimatedSpriteData>();
            data.damageAnimation = input.ReadObject<AnimatedSpriteData>();
            data.deathAnimation = input.ReadObject<AnimatedSpriteData>();
            data.revivedAnimation = input.ReadObject<AnimatedSpriteData>();
            data.victoryAnimation = input.ReadObject<AnimatedSpriteData>();
            data.itemAnimation = input.ReadObject<AnimatedSpriteData>();
            data.techniques = input.ReadObject<List<BattleTechniqueData>>();
            return data;
        }
    }

    public struct EnemyBattleCharacterData
    {
        public string name;
        public int drawOffset;
        public float runSpeed;
        public int HP;
        public int MP;
        public int ATK;
        public int DEF;
        public int MAG_ATK;
        public int MAG_DEF;
        public int SPD;
        public int EXPWorth;
        public AnimatedSpriteData idleAnimation;
        public AnimatedSpriteData runAnimation;
        public AnimatedSpriteData damageAnimation;
        public AnimatedSpriteData deathAnimation;
        public List<BattleTechniqueData> techniques;
    }
    public class EnemyBattleCharacterDataReader : ContentTypeReader<EnemyBattleCharacterData>
    {
        protected override EnemyBattleCharacterData Read(ContentReader input, EnemyBattleCharacterData existingInstance)
        {
            EnemyBattleCharacterData data = new EnemyBattleCharacterData();
            data.name = input.ReadString();
            data.drawOffset = input.ReadInt32();
            data.runSpeed = (float)input.ReadDouble();
            data.HP = input.ReadInt32();
            data.MP = input.ReadInt32();
            data.ATK = input.ReadInt32();
            data.DEF = input.ReadInt32();
            data.MAG_ATK = input.ReadInt32();
            data.MAG_DEF = input.ReadInt32();
            data.SPD = input.ReadInt32();
            data.EXPWorth = input.ReadInt32();
            data.idleAnimation = input.ReadObject<AnimatedSpriteData>();
            data.runAnimation = input.ReadObject<AnimatedSpriteData>();
            data.damageAnimation = input.ReadObject<AnimatedSpriteData>();
            data.deathAnimation = input.ReadObject<AnimatedSpriteData>();
            data.techniques = input.ReadObject<List<BattleTechniqueData>>();
            return data;
        }
    }

    public struct BattleTechniqueData
    {
        public string techniqueName;
        public string techniqueDescription;
        public BattleTechniqueRange techniqueRange;
        public BattleTechniqueTargetType techniqueTargeting;
        public int techniqueCost;
        public AnimatedSpriteData characterAnimation;
        public List<BattleTechniqueEffectData> effects;
        public List<BattleTechniqueActionData> actions;
        public List<BattleTechniqueBuffData> buffs;
        public List<BattleTechniqueSoundData> sounds;
        [ContentSerializer(Optional = true)]
        public List<BattleTechniqueAICategory> AICategories;
    }
    public class BattleTechniqueDataReader : ContentTypeReader<BattleTechniqueData>
    {
        protected override BattleTechniqueData Read(ContentReader input, BattleTechniqueData existingInstance)
        {
            BattleTechniqueData data = new BattleTechniqueData();
            data.techniqueName = input.ReadString();
            data.techniqueDescription = input.ReadString();
            data.techniqueRange = input.ReadObject<BattleTechniqueRange>();
            data.techniqueTargeting = input.ReadObject<BattleTechniqueTargetType>();
            data.techniqueCost = input.ReadInt32();
            data.characterAnimation = input.ReadObject<AnimatedSpriteData>();
            data.effects = input.ReadObject<List<BattleTechniqueEffectData>>();
            data.actions = input.ReadObject<List<BattleTechniqueActionData>>();
            data.buffs = input.ReadObject<List<BattleTechniqueBuffData>>();
            data.sounds = input.ReadObject<List<BattleTechniqueSoundData>>();
            data.AICategories = input.ReadObject<List<BattleTechniqueAICategory>>();
            return data;
        }
    }

    public struct BattleTechniqueEffectData
    {
        public EffectType effectType;
        public AnimatedSpriteData effectAnimation;
        public Vector2 relativePosition;
        public float speed;
        public int startTime;
        public int lifeTime;
        public List<BattleTechniqueActionData> effectActions;
        public List<BattleTechniqueBuffData> effectBuffs;
        public List<BattleTechniqueSoundData> effectSounds;
    }
    public class BattleTechniqueEffectDataReader : ContentTypeReader<BattleTechniqueEffectData>
    {
        protected override BattleTechniqueEffectData Read(ContentReader input, BattleTechniqueEffectData existingInstance)
        {
            BattleTechniqueEffectData data = new BattleTechniqueEffectData();
            data.effectType = input.ReadObject<EffectType>();
            data.effectAnimation = input.ReadObject<AnimatedSpriteData>();
            data.relativePosition = input.ReadVector2();
            data.speed = (float)input.ReadDouble();
            data.startTime = input.ReadInt32();
            data.lifeTime = input.ReadInt32();
            data.effectActions = input.ReadObject<List<BattleTechniqueActionData>>();
            data.effectBuffs = input.ReadObject<List<BattleTechniqueBuffData>>();
            data.effectSounds = input.ReadObject<List<BattleTechniqueSoundData>>();
            return data;
        }
    }

    public struct BattleTechniqueActionData
    {
        public int actionTime;
        public BattleTechniqueActionType actionType;
        public ActionStatType actionStatType;
        public int strength;
    }
    public class BattleTechniqueActionDataReader : ContentTypeReader<BattleTechniqueActionData>
    {
        protected override BattleTechniqueActionData Read(ContentReader input, BattleTechniqueActionData existingInstance)
        {
            BattleTechniqueActionData data = new BattleTechniqueActionData();
            data.actionTime = input.ReadInt32();
            data.actionType = input.ReadObject<BattleTechniqueActionType>();
            data.actionStatType = input.ReadObject<ActionStatType>();
            data.strength = input.ReadInt32();
            return data;
        }
    }

    public struct EnemyDifficultyData
    {
        public CharacterIdentity enemyID;
        public int difficultyLevel;
    }
    public class EnemyDifficultyDataReader : ContentTypeReader<EnemyDifficultyData>
    {
        protected override EnemyDifficultyData Read(ContentReader input, EnemyDifficultyData existingInstance)
        {
            EnemyDifficultyData data = new EnemyDifficultyData();
            data.enemyID = input.ReadObject<CharacterIdentity>();
            data.difficultyLevel = input.ReadInt32();
            return data;
        }
    }

    public struct BattleTechniqueBuffData
    {
        public int buffTime;
        public BattleTechniqueBuffType buffType;
        public int buffStrength;
        public int duration;
    }
    public class BattleTechniqueBuffDataReader : ContentTypeReader<BattleTechniqueBuffData>
    {
        protected override BattleTechniqueBuffData Read(ContentReader input, BattleTechniqueBuffData existingInstance)
        {
            BattleTechniqueBuffData data = new BattleTechniqueBuffData();
            data.buffTime = input.ReadInt32();
            data.buffType = input.ReadObject<BattleTechniqueBuffType>();
            data.buffStrength = input.ReadInt32();
            data.duration = input.ReadInt32();
            return data;
        }
    }

    public struct BattleTechniqueSoundData
    {
        public int soundTime;
        public string soundFileName;
    }
    public class BattleTechniqueSoundDataReader : ContentTypeReader<BattleTechniqueSoundData>
    {
        protected override BattleTechniqueSoundData Read(ContentReader input, BattleTechniqueSoundData existingInstance)
        {
            BattleTechniqueSoundData data = new BattleTechniqueSoundData();
            data.soundTime = input.ReadInt32();
            data.soundFileName = input.ReadString();
            return data;
        }
    }
}
