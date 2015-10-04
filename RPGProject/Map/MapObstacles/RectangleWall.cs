using System;
using Microsoft.Xna.Framework;
using RPGProjectLibrary;

namespace RPGProject
{
    class RectangleWall : MapObstacleObject
    {
        public RectangleWall(string textureFileName, Vector2 position, Vector2 velocity, int topCollisionOffset, int totalFrames, bool behindScenery)
            : base(textureFileName, position, velocity, totalFrames, ObstacleType.RectangleWall, behindScenery)
        {
            collisionBox = new CollisionBox(Sprite.FrameHeight - topCollisionOffset, Sprite.FrameWidth, new Vector2(0, topCollisionOffset / 2));
            collisionSensors.Add(SensorType.Top, new CollisionSensor(collisionBox, SensorType.Top, 10));
            collisionSensors.Add(SensorType.Bottom, new CollisionSensor(collisionBox, SensorType.Bottom, 10));
            collisionSensors.Add(SensorType.Left, new CollisionSensor(collisionBox, SensorType.Left, 5));
            collisionSensors.Add(SensorType.Right, new CollisionSensor(collisionBox, SensorType.Right, 5));
        }


         public override void HandleCollisions(MapCharacter character)
         {
             if (character.CurrentAction != MapCharacterAction.Death)
             {
                 if (CollisionSensor.CheckSensorCollision(character.CollisionSensors[SensorType.Bottom], collisionBox.Rectangle))
                 {
                     if (character.CurrentAction == MapCharacterAction.Fall)
                     {
                         character.InAir = false;
                         character.Velocity = velocity;
                         character.CurrentAction = MapCharacterAction.Land;
                         GameClass.SoundManager.PlaySoundEffect("Audio/land");
                         character.Position = new Vector2(character.Position.X, (collisionBox.Rectangle.Top - (character.CurrentCollisionBox.Rectangle.Height / 2) - character.CurrentCollisionBox.Offset.Y));
                     }
                 }

                 if (CollisionSensor.CheckSensorCollision(character.CollisionSensors[SensorType.Left], collisionBox.Rectangle))
                 {
                     character.Position = new Vector2(character.Position.X + character.CollisionSensors[SensorType.Left].SensorDepth, character.Position.Y);
                     character.Velocity = new Vector2(0, character.Velocity.Y);
                 }

                 if (CollisionSensor.CheckSensorCollision(character.CollisionSensors[SensorType.Right], collisionBox.Rectangle))
                 {
                     character.Position = new Vector2(character.Position.X - character.CollisionSensors[SensorType.Right].SensorDepth, character.Position.Y);
                     character.Velocity = new Vector2(0, character.Velocity.Y);
                 }

                 if (CollisionSensor.CheckSensorCollision(character.CollisionSensors[SensorType.Top], collisionBox.Rectangle))
                 {
                     character.Position = new Vector2(character.Position.X, character.Position.Y + character.CollisionSensors[SensorType.Top].SensorDepth);
                     character.CurrentAction = MapCharacterAction.Fall;
                     character.Velocity = new Vector2(character.Velocity.X, 0);
                 }
             }
             base.HandleCollisions(character);
         }
    }
}
