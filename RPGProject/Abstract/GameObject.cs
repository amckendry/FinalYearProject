
using Microsoft.Xna.Framework;

namespace RPGProject
{
    //This class is the base for most objects present in the game. 
    public abstract class GameObject
    {
        #region Fields
        protected Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        protected float maxVelocity = 20.0f;
        protected float maxFallSpeed = 60.0f;

        protected float mass = 0.0f;
        public float Mass
        {
            get
            {
                return mass;
            }
            set
            {
                mass = value;
            }
        }

        //Base and Current Friction and Gravity factors here in case othe robjects are created which modify these values(i.e. icy floors, anti-gravity rooms)
        protected const float baseFrictionFactor = 1.0035f;
        public float BaseFrictionFactor
        {
            get
            {
                return baseFrictionFactor;
            }
        }

        protected float currentFrictionFactor = baseFrictionFactor;
        public float CurrentFrictionFactor
        {
            get
            {
                return currentFrictionFactor;
            }
            set
            {
                currentFrictionFactor = value;
            }
        }

        protected const float baseGravityFactor = 9.8f;
        public float BaseGravityFactor
        {
            get
            {
                return baseGravityFactor;
            }
        }

        protected float currentGravityFactor = baseGravityFactor;
        public float CurrentGravityFactor
        {
            get
            {
                return currentGravityFactor;
            }
            set
            {
                currentGravityFactor = value;
            }
        }

        protected bool visible = true;
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }
        #endregion
        #region Methods
        public virtual void Update(GameTime gameTime)
        {
            position += velocity;
        }
        #endregion
    }
}
