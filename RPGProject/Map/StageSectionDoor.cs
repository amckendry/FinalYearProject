using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;


namespace RPGProject
{ // This class creates an instance of a door object for each 
    public enum DoorState
    {
        ClosedUnlocked,
        ClosedLocked,
        Opening
    }

    public class StageSectionDoor : MultipleSpriteAnimatingObject<DoorState>
    {
        MapStageSectionID destinationSectionID;
        public MapStageSectionID DestinationSectionID
        {
            get
            {
                return destinationSectionID;
            }
        }

        Vector2 playerDestinationPosition;
         public Vector2 PlayerDestinationPosition
         {
            get
            {
                return playerDestinationPosition;
            }
         }

        CollisionBox collisionBox;
        public CollisionBox CollisionBox
        {
            get
            {
                return collisionBox;
            }
        }

        DoorType doorType;
        public DoorType DoorType
        {
            get
            {
                return doorType;
            }
        }

        string openingSound;
        public string OpeningSound
        {
            get
            {
                return openingSound;
            }
        }

        List<QuestFlagChangeData> flagChangesOnEnter;
        public List<QuestFlagChangeData> FlagChangesOnEnter
        {
            get
            {
                return flagChangesOnEnter;
            }
        }

        QuestFlagData lockFlag;

        public StageSectionDoor()
        {
            Sprites = new Dictionary<DoorState, AnimatedSprite>();
        }
        public void LoadContent(StageSectionDoorData stageSectionDoorData)
        {
            this.position = stageSectionDoorData.position;
            this.destinationSectionID = stageSectionDoorData.destinationSection;
            this.playerDestinationPosition = stageSectionDoorData.destinationSectionPosition; 
            this.doorType = stageSectionDoorData.doorType;
            collisionBox = new CollisionBox(stageSectionDoorData.collisionBoundaryHeight, stageSectionDoorData.collisionBoundaryWidth, Vector2.Zero, Color.Purple);

            if (doorType == DoorType.Boundary)
            {
                Sprites.Add(DoorState.ClosedLocked, new AnimatedSprite(1, "Blank", 0));
                Sprites.Add(DoorState.ClosedUnlocked, Sprites[DoorState.ClosedLocked]);
                Sprites.Add(DoorState.Opening, Sprites[DoorState.ClosedLocked]);
            }
            else if (doorType == DoorType.ManualDoor)
            {
                Sprites.Add(DoorState.ClosedLocked, AnimatedSprite.CreateFromData(stageSectionDoorData.closedLockedSpriteData));
                Sprites.Add(DoorState.ClosedUnlocked, AnimatedSprite.CreateFromData(stageSectionDoorData.closedUnlockedSpriteData));
                Sprites.Add(DoorState.Opening, AnimatedSprite.CreateFromData(stageSectionDoorData.openingSpriteData));
                Sprites[DoorState.Opening].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
            }
            openingSound = stageSectionDoorData.openingSound;
            if (openingSound != null && openingSound != "")
            {
                GameClass.SoundManager.LoadSound(openingSound);
            }
            if (stageSectionDoorData.flagChangesOnEnter != null)
            {
                flagChangesOnEnter = stageSectionDoorData.flagChangesOnEnter;
            }
            lockFlag = new QuestFlagData();
            lockFlag = stageSectionDoorData.lockFlag;
            if (GlobalGameInfo.GetQuestFlagValue(lockFlag.flagName) == -1)
            {
                GlobalGameInfo.CurrentQuestProgress.Add(lockFlag);
            }
            else
            {
                lockFlag = GlobalGameInfo.GetFlagByName(lockFlag.flagName);
            }
            if (stageSectionDoorData.lockFlag.flagValue == 0)
            {
                CurrentAction = DoorState.ClosedUnlocked;
            }
            else if (stageSectionDoorData.lockFlag.flagValue == 1)
            {
                CurrentAction = DoorState.ClosedLocked;
            }
        }
        public override void Update(GameTime gameTime)
        {
            collisionBox.Update(position);

            foreach (QuestFlagData flag in GlobalGameInfo.CurrentQuestProgress)
            {
                if (flag.flagName == lockFlag.flagName)
                {
                    lockFlag = flag;
                }
            }

            if (lockFlag.flagValue == 0)
            {
                if (CurrentAction == DoorState.ClosedLocked)
                {
                    CurrentAction = DoorState.ClosedUnlocked;
                }
            }
            else if (lockFlag.flagValue == 1)
            {
                if (CurrentAction == DoorState.ClosedUnlocked)
                {
                    CurrentAction = DoorState.ClosedLocked;
                }
            }

            if (doorType == DoorType.ManualDoor)
            {
                base.Update(gameTime);
            }
        }
        public override void Draw()
        {
            if (doorType == DoorType.ManualDoor)
            {
                base.Draw();
            }
            collisionBox.Draw();
        }
    }
}
