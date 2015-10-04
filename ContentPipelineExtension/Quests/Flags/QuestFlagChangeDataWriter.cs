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
    public class QuestFlagChangeDataWriter : ContentTypeWriter<QuestFlagChangeData>
    {//XML writer for data relating to a change in a quest flags value triggered by another given object.
        protected override void Write(ContentWriter output, QuestFlagChangeData value)
        {
            output.Write(value.flagNameToChange);
            output.WriteObject<QuestFlagChangeType>(value.flagValueChangeType);
            output.Write(value.changeValue);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestFlagChangeDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
