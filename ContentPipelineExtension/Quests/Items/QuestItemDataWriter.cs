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
    public class QuestItemDataWriter : ContentTypeWriter<QuestItemData>
    {//XML writer for data relating to a collectable map item spawned from a given quest.
        protected override void Write(ContentWriter output, QuestItemData value)
        {
            output.WriteObject<MapStageSectionID>(value.stageSection);
            output.Write(value.position);
            output.WriteObject<AnimatedSpriteData>(value.notCollectedSpriteData);
            output.WriteObject<AnimatedSpriteData>(value.collectedSpriteData);
            output.WriteObject<CollisionBoxData>(value.collisionBoxData);
            output.Write(value.soundOnCollection);
            output.WriteObject<List<QuestFlagChangeData>>(value.flagChangesOnCollection);
            output.WriteObject<List<InventoryChangeData>>(value.inventoryChangesOnCollection);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestItemDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
