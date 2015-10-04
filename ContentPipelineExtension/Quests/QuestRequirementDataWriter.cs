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
    public class QuestRequirementDataWriter : ContentTypeWriter<QuestRequirementData>
    {//XML writer for data relating to a completion requirement for a given quest.
        protected override void Write(ContentWriter output, QuestRequirementData value)
        {
            output.Write(value.questRequirementName);
            output.WriteObject<List<QuestFlagData>>(value.completionFlagValues);
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestRequirementDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
