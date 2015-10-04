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
    public class QuestDialogSentenceDataWriter : ContentTypeWriter<QuestDialogSentenceData>
    {//XML writer for data relating to a sentence in a given dialog.
        protected override void Write(ContentWriter output, QuestDialogSentenceData value)
        {
            output.Write(value.sentenceID);
            output.WriteObject<CharacterIdentity>(value.characterTalking);
            output.Write(value.sentenceText);
            output.WriteObject<List<QuestDialogOptionData>>(value.responseOptions);

        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestDialogSentenceDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
