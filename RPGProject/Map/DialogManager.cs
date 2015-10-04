using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPGProjectLibrary;

namespace RPGProject
{
    public enum DialogBoxState
    {
        Hidden,
        FadingIn,
        FadingOut,
        FadingToInactive,
        ShowingActive,
        ShowingInactive
    }
    class DialogBox
    {//This class creates an instance of an object to store information about dialog between players and NPCs.

        DialogBoxState currentState;
        public DialogBoxState CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                currentState = value;
            }
        }
        Texture2D texture;
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
        }
        Texture2D portraitTexture;
        public Texture2D PortraitTexture
        {
            get
            {
                return portraitTexture;
            }
            set
            {
                portraitTexture = value;
            }
        }
        float alpha;
        public float Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
            }
        }
        string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
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
        CharacterIdentity dialogCharacter;
        public CharacterIdentity DialogCharacter
        {
            get
            {
                return dialogCharacter;
            }
            set
            {
                dialogCharacter = value;
            }
        }

        public DialogBox(Vector2 position)
        {
            this.position = position;
            texture = GameClass.LoadTextureData("General/Dialog/DialogBox");
            alpha = 0.0f;
            currentState = DialogBoxState.Hidden;
        }
    }
    class DialogManager
    { //This class creates an instance of an object to manage dialog display between players and NPCs.

        MapScreen mapScreen;

        PlayerMapCharacter player;
        CharacterIdentity NPCTriggerID;

        public enum DialogManagerState
        {
            IdleHidden,
            BeginShowDialog,
            EndShowDialog,
            ShowingText,
            AwaitingInput
        }
        DialogManagerState currentState;

        public enum InputIconState
        {
            FadingIn,
            Flashing,
            FadingOut,
            Hidden
        }
        InputIconState inputIconState;

        Texture2D inputIconTexture;
        float inputIconAlpha = 0.0f;
        float inputIconFlashSpeed = 0.04f;
        int inputFlashDirection = 0;

        DialogBox activeDialogBox;
        DialogBox inactiveDialogBox;
        DialogBox topDialogBox;
        DialogBox bottomDialogBox;

        int maxLinesPerBox = 4;
        int dialogTextAreaWidth = 179;
        string remainderText = "";

        QuestDialogData currentConversation;
        GamePadController controls;
        KeyboardController debugControls;

        int currentSentenceIndex;
        int currentSentenceOrderID;

        int sentenceProgress = 0;
        float currentTextTime = 0.0f;
        float currentTextSpeed = 0.1f;
        float slowTextSpeed = 0.1f;
        float fastTextSpeed = 0.01f;

        float dialogFadeSpeed = 0.05f;

        Texture2D screenCoverTexture;
        float screenCoverAlpha;

        bool autoDialog;

        public DialogManager()
        {
            currentState = DialogManagerState.IdleHidden;
        }
        public void LoadContent(MapScreen mapScreen)
        {
            this.mapScreen = mapScreen;
            controls = new GamePadController(PlayerIndex.One);
            debugControls = new KeyboardController();
            GameClass.SoundManager.LoadSound("Audio/dialogSound");
        }

        public void LoadConversation(PlayerMapCharacter player, NPCMapCharacter NPCConversationTrigger)
        {
            autoDialog = false;
            this.player = player;
            this.NPCTriggerID = NPCConversationTrigger.CharacterID;

            topDialogBox = new DialogBox(new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.ViewPortBounds.Width / 2, GameClass.CurrentGameCamera.Position.Y + 90));
            bottomDialogBox = new DialogBox(new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.ViewPortBounds.Width / 2, GameClass.CurrentGameCamera.Position.Y + 190));
            currentSentenceIndex = 0;
            currentSentenceOrderID = 0;
            inputIconTexture = GameClass.LoadTextureData("General/BButton");
            inputIconAlpha = 0.0f;
            inputIconState = InputIconState.Hidden;
            inputFlashDirection = 0;

            currentState = DialogManagerState.IdleHidden;

            player.CurrentAction = MapCharacterAction.Idle;

            topDialogBox.DialogCharacter = NPCConversationTrigger.CharacterID;
            topDialogBox.PortraitTexture = NPCConversationTrigger.PortraitTexture;

            bottomDialogBox.DialogCharacter = player.CharacterID;
            bottomDialogBox.PortraitTexture = player.PortraitTexture;

            screenCoverTexture = GameClass.LoadTextureData("Black");
            screenCoverAlpha = 0.0f;

            bool convoTriggered = false;
            foreach(QuestDialogData dialogData in NPCConversationTrigger.LinkedNPCData.dialog)
            {
                if (dialogData.flagsToTrigger != null)
                {
                    int triggerCount = dialogData.flagsToTrigger.Count;
                    int flagsTriggered = 0;
                    foreach (QuestFlagData flagTriggerRequirement in dialogData.flagsToTrigger)
                    {
                        if (GlobalGameInfo.CheckFlagTrigger(dialogData.flagsToTrigger) == true)
                        {
                            flagsTriggered += 1;
                        }
                    }

                    if (flagsTriggered == triggerCount)
                    {
                        convoTriggered = true;
                        currentConversation = dialogData;
                        break;
                    }
                }
                else
                {
                    convoTriggered = true;
                    currentConversation = dialogData;
                    break;
                }
            }

            if (convoTriggered == false)
            {
                throw new Exception("no conversation found.");
            }
            else
            {

                currentSentenceIndex = 0;
                currentSentenceOrderID = 0;
                currentState = DialogManagerState.BeginShowDialog;
            }

        }
        public void LoadConversation(PlayerMapCharacter player, QuestDialogData questDialogData)
        {
            autoDialog = true;
            this.player = player;

            foreach (QuestDialogSentenceData sentence in questDialogData.sentences)
            {
                if (sentence.characterTalking != player.CharacterID)
                {
                    this.NPCTriggerID = sentence.characterTalking;
                    break;
                }
            }

            topDialogBox = new DialogBox(new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.ViewPortBounds.Width / 2, GameClass.CurrentGameCamera.Position.Y + 90));
            bottomDialogBox = new DialogBox(new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.ViewPortBounds.Width / 2, GameClass.CurrentGameCamera.Position.Y + 190));
            currentSentenceIndex = 0;
            currentSentenceOrderID = 0;
            inputIconTexture = GameClass.LoadTextureData("General/BButton");
            inputIconAlpha = 0.0f;
            inputIconState = InputIconState.Hidden;
            inputFlashDirection = 0;

            currentState = DialogManagerState.IdleHidden;

            topDialogBox.DialogCharacter = NPCTriggerID;
            topDialogBox.PortraitTexture = GameClass.LoadTextureData("MapCharacters/" + NPCTriggerID.ToString() + "/" + NPCTriggerID.ToString() + "Portrait");

            bottomDialogBox.DialogCharacter = player.CharacterID;
            bottomDialogBox.PortraitTexture = player.PortraitTexture;

            screenCoverTexture = GameClass.LoadTextureData("Black");
            screenCoverAlpha = 0.0f;

            currentConversation = questDialogData;

            currentSentenceIndex = 0;
            currentSentenceOrderID = 0;
            currentState = DialogManagerState.BeginShowDialog;

        }
        public void UnloadConversation()
        {
            currentState = DialogManagerState.EndShowDialog;
            activeDialogBox.CurrentState = DialogBoxState.FadingOut;
            if (currentConversation.inventoryChangesOnDialog != null)
            {
                if (currentConversation.inventoryChangesOnDialog.Count > 0)
                {
                    mapScreen.DisplayInventoryUpdateBar();
                }

                foreach (InventoryChangeData inventoryChange in currentConversation.inventoryChangesOnDialog)
                {
                    GlobalGameInfo.ChangeInventory(inventoryChange.inventoryChangeType, inventoryChange.itemIDToChange, inventoryChange.QTYToChangeBy);
                }
            }

            if (currentConversation.requirementsAdditionOnDialog != null)
            {
                if (currentConversation.requirementsAdditionOnDialog.Count > 0)
                {
                    foreach (QuestRequirementData newRequirement in currentConversation.requirementsAdditionOnDialog)
                    {
                        bool requirementAlreadyExists = false;
                        foreach (QuestRequirementData currentRequirement in GlobalGameInfo.CurrentQuestRequirements)
                        {
                            if (currentRequirement.questRequirementName == newRequirement.questRequirementName)
                            {
                                requirementAlreadyExists = true;
                            }
                        }

                        if (requirementAlreadyExists == false)
                        {
                            GlobalGameInfo.CurrentQuestRequirements.Add(newRequirement);
                            mapScreen.DisplayQuestUpdateBar();
                        }
                    }
                }
            }

            if (autoDialog == true)
            {
                GlobalGameInfo.CurrentQuestAutoDialogs.Remove(currentConversation);
            }

            MapScreen.inputDelay = true;
        }
        public void NextSentence()
        {
            inputIconState = InputIconState.Hidden;


            int nextSentenceIndex = 0;
            if (remainderText == "")
            {
                int nextSentenceOrderID = 0;

                foreach (QuestDialogOptionData response in currentConversation.sentences[currentSentenceIndex].responseOptions)
                {
                    if (response.optionName == "DEFAULT")
                    {
                        if (response.flagChangesOnResponse != null)
                        {
                            foreach (QuestFlagChangeData flagChange in response.flagChangesOnResponse)
                            {
                                GlobalGameInfo.SetQuestFlag(flagChange.flagNameToChange, flagChange.changeValue, flagChange.flagValueChangeType);
                            }
                        }

                        nextSentenceOrderID = response.nextSentence;
                        if (nextSentenceOrderID == -1)
                        {
                            UnloadConversation();
                            break;
                        }
                        else
                        {
                            bool nextSentenceExists = false;
                            for (int i = 0; i < currentConversation.sentences.Count; i++)
                            {
                                if ((currentConversation.sentences[i].characterTalking == player.CharacterID || currentConversation.sentences[i].characterTalking == NPCTriggerID) && currentConversation.sentences[i].sentenceID == nextSentenceOrderID)
                                {
                                    nextSentenceIndex = i;
                                    nextSentenceExists = true;
                                }
                            }

                            if (!nextSentenceExists)
                            {
                                UnloadConversation();
                            }
                            else
                            {
                                currentSentenceIndex = nextSentenceIndex;
                                currentSentenceOrderID = nextSentenceOrderID;

                                inactiveDialogBox = activeDialogBox;
                                inactiveDialogBox.CurrentState = DialogBoxState.FadingToInactive;
                                if (currentConversation.sentences[currentSentenceIndex].characterTalking == player.CharacterID)
                                {
                                    activeDialogBox = bottomDialogBox;
                                }
                                else if (currentConversation.sentences[currentSentenceIndex].characterTalking == NPCTriggerID)
                                {
                                    activeDialogBox = topDialogBox;
                                }
                                activeDialogBox.Text = currentConversation.sentences[currentSentenceIndex].sentenceText;
                                sentenceProgress = 0;
                                activeDialogBox.CurrentState = DialogBoxState.FadingIn;
                                WrapText();
                            }
                        }
                    }
                }


                    
            }
            else
            {
                sentenceProgress = 0;
                activeDialogBox.Text = remainderText;
                currentState = DialogManagerState.ShowingText;
                remainderText = "";
                WrapText();
            }
        }
        public void Update(GameTime gameTime)
        {
            controls.Update(gameTime);
            debugControls.Update(gameTime);
            switch (currentState)
            {
                case DialogManagerState.BeginShowDialog:

                    if (screenCoverAlpha < 0.5f)
                    {
                        screenCoverAlpha += dialogFadeSpeed;
                    }
                    else
                    {
                        screenCoverAlpha = 0.5f;
                    }

                    if (screenCoverAlpha == 0.5f)
                    {

                        if (topDialogBox.DialogCharacter == currentConversation.sentences[0].characterTalking)
                        {
                            activeDialogBox = topDialogBox;
                        }

                        else if (bottomDialogBox.DialogCharacter == currentConversation.sentences[0].characterTalking)
                        {
                            activeDialogBox = bottomDialogBox;
                        }

                        activeDialogBox.CurrentState = DialogBoxState.FadingIn;
                        activeDialogBox.Text = currentConversation.sentences[0].sentenceText;
                        sentenceProgress = 0;
                        WrapText();
                        currentState = DialogManagerState.ShowingText;
                    }
                    break;

                case DialogManagerState.EndShowDialog:
                     if (screenCoverAlpha > 0.0f)
                    {
                        screenCoverAlpha -= dialogFadeSpeed;
                    }
                    else
                    {
                        screenCoverAlpha = 0.0f;
                    }

                     if (screenCoverAlpha == 0.0f)
                     {
                         activeDialogBox = null;
                         topDialogBox = null;
                         bottomDialogBox = null;
                         player.ControlsActive = true;
                         player.DebugControlsActive = true;
                         currentState = DialogManagerState.IdleHidden;
                     }
                    break;

                case DialogManagerState.ShowingText:
                    currentTextTime += GameClass.Elapsed;

                    if (currentTextTime > currentTextSpeed)
                    {
                        currentTextTime = 0.0f;
                        if (sentenceProgress < activeDialogBox.Text.Length)
                        {
                            sentenceProgress += 1;
                            GameClass.SoundManager.PlaySoundEffect("Audio/dialogSound");
                        }
                        else
                        {
                            currentState = DialogManagerState.AwaitingInput;
                            inputIconAlpha = 1.0f;

                            inputIconState = InputIconState.Flashing;
                        }
                    }

                        if (controls.ButtonPressed(Buttons.B, true, false) || debugControls.KeyPressed(Keys.RightShift, true, false))
                        {
                            currentTextSpeed = fastTextSpeed;
                        }

                        if (controls.ButtonReleased(Buttons.B) && debugControls.KeyReleased(Keys.RightShift))
                        {
                            currentTextSpeed = slowTextSpeed;
                        }

                    break;

                case DialogManagerState.AwaitingInput:

                        if (controls.ButtonPressed(Buttons.B, false, false) || debugControls.KeyPressed(Keys.RightShift, false, false))
                        {
                            NextSentence();
                            GameClass.SoundManager.PlaySoundEffect("Audio/menuConfirm");
                        }
                    break;
            }

            if (activeDialogBox != null)
            {
                switch (activeDialogBox.CurrentState)
                {
                    case DialogBoxState.FadingIn:
                        if (activeDialogBox.Alpha < 1.0f)
                        {
                            activeDialogBox.Alpha += dialogFadeSpeed;
                        }
                        else
                        {
                            activeDialogBox.CurrentState = DialogBoxState.ShowingActive;
                            currentState = DialogManagerState.ShowingText;
                        }
                        break;
                    case DialogBoxState.FadingOut:
                        if (activeDialogBox.Alpha > 0.0f)
                        {
                            activeDialogBox.Alpha -= dialogFadeSpeed;
                        }
                        else
                        {
                            activeDialogBox.CurrentState = DialogBoxState.Hidden;
                        }
                        break;
                }
            }

            if (inactiveDialogBox != null)
            {
                switch (inactiveDialogBox.CurrentState)
                {
                    case DialogBoxState.FadingToInactive:
                        if (inactiveDialogBox.Alpha > 0.75f)
                        {
                            inactiveDialogBox.Alpha -= dialogFadeSpeed;
                        }
                        else
                        {
                            inactiveDialogBox.CurrentState = DialogBoxState.ShowingInactive;
                        }
                        break;
                    case DialogBoxState.FadingOut:
                        if (inactiveDialogBox.Alpha > 0.0f)
                        {
                            inactiveDialogBox.Alpha -= dialogFadeSpeed;
                        }
                        else
                        {
                            inactiveDialogBox.CurrentState = DialogBoxState.Hidden;
                        }
                        break;
                }
            }

            switch (inputIconState)
            {
                case InputIconState.Hidden:
                    inputIconAlpha = 0.0f;
                    break;

                case InputIconState.FadingIn:
                    if (inputIconAlpha < 1.0f)
                    {
                        inputIconAlpha += inputIconFlashSpeed;
                    }
                    else
                    {
                        inputIconState = InputIconState.Flashing;
                    }
                    break;

                case InputIconState.Flashing:
                    if (inputFlashDirection == 0)
                    {
                        if (inputIconAlpha < 1.0f)
                        {
                            inputIconAlpha += inputIconFlashSpeed;
                        }
                        else
                        {
                            inputIconAlpha = 1.0f;
                            inputFlashDirection = 1;
                        }
                    }
                    else if (inputFlashDirection == 1)
                    {
                        if (inputIconAlpha > 0.5f)
                        {
                            inputIconAlpha -= inputIconFlashSpeed;
                        }
                        else
                        {
                            inputIconAlpha = 0.5f;
                            inputFlashDirection = 0;
                        }
                    }

                    break;
                case InputIconState.FadingOut:
                     if (inputIconAlpha > 0.0f)
                    {
                        inputIconAlpha -= inputIconFlashSpeed;
                    }
                    else
                    {
                        inputIconState = InputIconState.Hidden;
                    }
                    break;
            }
        }
        public void Draw()
        {
                if (currentState != DialogManagerState.IdleHidden)
                {
                    GameClass.SpriteBatch.Draw(screenCoverTexture, new Rectangle((int)GameClass.CurrentGameCamera.Position.X, (int)GameClass.CurrentGameCamera.Position.Y, GameClass.GraphicsManager.PreferredBackBufferWidth, GameClass.GraphicsManager.PreferredBackBufferHeight), new Color(1.0f, 1.0f, 1.0f, screenCoverAlpha));
                }

                if (activeDialogBox != null)
                {
                    string drawText = activeDialogBox.Text.Substring(0, sentenceProgress);
                        if (activeDialogBox == topDialogBox)
                        {
                            activeDialogBox.Position = new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.ViewPortBounds.Width / 2, GameClass.CurrentGameCamera.Position.Y + 150);

                            GameClass.SpriteBatch.Draw(activeDialogBox.Texture, activeDialogBox.Position, null, new Color(1.0f, 1.0f, 1.0f, activeDialogBox.Alpha), 0.0f, new Vector2(activeDialogBox.Texture.Width / 2, activeDialogBox.Texture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
                            GameClass.SpriteBatch.Draw(activeDialogBox.PortraitTexture, new Vector2(activeDialogBox.Position.X + 96, activeDialogBox.Position.Y - 7), null, new Color(1.0f, 1.0f, 1.0f, activeDialogBox.Alpha), 0.0f, new Vector2(activeDialogBox.PortraitTexture.Width / 2, activeDialogBox.PortraitTexture.Height / 2), 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                            GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + activeDialogBox.DialogCharacter, new Vector2(activeDialogBox.Position.X + 96, activeDialogBox.Position.Y + 22), new Color(1.0f, 1.0f, 0.0f, activeDialogBox.Alpha), 0.0f, new Vector2(GameClass.Size8Font.MeasureString("" + activeDialogBox.DialogCharacter).X / 2, GameClass.Size8Font.MeasureString("" + activeDialogBox.DialogCharacter).Y / 2), 1.0f, SpriteEffects.None, 0.0f);
                            GameClass.SpriteBatch.DrawString(GameClass.Size8Font, drawText, new Vector2(activeDialogBox.Position.X - 119, activeDialogBox.Position.Y - 27), new Color(1.0f, 1.0f, 1.0f, activeDialogBox.Alpha));
                            GameClass.SpriteBatch.Draw(inputIconTexture, new Vector2(activeDialogBox.Position.X - 123, activeDialogBox.Position.Y + 26), null, new Color(1.0f, 1.0f, 1.0f, inputIconAlpha), 0.0f, new Vector2(inputIconTexture.Width / 2, inputIconTexture.Height / 2), 0.7f, SpriteEffects.None, 0.0f);
                        }
                        else if (activeDialogBox == bottomDialogBox)
                        {
                            activeDialogBox.Position = new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.ViewPortBounds.Width / 2, GameClass.CurrentGameCamera.Position.Y + 230);

                            GameClass.SpriteBatch.Draw(activeDialogBox.Texture, activeDialogBox.Position, null, new Color(1.0f, 1.0f, 1.0f, activeDialogBox.Alpha), 0.0f, new Vector2(activeDialogBox.Texture.Width / 2, activeDialogBox.Texture.Height / 2), 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                            GameClass.SpriteBatch.Draw(activeDialogBox.PortraitTexture, new Vector2(activeDialogBox.Position.X - 96, activeDialogBox.Position.Y - 7), null, new Color(1.0f, 1.0f, 1.0f, activeDialogBox.Alpha), 0.0f, new Vector2(activeDialogBox.PortraitTexture.Width / 2, activeDialogBox.PortraitTexture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
                            GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + activeDialogBox.DialogCharacter, new Vector2(activeDialogBox.Position.X - 96, activeDialogBox.Position.Y + 22), new Color(1.0f, 1.0f, 0.0f, activeDialogBox.Alpha), 0.0f, new Vector2(GameClass.Size8Font.MeasureString("" + activeDialogBox.DialogCharacter).X / 2, GameClass.Size8Font.MeasureString("" + activeDialogBox.DialogCharacter).Y / 2), 1.0f, SpriteEffects.None, 0.0f);
                            GameClass.SpriteBatch.DrawString(GameClass.Size8Font, drawText, new Vector2(activeDialogBox.Position.X - 58, activeDialogBox.Position.Y - 27), new Color(1.0f, 1.0f, 1.0f, activeDialogBox.Alpha));
                            GameClass.SpriteBatch.Draw(inputIconTexture, new Vector2(activeDialogBox.Position.X + 123, activeDialogBox.Position.Y + 26), null, new Color(1.0f, 1.0f, 1.0f, inputIconAlpha), 0.0f, new Vector2(inputIconTexture.Width / 2, inputIconTexture.Height / 2), 0.7f, SpriteEffects.None, 0.0f);
                        }
                }

                if (inactiveDialogBox != null)
                {
                    if (inactiveDialogBox == topDialogBox)
                    {
                        inactiveDialogBox.Position = new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.ViewPortBounds.Width / 2, GameClass.CurrentGameCamera.Position.Y + 150);

                        GameClass.SpriteBatch.Draw(inactiveDialogBox.Texture, inactiveDialogBox.Position, null, new Color(1.0f, 1.0f, 1.0f, inactiveDialogBox.Alpha), 0.0f, new Vector2(inactiveDialogBox.Texture.Width / 2, inactiveDialogBox.Texture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
                        GameClass.SpriteBatch.Draw(inactiveDialogBox.PortraitTexture, new Vector2(inactiveDialogBox.Position.X + 96, inactiveDialogBox.Position.Y - 7), null, new Color(1.0f, 1.0f, 1.0f, inactiveDialogBox.Alpha), 0.0f, new Vector2(inactiveDialogBox.PortraitTexture.Width / 2, inactiveDialogBox.PortraitTexture.Height / 2), 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                        GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + inactiveDialogBox.DialogCharacter, new Vector2(inactiveDialogBox.Position.X + 96, inactiveDialogBox.Position.Y + 22), new Color(1.0f, 1.0f, 0.0f, inactiveDialogBox.Alpha), 0.0f, new Vector2(GameClass.Size8Font.MeasureString("" + inactiveDialogBox.DialogCharacter).X / 2, GameClass.Size8Font.MeasureString("" + inactiveDialogBox.DialogCharacter).Y / 2), 1.0f, SpriteEffects.None, 0.0f);

                        GameClass.SpriteBatch.DrawString(GameClass.Size8Font, inactiveDialogBox.Text, new Vector2(inactiveDialogBox.Position.X - 119, inactiveDialogBox.Position.Y - 27), new Color(1.0f, 1.0f, 1.0f, inactiveDialogBox.Alpha));
                    }
                    else if (inactiveDialogBox == bottomDialogBox)
                    {
                        inactiveDialogBox.Position = new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.ViewPortBounds.Width / 2, GameClass.CurrentGameCamera.Position.Y + 230);

                        GameClass.SpriteBatch.Draw(inactiveDialogBox.Texture, inactiveDialogBox.Position, null, new Color(1.0f, 1.0f, 1.0f, inactiveDialogBox.Alpha), 0.0f, new Vector2(inactiveDialogBox.Texture.Width / 2, inactiveDialogBox.Texture.Height / 2), 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                        GameClass.SpriteBatch.Draw(inactiveDialogBox.PortraitTexture, new Vector2(inactiveDialogBox.Position.X - 96, inactiveDialogBox.Position.Y - 7), null, new Color(1.0f, 1.0f, 1.0f, inactiveDialogBox.Alpha), 0.0f, new Vector2(inactiveDialogBox.PortraitTexture.Width / 2, inactiveDialogBox.PortraitTexture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
                        GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + inactiveDialogBox.DialogCharacter, new Vector2(inactiveDialogBox.Position.X - 96, inactiveDialogBox.Position.Y + 22), new Color(1.0f, 1.0f, 0.0f, inactiveDialogBox.Alpha), 0.0f, new Vector2(GameClass.Size8Font.MeasureString("" + inactiveDialogBox.DialogCharacter).X / 2, GameClass.Size8Font.MeasureString("" + inactiveDialogBox.DialogCharacter).Y / 2), 1.0f, SpriteEffects.None, 0.0f);

                        GameClass.SpriteBatch.DrawString(GameClass.Size8Font, inactiveDialogBox.Text, new Vector2(inactiveDialogBox.Position.X - 58, inactiveDialogBox.Position.Y - 27), new Color(1.0f, 1.0f, 1.0f, inactiveDialogBox.Alpha));
                    }
                }

       }
        public void WrapText()
        {
            List<string> lines = new List<string>();
            string[] words = activeDialogBox.Text.Split(' ');
            int currentLines = 0;
            string line = "";
            string newString = "";

            foreach (string word in words)
            {
                if (GameClass.Size8Font.MeasureString(line + " " + word).X > dialogTextAreaWidth)
                {
                    if (currentLines < maxLinesPerBox)
                    {
                        lines.Add(line);
                        newString += line + "\n";
                        currentLines += 1;
                        line = word;
                    }
                    else
                    {
                        line += " " + word;
                    }
                }
                else
                {
                    line += " " + word;
                }
            }

                if (!string.IsNullOrEmpty(line))
                {
                    if (currentLines < maxLinesPerBox)
                    {
                        lines.Add(line);
                        newString += line;
                    }
                    else
                    {
                        remainderText += line;
                    }
                }

            activeDialogBox.Text = newString;
        }
        public bool IsHidden()
        {
            return (currentState == DialogManagerState.IdleHidden);
        }
    }
}
