using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace RPGProjectLibrary
{
    public enum MapCharacterAction
    {
        Idle,
        Run,
        Jump,
        Fall,
        Land
    }
    public enum CharacterFacing
    {
        Left,
        Right
    }

    public enum PlayerIdentity
    {
        Cross
    }
    public enum EnemyIdentity
    {
        Turtle
    }
    public enum NPCIdentity
    {
        Fred
    }

    [Serializable]
    public struct PlayerCharacterObjectData
    {
        public AnimatedSpriteData idleSpriteData;
        public AnimatedSpriteData runSpriteData;
        public AnimatedSpriteData jumpSpriteData;
        public AnimatedSpriteData fallSpriteData;
        public AnimatedSpriteData landSpriteData;
    }
    public class PlayerCharacterObjectDataReader : ContentTypeReader<PlayerCharacterObjectData>
    {
        protected override PlayerCharacterObjectData Read(ContentReader input, PlayerCharacterObjectData existingInstance)
        {
            PlayerCharacterObjectData data = new PlayerCharacterObjectData();

            data.idleSpriteData = input.ReadObject<AnimatedSpriteData>();
            data.runSpriteData = input.ReadObject<AnimatedSpriteData>();
            data.jumpSpriteData = input.ReadObject<AnimatedSpriteData>();
            data.fallSpriteData = input.ReadObject<AnimatedSpriteData>();
            data.landSpriteData = input.ReadObject<AnimatedSpriteData>();
            return data;
        }
    }

    [Serializable]
    public struct EnemyCharacterObjectData
    {
        public EnemyIdentity enemyID;
        public Vector2 position;
        public CharacterFacing facing;
        public MapCharacterAction currentAction;
    }
    public class EnemyCharacterObjectDataReader : ContentTypeReader<EnemyCharacterObjectData>
    {
        protected override EnemyCharacterObjectData Read(ContentReader input, EnemyCharacterObjectData existingInstance)
        {
            EnemyCharacterObjectData data = new EnemyCharacterObjectData();

            data.enemyID = input.ReadObject<EnemyIdentity>();
            data.position = input.ReadVector2();
            data.facing = input.ReadObject<CharacterFacing>();
            data.currentAction = input.ReadObject<MapCharacterAction>();

            return data;
        }
    }

    [Serializable]
    public struct NPCCharacterObjectData
    {
        public NPCIdentity NPCID;
        public Vector2 position;
        public CharacterFacing facing;
        public MapCharacterAction currentAction;
    }
    public class NPCCharacterObjectDataReader : ContentTypeReader<NPCCharacterObjectData>
    {
        protected override NPCCharacterObjectData Read(ContentReader input, NPCCharacterObjectData existingInstance)
        {
            NPCCharacterObjectData data = new NPCCharacterObjectData();

            data.NPCID = input.ReadObject<NPCIdentity>();
            data.position = input.ReadVector2();
            data.facing = input.ReadObject<CharacterFacing>();
            data.currentAction = input.ReadObject<MapCharacterAction>();

            return data;
        }
    }


}
