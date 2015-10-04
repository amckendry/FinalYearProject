using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace RPGProject
{ // This class creates an instance of a controller to handle input from a 360 gamepad.
    public class GamePadController
    {
        GamePadState previousGamePadState;
        GamePadState currentGamePadState;
        PlayerIndex player;
        double buttonRepeatDelay = 100;

        GameTime gameTime;
        TimeSpan lastButtonPress = new TimeSpan(0);
        bool doRepeat = true;

        bool controlsActive = true;
        public bool ControlsActive
        {
            get
            {
                return controlsActive;
            }
            set
            {
                controlsActive = value;
            }

        }

        public bool ButtonPressed(Buttons button, bool repeat, bool delayRepeat)
        {
            bool returnValue = false;


            if (controlsActive == true)
            {
                if (repeat == true)
                {
                    if (delayRepeat == true)
                    {
                        returnValue = (currentGamePadState.IsButtonDown(button) && doRepeat);
                    }
                    else if (delayRepeat == false)
                    {
                        returnValue = currentGamePadState.IsButtonDown(button);
                    }
                }
                else if (repeat == false)
                {
                    returnValue = (currentGamePadState.IsButtonDown(button) && !previousGamePadState.IsButtonDown(button));
                }
            }
            else
            {
                returnValue = false;
            }

            if (returnValue == true)
            {
                lastButtonPress = gameTime.TotalGameTime;
            }

            return returnValue;
        }
        public bool ButtonReleased(Buttons button)
        {
            if (controlsActive == true)
            {
                return currentGamePadState.IsButtonUp(button);
            }
            else
            {
                return false;
            }
        }
        public void SetControllerVibration(float vibrationLeft, float vibrationRight)
        {
            vibrationLeft = MathHelper.Clamp(vibrationLeft + 0.03f, 0.0f, 1.0f);
            vibrationRight = MathHelper.Clamp(vibrationRight + 0.03f, 0.0f, 1.0f);
            GamePad.SetVibration(player, vibrationLeft, vibrationRight);
        }

        public Vector2 LeftThumbStick()
        {
            if (controlsActive == true)
            {
                return currentGamePadState.ThumbSticks.Left;
            }
            return Vector2.Zero;
        }
        public float LeftTrigger()
        {
            if (controlsActive == true)
            {
                return currentGamePadState.Triggers.Left;
            }
            return 0.0f;
        }
        public float RightTrigger()
        {
            if (controlsActive == true)
            {
                return currentGamePadState.Triggers.Right;
            }
            return 0.0f;
        }

        public GamePadController(PlayerIndex player)
        {
            this.player = player;
        }
        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;

            if(controlsActive == true)
            {
                previousGamePadState = currentGamePadState;
                currentGamePadState = GamePad.GetState(player);

                if ((gameTime.TotalGameTime - lastButtonPress) > TimeSpan.FromMilliseconds(buttonRepeatDelay))
                {
                    doRepeat = true;
                }
                else
                {
                   doRepeat = false;
                }
            }
        }
    }
}
