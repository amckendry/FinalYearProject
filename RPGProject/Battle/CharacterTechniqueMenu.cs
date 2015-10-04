using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RPGProject
{
    class CharacterTechniqueMenu : Menu
    {
        List<BattleTechnique> battleTechniques;

        public CharacterTechniqueMenu(Vector2 position, string cursorTextureFile, string BGTextureFile, Vector2 cursorRelativePosition)
            : base("Character Technique Menu", position, 18, BGTextureFile, cursorTextureFile, cursorRelativePosition, new Vector2(-95, 8), true)
        {
            battleTechniques = new List<BattleTechnique>();
        }

        public void LoadTechniques(BattleCharacter character)
        {
            battleTechniques = new List<BattleTechnique>();
            menuOptions = new List<MenuOption>();
            currentMenuOption = null;
            defaultMenuOption = null;
            foreach (BattleTechnique battleTechnique in character.Techniques)
            {
                if (battleTechnique.techniqueName != "Attack" && battleTechnique.techniqueName != "Defend")
                {
                    MenuOption option = new MenuOption(battleTechnique.techniqueName, battleTechnique.techniqueDescription, Color.Yellow, Color.White, "blank", "blank");
                    AddMenuOption(option, (menuOptions.Count == 0));
                    battleTechniques.Add(battleTechnique);
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            for (int count = 0; count < battleTechniques.Count; count++)
            {
                int drawPosY = (int)(Position.Y + 5 - (Texture.Height / 2) + (count * (menuOptionSpacing)));

                Vector2 optionDrawPosition = new Vector2(Position.X + menuItemsDrawOffset.X, drawPosY + +menuItemsDrawOffset.Y);
                if (menuOptions[count].MenuOptionState == MenuOptionState.Selected)
                {
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + battleTechniques[count].techniqueCost, new Vector2(optionDrawPosition.X + 140, optionDrawPosition.Y + 5), new Color(menuOptions[count].SelectedNameColour.R, menuOptions[count].SelectedNameColour.G, menuOptions[count].SelectedNameColour.B, Tint.A));
                    
                }
                else if (menuOptions[count].MenuOptionState == MenuOptionState.NotSelected)
                {
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + battleTechniques[count].techniqueCost, new Vector2(optionDrawPosition.X + 140, optionDrawPosition.Y + 5), new Color(menuOptions[count].NotSelectedNameColour.R, menuOptions[count].NotSelectedNameColour.G, menuOptions[count].NotSelectedNameColour.B, Tint.A));
                }
            }
        }
    }
}
