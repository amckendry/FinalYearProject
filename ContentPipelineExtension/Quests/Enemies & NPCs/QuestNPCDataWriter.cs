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
    public class QuestNPCDataWriter : ContentTypeWriter<QuestNPCData>
    {//XML writer for data relating to a Non-Player Character map character generated from a given quest.
        protected override void Write(ContentWriter output, QuestNPCData value)
        {
            output.WriteObject<CharacterIdentity>(value.NPCID);
            output.WriteObject<MapStageSectionID>(value.stageSection);
            output.Write(value.position);
            output.WriteObject<List<QuestDialogData>>(value.dialog);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestNPCDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
