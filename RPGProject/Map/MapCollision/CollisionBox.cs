using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysicsDemos.DrawingSystem;
using RPGProjectLibrary;

namespace RPGProject
{//This class creates an object to represent the data for a given MapCharacters collision border. CollisionSensor objects will use this to position themselves correctly around the character.
    public class CollisionBox
    {
        protected LineBrush lineBrush;

        protected Rectangle rectangle;
        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
        }

        Vector2 offset;
        public Vector2 Offset
            {
                get
                {
                    return offset;
                }
            }

        public CollisionBox(int height, int width, Vector2 offset)
        {
            this.offset = offset;
            rectangle = new Rectangle(0, 0, width, height);
            lineBrush = new LineBrush(1, Color.Red);
            lineBrush.Load(GameClass.GraphicsManager.GraphicsDevice);
        }
        public CollisionBox(int height, int width, Vector2 offset, Color debugDrawColor)
        {
            this.offset = offset;
            rectangle = new Rectangle(0, 0, width, height);
            lineBrush = new LineBrush(1, debugDrawColor);
            lineBrush.Load(GameClass.GraphicsManager.GraphicsDevice);
        }

        public virtual void Update(Vector2 position)
        {
            rectangle.X = (int)(position.X - rectangle.Width / 2 + offset.X);
            rectangle.Y = (int)(position.Y - rectangle.Height / 2 + offset.Y);
        }
        public void Update(Vector2 position, CharacterFacing facing)
        {
            switch (facing)
            {
                case CharacterFacing.Left:
                    rectangle.X = (int)(position.X - rectangle.Width / 2 - offset.X);
                    rectangle.Y = (int)(position.Y - rectangle.Height / 2 + offset.Y);
                    break;

                case CharacterFacing.Right:
                    rectangle.X = (int)(position.X - rectangle.Width / 2 + offset.X);
                    rectangle.Y = (int)(position.Y - rectangle.Height / 2 + offset.Y);
                    break;
            }
        }
        public void Draw()
        {
            if (GameClass.DrawDebugInfo == true)
            {
                Vector2 topLeft = new Vector2(rectangle.Left, rectangle.Top);
                Vector2 topRight = new Vector2(rectangle.Right, rectangle.Top);
                Vector2 bottomLeft = new Vector2(rectangle.Left, rectangle.Bottom);
                Vector2 bottomRight = new Vector2(rectangle.Right, rectangle.Bottom);

                lineBrush.Draw(GameClass.SpriteBatch, topLeft, bottomLeft);
                lineBrush.Draw(GameClass.SpriteBatch, topLeft, topRight);
                lineBrush.Draw(GameClass.SpriteBatch, bottomRight, topRight);
                lineBrush.Draw(GameClass.SpriteBatch, bottomRight, bottomLeft);
            }
        }

        public static CollisionBoxData ConvertToData(CollisionBox collisionBox)
        {
            CollisionBoxData data = new CollisionBoxData();

            data.height = collisionBox.rectangle.Height;
            data.width = collisionBox.rectangle.Width;
            data.offset = collisionBox.offset;

            return data;
        }

        public static CollisionBox CreateFromData(CollisionBoxData data)
        {
            CollisionBox collisionBox = new CollisionBox(data.height, data.width, data.offset);
            return collisionBox;
        }
        public static Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
        public static Vector2 GetBottomCenter(Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }
    }
}
