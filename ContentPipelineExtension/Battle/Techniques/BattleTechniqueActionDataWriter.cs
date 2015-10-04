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
    public class BattleTechniqueActionDataWriter : ContentTypeWriter<BattleTechniqueActionData>
    {//XML writer for data relating to a damage action fora given battle technique.
        protected override void Write(ContentWriter output, BattleTechniqueActionData value)
        {
            output.Write(value.actionTime);
            output.WriteObject<BattleTechniqueActionType>(value.actionType);
            output.WriteObject<ActionStatType>(value.actionStatType);
            output.Write(value.strength);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BattleTechniqueActionDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
