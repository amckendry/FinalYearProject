using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysicsDemos.DrawingSystem;

namespace RPGProject
{ //This class creates an instance of a sensor to handle a character's collisions with the environment.
    
    public enum SensorType
    {
        Left,
        Right,
        Top,
        Bottom
    }
    public class CollisionSensor : CollisionBox
    {
        CollisionBox linkedCollisionBox;

        SensorType sensorType;
        public SensorType SensorType
        {
            get
            {
                return sensorType;
            }
        }

        int sensorDepth;
        public int SensorDepth
        {
            get
            {
                return sensorDepth;
            }
        }

        public CollisionSensor(CollisionBox linkedCollisionBox, SensorType sensorType, int sensorDepth) : base(0, 0, Vector2.Zero, Color.Blue)
        {
            this.linkedCollisionBox = linkedCollisionBox;
            this.sensorType = sensorType;
            this.sensorDepth = sensorDepth;
            SetSensorDetails();
        }

        private void SetSensorDetails()
        {
            switch (sensorType)
            {
                case SensorType.Top:
                    rectangle.X = linkedCollisionBox.Rectangle.X;
                    rectangle.Y = linkedCollisionBox.Rectangle.Y - sensorDepth;
                    rectangle.Width = linkedCollisionBox.Rectangle.Width;
                    rectangle.Height = sensorDepth;
                    break;

                case SensorType.Bottom:
                    rectangle.X = linkedCollisionBox.Rectangle.X;
                    rectangle.Y = linkedCollisionBox.Rectangle.Y + linkedCollisionBox.Rectangle.Height;
                    rectangle.Width = linkedCollisionBox.Rectangle.Width;
                    rectangle.Height = sensorDepth;
                    break;

                case SensorType.Right:
                    rectangle.X = linkedCollisionBox.Rectangle.X + linkedCollisionBox.Rectangle.Width;
                    rectangle.Y = linkedCollisionBox.Rectangle.Y;
                    rectangle.Width = sensorDepth;
                    rectangle.Height = linkedCollisionBox.Rectangle.Height;
                    break;

                case SensorType.Left:
                    rectangle.X = linkedCollisionBox.Rectangle.X - sensorDepth;
                    rectangle.Y = linkedCollisionBox.Rectangle.Y;
                    rectangle.Width = sensorDepth;
                    rectangle.Height = linkedCollisionBox.Rectangle.Height;
                    break;
            }
        }


        public void SetLinkedCollisionBox(CollisionBox newCollisionBox)
        {
            linkedCollisionBox = newCollisionBox;
            SetSensorDetails();
        }
        public override void Update(Vector2 position)
        {
            SetSensorDetails();
        }

        public static bool CheckSensorCollision(CollisionSensor sensorToCheck, Rectangle rectangle)
        {
            if (sensorToCheck.Rectangle.Intersects(rectangle))
            {
                return true;
            }
            else return false;
        }
        public static bool CheckSensorCollision(CollisionSensor sensor1, CollisionSensor sensor2)
        {
            if (sensor1.Rectangle.Intersects(sensor2.Rectangle))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool CheckSensorCollision(CollisionSensor sensorToCheck, Dictionary<SensorType, CollisionSensor> sensors)
        {
            foreach (KeyValuePair<SensorType, CollisionSensor> item in sensors)
            {
                if (sensorToCheck.Rectangle.Intersects(item.Value.Rectangle))
                {
                    return true;
                }
            }
                return false;
        }
    }
}
