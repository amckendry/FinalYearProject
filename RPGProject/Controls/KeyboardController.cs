using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace RPGProject
{
    public class KeyboardController
    {
        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;
        double keyRepeatDelay = 100;

        GameTime gameTime;
        TimeSpan lastKeyPress = new TimeSpan(0);
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

        public bool KeyPressed(Keys key, bool repeat, bool delayRepeat)
        {
            bool returnValue = false;


            if (controlsActive == true)
            {
                if (repeat == true)
                {
                    if (delayRepeat == true)
                    {
                        returnValue = (currentKeyboardState.IsKeyDown(key) && doRepeat);
                    }
                    else if (delayRepeat == false)
                    {
                        returnValue = currentKeyboardState.IsKeyDown(key);
                    }
                }
                else if (repeat == false)
                {
                    returnValue = (currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key));
                }
            }
            else
            {
                returnValue = false;
            }

            if (returnValue == true)
            {
                lastKeyPress = gameTime.TotalGameTime;
            }

            return returnValue;
        }
        public bool KeyReleased(Keys key)
        {
            if (controlsActive == true)
            {
                return currentKeyboardState.IsKeyUp(key);
            }
            else
            {
                return false;
            }
        }

        public KeyboardController()
        {
        }

        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;

            if(controlsActive == true)
            {
                previousKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();

                if ((gameTime.TotalGameTime - lastKeyPress) > TimeSpan.FromMilliseconds(keyRepeatDelay))
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
