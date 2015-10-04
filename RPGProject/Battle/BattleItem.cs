using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RPGProjectLibrary;

namespace RPGProject
{
    public class BattleItem
    {//A class to represent an item with a use in battle. Maps directly from RPGProjectLibrary.BattleItemData and adds QTY attribute.

        #region Fields
        InventoryItemIdentity itemID;
        public InventoryItemIdentity ItemID
        {
            get
            {
                return itemID;
            }
        }

        string itemDescription;
        public string ItemDescription
        {
            get
            {
                return itemDescription;
            }
        }

        //Represents how much of the battle item the player has to use.
        int qty;
        public int QTY
        {
            get
            {
                return qty;
            }
            set
            {
                qty = value;
            }
        }

        //What targets the item can be used on.
        BattleTechniqueTargetType battleEffectTargeting;
        public BattleTechniqueTargetType BattleEffectTargeting
        {
            get
            {
                return battleEffectTargeting;
            }
        }

        //The sprite effect for using the item.
        BattleTechniqueEffect battleEffect;
        public BattleTechniqueEffect BattleEffect
        {
            get
            {
                return battleEffect;
            }
        }
        #endregion
        #region Methods
        public void LoadContent(BattleItemData itemData, int QTY)
        {//Loads from a BattleItemData object.
            itemID = itemData.itemID;
            itemDescription = itemData.itemDescription;
            qty = QTY;
            battleEffectTargeting = itemData.itemBattleEffectTargeting;
            battleEffect = new BattleTechniqueEffect();
            battleEffect.LoadContent(itemData.itemBattleEffect);
        }
        #endregion
        #region Contructors
        public BattleItem()
        {
        }
        #endregion
    }
}
