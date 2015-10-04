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
    public class EnemyDifficultyDataWriter : ContentTypeWriter<EnemyDifficultyData>
    {//XML Writer class for the data relating an enemy ID with a given difficulty level.
        protected override void Write(ContentWriter output, EnemyDifficultyData value)
        {
            output.WriteObject<CharacterIdentity>(value.enemyID);
            output.Write(value.difficultyLevel);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(EnemyDifficultyDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
