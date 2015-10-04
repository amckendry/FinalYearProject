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
    public class QuestDataWriter : ContentTypeWriter<QuestData>
    {//XML writer for data relating to a quest the player can be on.
        protected override void Write(ContentWriter output, QuestData value)
        {
            output.Write(value.questName);
            output.WriteObject<List<QuestFlagData>>(value.questFlags);
            output.WriteObject<List<QuestRequirementData>>(value.questRequirements);
            output.WriteObject<List<QuestItemData>>(value.items);
            output.WriteObject<List<QuestEnemyData>>(value.enemies);
            output.WriteObject<List<QuestNPCData>>(value.NPCs);
            output.WriteObject<List<QuestDialogData>>(value.autoDialogs);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
