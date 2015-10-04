using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RPGProjectLibrary;

namespace RPGProject
{

    public class MapObstacleObject : SingleSpriteAnimatingObject
    {
        ObstacleType obstacleType;
        public ObstacleType ObstacleType
        {
            get
            {
                return obstacleType;
            }
        }

        protected CollisionBox collisionBox;
        public CollisionBox CollisionBox
        {
            get
            {
                return collisionBox;
            }
        }

        protected bool behindScenery = false;
        public bool BehindScenery
        {
            get
            {
                return behindScenery;
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

        protected MapObstacleObject(String textureFileName, Vector2 position, Vector2 velocity,  int totalFrames, ObstacleType obstacleType, bool behindScenery): base(textureFileName, position, velocity, totalFrames)
        {
            this.obstacleType = obstacleType;
            collisionSensors = new Dictionary<SensorType, CollisionSensor>();
            this.behindScenery = behindScenery;
        }

        public virtual void HandleCollisions(MapCharacter character)
        {
        }

        public virtual bool ContestingFallMovement(MapCharacter character)
        {
            if (CollisionSensor.CheckSensorCollision(character.CollisionSensors[SensorType.Bottom], collisionSensors[SensorType.Top]))
            {
                return true;
            }
            return false;
        }

        public static StageSectionObstacleData ConvertToData(MapObstacleObject mapObstacleObject)
        {
            StageSectionObstacleData data = new StageSectionObstacleData();
            data.obstacleType = mapObstacleObject.obstacleType;
            data.position = mapObstacleObject.Position;
            data.velocity = mapObstacleObject.Velocity;
            return data;
        }

        public override void Update(GameTime gameTime)
        {
            collisionBox.Update(position);
            foreach (KeyValuePair<SensorType, CollisionSensor> sensorItem in collisionSensors)
            {
                sensorItem.Value.Update(position);
            }
            base.Update(gameTime);
        }
        public override void Draw()
        {
            foreach (KeyValuePair<SensorType, CollisionSensor> sensorItem in collisionSensors)
            {
                sensorItem.Value.Draw();
            }
            base.Draw();
            collisionBox.Draw();
        }
    }
}
