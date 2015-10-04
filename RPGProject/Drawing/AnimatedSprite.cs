using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using RPGProjectLibrary;

namespace RPGProject
{
    //this class creates an animating texture object for use with other objects.

   
    public class AnimatedSprite
    {

        int frameHeight;// the height of each individual frame.
        public int FrameHeight
        {
            get
            {
                return frameHeight;
            }
        }

        int frameWidth;// the width of each individual frame.
        public int FrameWidth
        {
            get
            {
                return frameWidth;
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

        string textureFileName;
        public string TextureFileName
        {
           get
           {
               return textureFileName;
           }
       }

        bool singleAnimationFinished;
        public bool SingleAnimationFinished
        {
            get
            {
                return singleAnimationFinished;
            }
        }

        float frameTimer; //timer used to continue frame animation at the appropriate time.

        float fps; //Frames Per Second. The rate at which the sprite will animate.
        public float FPS
        {
            get
            {
                return fps;
            }
            set
            {
                fps = value;
            }
        }

        int currentFrame; //the frame number of the current frame.
        public int CurrentFrame
        {
            get
            {
                return currentFrame;
            }
            set
            {
                currentFrame = value;
            }
        }

        int totalFrames; //the total number of frames within the sprite.
        public int TotalFrames
        {
            get
            {
                return totalFrames;
            }
        }

        SpriteAnimationType animationType; //the sprite's current animation type.
        public SpriteAnimationType AnimationType
        {
            get
            {
                return animationType;
            }
            set
            {
                animationType = value;
            }
        }

        enum SpriteAnimationDirection
        {
            Forward,
            Backward
        }//enumeration of both possible sprite animation directions.

        SpriteAnimationDirection animationDirection; //the animation direction of the sprite. Used with 'alternating' animation type to determine which direction the sprite is currently animating in.

        public void Update(GameTime gameTime)
        {
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds; //update frameTimer with current elapsed time.
            if (frameTimer > (float)(1 / fps)) //if its time to animate to the next frame.
            {
                frameTimer = 0.0f;
                switch (animationType)
                {
                    case SpriteAnimationType.Repeating: //sprite animates to the end of it's frames, and then starts over.
                        animationDirection = SpriteAnimationDirection.Forward;

                        if (!(currentFrame > totalFrames - 2))
                        {
                            currentFrame += 1;
                        }
                        else
                        {
                            currentFrame = 0;
                        }
                        break;
                    case SpriteAnimationType.AnimateOnceMaintainFirst: //sprite animates to the end of it's frames once, goes back to it's first frame, and then stops.
                        {
                            animationDirection = SpriteAnimationDirection.Forward;
                            if (!(currentFrame > totalFrames - 2))
                            {
                                currentFrame += 1;
                                singleAnimationFinished = false;
                            }
                            else
                            {
                                currentFrame = 0;
                                singleAnimationFinished = true;
                            }
                            break;
                        }
                    case SpriteAnimationType.AnimateOnceMaintainLast: //sprite animates to the end of it's frames once, and stops at the last frame.
                        {
                            animationDirection = 0;
                            if ((currentFrame < totalFrames - 1))
                            {
                                currentFrame += 1;
                                singleAnimationFinished = false;
                            }
                            else
                            {
                                currentFrame = totalFrames - 1;
                                singleAnimationFinished = true;
                            }
                            break;
                        }

                    case SpriteAnimationType.RepeatingReverse: //sprite animates backwards indefinitely.
                        {
                            animationDirection = SpriteAnimationDirection.Backward;
                            if ((currentFrame > 0))
                            {
                                currentFrame -= 1;
                            }
                            else
                            {
                                currentFrame = totalFrames - 1;
                            }
                            break;
                        }

                    case SpriteAnimationType.AlternatingReverse: //sprite animates forward until the end of it's frames, then animates back to the start, and repeats this cycle.
                        {
                            if (animationDirection == SpriteAnimationDirection.Forward)
                            {
                                if (!(currentFrame > totalFrames - 2))
                                {
                                    currentFrame += 1;
                                }
                                else
                                {
                                    currentFrame = totalFrames - 1;
                                    animationDirection = SpriteAnimationDirection.Backward;
                                }
                            }
                            else if (animationDirection == SpriteAnimationDirection.Backward)
                            {
                                if ((currentFrame > 0))
                                {
                                    currentFrame -= 1;
                                }
                                else
                                {
                                    currentFrame = 0;
                                    animationDirection = SpriteAnimationDirection.Forward;
                                }
                            }
                            break;
                        }
                }
            }
        }
        public void Draw(DrawableGameObject parentObject)//pass the object that the sprite belongs to draw it using it's properties.
        {
            GameClass.SpriteBatch.Draw(texture, new Vector2((float)Math.Round((double)parentObject.Position.X), (float)Math.Round((double)parentObject.Position.Y)), new Rectangle((currentFrame * frameWidth), 0, frameWidth, frameHeight), parentObject.Tint, parentObject.Rotation, parentObject.DrawCentre, parentObject.Scale, parentObject.SpriteEffect, 0.0f);
        }

        public AnimatedSprite(int totalFrames, string textureFileName, float fps, SpriteAnimationType animationType)
        {
            this.totalFrames = totalFrames;
            this.textureFileName = textureFileName;
            texture = GameClass.LoadTextureData(textureFileName);
            this.frameHeight = texture.Height;
            currentFrame = 0;
            frameWidth = (int)(texture.Width / totalFrames);
            this.fps = fps;
            this.animationType = animationType;
            animationDirection = SpriteAnimationDirection.Forward;
        }
        public AnimatedSprite(int totalFrames, string textureFileName, float fps)
        {
            this.totalFrames = totalFrames;
            this.textureFileName = textureFileName;
            texture = GameClass.LoadTextureData(textureFileName);
            this.frameHeight = texture.Height;
            currentFrame = 0;
            frameWidth = (int)(texture.Width / totalFrames);
            this.fps = fps;
            this.animationType = SpriteAnimationType.Repeating;
            animationDirection = SpriteAnimationDirection.Forward;
        }

        public static AnimatedSpriteData ConvertToData(AnimatedSprite animatedSprite)
        {
            AnimatedSpriteData data = new AnimatedSpriteData();

            data.textureFileName = animatedSprite.texture.Name;
            data.totalFrames = animatedSprite.totalFrames;
            data.fps = animatedSprite.fps;

            return data;
        }

        public static AnimatedSprite CreateFromData(AnimatedSpriteData data)
        {
            AnimatedSprite sprite = new AnimatedSprite(data.totalFrames, data.textureFileName, data.fps);
            return sprite;
        }
        public static AnimatedSprite CreateFromData(CharacterAnimatedSpriteData data)
        {
            AnimatedSprite sprite = new AnimatedSprite(data.totalFrames, data.textureFileName, data.fps);
            return sprite;
        }
    }
}
