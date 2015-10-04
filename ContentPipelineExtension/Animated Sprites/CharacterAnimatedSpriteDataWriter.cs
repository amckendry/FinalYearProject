using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using RPGProjectLibrary;

namespace ContentPipelineExtension
{
    [ContentTypeWriter]
    public class CharacterAnimatedSpriteDataWriter : ContentTypeWriter<CharacterAnimatedSpriteData>
    {//XML writer class for data relating to animated sprites with collision data mapped to them.
        protected override void Write(ContentWriter output, CharacterAnimatedSpriteData value)
        {
            output.Write(value.textureFileName);
            output.Write(value.totalFrames);
            output.Write((double)value.fps);//Value converted to double; XML has no float data type.
            output.WriteObject<CollisionBoxData>(value.collisionBoxData);//CollisionBoxData is also a custom type with it's own writer and reader. WriteObject calls the appropriate writer for it.
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(CharacterAnimatedSpriteDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}