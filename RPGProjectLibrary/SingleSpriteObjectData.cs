using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RPGProjectLibrary
{
    public enum SpriteAnimationType
    {
        Stopped,
        AnimateOnceMaintainFirst,
        AnimateOnceMaintainLast,
        Repeating,
        AlternatingReverse,
        RepeatingReverse
    }

    [Serializable]
    public struct SingleSpriteObjectData
    {
        public String textureFileName;
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float scale;
        public Color tint;
        public SpriteEffects spriteEffect;
        public int totalFrames;
        public float fps;
        public SpriteAnimationType animationType;
    }
    public class SingleSpriteObjectDataReader : ContentTypeReader<SingleSpriteObjectData>
    {
        protected override SingleSpriteObjectData Read(ContentReader input, SingleSpriteObjectData existingInstance)
        {
             SingleSpriteObjectData data = new SingleSpriteObjectData();

            data.textureFileName = input.ReadString();
            data.position = input.ReadVector2();
            data.velocity = input.ReadVector2();
            data.rotation = MathHelper.ToRadians((float)input.ReadDouble());
            data.scale = (float)input.ReadDouble();
            data.tint = input.ReadColor();
            data.spriteEffect = input.ReadObject<SpriteEffects>();
            data.totalFrames = input.ReadInt32();
            data.fps = (float)input.ReadDouble();
            data.animationType = input.ReadObject<SpriteAnimationType>();

            return data;
        }
    }
}
