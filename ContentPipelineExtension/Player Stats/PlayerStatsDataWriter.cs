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
    public class PlayerStatsDataWriter : ContentTypeWriter<PlayerStatsData>
    {//XML writer for data relating to initial and growth statistics for a players Party Character.
        protected override void Write(ContentWriter output, PlayerStatsData value)
        {
            output.WriteObject<CharacterIdentity>(value.characterID);
            output.Write(value.portraitTextureFileName);
            output.Write(value.initialLevel);
            output.Write(value.initialHP);
            output.Write(value.initialMP);
            output.Write(value.initialATK);
            output.Write(value.initialDEF);
            output.Write(value.initialMAG_ATK);
            output.Write(value.initialMAG_DEF);
            output.Write(value.initialSPD);
            output.Write(value.HPGrowth);
            output.Write(value.MPGrowth);
            output.Write(value.ATKGrowth);
            output.Write(value.DEFGrowth);
            output.Write(value.MAG_ATKGrowth);
            output.Write(value.MAG_DEFGrowth);
            output.Write(value.SPDGrowth);
            output.WriteObject<List<BattleTechniqueLevelData>>(value.techniqueLevelData);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(PlayerStatsDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
