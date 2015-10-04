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
    public class CollisionBoxDataWriter : ContentTypeWriter<CollisionBoxData>
    {//XML writer for data relating to a collision box.
        protected override void Write(ContentWriter output, CollisionBoxData value)
        {
            output.Write(value.height);
            output.Write(value.width);
            output.Write(value.offset);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(CollisionBoxDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
