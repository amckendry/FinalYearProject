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
    public class SingleSpriteObjectDataWriter : ContentTypeWriter<SingleSpriteObjectData>
    {//XML writer class for data relating to a single sprite animating object.
        protected override void Write(ContentWriter output, SingleSpriteObjectData value)
        {
            //Some values converted to double here as XML has no float data type.
            output.Write(value.textureFileName);
            output.Write(value.position);
            output.Write(value.velocity);
            output.Write((double)MathHelper.ToDegrees(value.rotation)); //value of rotation is converted to Degrees to avoid accuracy issues with specifying angles in Radians.
            output.Write((double)value.scale);
            output.Write(value.tint);
            output.WriteObject<SpriteEffects>(value.spriteEffect);
            output.Write(value.totalFrames);
            output.Write((double)value.fps);
            output.WriteObject<SpriteAnimationType>(value.animationType);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(SingleSpriteObjectDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
