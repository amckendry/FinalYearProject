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
    public class PlayerBattleCharacterDataWriter : ContentTypeWriter<PlayerBattleCharacterData>
    {//XMl Writer for data relating to a battle character with is a player character.
        protected override void Write(ContentWriter output, PlayerBattleCharacterData value)
        {
            output.Write(value.name);
            output.Write(value.battlePortraitFileName);
            output.Write(value.drawOffset);
            output.Write((double)value.runSpeed);
            output.WriteObject<AnimatedSpriteData>(value.battleStartAnimation);
            output.WriteObject<AnimatedSpriteData>(value.idleAnimation);
            output.WriteObject<AnimatedSpriteData>(value.runAnimation);
            output.WriteObject<AnimatedSpriteData>(value.damageAnimation);
            output.WriteObject<AnimatedSpriteData>(value.deathAnimation);
            output.WriteObject<AnimatedSpriteData>(value.revivedAnimation);
            output.WriteObject<AnimatedSpriteData>(value.victoryAnimation);
            output.WriteObject<AnimatedSpriteData>(value.itemAnimation);
            output.WriteObject<List<BattleTechniqueData>>(value.techniques);
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(PlayerBattleCharacterDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
