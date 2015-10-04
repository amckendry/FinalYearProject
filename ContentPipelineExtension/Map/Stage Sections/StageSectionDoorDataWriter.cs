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
    public class StageSectionDoorDataWriter : ContentTypeWriter<StageSectionDoorData>
    {//XML writer for data relating to a door with in given map stage section.
        protected override void Write(ContentWriter output, StageSectionDoorData value)
        {
            output.Write(value.position);
            output.WriteObject<MapStageSectionID>(value.destinationSection);
            output.Write(value.destinationSectionPosition);
            output.WriteObject<DoorType>(value.doorType);
            output.Write(value.collisionBoundaryWidth);
            output.Write(value.collisionBoundaryHeight);
            output.WriteObject<AnimatedSpriteData>(value.closedUnlockedSpriteData);
            output.WriteObject<AnimatedSpriteData>(value.closedLockedSpriteData);
            output.WriteObject<AnimatedSpriteData>(value.openingSpriteData);
            output.WriteObject<QuestFlagData>(value.lockFlag);
            output.Write(value.openingSound);
            output.WriteObject<List<QuestFlagChangeData>>(value.flagChangesOnEnter);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(StageSectionDoorDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
