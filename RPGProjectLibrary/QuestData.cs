using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace RPGProjectLibrary
{
    public enum EnemyAIType
    {
        StandStill,
        PaceRight,
        PaceLeft,
        ChasePlayer,
        AvoidPlayer
    }
    public enum CharacterIdentity
    {
        Cross,
        Shelly,
        Guy,
        Turtle,
        Montagne,
        Beast,
        Pirate,
        Boy,
        Girl,
	    Thug,
        Security,
        King
    }
    public enum QuestFlagChangeType
    {
        Absolute,
        Increment
    }
    public enum InventoryChangeType
    {
        Add,
        Subtract
    }
    public enum MapItemType
    {
        FloatingPickup,
        TreasureChest
    }
    
    public struct QuestData
    {
        public string questName;
        public List<QuestFlagData> questFlags;
        public List<QuestRequirementData> questRequirements;
        public List<QuestItemData> items;
        public List<QuestEnemyData> enemies;
        public List<QuestNPCData> NPCs;
        public List<QuestDialogData> autoDialogs;
    }
    public class QuestDataReader : ContentTypeReader<QuestData>
    {
        protected override QuestData Read(ContentReader input, QuestData existingInstance)
        {
            QuestData data = new QuestData();
            data.questName = input.ReadString();
            data.questFlags = input.ReadObject<List<QuestFlagData>>();
            data.questRequirements = input.ReadObject<List<QuestRequirementData>>();
            data.items = input.ReadObject<List<QuestItemData>>();
            data.enemies = input.ReadObject<List<QuestEnemyData>>();
            data.NPCs = input.ReadObject<List<QuestNPCData>>();
            data.autoDialogs = input.ReadObject<List<QuestDialogData>>();
            return data;
        }
    }

    [Serializable]
    public struct QuestEnemyData
    {
        public CharacterIdentity enemyType;
        public MapStageSectionID stageSection;
        public Vector2 position;
        public EnemyAIType enemyAIType;
        public List<QuestFlagChangeData> flagChangesOnDefeat;
    }
    public class QuestEnemyDataReader : ContentTypeReader<QuestEnemyData>
    {
        protected override QuestEnemyData Read(ContentReader input, QuestEnemyData existingInstance)
        {
            QuestEnemyData data = new QuestEnemyData();
            data.enemyType = input.ReadObject<CharacterIdentity>();
            data.stageSection = input.ReadObject<MapStageSectionID>();
            data.position = input.ReadVector2();
            data.enemyAIType = input.ReadObject<EnemyAIType>();
            data.flagChangesOnDefeat = input.ReadObject<List<QuestFlagChangeData>>();
            return data;
        }
    }

    [Serializable]
    public struct QuestNPCData
    {
        public CharacterIdentity NPCID;
        public MapStageSectionID stageSection;
        public Vector2 position;
        public List<QuestDialogData> dialog;
    }
    public class QuestNPCDataReader : ContentTypeReader<QuestNPCData>
    {
        protected override QuestNPCData Read(ContentReader input, QuestNPCData existingInstance)
        {
            QuestNPCData data = new QuestNPCData();

            data.NPCID = input.ReadObject<CharacterIdentity>();
            data.stageSection = input.ReadObject<MapStageSectionID>();
            data.position = input.ReadVector2();
            data.dialog = input.ReadObject<List<QuestDialogData>>();

            return data;
        }
    }

    [Serializable]
    public struct QuestRequirementData
    {
        public string questRequirementName;
        public List<QuestFlagData> completionFlagValues;
    }
    public class QuestRequirementDataReader : ContentTypeReader<QuestRequirementData>
    {
        protected override QuestRequirementData Read(ContentReader input, QuestRequirementData existingInstance)
        {
            QuestRequirementData data = new QuestRequirementData();
            data.questRequirementName = input.ReadString();
            data.completionFlagValues = input.ReadObject<List<QuestFlagData>>();

            return data;
        }
    }

    [Serializable]
    public struct QuestFlagData
    {
        public string flagName;
        public int flagValue;
    }
    public class QuestFlagDataReader : ContentTypeReader<QuestFlagData>
    {
        protected override QuestFlagData Read(ContentReader input, QuestFlagData existingInstance)
        {
            QuestFlagData data = new QuestFlagData();

            data.flagName = input.ReadString();
            data.flagValue = input.ReadInt32();
            return data;
        }
    }

    [Serializable]
    public struct QuestFlagChangeData
    {
        public string flagNameToChange;
        public QuestFlagChangeType flagValueChangeType;
        public int changeValue;
    }
    public class QuestFlagChangeDataReader : ContentTypeReader<QuestFlagChangeData>
    {
        protected override QuestFlagChangeData Read(ContentReader input, QuestFlagChangeData existingInstance)
        {
            QuestFlagChangeData data = new QuestFlagChangeData();

            data.flagNameToChange = input.ReadString();
            data.flagValueChangeType = input.ReadObject<QuestFlagChangeType>();
            data.changeValue = input.ReadInt32();

            return data;
        }
    }

    [Serializable]
    public struct QuestDialogData
    {
       [ContentSerializer(Optional = true)]public List<QuestFlagData> flagsToTrigger;
       public List<QuestDialogSentenceData> sentences;
       [ContentSerializer(Optional = true)]public List<InventoryChangeData> inventoryChangesOnDialog;
       [ContentSerializer(Optional = true)]public List<QuestRequirementData> requirementsAdditionOnDialog;
    }
    public class QuestDialogDataReader : ContentTypeReader<QuestDialogData>
    {
        protected override QuestDialogData Read(ContentReader input, QuestDialogData existingInstance)
        {
            QuestDialogData data = new QuestDialogData();

            data.flagsToTrigger = input.ReadObject<List<QuestFlagData>>();
            data.sentences = input.ReadObject<List<QuestDialogSentenceData>>();
            data.inventoryChangesOnDialog = input.ReadObject<List<InventoryChangeData>>();
            data.requirementsAdditionOnDialog = input.ReadObject<List<QuestRequirementData>>();
            return data;
        }
    }

    [Serializable]
    public struct QuestDialogSentenceData
    {
        public int sentenceID;
        public CharacterIdentity characterTalking;
        public string sentenceText;
        public List<QuestDialogOptionData> responseOptions;
    }
    public class QuestDialogSentenceDataReader : ContentTypeReader<QuestDialogSentenceData>
    {
        protected override QuestDialogSentenceData Read(ContentReader input, QuestDialogSentenceData existingInstance)
        {
            QuestDialogSentenceData data = new QuestDialogSentenceData();

            data.sentenceID = input.ReadInt32();
            data.characterTalking = input.ReadObject<CharacterIdentity>();
            data.sentenceText = input.ReadString();
            data.responseOptions = input.ReadObject<List<QuestDialogOptionData>>();

            return data;
        }
    }

    [Serializable]
    public struct QuestDialogOptionData
    {
       public string optionName;
       [ContentSerializer(Optional = true)]public List<QuestFlagChangeData> flagChangesOnResponse;
       public int nextSentence;

    }
    public class QuestDialogOptionDataReader : ContentTypeReader<QuestDialogOptionData>
    {
        protected override QuestDialogOptionData Read(ContentReader input, QuestDialogOptionData existingInstance)
        {
            QuestDialogOptionData data = new QuestDialogOptionData();

            data.optionName = input.ReadString();
            data.flagChangesOnResponse = input.ReadObject<List<QuestFlagChangeData>>();
            data.nextSentence = input.ReadInt32();

            return data;
        }
    }

    [Serializable]
    public struct InventoryChangeData
    {
        public InventoryChangeType inventoryChangeType;
        public InventoryItemIdentity itemIDToChange;
        public int QTYToChangeBy;
    }
    public class InventoryChangeDataReader : ContentTypeReader<InventoryChangeData>
    {
        protected override InventoryChangeData Read(ContentReader input, InventoryChangeData existingInstance)
        {
            InventoryChangeData data = new InventoryChangeData();
            data.inventoryChangeType = input.ReadObject<InventoryChangeType>();
            data.itemIDToChange = input.ReadObject<InventoryItemIdentity>();
            data.QTYToChangeBy = input.ReadInt32();
            return data;
        }
    }

    [Serializable]
    public struct QuestItemData
    {
        public MapStageSectionID stageSection;
        public Vector2 position;
        public AnimatedSpriteData notCollectedSpriteData;
        public AnimatedSpriteData collectedSpriteData;
        public CollisionBoxData collisionBoxData;
        public string soundOnCollection;
        public List<QuestFlagChangeData> flagChangesOnCollection;
        public List<InventoryChangeData> inventoryChangesOnCollection;
    }
    public class QuestItemDataReader : ContentTypeReader<QuestItemData>
    {
        protected override QuestItemData Read(ContentReader input, QuestItemData existingInstance)
        {
            QuestItemData data = new QuestItemData();

            data.stageSection = input.ReadObject<MapStageSectionID>();
            data.position = input.ReadVector2();
            data.notCollectedSpriteData = input.ReadObject<AnimatedSpriteData>();
            data.collectedSpriteData = input.ReadObject<AnimatedSpriteData>();
            data.collisionBoxData = input.ReadObject<CollisionBoxData>();
            data.soundOnCollection = input.ReadString();
            data.flagChangesOnCollection = input.ReadObject<List<QuestFlagChangeData>>();
            data.inventoryChangesOnCollection = input.ReadObject<List<InventoryChangeData>>();

            return data;
        }
    }
}
