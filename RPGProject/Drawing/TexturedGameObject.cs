using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGProject
{

    //this class creates a drawable object with a non-animating texture.
    public class TexturedGameObject : DrawableGameObject
    {
        protected Texture2D texture;
        public Texture2D Texture
       {
           get
           {
               return texture;
           }
       }
        public override void Draw()
        {
            Rectangle drawBounds = new Rectangle((int)Position.X - texture.Width / 2 - (int)GameClass.CurrentGameCamera.Position.X, (int)Position.Y - texture.Height / 2 - (int)GameClass.CurrentGameCamera.Position.Y, texture.Width, texture.Height);

            if (drawBounds.Intersects(GameClass.ViewPortBounds) && Visible == true)
            {
                GameClass.SpriteBatch.Draw(texture, Position, null, Tint, Rotation, DrawCentre, Scale, SpriteEffect, 0.0f);
            }
        }

        public TexturedGameObject()
        {
        }
        public TexturedGameObject(String textureFileName, Vector2 position, Vector2 velocity)
        {
            texture = GameClass.ContentManager.Load<Texture2D>(textureFileName);
            this.Position = position;
            this.Velocity = velocity;
            this.DrawCentre = new Vector2(texture.Width / 2, texture.Height / 2);

        }
        public TexturedGameObject(String textureFileName, Vector2 position, Vector2 velocity, float rotation, float scale, Color tint, SpriteEffects spriteEffect)
        {
            texture = GameClass.ContentManager.Load<Texture2D>(textureFileName);
            this.Position = position;
            this.Velocity = velocity;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Tint = tint;
            this.SpriteEffect = spriteEffect;
            this.DrawCentre = new Vector2(texture.Width / 2, texture.Height / 2);
        }
    }
}
