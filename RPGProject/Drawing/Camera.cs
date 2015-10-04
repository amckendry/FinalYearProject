using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RPGProject
{ //this class creates a camera object for the Screen class and GameClass class to use. The translationMatrix variable is used in the SpriteBatch. Draw method in the GameClass class to draw objects in the correct places in relation to the camera movement.
    public class Camera
    {
        public Matrix TransformationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0.0f)) * Matrix.CreateScale((float)(GameClass.GraphicsManager.GraphicsDevice.DisplayMode.Width) / (float)(GameClass.GraphicsManager.PreferredBackBufferWidth), (float)(GameClass.GraphicsManager.GraphicsDevice.DisplayMode.Height) / (float)(GameClass.GraphicsManager.PreferredBackBufferHeight), 1.0f);
            }
        }

        Vector2 position;
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

        Vector2 size;
        public Vector2 Size
        {
            get
            {
                return size;
            }
        }

        public Camera(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }
    }
}
