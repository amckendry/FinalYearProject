using System;
using Microsoft.Xna.Framework;
using RPGProjectLibrary;

namespace RPGProject
{
    public class RectanglePlatform : MapObstacleObject
    {
        public RectanglePlatform(string textureFileName, Vector2 position, Vector2 velocity, int topCollisionOffset,  int totalFrames, bool behindScenery)
            : base(textureFileName, position, velocity, totalFrames, ObstacleType.RectanglePlatform, behindScenery)
        {
            collisionBox = new CollisionBox(Sprite.FrameHeight - topCollisionOffset, Sprite.FrameWidth, new Vector2(0, topCollisionOffset / 2));
            collisionSensors.Add(SensorType.Top, new CollisionSensor(collisionBox, SensorType.Top, 10));
        }


        public override void HandleCollisions(MapCharacter character)
        {
            if (CollisionSensor.CheckSensorCollision(character.CollisionSensors[SensorType.Bottom], collisionSensors[SensorType.Top]) && character.CurrentAction == MapCharacterAction.Fall)
            {

                character.CurrentAction = MapCharacterAction.Land;
                GameClass.SoundManager.PlaySoundEffect("Audio/land");
                character.Velocity = Vector2.Zero;
                character.InAir = false;
                character.Position = new Vector2(character.Position.X, (collisionBox.Rectangle.Top - (character.CurrentCollisionBox.Rectangle.Height / 2) - character.CurrentCollisionBox.Offset.Y + 1));
            }

            base.HandleCollisions(character);
        }
    }
}
