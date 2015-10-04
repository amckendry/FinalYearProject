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
    public class BattleItemDataWriter : ContentTypeWriter<BattleItemData>
    {//XML writer for data relating to an inventory items effect in battle.
        protected override void Write(ContentWriter output, BattleItemData value)
        {
            output.WriteObject<InventoryItemIdentity>(value.itemID);
            output.Write(value.itemDescription);
            output.WriteObject<BattleTechniqueTargetType>(value.itemBattleEffectTargeting);
            output.WriteObject<BattleTechniqueEffectData>(value.itemBattleEffect);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BattleItemDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
