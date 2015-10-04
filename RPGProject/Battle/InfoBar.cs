using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGProject
{
    public enum InfoBarFadeState
    {
        FadingIn,
        FadingOut,
        IdleFaded,
        IdleOpaque
    }

    class InfoBar : TexturedGameObject
    {
        InfoBarFadeState infoBarFadeState;
        float fadeSpeed;
        SpriteFont infoFont;
        Color fontColour;
        Vector2 textOffset;
        string infoText = "";
        public string InfoText
        {
            set
            {
                infoText = value;
                WrapText();
            }
        }
        float alpha;

        public InfoBar(string BGTextureName, Vector2 position, SpriteFont infoFont, Color fontColour, Vector2 textOffset, float fadeSpeed) : base(BGTextureName, position, Vector2.Zero)
        {
            infoBarFadeState = InfoBarFadeState.IdleFaded;
            this.position = position;
            this.infoFont = infoFont;
            this.textOffset = textOffset;
            this.fontColour = fontColour;
            this.fadeSpeed = fadeSpeed;
            alpha = 0.0f;
        }

        public void Show()
        {
            infoBarFadeState = InfoBarFadeState.FadingIn;
        }
        public void Hide()
        {
            infoBarFadeState = InfoBarFadeState.FadingOut;
        }

        public override void Update(GameTime gameTime)
        {
            switch (infoBarFadeState)
            {
                case InfoBarFadeState.FadingIn:
                    if (alpha < 1.0f)
                    {
                        alpha += fadeSpeed;
                    }
                    else
                    {
                        alpha = 1.0f;
                        infoBarFadeState = InfoBarFadeState.IdleOpaque;
                    }
                    break;

                case InfoBarFadeState.FadingOut:
                    if (alpha > 0.0f)
                    {
                        alpha -= fadeSpeed;
                    }
                    else
                    {
                        alpha = 0.0f;
                        infoBarFadeState = InfoBarFadeState.IdleFaded;
                    }
                    break;
            }

            base.Update(gameTime);
        }
        public override void Draw()
        {
           Tint = new Color(1.0f, 1.0f, 1.0f, alpha);
           base.Draw();

           fontColour = new Color(fontColour.R, fontColour.G, fontColour.B, alpha);
           if (infoText != null)
           {
               GameClass.SpriteBatch.DrawString(infoFont, infoText, new Vector2(position.X + textOffset.X, position.Y + textOffset.Y), fontColour);
           }
        }

        void WrapText()
        {
            List<string> lines = new List<string>();
            string[] words = infoText.Split(' ');
            int currentLines = 0;
            string line = "";
            string newString = "";

            foreach (string word in words)
            {
                if (GameClass.Size8Font.MeasureString(line + " " + word).X > Texture.Width)
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
        }  
    }
}
