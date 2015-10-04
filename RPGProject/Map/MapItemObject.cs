using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RPGProjectLibrary;

namespace RPGProject
{
    public enum MapItemState
    {
        NotCollected,
        Collected
    }
    public class MapItemObject : MultipleSpriteAnimatingObject<MapItemState>
    {
        MapItemState mapItemState;
        public MapItemState MapItemState
        {
            get
            {
                return mapItemState;
            }
            set
            {
                mapItemState = value;
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

        string soundOnCollection;
        public string SoundOnCollection
        {
            get
            {
                return soundOnCollection;
            }
        }

        List<QuestFlagChangeData> flagChangesOnCollection;
        public List<QuestFlagChangeData> FlagChangesOnCollection
        {
            get
            {
                return flagChangesOnCollection;
            }
        }

        List<InventoryChangeData> inventoryChangesOnCollection;
        public List<InventoryChangeData> InventoryChangesOnCollection
        {
            get
            {
                return inventoryChangesOnCollection;
            }
        }

        QuestItemData itemData;
        public QuestItemData ItemData
        {
            get
            {
                return itemData;
            }
        }

        public MapItemObject(QuestItemData questItemData)
        {
            itemData = questItemData;
            Sprites = new Dictionary<MapItemState, AnimatedSprite>();
            Sprites[MapItemState.NotCollected] = AnimatedSprite.CreateFromData(questItemData.notCollectedSpriteData);
            Sprites[MapItemState.Collected] = AnimatedSprite.CreateFromData(questItemData.collectedSpriteData);
            Sprites[MapItemState.Collected].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
            position = questItemData.position;
            collisionBox = CollisionBox.CreateFromData(questItemData.collisionBoxData);
            mapItemState = MapItemState.NotCollected;
            CurrentAction = MapItemState.NotCollected;
            soundOnCollection = questItemData.soundOnCollection;
            flagChangesOnCollection = questItemData.flagChangesOnCollection;
            inventoryChangesOnCollection = questItemData.inventoryChangesOnCollection;
            GameClass.SoundManager.LoadSound(soundOnCollection);
        }
        public override void Update(GameTime gameTime)
        {
            collisionBox.Update(position);
            base.Update(gameTime);
        }

        public override void Draw()
        {
            collisionBox.Draw();

            if (!CurrentSprite.SingleAnimationFinished)
            {
                base.Draw();
            }
        }
    }
}
