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
    public class QuestDialogOptionDataWriter : ContentTypeWriter<QuestDialogOptionData>
    {//XML writer for data relating to response options for a given dialog sentence.
        protected override void Write(ContentWriter output, QuestDialogOptionData value)
        {
            output.Write(value.optionName);
            output.WriteObject<List<QuestFlagChangeData>>(value.flagChangesOnResponse);
            output.Write(value.nextSentence);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestDialogOptionDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
