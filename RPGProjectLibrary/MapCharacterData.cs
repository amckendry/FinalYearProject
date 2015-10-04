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
        Land,
        Death
    }
    public enum CharacterFacing
    {
        Left,
        Right
    }

    [Serializable]
    public struct MapCharacterData
    {
        public CharacterAnimatedSpriteData idleSpriteData;
        public CharacterAnimatedSpriteData runSpriteData;
        public CharacterAnimatedSpriteData jumpSpriteData;
        public CharacterAnimatedSpriteData fallSpriteData;
        public CharacterAnimatedSpriteData landSpriteData;
        public CharacterAnimatedSpriteData deathSpriteData;
        public string portraitTextureName;
        public float walkSpeed;
        public float airInfluence;
        public float frictionFactor;
        public float jumpStrength;
        public float mass;
    }
    public class MapCharacterDataReader : ContentTypeReader<MapCharacterData>
    {
        protected override MapCharacterData Read(ContentReader input, MapCharacterData existingInstance)
        {
            MapCharacterData data = new MapCharacterData();

            data.idleSpriteData = input.ReadObject<CharacterAnimatedSpriteData>();
            data.runSpriteData = input.ReadObject<CharacterAnimatedSpriteData>();
            data.jumpSpriteData = input.ReadObject<CharacterAnimatedSpriteData>();
            data.fallSpriteData = input.ReadObject<CharacterAnimatedSpriteData>();
            data.landSpriteData = input.ReadObject<CharacterAnimatedSpriteData>();
            data.deathSpriteData = input.ReadObject<CharacterAnimatedSpriteData>();
            data.portraitTextureName = input.ReadString();
            data.walkSpeed = (float)input.ReadDouble();
            data.airInfluence = (float)input.ReadDouble();
            data.frictionFactor = (float)input.ReadDouble();
            data.jumpStrength = (float)input.ReadDouble();
            data.mass = (float)input.ReadDouble();

            return data;
        }
    }


}
