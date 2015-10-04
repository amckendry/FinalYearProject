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
    public class BattleTechniqueLevelDataWriter : ContentTypeWriter<BattleTechniqueLevelData>
    {//XML writer for data relating to a level requirement for a given battle technique.
        protected override void Write(ContentWriter output, BattleTechniqueLevelData value)
        {
            output.Write(value.techniqueName);
            output.Write(value.levelLearnt);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BattleTechniqueLevelDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
