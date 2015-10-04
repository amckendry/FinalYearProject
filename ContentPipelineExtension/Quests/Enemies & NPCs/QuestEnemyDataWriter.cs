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
    public class QuestEnemyDataWriter : ContentTypeWriter<QuestEnemyData>
    {//XML writer for data relating to an enemy map character generated from a given quest.
        protected override void Write(ContentWriter output, QuestEnemyData value)
        {
            output.WriteObject<CharacterIdentity>(value.enemyType);
            output.WriteObject<MapStageSectionID>(value.stageSection);
            output.Write(value.position);
            output.WriteObject<EnemyAIType>(value.enemyAIType);
            output.WriteObject<List<QuestFlagChangeData>>(value.flagChangesOnDefeat);
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(QuestEnemyDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
