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
    public class StageSectionDataWriter : ContentTypeWriter<StageSectionData>
    {//XML writer for data relating to a map stage section.
        protected override void Write(ContentWriter output, StageSectionData value)
        {
            output.WriteObject<MapStageID>(value.mapStageID);
            output.Write(value.sectionSize);
            output.Write(value.backgroundMusicFileName);
            output.WriteObject<MapStageSectionID>(value.currentSectionID);
            output.WriteObject<MapStageSectionID>(value.nextSectionID);
            output.WriteObject<MapStageSectionID>(value.previousSectionID);
            output.WriteObject<List<StageSectionObstacleData>>(value.obstacles);
            output.WriteObject<List<SingleSpriteObjectData>>(value.backgroundObjects);
            output.WriteObject<List<SingleSpriteObjectData>>(value.foregroundObjects);
            output.WriteObject<List<StageSectionDoorData>>(value.doors);
            output.Write(value.mapBGTextureName);
            output.Write(value.battleBGTextureName);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(StageSectionDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
