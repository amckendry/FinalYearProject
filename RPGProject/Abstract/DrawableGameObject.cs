using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGProject
{

    //this class contains variables and functionality that apply to all objects in the game that will be drawn.
    public abstract class DrawableGameObject : GameObject
    {
        #region Fields
        float rotation = 0.0f;
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        float scale = 1.0f;
        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        Color tint = Color.White;
        public Color Tint
        {
            get
            {
                return tint;
            }
            set
            {
                tint = value;
            }
        }

        Vector2 drawCentre;
        public Vector2 DrawCentre
        {
            get
            {
                return drawCentre;
            }
            set
            {
                drawCentre = value;
            }
        }

        SpriteEffects spriteEffect = SpriteEffects.None;
        public SpriteEffects SpriteEffect
        {
            get
            {
                return spriteEffect;
            }
            set
            {
                spriteEffect = value;
            }
        }
        #endregion
        #region Methods
        public abstract void Draw();
        #endregion
    }
}
