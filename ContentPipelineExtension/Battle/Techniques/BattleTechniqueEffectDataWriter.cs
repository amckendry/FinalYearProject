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
    public class BattleTechniqueEffectDataWriter : ContentTypeWriter<BattleTechniqueEffectData>
    {//XML Writer for data reltating to a sprite effect for a given battle technique.
        protected override void Write(ContentWriter output, BattleTechniqueEffectData value)
        {
            output.WriteObject<EffectType>(value.effectType);
            output.WriteObject<AnimatedSpriteData>(value.effectAnimation);
            output.Write(value.relativePosition);
            output.Write((double)value.speed);
            output.Write(value.startTime);
            output.Write(value.lifeTime);
            output.WriteObject<List<BattleTechniqueActionData>>(value.effectActions);
            output.WriteObject<List<BattleTechniqueBuffData>>(value.effectBuffs);
            output.WriteObject<List<BattleTechniqueSoundData>>(value.effectSounds);
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BattleTechniqueEffectDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
