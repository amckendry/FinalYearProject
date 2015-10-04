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
    public class QuestFlagDataWriter : ContentTypeWriter<QuestFlagData>
    {//XML writer for data relating to a quest flag for a given quest.
        protected override void Write(ContentWriter output, QuestFlagData flagData)
        {
            output.Write(flagData.flagName);
            output.Write(flagData.flagValue);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestFlagDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
