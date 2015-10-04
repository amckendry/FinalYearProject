using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    public enum PartyManagerState
    {
        IdleHidden,
        BeginShowSelectionMenu,
        EndShowSelectionMenu,
        CharacterSelection,
        ShowingStatus

    }
    public class PartyManager
    {
        MapScreen mapScreen;

        Texture2D inventoryStatusBG;
        Texture2D questStatusBG;
        PlayerCharacterStatusBox selectionStatusBox;
        Menu selectPartyMemberMenu;
        PartyManagerState currentState;

        GamePadController controls;
        KeyboardController debugControls;

        Texture2D screenCoverTexture;
        float screenCoverAlpha;
        float alpha;

        public PartyManager() 
        {
            currentState = PartyManagerState.IdleHidden;
            controls = new GamePadController(PlayerIndex.One);
            debugControls = new KeyboardController();
        }
        public void LoadContent(MapScreen mapScreen)
        {
            this.mapScreen = mapScreen;
            selectPartyMemberMenu = new Menu("Party Character Select Menu", Vector2.Zero, 6, "General/selectCharacterMenuBG", "General/TurnChartSelection", new Vector2(-27, -5), new Vector2(0, 15), false);
            foreach (PartyCharacter character in GlobalGameInfo.CurrentPlayerParty)
            {
                if (character != null)
                {
                    MenuOption newOption = new MenuOption(character.CharacterID.ToString(), character.PortraitTexture, character.PortraitTexture);
                    selectPartyMemberMenu.AddMenuOption(newOption, (selectPartyMemberMenu.GetMenuOptionCount() == 0));
                }
            }

            inventoryStatusBG = GameClass.ContentManager.Load<Texture2D>("General/inventoryStatusBG");
            questStatusBG = GameClass.ContentManager.Load<Texture2D>("General/questStatusBG");

            selectionStatusBox = new PlayerCharacterStatusBox(Vector2.Zero, PlayerCharacterStatusBoxMode.MapStatus);
            screenCoverTexture = GameClass.LoadTextureData("Black");
            screenCoverAlpha = 0.0f;
            alpha = 0.0f;

        }
        public void Show()
        {
            currentState = PartyManagerState.BeginShowSelectionMenu;
            screenCoverAlpha = 0.0f;
            alpha = 0.0f;
        }
        public bool IsHidden()
        {
            return (currentState == PartyManagerState.IdleHidden);
        }

        public void Update(GameTime gameTime)
        {
            controls.Update(gameTime);
            debugControls.Update(gameTime);
            selectPartyMemberMenu.Update(gameTime);
            selectionStatusBox.Update(gameTime);

            switch (currentState)
            {
                case PartyManagerState.BeginShowSelectionMenu:
                    if (alpha < 1.0f)
                    {
                        alpha += 0.1f;
                    }
                    else
                    {
                        alpha = 1.0f;
                    }

                    if (screenCoverAlpha < 0.5f)
                    {
                        screenCoverAlpha += 0.1f;
                    }
                    else
                    {
                        screenCoverAlpha = 0.5f;
                    }

                    if (screenCoverAlpha == 0.5f && alpha == 1.0f)
                    {
                        currentState = PartyManagerState.CharacterSelection;
                    }
                    break;

                case PartyManagerState.CharacterSelection:
                    if (controls.ButtonPressed(Buttons.A, false, false) || debugControls.KeyPressed(Keys.Back, false, false))
                    {
                        GameClass.SoundManager.PlaySoundEffect("Audio/menuCancel");

                        selectPartyMemberMenu.Hide();
                        currentState = PartyManagerState.EndShowSelectionMenu;
                    }

                    switch (selectPartyMemberMenu.MenuState)
                    {
                        case MenuState.IdleFaded:
                            selectPartyMemberMenu.Show();
                            break;


                        case MenuState.IdleValueSelected:
                            foreach (PartyCharacter character in GlobalGameInfo.CurrentPlayerParty)
                            {
                                if (character.CharacterID.ToString() == selectPartyMemberMenu.GetSelection())
                                {
                                    selectionStatusBox.LoadContent(character);
                                    selectPartyMemberMenu.HalfHide();
                                    selectionStatusBox.Show();
                                    currentState = PartyManagerState.ShowingStatus;
                                    break;
                                }
                            }
                            break;
                    }

                    break;

                case PartyManagerState.ShowingStatus:
                    if (controls.ButtonPressed(Buttons.A, false, false) || debugControls.KeyPressed(Keys.Back, false, false))
                    {
                        GameClass.SoundManager.PlaySoundEffect("Audio/menuCancel");
                        selectionStatusBox.Hide();
                        selectPartyMemberMenu.Show();
                        currentState = PartyManagerState.CharacterSelection;
                    }
                    break;
                case PartyManagerState.EndShowSelectionMenu:
                    if (alpha > 0.0f)
                    {
                        alpha = 0.0f;
                    }
                    else
                    {
                        alpha = 0.0f;
                    }


                    if (screenCoverAlpha > 0.0f)
                    {
                        screenCoverAlpha -= 0.1f;
                    }
                    else
                    {
                        screenCoverAlpha = 0.0f;
                    }

                    if (screenCoverAlpha == 0.0f && alpha == 0.0f)
                    {
                        currentState = PartyManagerState.IdleHidden;
                        mapScreen.Player.ControlsActive = true;
                        mapScreen.Player.DebugControlsActive = true;
                    }
                    break;
            }
        }
        public void Draw()
        {
            if (currentState != PartyManagerState.IdleHidden)
            {
                GameClass.SpriteBatch.Draw(screenCoverTexture, new Rectangle((int)GameClass.CurrentGameCamera.Position.X, (int)GameClass.CurrentGameCamera.Position.Y, GameClass.GraphicsManager.PreferredBackBufferWidth, GameClass.GraphicsManager.PreferredBackBufferHeight), new Color(1.0f, 1.0f, 1.0f, screenCoverAlpha));
            }

            selectPartyMemberMenu.Position = new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.GraphicsManager.PreferredBackBufferWidth / 6, GameClass.CurrentGameCamera.Position.Y + GameClass.GraphicsManager.PreferredBackBufferHeight / 2);
            selectPartyMemberMenu.Draw();
            selectionStatusBox.Position = new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.GraphicsManager.PreferredBackBufferWidth / 6, GameClass.CurrentGameCamera.Position.Y + GameClass.GraphicsManager.PreferredBackBufferHeight / 2);
            selectionStatusBox.Draw();


            Vector2 inventoryStatusDrawPos = new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.GraphicsManager.PreferredBackBufferWidth * 5 / 6, GameClass.CurrentGameCamera.Position.Y + GameClass.GraphicsManager.PreferredBackBufferHeight / 2);
            GameClass.SpriteBatch.Draw(inventoryStatusBG, inventoryStatusDrawPos, null, new Color(1.0f, 1.0f, 1.0f, alpha), 0.0f, new Vector2(inventoryStatusBG.Width / 2, inventoryStatusBG.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
            int drawPosY = 0;
            if (GlobalGameInfo.Inventory.Count > 0)
            {
                foreach (KeyValuePair<InventoryItemIdentity, int> item in GlobalGameInfo.Inventory)
                {
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, item.Key.ToString(), new Vector2(inventoryStatusDrawPos.X - 61, inventoryStatusDrawPos.Y - 32 + drawPosY), new Color(1.0f, 1.0f, 1.0f, alpha));
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, item.Value.ToString(), new Vector2(inventoryStatusDrawPos.X + 34, inventoryStatusDrawPos.Y - 32 + drawPosY), new Color(1.0f, 1.0f, 1.0f, alpha));
                    drawPosY += 12;
                }
            }
            else
            {
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "You have no items.", new Vector2(inventoryStatusDrawPos.X - 54, inventoryStatusDrawPos.Y - 32 + drawPosY), new Color(1.0f, 1.0f, 1.0f, alpha));
            }


            Vector2 questStatusDrawPos = new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.GraphicsManager.PreferredBackBufferWidth / 2, GameClass.CurrentGameCamera.Position.Y + GameClass.GraphicsManager.PreferredBackBufferHeight / 2);
            GameClass.SpriteBatch.Draw(questStatusBG, questStatusDrawPos, null, new Color(1.0f, 1.0f, 1.0f, alpha), 0.0f, new Vector2(questStatusBG.Width / 2, questStatusBG.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
            GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Current Quest: " + GlobalGameInfo.CurrentQuestName, new Vector2(questStatusDrawPos.X - 79, questStatusDrawPos.Y - 56), new Color(1.0f, 1.0f, 1.0f, alpha));

            drawPosY = 0;
            if (GlobalGameInfo.CurrentQuestRequirements.Count > 0)
            {
                foreach (QuestRequirementData requirement in GlobalGameInfo.CurrentQuestRequirements)
                {
                    if (GlobalGameInfo.QuestRequirementCompleted(requirement))
                    {
                        GameClass.SpriteBatch.DrawString(GameClass.Size8Font, requirement.questRequirementName, new Vector2(questStatusDrawPos.X - 79, questStatusDrawPos.Y - 26 + drawPosY), new Color(0.5f, 0.5f, 0.5f, alpha));
                    }
                    else
                    if (!GlobalGameInfo.QuestRequirementCompleted(requirement))
                    {
                        GameClass.SpriteBatch.DrawString(GameClass.Size8Font, requirement.questRequirementName, new Vector2(questStatusDrawPos.X - 79, questStatusDrawPos.Y - 26 + drawPosY), new Color(1.0f, 1.0f, 1.0f, alpha));
                    }
                    drawPosY += 12;
                }
            }
            else
            {
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "No quest requirements.", new Vector2(questStatusDrawPos.X - 69, questStatusDrawPos.Y - 26), new Color(1.0f, 1.0f, 1.0f, alpha));
            }
        }
    }
}
