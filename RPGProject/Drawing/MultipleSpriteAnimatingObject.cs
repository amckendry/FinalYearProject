using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RPGProject
{
    //This class creates an animating object with a dictionary of sprites as needed. The Dictionary's items are told apart by a defined key type in it's constructor.
    //For example, a MapCharacter's sprites would use the MapCharacterAction enum as a type, so that the Map Character would have each of it's defined sprites linked to one of it's defined actions, as required.
    public abstract class MultipleSpriteAnimatingObject<T> : DrawableGameObject
    {

        Dictionary<T, AnimatedSprite> sprites;
        public Dictionary<T, AnimatedSprite> Sprites
        {
            get
            {
                return sprites;
            }
            set
            {
                sprites = value;
            }
        }

        AnimatedSprite currentSprite;
        public AnimatedSprite CurrentSprite
        {
            get
            {
                return currentSprite;
            }
        }

        T currentAction;
        public T CurrentAction
        {
            get
            {
                return currentAction;
            }

            set
            {
                lastAction = currentAction;
                currentAction = value;
                currentSprite = sprites[value];
                currentSprite.CurrentFrame = 0;
            }
        }

        T lastAction;
        public T LastAction
        {
            get
            {
                return lastAction;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            currentSprite.Update(gameTime);
        }
        public override void Draw()
        {
            if (Visible == true)
            {
                DrawCentre = new Vector2(currentSprite.FrameWidth / 2, currentSprite.FrameHeight / 2);
                currentSprite.Draw(this);
            }
        }
    }
}
