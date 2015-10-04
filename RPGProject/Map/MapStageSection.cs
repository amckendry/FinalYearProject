using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RPGProjectLibrary;

namespace RPGProject
{
    public class MapStageSection
    {
        MapStageID mapStageID;
        public MapStageID MapStageID
        {
            get
            {
                return mapStageID;
            }
        }

        MapStageSectionID currentSectionID;
        public MapStageSectionID CurrentSectionID
        {
            get
            {
                return currentSectionID;
            }
        }

        MapStageSectionID nextSectionID;
        public MapStageSectionID NextSectionID
        {
            get
            {
                return nextSectionID;
            }
        }

        MapStageSectionID previousSectionID;
        public MapStageSectionID PreviousSectionID
        {
            get
            {
                return previousSectionID;
            }
        }

        List<MapObstacleObject> sectionObstacles;
        public List<MapObstacleObject> SectionObstacles
        {
            get
            {
                return sectionObstacles;
            }
        }

        Texture2D mapBG;
        public Texture2D MapBG
        {
            get
            {
                return mapBG;
            }
        }


        List<SingleSpriteAnimatingObject> sectionBackgroundObjects;
        public List<SingleSpriteAnimatingObject> SectionBackgroundObjects
        {
            get
            {
                return sectionBackgroundObjects;
            }
        }

        List<SingleSpriteAnimatingObject> sectionForegroundObjects;
        public List<SingleSpriteAnimatingObject> SectionForegroundObjects
        {
            get
            {
                return sectionForegroundObjects;
            }
        }

        List<StageSectionDoor> sectionDoorObjects;
        public List<StageSectionDoor> SectionDoorObjects
        {
            get
            {
                return sectionDoorObjects;
            }
        }

        Rectangle stageSectionBounds;
        public Rectangle StageSectionBounds
        {
            get
            {
                return stageSectionBounds;
            }
        }

        string backgroundMusicFileName;
        public string BackgroundMusicFileName
        {
            get
            {
                return backgroundMusicFileName;
            }
        }
       
        private MapStageSection()
        {
        }

        public static MapStageSection LoadStageSection(MapStageSectionID mapStageSection)
        {
            MapStageSection newSection = new MapStageSection();
            newSection.currentSectionID = mapStageSection;

            StageSectionData data = GameClass.ContentManager.Load<StageSectionData>(@"" + mapStageSection);

            newSection.mapStageID = data.mapStageID;
            newSection.stageSectionBounds = new Rectangle(0, 0, (int)data.sectionSize.X, (int)data.sectionSize.Y);
            newSection.nextSectionID = data.nextSectionID;
            newSection.previousSectionID = data.previousSectionID;

            newSection.sectionObstacles = new List<MapObstacleObject>();
            newSection.sectionBackgroundObjects = new List<SingleSpriteAnimatingObject>();
            newSection.sectionForegroundObjects = new List<SingleSpriteAnimatingObject>();
            newSection.sectionDoorObjects = new List<StageSectionDoor>();

            newSection.mapBG = GameClass.LoadTextureData(data.mapBGTextureName);
            GlobalGameInfo.CurrentBattleBG = GameClass.LoadTextureData(data.battleBGTextureName);
            foreach (StageSectionObstacleData obstacleData in data.obstacles)
            {
                switch (obstacleData.obstacleType)
                {
                    case ObstacleType.RectanglePlatform:
                        RectanglePlatform rectanglePlatform = new RectanglePlatform(obstacleData.textureName, obstacleData.position, obstacleData.velocity, obstacleData.collisionOffset, 1, obstacleData.behindScenery);
                        newSection.sectionObstacles.Add(rectanglePlatform);
                        break;

                    case ObstacleType.RectangleWall:
                        RectangleWall rectangleWall = new RectangleWall(obstacleData.textureName, obstacleData.position, obstacleData.velocity, obstacleData.collisionOffset, 1, obstacleData.behindScenery);
                        newSection.sectionObstacles.Add(rectangleWall);
                        break;
                }
            }

            foreach (SingleSpriteObjectData backgroundObjectData in data.backgroundObjects)
            {
                SingleSpriteAnimatingObject backgroundObject = new SingleSpriteAnimatingObject(backgroundObjectData.textureFileName, backgroundObjectData.position, backgroundObjectData.velocity, backgroundObjectData.rotation, backgroundObjectData.scale, backgroundObjectData.tint, backgroundObjectData.spriteEffect, backgroundObjectData.totalFrames, backgroundObjectData.fps, backgroundObjectData.animationType);
                newSection.sectionBackgroundObjects.Add(backgroundObject);
            }

            foreach (SingleSpriteObjectData foregroundObjectData in data.foregroundObjects)
            {
                SingleSpriteAnimatingObject foregroundObject = new SingleSpriteAnimatingObject(foregroundObjectData.textureFileName, foregroundObjectData.position, foregroundObjectData.velocity, foregroundObjectData.rotation, foregroundObjectData.scale, foregroundObjectData.tint, foregroundObjectData.spriteEffect, foregroundObjectData.totalFrames, foregroundObjectData.fps, foregroundObjectData.animationType);
                newSection.sectionForegroundObjects.Add(foregroundObject);
            }

            foreach (StageSectionDoorData doorData in data.doors)
            {
                StageSectionDoor door = new StageSectionDoor();
                door.LoadContent(doorData);

                newSection.sectionDoorObjects.Add(door);
            }
            newSection.backgroundMusicFileName = data.backgroundMusicFileName;
            GameClass.SoundManager.LoadSound(data.backgroundMusicFileName);
            return newSection;

        }
    }
}
