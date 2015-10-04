using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace RPGProjectLibrary
{
    [Serializable]
    public struct CharacterAnimatedSpriteData
    {
        public string textureFileName;
        public int totalFrames;
        public float fps;
        public CollisionBoxData collisionBoxData;

    }
    public class CharacterAnimatedSpriteDataReader : ContentTypeReader<CharacterAnimatedSpriteData>
    {
        protected override CharacterAnimatedSpriteData Read(ContentReader input, CharacterAnimatedSpriteData existingInstance)
        {
            CharacterAnimatedSpriteData data = new CharacterAnimatedSpriteData();

            data.textureFileName = input.ReadString();
            data.totalFrames = input.ReadInt32();
            data.fps = (float)input.ReadDouble();
            data.collisionBoxData = input.ReadObject<CollisionBoxData>();
            return data;
        }
    }

    [Serializable]
    public struct AnimatedSpriteData
    {
        public string textureFileName;
        public int totalFrames;
        public float fps;
    }
    public class AnimatedSpriteDataReader : ContentTypeReader<AnimatedSpriteData>
    {
        protected override AnimatedSpriteData Read(ContentReader input, AnimatedSpriteData existingInstance)
        {
            AnimatedSpriteData data = new AnimatedSpriteData();

            data.textureFileName = input.ReadString();
            data.totalFrames = input.ReadInt32();
            data.fps = (float)input.ReadDouble();
            return data;
        }
    }
}
