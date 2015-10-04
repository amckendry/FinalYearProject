using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace RPGProjectLibrary
{
    public enum ObstacleType
    {
        RectanglePlatform,
        RectangleWall,
    }

    [Serializable]
    public struct StageSectionObstacleData
    {
        public ObstacleType obstacleType;
        public string textureName;
        public int collisionOffset;
        public Vector2 position;
        public Vector2 velocity;
        public bool behindScenery;
    }

    public class StageSectionObstacleDataReader : ContentTypeReader<StageSectionObstacleData>
    {
        protected override StageSectionObstacleData Read(ContentReader input, StageSectionObstacleData existingInstance)
        {
            StageSectionObstacleData data = new StageSectionObstacleData();

            data.obstacleType = input.ReadObject<ObstacleType>();
            data.textureName = input.ReadString();
            data.collisionOffset = input.ReadInt32();
            data.position = input.ReadVector2();
            data.velocity = input.ReadVector2();
            data.behindScenery = input.ReadBoolean();
            return data;
        }
    }
}
