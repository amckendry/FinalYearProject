using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGProject
{
    public enum StatusUpdateBarState
    {
        IdleHidden,
        Appearing,
        IdleShowing,
        Disappearing
    }
    public class StatusUpdateBar : TexturedGameObject
    {
        string updateMessage = "";
        float appearanceTimer;
        float appearanceDuration = 2.0f;
        float appearanceSpeed = 30.0f;
        float offsetX;
        float offsetY;
        StatusUpdateBarState state;

        public StatusUpdateBar(int offsetY)
            : base("General/updateBarBG", new Vector2(GameClass.CurrentGameCamera.Position.Y - 75, GameClass.CurrentGameCamera.Position.Y + offsetY), Vector2.Zero)
        {
            appearanceTimer = 0.0f;
            state = StatusUpdateBarState.IdleHidden;
            offsetX = -75;
            this.offsetY = offsetY;
        }
        public void Show(string message)
        {
            updateMessage = message;
            GameClass.SoundManager.PlaySoundEffect("Audio/menuPopUp");
            state = StatusUpdateBarState.Appearing;
        }
        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
                case StatusUpdateBarState.Appearing:
                    if (!(offsetX < 75))
                    {
                        offsetX = 75;
                        state = StatusUpdateBarState.IdleShowing;
                    }
                    else
                    {
                        offsetX += appearanceSpeed;
                    }
                    break;

                case StatusUpdateBarState.Disappearing:
                    if (!(offsetX >  -75))
                    {
                        offsetX = -75;
                        state = StatusUpdateBarState.IdleHidden;
                    }
                    else
                    {
                        offsetX -= appearanceSpeed;
                    }
                    break;
                case StatusUpdateBarState.IdleShowing:

                    if (appearanceTimer > appearanceDuration)
                    {
                        appearanceTimer = 0.0f;
                        state = StatusUpdateBarState.Disappearing;
                    }
                    else
                    {
                        appearanceTimer += GameClass.Elapsed;
                    }
                    break;
            }

            position = new Vector2(GameClass.CurrentGameCamera.Position.X + offsetX, GameClass.CurrentGameCamera.Position.Y + offsetY);
            base.Update(gameTime);
        }
        public override void Draw()
        {
            position = new Vector2(GameClass.CurrentGameCamera.Position.X + offsetX, GameClass.CurrentGameCamera.Position.Y + offsetY);
            base.Draw();
            GameClass.SpriteBatch.DrawString(GameClass.Size8Font, updateMessage, new Vector2(position.X - 72, position.Y - 10), Color.White);
        }

    }
}
