using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    public enum PlayerCharacterStatusBoxMode
    {
        EndOfBattle,
        MapStatus
    }
    public enum PlayerCharacterStatusBoxState
    {
        AddingEXP,
        AwaitingInput
    }
    public class PlayerCharacterStatusBox : TexturedGameObject
    {
        Menu spendPointsMenu;
        InfoBar spendPointsInfoBar;
        GamePadController controls;
        KeyboardController debugControls;
        PartyCharacter linkedCharacter;
        PlayerCharacterStatusBoxMode mode;
        PlayerCharacterStatusBoxState state;
        public PlayerCharacterStatusBoxState State
        {
            get
            {
                return state;
            }
        }
        FadeState fadeState;
        float alpha;
        bool levelUpOccurred;
        bool newTechniqueLearned;
        int EXPToAdd;
        float addEXPTimer;

        public PlayerCharacterStatusBox(Vector2 position, PlayerCharacterStatusBoxMode mode) : base("General/StatusBoxBG", position, Vector2.Zero)
        {
           this.mode = mode;
           alpha = 0.0f;
           fadeState = FadeState.IdleFaded;
           levelUpOccurred = false;
           newTechniqueLearned = false;
           controls = new GamePadController(PlayerIndex.One);
           debugControls = new KeyboardController();
           addEXPTimer = 0.0f;
        }

        public void LoadContent(PartyCharacter partyCharacter)
        {
            linkedCharacter = partyCharacter;
            switch (mode)
            {
                case PlayerCharacterStatusBoxMode.EndOfBattle:
                    GameClass.SoundManager.LoadSound("Audio/expAddSound");
                    GameClass.SoundManager.LoadSound("Audio/levelUpSound");
                    GameClass.SoundManager.LoadSound("Audio/newTechniqueSound");
                    break;
                case PlayerCharacterStatusBoxMode.MapStatus:
                    spendPointsMenu = new Menu("Spend Points Menu", Vector2.Zero, 5, "Blank", "General/playerTurnMenuCursor", Vector2.Zero, Vector2.Zero, false);
                    MenuOption HPOption = new MenuOption("AddPointsHP", "Spend a point to increase " + linkedCharacter.CharacterID.ToString() + "'s HP by 2.", Color.White, Color.White, "General/spendPointsSelected", "General/spendPointsNotSelected");
                    spendPointsMenu.AddMenuOption(HPOption, true);
                    MenuOption MPOption = new MenuOption("AddPointsMP", "Spend a point to increase " + linkedCharacter.CharacterID.ToString() + "'s MP by 2.", Color.White, Color.White, "General/spendPointsSelected", "General/spendPointsNotSelected");
                    spendPointsMenu.AddMenuOption(MPOption, false);
                    MenuOption ATKOption = new MenuOption("AddPointsATK", "Spend a point to increase " + linkedCharacter.CharacterID.ToString() + "'s ATK by 1.", Color.White, Color.White, "General/spendPointsSelected", "General/spendPointsNotSelected");
                    spendPointsMenu.AddMenuOption(ATKOption, false);
                    MenuOption DEFOption = new MenuOption("AddPointsDEF", "Spend a point to increase " + linkedCharacter.CharacterID.ToString() + "'s DEF by 1.", Color.White, Color.White, "General/spendPointsSelected", "General/spendPointsNotSelected");
                    spendPointsMenu.AddMenuOption(DEFOption, false);
                    MenuOption MAG_ATKOption = new MenuOption("AddPointsMAG_ATK", "Spend a point to increase " + linkedCharacter.CharacterID.ToString() + "'s MAG_ATK by 1.", Color.White, Color.White, "General/spendPointsSelected", "General/spendPointsNotSelected");
                    spendPointsMenu.AddMenuOption(MAG_ATKOption, false);
                    MenuOption MAG_DEFOption = new MenuOption("AddPointsMAG_DEF", "Spend a point to increase " + linkedCharacter.CharacterID.ToString() + "'s MAG_DEF by 1.", Color.White, Color.White, "General/spendPointsSelected", "General/spendPointsNotSelected");
                    spendPointsMenu.AddMenuOption(MAG_DEFOption, false);
                    MenuOption SPDOption = new MenuOption("AddPointsSPD", "Spend a point to increase " + linkedCharacter.CharacterID.ToString() + "'s SPD by 1.", Color.White, Color.White, "General/spendPointsSelected", "General/spendPointsNotSelected");
                    spendPointsMenu.AddMenuOption(SPDOption, false);

                    spendPointsInfoBar = new InfoBar("General/InfoBarBG", Vector2.Zero, GameClass.Size8Font, Color.White, new Vector2(-225, -10), 0.1f);
                    break;
            }


        }
        public override void Update(GameTime gameTime)
        {
            controls.Update(gameTime);
            debugControls.Update(gameTime);

            switch (fadeState)
            {
                case FadeState.FadingIn:
                    if (alpha < 1.0f)
                    {
                        alpha += 0.1f;
                    }
                    else
                    {
                        alpha = 1.0f;
                        fadeState = FadeState.IdleOpaque;
                    }
                    break;

                case FadeState.FadingOut:
                    if (alpha > 0.0f)
                    {
                        alpha -= 0.1f;
                    }
                    else
                    {
                        alpha = 0.0f;
                        fadeState = FadeState.IdleFaded;
                    }
                    break;
            }

            switch (mode)
            {
                case PlayerCharacterStatusBoxMode.EndOfBattle:


                    switch (state)
                    {
                        case PlayerCharacterStatusBoxState.AddingEXP:
                            if (EXPToAdd > 0)
                            {
                                if (!(linkedCharacter.Level == GlobalGameInfo.LevelLimit))
                                {


                                    addEXPTimer += GameClass.Elapsed;

                                    if (addEXPTimer > 0.1f)
                                    {
                                        addEXPTimer = 0.0f;
                                        GameClass.SoundManager.PlaySoundEffect("Audio/expAddSound");
                                        EXPToAdd -= 1;
                                        linkedCharacter.ExpToNext -= 1;
                                        if (linkedCharacter.ExpToNext == 0)
                                        {
                                            int previousLevel = linkedCharacter.Level;
                                            bool playTechniqueLevelUpSound = false;
                                            linkedCharacter.LevelUp();
                                            foreach (BattleTechniqueLevelData techniqueLevelData in linkedCharacter.TechniqueLevelData)
                                            {
                                                if (previousLevel < techniqueLevelData.levelLearnt && !(linkedCharacter.Level < techniqueLevelData.levelLearnt))
                                                {
                                                    newTechniqueLearned = true;
                                                    playTechniqueLevelUpSound = true;
                                                }
                                            }

                                            if (playTechniqueLevelUpSound == true)
                                            {
                                                GameClass.SoundManager.PlaySoundEffect("Audio/newTechniqueSound");
                                            }
                                            else
                                            {
                                                GameClass.SoundManager.PlaySoundEffect("Audio/levelUpSound");
                                            }
                                            levelUpOccurred = true;
                                        }
                                    }
                                }
                                else
                                {
                                    EXPToAdd = 0;
                                    linkedCharacter.ExpToNext = 0;
                                    state = PlayerCharacterStatusBoxState.AwaitingInput;
                                }
                            }
                            else
                            {
                                addEXPTimer = 0.0f;
                                EXPToAdd = 0;
                                state = PlayerCharacterStatusBoxState.AwaitingInput;
                            }

                            break;
                    }
                    break;
                case PlayerCharacterStatusBoxMode.MapStatus:
                    if (spendPointsMenu != null && spendPointsInfoBar != null)
                    {
                        spendPointsMenu.Update(gameTime);
                        spendPointsInfoBar.Update(gameTime);

                        switch (spendPointsMenu.MenuState)
                        {
                            case MenuState.IdleFaded:
                                spendPointsInfoBar.InfoText = spendPointsMenu.GetSelectionDescription();
                                break;

                            case MenuState.IdleAwaitingInput:
                                spendPointsInfoBar.InfoText = spendPointsMenu.GetSelectionDescription();
                                break;

                            case MenuState.IdleValueSelected:
                                if (linkedCharacter.PointsToSpend > 0)
                                {
                                    switch (spendPointsMenu.GetSelection())
                                    {
                                        case "AddPointsHP":
                                            linkedCharacter.HP += 2;
                                            linkedCharacter.PointsToSpend -= 1;
                                            break;

                                        case "AddPointsMP":
                                            linkedCharacter.MP += 2;
                                            linkedCharacter.PointsToSpend -= 1;
                                            break;

                                        case "AddPointsATK":
                                            linkedCharacter.ATK += 1;
                                            linkedCharacter.PointsToSpend -= 1;
                                            break;

                                        case "AddPointsDEF":
                                            linkedCharacter.DEF += 1;
                                            linkedCharacter.PointsToSpend -= 1;
                                            break;

                                        case "AddPointsMAG_ATK":
                                            linkedCharacter.MAG_ATK += 1;
                                            linkedCharacter.PointsToSpend -= 1;
                                            break;

                                        case "AddPointsMAG_DEF":
                                            linkedCharacter.MAG_DEF += 1;
                                            linkedCharacter.PointsToSpend -= 1;
                                            break;

                                        case "AddPointsSPD":
                                            linkedCharacter.SPD += 1;
                                            linkedCharacter.PointsToSpend -= 1;
                                            break;
                                    }
                                }
                                spendPointsMenu.ListenForNewSelection();


                                break;
                        }
                    }

                    break;
            }

            base.Update(gameTime);
        }

        public void Show()
        {
            fadeState = FadeState.FadingIn;
            if (mode == PlayerCharacterStatusBoxMode.MapStatus)
            {
                spendPointsMenu.Show();
                spendPointsInfoBar.Show();
            }
        }
        public void Hide()
        {
            fadeState = FadeState.FadingOut;
            if (mode == PlayerCharacterStatusBoxMode.MapStatus)
            {
                spendPointsMenu.Hide();
                spendPointsInfoBar.Hide();
            }
        }
        public void AddEXP(int EXPToAdd)
        {
            this.EXPToAdd = EXPToAdd;
            state = PlayerCharacterStatusBoxState.AddingEXP;
        }
        public override void Draw()
        {
            Tint = new Color(Tint.R, Tint.G, Tint.B, alpha);        
            base.Draw();
            if (linkedCharacter != null)
            {
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, linkedCharacter.CharacterID.ToString(), new Vector2(position.X - (int)(GameClass.Size8Font.MeasureString(linkedCharacter.CharacterID.ToString()).X / 2), position.Y - 95), Tint);
                Color drawColour = new Color(Tint.R, Tint.G, Tint.B, alpha);
                if (levelUpOccurred == true)
                {
                    drawColour = new Color(1.0f, 1.0f, 0.0f, alpha);
                }
                GameClass.SpriteBatch.Draw(linkedCharacter.PortraitTexture, new Vector2(position.X, position.Y - 75), null, Tint, 0.0f, new Vector2(linkedCharacter.PortraitTexture.Width / 2, linkedCharacter.PortraitTexture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Level:", new Vector2(position.X - 20, position.Y - 70), Tint);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "EXP to Next:", new Vector2(position.X - 60, position.Y - 58), Tint);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Points to Spend:", new Vector2(position.X - 60, position.Y - 46), Tint);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "HP:", new Vector2(position.X - 50, position.Y - 27), Tint);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "MP:", new Vector2(position.X - 50, position.Y - 12), Tint);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "ATK:", new Vector2(position.X - 50, position.Y + 3), Tint);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "DEF:", new Vector2(position.X - 50, position.Y + 18), Tint);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "MAG_ATK:", new Vector2(position.X - 50, position.Y + 33), Tint);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "MAG_DEF:", new Vector2(position.X - 50, position.Y + 48), Tint);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "SPD:", new Vector2(position.X - 50, position.Y + 63), Tint);

                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + "" + linkedCharacter.Level, new Vector2(position.X + 15, position.Y - 70), drawColour);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + linkedCharacter.ExpToNext, new Vector2(position.X + 15, position.Y - 58), drawColour);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + linkedCharacter.PointsToSpend, new Vector2(position.X + 35, position.Y - 46), drawColour);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + linkedCharacter.HP, new Vector2(position.X + 15, position.Y - 27), drawColour);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + linkedCharacter.MP, new Vector2(position.X + 15, position.Y - 12), drawColour);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + linkedCharacter.ATK, new Vector2(position.X + 15, position.Y + 3), drawColour);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + linkedCharacter.DEF, new Vector2(position.X + 15, position.Y + 18), drawColour);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + linkedCharacter.MAG_ATK, new Vector2(position.X + 15, position.Y + 33), drawColour);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + linkedCharacter.MAG_DEF, new Vector2(position.X + 15, position.Y + 48), drawColour);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + linkedCharacter.SPD, new Vector2(position.X + 15, position.Y + 63), drawColour);

                if (newTechniqueLearned == true)
                {
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "New skill!", new Vector2(position.X - (int)(GameClass.Size8Font.MeasureString("New skill!").X / 2), position.Y + 80), drawColour);
                }

                if (mode == PlayerCharacterStatusBoxMode.MapStatus)
                {
                    spendPointsInfoBar.Position = new Vector2(GameClass.CurrentGameCamera.Position.X + spendPointsInfoBar.Texture.Width / 2, GameClass.CurrentGameCamera.Position.Y + spendPointsInfoBar.Texture.Height / 2);
                    spendPointsMenu.Position = new Vector2(Position.X + 47, Position.Y - 29);

                    spendPointsInfoBar.Draw();
                    spendPointsMenu.Draw();
                }
            }
        }
    }
}
