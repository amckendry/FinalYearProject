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
    public class BattleTechniqueDataWriter : ContentTypeWriter<BattleTechniqueData>
    {//XML Writer for data relating to a battle technique performed by a battle character.
        protected override void Write(ContentWriter output, BattleTechniqueData value)
        {
            output.Write(value.techniqueName);
            output.Write(value.techniqueDescription);
            output.WriteObject<BattleTechniqueRange>(value.techniqueRange);
            output.WriteObject<BattleTechniqueTargetType>(value.techniqueTargeting);
            output.Write(value.techniqueCost);
            output.WriteObject<AnimatedSpriteData>(value.characterAnimation);
            output.WriteObject<List<BattleTechniqueEffectData>>(value.effects);
            output.WriteObject<List<BattleTechniqueActionData>>(value.actions);
            output.WriteObject<List<BattleTechniqueBuffData>>(value.buffs);
            output.WriteObject<List<BattleTechniqueSoundData>>(value.sounds);
            output.WriteObject<List<BattleTechniqueAICategory>>(value.AICategories);
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BattleTechniqueDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}