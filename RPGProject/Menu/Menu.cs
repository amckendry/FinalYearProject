using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPGProject
{
    public enum MenuState
    {
        FadingIn,
        IdleAwaitingInput,
        IdleValueSelected,
        FadingOut,
        FadingOutHalf,
        IdleHalfFaded,
        IdleFaded
    }
  public class Menu : TexturedGameObject
  {// This object creates an instance of a menu within the game.
      protected MenuState menuState;
      public MenuState MenuState
      {
          get
          {
              return menuState;
          }
      }
      protected float alpha;

      protected static float fadeSpeed = 0.1f;
      protected Texture2D cursorTexture;
      protected Vector2 cursorRelativePosition;
      protected List<MenuOption> menuOptions;
      protected MenuOption defaultMenuOption;
      protected MenuOption currentMenuOption;
      protected int menuOptionSpacing;
      protected GamePadController controls;
      protected KeyboardController debugControls;
      protected Vector2 menuItemsDrawOffset = Vector2.Zero;
      protected bool displayItemNames;

      public Menu(string name, Vector2 position, int menuOptionSpacing, string BGTextureFile, string cursorTextureFile, Vector2 cursorRelativePosition, Vector2 menuItemsDrawOffset, bool displayItemNames)
          : base(BGTextureFile, position, Vector2.Zero)
      {
          cursorTexture = GameClass.LoadTextureData(cursorTextureFile);
          this.menuOptionSpacing = menuOptionSpacing;
          this.cursorRelativePosition = cursorRelativePosition;
          menuOptions = new List<MenuOption>();
          alpha = 0.0f;
          menuState = MenuState.IdleFaded;
          controls = new GamePadController(PlayerIndex.One);
          debugControls = new KeyboardController();
          controls.ControlsActive = false;
          debugControls.ControlsActive = false;
          this.menuItemsDrawOffset = menuItemsDrawOffset;
          this.displayItemNames = displayItemNames;
      }

      public void SetDefaultMenuOption(int menuOptionIndex)
      {
          defaultMenuOption = menuOptions[menuOptionIndex];
      }
      public void AddMenuOption(MenuOption menuOption, bool setDefault)
      {
          menuOptions.Add(menuOption);
          if (setDefault == true)
          {
              defaultMenuOption = menuOption;
          }
      }
      public void ClearOptions()
      {
          menuOptions = new List<MenuOption>();
          defaultMenuOption = null;
      }
      public void ResetSelection()
      {
          currentMenuOption = defaultMenuOption;
      }
      public string GetSelection()
      {
          if (menuState != MenuState.IdleValueSelected)
          {
              throw new Exception("Attempted to get selection when it hasn't been made yet.");
          }

          return currentMenuOption.ReturnValue();
      }
      public string GetSelectionDescription()
      {
          return currentMenuOption.Description;
      }
      public void ListenForNewSelection()
      {
          menuState = MenuState.IdleAwaitingInput;
          controls.ControlsActive = true;
          debugControls.ControlsActive = true;
      }
      public void Show()
      {
          menuState = MenuState.FadingIn;
          if (currentMenuOption == null)
          {
              currentMenuOption = defaultMenuOption;
              currentMenuOption.MenuOptionState = MenuOptionState.Selected;
          }
          controls.ControlsActive = true;
          debugControls.ControlsActive = true;
      }
      public void Hide()
      {
          controls.ControlsActive = false;
          debugControls.ControlsActive = false;
          menuState = MenuState.FadingOut;
      }
      public void HalfHide()
      {
          controls.ControlsActive = false;
          debugControls.ControlsActive = false;
          menuState = MenuState.FadingOutHalf;
      }
      public int GetMenuOptionCount()
      {
          return menuOptions.Count;
      }

      public override void Update(GameTime gameTime)
      {
          if (menuState != MenuState.IdleAwaitingInput)
          {
              controls.ControlsActive = false;
              debugControls.ControlsActive = false;
          }

          switch(menuState)
          {
              case MenuState.FadingIn:
                  if (alpha < 1.0f)
                  {
                      alpha += fadeSpeed;
                  }
                  else
                  {
                      alpha = 1.0f;
                      menuState = MenuState.IdleAwaitingInput;
                      controls.ControlsActive = true;
                      debugControls.ControlsActive = true;
                  }
                  break;

              case MenuState.FadingOut:
                  if (alpha > 0.0f)
                  {
                      alpha -= fadeSpeed;
                  }
                  else
                  {
                      alpha = 0.0f;
                      menuState = MenuState.IdleFaded;
                  }
                  break;

              case MenuState.FadingOutHalf:
                  if (alpha > 0.5f)
                  {
                      alpha -= fadeSpeed;
                  }
                  else
                  {
                      alpha = 0.5f;
                      menuState = MenuState.IdleHalfFaded;
                  }
                  break;
          }



          controls.Update(gameTime);
          debugControls.Update(gameTime);

              if (controls.ButtonPressed(Buttons.DPadDown, true, true) || debugControls.KeyPressed(Keys.Down, true, true))
              {
                  GameClass.SoundManager.PlaySoundEffect("Audio/cursorMove");
                  currentMenuOption.MenuOptionState = MenuOptionState.NotSelected;
                  if (menuOptions.IndexOf(currentMenuOption) < menuOptions.Count - 1)
                  {
                      currentMenuOption = menuOptions[menuOptions.IndexOf(currentMenuOption) + 1];
                  }
                  else
                  {
                      currentMenuOption = menuOptions[0];
                  }
                  currentMenuOption.MenuOptionState = MenuOptionState.Selected;
              }
              else if (controls.ButtonPressed(Buttons.DPadUp, true, true) || debugControls.KeyPressed(Keys.Up, true, true))
                    {
                        GameClass.SoundManager.PlaySoundEffect("Audio/cursorMove");
                        currentMenuOption.MenuOptionState = MenuOptionState.NotSelected;
                        if (menuOptions.IndexOf(currentMenuOption) > 0)
                        {
                            currentMenuOption = menuOptions[menuOptions.IndexOf(currentMenuOption) - 1];
                        }
                        else
                        {
                            currentMenuOption = menuOptions[menuOptions.Count - 1];
                        }
                        currentMenuOption.MenuOptionState = MenuOptionState.Selected;
                    }
                    else if (controls.ButtonPressed(Buttons.B, false, false) || debugControls.KeyPressed(Keys.Space, false, false))
                    {
                        menuState = MenuState.IdleValueSelected;
                        GameClass.SoundManager.PlaySoundEffect("Audio/menuConfirm");
                        controls.ControlsActive = false;
                        debugControls.ControlsActive = false;
                    }
          base.Update(gameTime);
      }
      public override void Draw()
      {
          Tint = new Color(1.0f, 1.0f, 1.0f, alpha);
          base.Draw();
          Vector2 cursorDrawPosition = Vector2.Zero;
          for(int count = 0; count<menuOptions.Count; count++)
          {
              int drawPosY = (int)(Position.Y + 5 + (menuOptions[count].SelectedTexture.Height / 2) - (Texture.Height / 2) + (count * (menuOptions[count].SelectedTexture.Height + menuOptionSpacing)));

              Vector2 optionDrawPosition = new Vector2(Position.X + menuItemsDrawOffset.X, drawPosY + +menuItemsDrawOffset.Y);
              if (menuOptions[count].MenuOptionState == MenuOptionState.Selected)
              {
                  GameClass.SpriteBatch.Draw(menuOptions[count].SelectedTexture, optionDrawPosition, null, Tint, 0.0f, new Vector2(menuOptions[count].SelectedTexture.Width / 2, menuOptions[count].SelectedTexture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
                  cursorDrawPosition = new Vector2(optionDrawPosition.X + cursorRelativePosition.X, optionDrawPosition.Y + cursorRelativePosition.Y);
                  if (displayItemNames == true)
                  {
                      GameClass.SpriteBatch.DrawString(GameClass.Size8Font, menuOptions[count].Name, new Vector2(optionDrawPosition.X + 20 - menuOptions[count].SelectedTexture.Width / 2, optionDrawPosition.Y + 5 - menuOptions[count].SelectedTexture.Height / 2), new Color(menuOptions[count].SelectedNameColour.R, menuOptions[count].SelectedNameColour.G, menuOptions[count].SelectedNameColour.B, Tint.A));
                  }
              }
              else if (menuOptions[count].MenuOptionState == MenuOptionState.NotSelected)
              {
                  GameClass.SpriteBatch.Draw(menuOptions[count].NotSelectedTexture, optionDrawPosition, null, Tint, 0.0f, new Vector2(menuOptions[count].NotSelectedTexture.Width / 2, menuOptions[count].NotSelectedTexture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
                  if (displayItemNames == true)
                  {
                      GameClass.SpriteBatch.DrawString(GameClass.Size8Font, menuOptions[count].Name, new Vector2(optionDrawPosition.X + 20 - menuOptions[count].NotSelectedTexture.Width / 2, optionDrawPosition.Y + 5 - menuOptions[count].NotSelectedTexture.Height / 2), new Color(menuOptions[count].NotSelectedNameColour.R, menuOptions[count].NotSelectedNameColour.G, menuOptions[count].NotSelectedNameColour.B, Tint.A));
                  }
              }
          }
          GameClass.SpriteBatch.Draw(cursorTexture, cursorDrawPosition, Tint);
      }
  }
}
