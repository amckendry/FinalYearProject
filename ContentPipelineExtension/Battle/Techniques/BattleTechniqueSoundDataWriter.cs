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
    public class BattleTechniqueSoundDataWriter : ContentTypeWriter<BattleTechniqueSoundData>
    {//XML Writer for data relating to sounds for a given battle technique.
        protected override void Write(ContentWriter output, BattleTechniqueSoundData value)
        {
            output.Write(value.soundTime);
            output.Write(value.soundFileName);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BattleTechniqueSoundDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
