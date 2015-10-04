using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace RPGProjectLibrary
{


    [Serializable]
    public struct CollisionBoxData
    {
        public int height;
        public int width;
        public Vector2 offset;
    }
    public class CollisionBoxDataReader : ContentTypeReader<CollisionBoxData>
    {
        protected override CollisionBoxData Read(ContentReader input, CollisionBoxData existingInstance)
        {
            CollisionBoxData data = new CollisionBoxData();

            data.height = input.ReadInt32();
            data.width = input.ReadInt32();
            data.offset = input.ReadVector2();

            return data;
        }
    }
}