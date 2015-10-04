using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    //this class creates a single sprite animating object.
    public class SingleSpriteAnimatingObject : DrawableGameObject
    {

        protected AnimatedSprite sprite;
        public AnimatedSprite Sprite
        {
            get
            {
                return sprite;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            sprite.Update(gameTime);
        }
        public override void Draw()
        {
            if (Visible == true)
            {
                sprite.Draw(this);
            }
        }

        public SingleSpriteAnimatingObject()
        {
        }
        public SingleSpriteAnimatingObject(String textureFileName, Vector2 position, Vector2 velocity,  int totalFrames)
        {
            this.Position = position;
            this.Velocity = velocity;
            sprite = new AnimatedSprite(totalFrames, textureFileName, 15f, SpriteAnimationType.Repeating);
            this.DrawCentre = new Vector2(sprite.FrameWidth / 2, sprite.FrameHeight / 2);
        }
        public SingleSpriteAnimatingObject(String textureFileName, Vector2 position, Vector2 velocity,  float rotation, float scale, Color tint, SpriteEffects spriteEffect, int totalFrames, float fps, SpriteAnimationType animationType)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Tint = tint;
            this.SpriteEffect = spriteEffect;

            sprite = new AnimatedSprite(totalFrames, textureFileName, fps, animationType);
            this.DrawCentre = new Vector2(sprite.FrameWidth / 2, sprite.FrameHeight / 2);
        }
    }
}
