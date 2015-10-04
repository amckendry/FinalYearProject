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
    public class QuestDialogDataWriter : ContentTypeWriter<QuestDialogData>
    {//XML writer for data relating to dialog for a given Non-Player Character.
        protected override void Write(ContentWriter output, QuestDialogData value)
        {
            output.WriteObject<List<QuestFlagData>>(value.flagsToTrigger);
            output.WriteObject<List<QuestDialogSentenceData>>(value.sentences);
            output.WriteObject<List<InventoryChangeData>>(value.inventoryChangesOnDialog);
            output.WriteObject<List<QuestRequirementData>>(value.requirementsAdditionOnDialog);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestDialogDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
