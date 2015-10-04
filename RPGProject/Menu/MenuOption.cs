using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGProject
{
    public enum MenuOptionState
    {
        Selected,
        NotSelected
    }
    public class MenuOption
    {// This object creates an instance of a menu item within a menu.
        protected string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        protected string description;
        public string Description
        {
            get
            {
                return description;
            }
        }

        Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        Texture2D selectedTexture;
        public Texture2D SelectedTexture
        {
            get
            {
                return selectedTexture;
            }
        }
        Color selectedNameColour;
        public Color SelectedNameColour
        {
            get
            {
                return selectedNameColour;
            }
        }
        Texture2D notSelectedTexture;
        public Texture2D NotSelectedTexture
        {
            get
            {
                return notSelectedTexture;
            }
        }
        Color notSelectedNameColour;
        public Color NotSelectedNameColour
        {
            get
            {
                return notSelectedNameColour;
            }
        }
        MenuOptionState menuOptionState;
        public MenuOptionState MenuOptionState
        {
            get
            {
                return menuOptionState;
            }
            set
            {
                menuOptionState = value;
            }
        }


        public MenuOption(string name, string description, Color selectedNameColour, Color notSelectedNameColour, string selectedTextureFileName, string notSelectedTextureFileName)
        {
            this.name = name;
            this.description = description;
            selectedTexture = GameClass.LoadTextureData(selectedTextureFileName);
            notSelectedTexture = GameClass.LoadTextureData(notSelectedTextureFileName);
            this.selectedNameColour = selectedNameColour;
            this.notSelectedNameColour = notSelectedNameColour;
            menuOptionState = MenuOptionState.NotSelected;
        }
        public MenuOption(string name, Texture2D selectedTexture, Texture2D notSelectedTexture)
        {
            this.name = name;
            this.description = "";
            this.selectedTexture = selectedTexture;
            this.notSelectedTexture = notSelectedTexture;
            this.selectedNameColour = Color.White;
            this.notSelectedNameColour = Color.White;
            menuOptionState = MenuOptionState.NotSelected;
        }
        public string ReturnValue()
        {
            return name;
        }
    }
}
