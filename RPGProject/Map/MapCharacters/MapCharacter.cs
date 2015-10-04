using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{//This class creates an animating object used to graphically represent a character on the Screen.

    public class MapCharacter : MultipleSpriteAnimatingObject<MapCharacterAction>
    {
        protected CharacterIdentity characterID;
        public CharacterIdentity CharacterID
        {
            get
            {
                return characterID;
            }
            set
            {
                characterID = value;
            }
        }

        CharacterFacing facing;
        public CharacterFacing Facing
        {
            get
            {
                return facing;
            }
            set
            {
                facing = value;
            }
        }

        protected Texture2D portraitTexture;
        public Texture2D PortraitTexture
        {
            get
            {
                return portraitTexture;
            }
        }

        protected float walkSpeed;
        public float WalkSpeed
        {
            get
            {
                return walkSpeed;
            }
        }

        protected float jumpStrength;
        public float JumpStrength
        {
            get
            {
                return jumpStrength;
            }
        }

        protected float airInfluence;
        public float AirInfluence
        {
            get
            {
                return airInfluence;
            }
        }

        bool inAir = true;
        public bool InAir
        {
            get
            {
                return inAir;
            }
            set
            {
                inAir = value;
            }
        }

        float landingLagDuration = 0.2f;
        float currentLandingTime = 0.0f;
        
        Dictionary<MapCharacterAction, CollisionBox> collisionBoxes;
        protected Dictionary<MapCharacterAction, CollisionBox> CollisionBoxes
        {
            get
            {
                return collisionBoxes;
            }
            set
            {
                collisionBoxes = value;
            }
        }

        protected CollisionBox currentCollisionBox;
        public CollisionBox CurrentCollisionBox
        {
            get
            {
                return currentCollisionBox;
            }
        }

        protected Dictionary<SensorType, CollisionSensor> collisionSensors;
        public Dictionary<SensorType, CollisionSensor> CollisionSensors
        {
            get
            {
                return collisionSensors;
            }
        }

        protected MapCharacter(CharacterFacing initialFacing)
        {
            Sprites = new Dictionary<MapCharacterAction, AnimatedSprite>();
            collisionSensors = new Dictionary<SensorType, CollisionSensor>();
            facing = initialFacing;
        }

        public override void Update(GameTime gameTime)
        {
            currentCollisionBox = collisionBoxes[CurrentAction];
            foreach (KeyValuePair<MapCharacterAction, CollisionBox> collisionBoxItem in collisionBoxes)
            {
                collisionBoxItem.Value.Update(Position, Facing);
            }

            foreach (KeyValuePair<SensorType, CollisionSensor> sensorItem in collisionSensors)
            {
                sensorItem.Value.SetLinkedCollisionBox(currentCollisionBox);
                sensorItem.Value.Update(Position);
            }

            switch (facing)
            {
                case CharacterFacing.Left:
                    SpriteEffect = SpriteEffects.FlipHorizontally;
                    break;

                case CharacterFacing.Right:
                    SpriteEffect = SpriteEffects.None;
                    break;
            }

            if (InAir == true)
            {
                velocity.Y += mass * currentGravityFactor;
                velocity *= (1 - (0.0015f * gameTime.ElapsedGameTime.Milliseconds)); 


                if (Velocity.Y > 0 && CurrentAction != MapCharacterAction.Death)
                {
                    CurrentAction = MapCharacterAction.Fall;
                }
            }
            else
            {
                if (velocity.X > 0.01f || velocity.X < 0.01f)
                {
                    velocity.X /= currentFrictionFactor;
                }
                else
                {
                    velocity.X = 0;
                }
            }


            if (CurrentAction == MapCharacterAction.Land)
            {
                currentLandingTime += GameClass.Elapsed;

                if (currentLandingTime > landingLagDuration)
                {
                    currentLandingTime = 0.0f;
                    CurrentAction = MapCharacterAction.Idle;
                }
                else
                {
                    currentLandingTime += GameClass.Elapsed;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw()
        {

            if (currentCollisionBox != null)
            {
                currentCollisionBox.Draw();
            }

            foreach (KeyValuePair<SensorType, CollisionSensor> sensorItem in collisionSensors)
            {
                sensorItem.Value.Draw();
            }
            base.Draw();
        }
        public static bool FallMovementContested(MapCharacter character, List<MapObstacleObject> possibleContestingObstacles)
        {
            foreach (MapObstacleObject obstacle in possibleContestingObstacles)
            {
                if(obstacle.ContestingFallMovement(character))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
