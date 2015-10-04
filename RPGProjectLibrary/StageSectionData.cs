using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace RPGProjectLibrary
{
        public enum MapStageID
        {
            Town,
            Forest,
            KingsRoom
        }
        public enum MapStageSectionID
        {
            None,
            InsideTutorialBuilding,
            Town1,
            Town2,
            Town3,
            Forest1,
            Forest2,
            Forest3,
            KingsRoom
        }
        public enum DoorType
        {
            Boundary,
            ManualDoor
        }

        [Serializable]
        public struct StageSectionData
        {
            public MapStageID mapStageID;
            public Vector2 sectionSize;
            public string backgroundMusicFileName;
            public MapStageSectionID currentSectionID;
            public MapStageSectionID nextSectionID;
            public MapStageSectionID previousSectionID;
            public List<StageSectionObstacleData> obstacles;
            public List<SingleSpriteObjectData> backgroundObjects;
            public List<SingleSpriteObjectData> foregroundObjects;
            public List<StageSectionDoorData> doors;
            public string mapBGTextureName;
            public string battleBGTextureName;

        }
        public class StageSectionDataReader : ContentTypeReader<StageSectionData>
        {
            protected override StageSectionData Read(ContentReader input, StageSectionData existingInstance)
            {
                StageSectionData data = new StageSectionData();

                data.mapStageID = input.ReadObject<MapStageID>();
                data.sectionSize = input.ReadVector2();
                data.backgroundMusicFileName = input.ReadString();
                data.currentSectionID = input.ReadObject<MapStageSectionID>();
                data.nextSectionID = input.ReadObject<MapStageSectionID>();
                data.previousSectionID = input.ReadObject<MapStageSectionID>();
                data.obstacles = input.ReadObject<List<StageSectionObstacleData>>();
                data.backgroundObjects = input.ReadObject<List<SingleSpriteObjectData>>();
                data.foregroundObjects = input.ReadObject<List<SingleSpriteObjectData>>();
                data.doors = input.ReadObject<List<StageSectionDoorData>>();
                data.mapBGTextureName = input.ReadString();
                data.battleBGTextureName = input.ReadString();
                return data;
            }
        }

        [Serializable]
        public struct StageSectionDoorData
        {
            public Vector2 position;
            public MapStageSectionID destinationSection;
            public Vector2 destinationSectionPosition;
            public DoorType doorType;
            public int collisionBoundaryWidth;
            public int collisionBoundaryHeight;
            public AnimatedSpriteData closedUnlockedSpriteData;
            public AnimatedSpriteData closedLockedSpriteData;
            public AnimatedSpriteData openingSpriteData;
            public QuestFlagData lockFlag;
            [ContentSerializer(Optional = true)]public string openingSound;
            [ContentSerializer(Optional = true)]public List<QuestFlagChangeData> flagChangesOnEnter;
        }
        public class StageSectionDoorDataReader : ContentTypeReader<StageSectionDoorData>
        {
            protected override StageSectionDoorData Read(ContentReader input, StageSectionDoorData existingInstance)
            {
                StageSectionDoorData data = new StageSectionDoorData();
                data.position = input.ReadVector2();
                data.destinationSection = input.ReadObject<MapStageSectionID>();
                data.destinationSectionPosition = input.ReadVector2();
                data.doorType = input.ReadObject<DoorType>();
                data.collisionBoundaryWidth = input.ReadInt32();
                data.collisionBoundaryHeight = input.ReadInt32();
                data.closedUnlockedSpriteData = input.ReadObject<AnimatedSpriteData>();
                data.closedLockedSpriteData = input.ReadObject<AnimatedSpriteData>();
                data.openingSpriteData = input.ReadObject<AnimatedSpriteData>();
                data.lockFlag = input.ReadObject<QuestFlagData>();
                data.openingSound = input.ReadString();
                data.flagChangesOnEnter = input.ReadObject<List<QuestFlagChangeData>>();
                return data;
            }
        }

}
