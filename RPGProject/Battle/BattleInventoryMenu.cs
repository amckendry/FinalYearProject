using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGProject
{
    public class BattleInventoryMenu : Menu
    {//This is an extension of the Menu class to display the players inventory during battle and return their selected item.

        #region Fields
        //The list of items to display. Populated in LoadContent();
        List<BattleItem> battleItemInventory;
        #endregion
        #region Methods
        public void LoadBattleItems(List<BattleItem> battleItemInventory)
        {//Generates Menu Options from the passed inventory list, and sets the first option as the default.
            this.battleItemInventory = battleItemInventory;
            menuOptions = new List<MenuOption>();
            currentMenuOption = null;
            defaultMenuOption = null;
            foreach (BattleItem battleItem in battleItemInventory)
            {
                if (battleItem.QTY > 0)
                {
                    MenuOption option = new MenuOption(battleItem.ItemID.ToString(), battleItem.ItemDescription, Color.Yellow, Color.White, "blank", "blank");
                    AddMenuOption(option, (menuOptions.Count == 0));
                }
            }
        }
        public override void Draw()
        {
            //The base Draw will display the items names.
            base.Draw();
            //In addition to the standard Menu Draw, this will draw the QTY of the items in an extra column.
            for (int count = 0; count < battleItemInventory.Count; count++)
            {
                int drawPosY = (int)(Position.Y + 5 - (Texture.Height / 2) + (count * (menuOptionSpacing)));

                Vector2 optionDrawPosition = new Vector2(Position.X + menuItemsDrawOffset.X, drawPosY + +menuItemsDrawOffset.Y);
                if (menuOptions[count].MenuOptionState == MenuOptionState.Selected)
                {
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + battleItemInventory[count].QTY, new Vector2(optionDrawPosition.X + 124, optionDrawPosition.Y + 5), new Color(menuOptions[count].SelectedNameColour.R, menuOptions[count].SelectedNameColour.G, menuOptions[count].SelectedNameColour.B, Tint.A));
                }
                else if (menuOptions[count].MenuOptionState == MenuOptionState.NotSelected)
                {

                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + battleItemInventory[count].QTY, new Vector2(optionDrawPosition.X + 124, optionDrawPosition.Y + 5), new Color(menuOptions[count].NotSelectedNameColour.R, menuOptions[count].NotSelectedNameColour.G, menuOptions[count].NotSelectedNameColour.B, Tint.A));
                }
            }
        }
        #endregion
        #region Contructors
        public BattleInventoryMenu(Vector2 position, string cursorTextureFile, string BGTextureFile, Vector2 cursorRelativePosition)
            : base("Battle Inventory Menu", position, 18, BGTextureFile, cursorTextureFile, cursorRelativePosition, new Vector2(-88, 8), true)
        {
            battleItemInventory = new List<BattleItem>();
        }
        #endregion
    }
}
