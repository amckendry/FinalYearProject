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
    public class StageSectionObstacleDataWriter : ContentTypeWriter<StageSectionObstacleData>
    {//XML writer for data relating to an obstacle in a given map stage section.
        protected override void Write(ContentWriter output, StageSectionObstacleData value)
        {
            output.WriteObject<ObstacleType>(value.obstacleType);
            output.Write(value.textureName);
            output.Write(value.collisionOffset);
            output.Write(value.position);
            output.Write(value.velocity);
            output.Write(value.behindScenery);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(StageSectionObstacleDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
