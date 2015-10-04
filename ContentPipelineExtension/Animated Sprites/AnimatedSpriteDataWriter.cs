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
    public class AnimatedSpriteDataWriter : ContentTypeWriter<AnimatedSpriteData>
    {//XML writer class for data relating to animated sprites.
        protected override void Write(ContentWriter output, AnimatedSpriteData value)
        {
            output.Write(value.textureFileName);
            output.Write(value.totalFrames);
            output.Write((double)value.fps);//Value converted to double; XML has no float data type.
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AnimatedSpriteDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
