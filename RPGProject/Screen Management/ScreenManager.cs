using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    public enum ScreenTransitionState
    {
        Idle,
        CurrentScreenOut,
        NewScreenIn,
    }
    public enum ScreenFadeState
    {
        FadeToBlack,
        FadeFromBlack,
        IdleFaded,
        IdleBlack,
    }
    public enum ScreenIdentity
    {
        Map,
        Battle,
        ThanksForPlaying
    }

    public class ScreenManager
    {//This class creates an instance of a object to handle the storage and transition of screens in the game.

        MapScreen mapScreen;
        public MapScreen MapScreen
        {
            get
            {
                return mapScreen;
            }
        }
        BattleScreen battleScreen;
        ThanksForPlayingScreen thanksForPlayingScreen;

        Screen currentScreen;
        Screen nextScreen;
        Screen previousScreen;
        ScreenTransitionState screenTransitionState;

        Texture2D screenFadeTexture;

        static ScreenFadeState screenFadeState;
        public static ScreenFadeState CurrentFadeState
        {
            get
            {
                return screenFadeState;
            }
        }

        Color fadeColor;
        static float fadeSpeed = 0.1f;
        static float alphaValue = 1.0f;

        public ScreenManager()
        {
        }

        public void Initialise()
        {
            mapScreen = new MapScreen();
            mapScreen.Initialise();
            battleScreen = new BattleScreen();
            battleScreen.Initialise();
            //thanksForPlayingScreen = new ThanksForPlayingScreen();
            //thanksForPlayingScreen.Initialise();

            currentScreen = mapScreen;
            screenTransitionState = ScreenTransitionState.NewScreenIn;
        }
        public void LoadContent()
        {
            //TEST SAVE DATA
            QuestData quest = GameClass.ContentManager.Load<QuestData>("storyquest1");
            GlobalGameInfo.CurrentMapStageSection = MapStageSectionID.InsideTutorialBuilding;
            GlobalGameInfo.CurrentPlayerCharacter = CharacterIdentity.Cross;
            GlobalGameInfo.CurrentPlayerPosition = new Vector2(350, 150);
            GlobalGameInfo.CurrentQuestName = quest.questName;
            GlobalGameInfo.CurrentQuestEnemies = quest.enemies;
            GlobalGameInfo.CurrentQuestNPCs = quest.NPCs;
            GlobalGameInfo.CurrentQuestProgress = quest.questFlags;
            GlobalGameInfo.QuestCompletionRequirements = quest.questRequirements;
            GlobalGameInfo.CurrentQuestRequirements = new List<QuestRequirementData>();
            GlobalGameInfo.CurrentQuestAutoDialogs = quest.autoDialogs;
            GlobalGameInfo.CurrentQuestItems = quest.items;

            GlobalGameInfo.CurrentPlayerParty = new PartyCharacter[3];

            GlobalGameInfo.PlayerStatsList = GameClass.ContentManager.Load<List<PlayerStatsData>>("PlayerStatsList");

            GlobalGameInfo.CurrentPlayerParty[0] = new PartyCharacter();
            GlobalGameInfo.CurrentPlayerParty[0].LoadContent(GlobalGameInfo.GetPlayerStatsData(CharacterIdentity.Cross));
            GlobalGameInfo.CurrentPlayerParty[1] = new PartyCharacter();
            GlobalGameInfo.CurrentPlayerParty[1].LoadContent(GlobalGameInfo.GetPlayerStatsData(CharacterIdentity.Guy));
            GlobalGameInfo.CurrentPlayerParty[2] = new PartyCharacter();
            GlobalGameInfo.CurrentPlayerParty[2].LoadContent(GlobalGameInfo.GetPlayerStatsData(CharacterIdentity.Shelly));

            GlobalGameInfo.Inventory = new Dictionary<InventoryItemIdentity, int>();
            //END TEST SAVE DATA

            screenFadeTexture = GameClass.ContentManager.Load<Texture2D>("Black");
            SetFade(ScreenFadeState.IdleBlack, 0.0f);
            screenTransitionState = ScreenTransitionState.Idle;
            currentScreen.LoadContent();
            GameClass.SoundManager.LoadSound("Audio/cursorMove");
            GameClass.SoundManager.LoadSound("Audio/menuPopUp");
            GameClass.SoundManager.LoadSound("Audio/enemyDeath");
            GameClass.SoundManager.LoadSound("Audio/menuConfirm");
            GameClass.SoundManager.LoadSound("Audio/menuCancel");
        }
        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
            switch (screenTransitionState)
            {
                case ScreenTransitionState.Idle:
                    break;

                case ScreenTransitionState.CurrentScreenOut:

                    if (screenFadeState == ScreenFadeState.IdleBlack)
                    {
                        if (nextScreen == mapScreen && previousScreen == battleScreen)
                        {
                            currentScreen = nextScreen;
                            UnloadScreen(battleScreen);
                            previousScreen = battleScreen;
                            GameClass.CurrentGameCamera.Position = GlobalGameInfo.PreviousMapCameraPosition;
                            GlobalGameInfo.CurrentEnemyInBattle.CurrentAction = MapCharacterAction.Death;
                        }
                        else
                        {
                            UnloadScreen(currentScreen);
                            currentScreen = nextScreen;
                            currentScreen.LoadContent();
                        }
                        screenTransitionState = ScreenTransitionState.NewScreenIn;
                    }
                    break;

                case ScreenTransitionState.NewScreenIn:

                    if (ScreenManager.CurrentFadeState != ScreenFadeState.IdleFaded)
                    {
                        if (ScreenManager.CurrentFadeState != ScreenFadeState.FadeFromBlack)
                        {
                            ScreenManager.SetFade(ScreenFadeState.FadeFromBlack, fadeSpeed);
                        }
                    }
                    else
                    {
                            screenTransitionState = ScreenTransitionState.Idle;
                            if (currentScreen == mapScreen && previousScreen == battleScreen)
                            {
                                MapScreen.battleTriggered = false;
                                GameClass.SoundManager.PlaySoundEffect("Audio/enemyDeath");
                                GameClass.SoundManager.StopBackgroundMusic();
                                GameClass.SoundManager.SetBackgroundMusic(mapScreen.CurrentMapStageSection.BackgroundMusicFileName, true);
                                GameClass.SoundManager.PlayBackgroundMusic();
                            }
                    }
                    break;
            }

                switch (screenFadeState)
                {
                    case ScreenFadeState.FadeFromBlack:
                        if (alphaValue > 0.0f)
                        {
                            alphaValue -= fadeSpeed;
                        }
                        else
                        {
                            screenFadeState = ScreenFadeState.IdleFaded;
                        }
                        break;

                    case ScreenFadeState.FadeToBlack:
                        if (alphaValue < 1.0f)
                        {
                            alphaValue += fadeSpeed;
                        }
                        else
                        {
                            screenFadeState = ScreenFadeState.IdleBlack;
                        }
                        break;
                }

                fadeColor = new Color(new Vector4(1.0f, 1.0f, 1.0f, alphaValue));
        }
        public void Draw()
        {
                currentScreen.Draw();
                GameClass.SpriteBatch.Draw(screenFadeTexture, new Rectangle((int)GameClass.CurrentGameCamera.Position.X, (int)GameClass.CurrentGameCamera.Position.Y, GameClass.GraphicsManager.GraphicsDevice.Viewport.Width, GameClass.GraphicsManager.GraphicsDevice.Viewport.Height), fadeColor);
        }
        public static void SetFade(ScreenFadeState fadeAction, float speed)
        {
            screenFadeState = fadeAction;
            fadeSpeed = speed;

            switch (screenFadeState)
            {
                case ScreenFadeState.IdleBlack:
                    alphaValue = 1.0f;
                    break;
                case ScreenFadeState.FadeFromBlack:
                    break;
                case ScreenFadeState.FadeToBlack:
                    break;
                case ScreenFadeState.IdleFaded:
                    alphaValue = 0.0f;
                    break;
            }
        }
        public void ScreenTransition(ScreenIdentity newScreenID)
        {
            screenTransitionState = ScreenTransitionState.CurrentScreenOut;
            ScreenManager.SetFade(ScreenFadeState.FadeToBlack, fadeSpeed);

            previousScreen = currentScreen;
            switch (newScreenID)
            {
                case ScreenIdentity.Battle:
                    nextScreen = battleScreen;
                    break;
                case ScreenIdentity.Map:
                    nextScreen = mapScreen;
                    break;
                case ScreenIdentity.ThanksForPlaying:
                    nextScreen = thanksForPlayingScreen;
                    break;
            }
        }
        public void UnloadScreen(Screen screen)
        {
            switch(screen.ScreenID)
            {
                case ScreenIdentity.Battle:
                    battleScreen = new BattleScreen();
                    battleScreen.Initialise();
                    nextScreen = battleScreen;
                    break;
            }
        }
    }
}
