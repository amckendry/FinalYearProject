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
    public enum BattleTargetSelectorState
    {
        Inactive,
        AwaitingInput,
        AllTargetsSelected
    }

    public class BattleTargetSelector
    {
        bool flicker = false;
        float flickerTimer = 0.0f;
        float flickerSpeed = 0.1f;
        GamePadController controls;
        KeyboardController debugControls;
        Texture2D cursorTexture;
        int currentCursorSelectionIndex = 0;
        BattleTechniqueTargetType targetType;
        List<BattleCharacter> availableTargets;
        int noOfPossibleSelections;
        List<BattleCharacter> selectedTargets;
        BattleTargetSelectorState currentState;
        public BattleTargetSelectorState CurrentState
        {
            get
            {
                return currentState;
            }
        }
        bool inputDelay;

        public BattleTargetSelector(string cursorTextureFileName)
        {
            cursorTexture = GameClass.LoadTextureData("General/playerTurnMenuCursor");
            controls = new GamePadController(PlayerIndex.One);
            debugControls = new KeyboardController();
            controls.ControlsActive = false;
            currentState = BattleTargetSelectorState.Inactive;
            debugControls.ControlsActive = false;
            selectedTargets = new List<BattleCharacter>();

        }
        public void BeginTargetSelection(BattleTechniqueTargetType targetType, List<BattleCharacter> availableTargets)
        {
            this.targetType = targetType;
            switch (targetType)
            {
                case BattleTechniqueTargetType.AllActiveAllies:
                case BattleTechniqueTargetType.AllEnemies:
                case BattleTechniqueTargetType.Everyone:
                case BattleTechniqueTargetType.Self:
                case BattleTechniqueTargetType.SingleActiveAlly:
                case BattleTechniqueTargetType.SingleEnemy:
                case BattleTechniqueTargetType.SingleKOdAlly:
                    noOfPossibleSelections = 1;
                    // THIS SWITCH IS HERE TO ALLOW ADDITION OF MULTIPLE SELECTION TARGETTING TYPES. 
                    break;
            }

            this.availableTargets = availableTargets;
            currentState = BattleTargetSelectorState.AwaitingInput;
            controls.ControlsActive = true;
            debugControls.ControlsActive = true;
            currentCursorSelectionIndex = 0;
            inputDelay = true;
        }
        public void Reset()
        {
            inputDelay = true;
            selectedTargets = new List<BattleCharacter>();
            availableTargets = new List<BattleCharacter>();
            currentState = BattleTargetSelectorState.Inactive;
            controls.ControlsActive = false;
            debugControls.ControlsActive = false;
        }

        public void Update(GameTime gameTime)
        {
            controls.Update(gameTime);
            debugControls.Update(gameTime);

            if (currentState != BattleTargetSelectorState.Inactive)
            {
                switch (currentState)
                {
                    case BattleTargetSelectorState.AwaitingInput:

                        if (targetType != BattleTechniqueTargetType.AllActiveAllies && targetType != BattleTechniqueTargetType.AllEnemies && targetType != BattleTechniqueTargetType.Everyone && targetType != BattleTechniqueTargetType.Self)
                        {
                            if (controls.ButtonPressed(Buttons.DPadUp, true, true) || debugControls.KeyPressed(Keys.Up, true, true))
                            {
                                GameClass.SoundManager.PlaySoundEffect("Audio/cursorMove");
                                if (currentCursorSelectionIndex > 0)
                                {
                                    currentCursorSelectionIndex--;
                                }
                                else
                                {
                                    currentCursorSelectionIndex = availableTargets.Count - 1;
                                }
                            }
                            else if (controls.ButtonPressed(Buttons.DPadDown, true, true) || debugControls.KeyPressed(Keys.Down, true, true))
                            {
                                GameClass.SoundManager.PlaySoundEffect("Audio/cursorMove");
                                if (currentCursorSelectionIndex < availableTargets.Count - 1)
                                {
                                    currentCursorSelectionIndex++;
                                }
                                else
                                {
                                    currentCursorSelectionIndex = 0;
                                }
                            }


                            if (!inputDelay)
                            {
                                if (controls.ButtonPressed(Buttons.B, false, false) || debugControls.KeyPressed(Keys.Space, false, false))
                                {
                                    selectedTargets.Add(availableTargets[currentCursorSelectionIndex]);
                                    availableTargets.Remove(availableTargets[currentCursorSelectionIndex]);
                                    currentCursorSelectionIndex = 0;
                                    if (availableTargets.Count == 0 || selectedTargets.Count == noOfPossibleSelections)
                                    {
                                        currentState = BattleTargetSelectorState.AllTargetsSelected;
                                        BattleScreen.inputDelay = true;
                                    }
                                }
                            }
                            else if(inputDelay)
                            {
                                if (controls.ButtonReleased(Buttons.B) || debugControls.KeyReleased(Keys.Space))
                                {
                                    inputDelay = false;
                                }
                            }
                        }
                        else
                        {
                           selectedTargets = availableTargets;
                           currentState = BattleTargetSelectorState.AllTargetsSelected;
                           break;
                        }
                        break;
                }

                flickerTimer += GameClass.Elapsed;
                if (flickerTimer > flickerSpeed)
                {
                    flickerTimer = 0.0f;
                    flicker = !flicker;
                }
            }
        }
        public void Draw()
        {
            if (currentState != BattleTargetSelectorState.Inactive)
            {
                if (flicker)
                {
                    foreach (BattleCharacter selection in selectedTargets)
                    {
                        GameClass.SpriteBatch.Draw(cursorTexture, selection.Position, Color.White);
                    }
                }

                if (currentState == BattleTargetSelectorState.AwaitingInput)
                {
                    GameClass.SpriteBatch.Draw(cursorTexture, availableTargets[currentCursorSelectionIndex].Position, Color.White);
                }
            }
        }
        public List<BattleCharacter> GetSelections()
        {
            return selectedTargets;
        }
    }
}
