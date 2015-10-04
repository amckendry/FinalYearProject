using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPGProject
{
    public class ThanksForPlayingScreen : Screen
    {
        Texture2D BGTexture;
        GamePadController controls;
        KeyboardController debugControls;

        public override void Initialise()
        {
            controls = new GamePadController(PlayerIndex.One);
            debugControls = new KeyboardController();
            base.Initialise();
        }
        public override void LoadContent()
        {
            BGTexture = GameClass.ContentManager.Load<Texture2D>("General/ThanksForPlayingBG");
            GameClass.SoundManager.LoadSound("Audio/thanksForPlaying");
            GameClass.SoundManager.PlaySoundEffect("Audio/thanksForPlaying");
            GameClass.CurrentGameCamera.Position = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            controls.Update(gameTime);
            debugControls.Update(gameTime);

            if(controls.ButtonPressed(Buttons.Back, false, false) || debugControls.KeyPressed(Keys.Back, false, false))
            {
                GameClass.StartExit();
            }
        }
        public override void Draw()
        {
            GameClass.SpriteBatch.Draw(BGTexture, Vector2.Zero, Color.White);
            base.Draw();
        }
    }
}
