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
    public class BattleTechniqueBuffDataWriter : ContentTypeWriter<BattleTechniqueBuffData>
    {//XML Writer for data relating to a stats buffing action for a given technique.
        protected override void Write(ContentWriter output, BattleTechniqueBuffData value)
        {
            output.Write(value.buffTime);
            output.WriteObject<BattleTechniqueBuffType>(value.buffType);
            output.Write(value.buffStrength);
            output.Write(value.duration);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BattleTechniqueBuffDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
