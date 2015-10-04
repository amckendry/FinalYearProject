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
    public class EnemyBattleCharacterDataWriter : ContentTypeWriter<EnemyBattleCharacterData>
    {//XML writer class for enemy battle characters.
        protected override void Write(ContentWriter output, EnemyBattleCharacterData value)
        {
            output.Write(value.name);
            output.Write(value.drawOffset);
            output.Write((double)value.runSpeed); //Value converted to double as XML has no float data type.
            output.Write(value.HP);
            output.Write(value.MP);
            output.Write(value.ATK);
            output.Write(value.DEF);
            output.Write(value.MAG_ATK);
            output.Write(value.MAG_DEF);
            output.Write(value.SPD);
            output.Write(value.EXPWorth);
            output.WriteObject<AnimatedSpriteData>(value.idleAnimation);
            output.WriteObject<AnimatedSpriteData>(value.runAnimation);
            output.WriteObject<AnimatedSpriteData>(value.damageAnimation);
            output.WriteObject<AnimatedSpriteData>(value.deathAnimation);
            output.WriteObject<List<BattleTechniqueData>>(value.techniques);
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(EnemyBattleCharacterDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
