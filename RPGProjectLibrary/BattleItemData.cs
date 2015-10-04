using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace RPGProjectLibrary
{
    public enum InventoryItemIdentity
    {
        Potion,
        RedCrystal
    }

    public struct BattleItemData
    {
        public InventoryItemIdentity itemID;
        public string itemDescription;
        public BattleTechniqueTargetType itemBattleEffectTargeting;
        public BattleTechniqueEffectData itemBattleEffect;
    }
    public class BattleItemDataReader : ContentTypeReader<BattleItemData>
    {
        protected override BattleItemData Read(ContentReader input, BattleItemData existingInstance)
        {
            BattleItemData data = new BattleItemData();
            data.itemID = input.ReadObject<InventoryItemIdentity>();
            data.itemDescription = input.ReadString();
            data.itemBattleEffectTargeting = input.ReadObject<BattleTechniqueTargetType>();
            data.itemBattleEffect = input.ReadObject<BattleTechniqueEffectData>();
            return data;
        }
    }
}
