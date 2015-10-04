using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using RPGProjectLibrary;

using System.Xml.Serialization;
using System.IO;

namespace RPGProject
{
   public class MapScreen : Screen
    {
        PlayerMapCharacter player;
        public PlayerMapCharacter Player
        {
            get
            {
                return player;
            }
        }
        MapStageSection currentMapStageSection;
        public MapStageSection CurrentMapStageSection
        {
            get
            {
                return currentMapStageSection;
            }
        }

        List<MapCharacter> MapCharacters;
        List<MapItemObject> mapItemObjects;

        bool changingSection = false;
        MapStageSectionID sectionIDChangingTo;
        Vector2 playerPositionChangingTo;

        DialogManager dialogManager;
        public static bool battleTriggered = false;

        public static bool inputDelay;

        StatusUpdateBar inventoryUpdateBar;
        StatusUpdateBar questUpdateBar;

        PartyManager partyManager;
        bool questCompleteTriggered;

        public MapScreen()
        {   
        }
        public override void Initialise()
        {
            screenID = ScreenIdentity.Map;
            MapCharacters = new List<MapCharacter>();
            mapItemObjects = new List<MapItemObject>();
            dialogManager = new DialogManager();
            partyManager = new PartyManager();
            player = new PlayerMapCharacter();
            inputDelay = false;
            base.Initialise();
        }
        public override void LoadContent()
        {
            questCompleteTriggered = false;
            player.LoadContent(GlobalGameInfo.CurrentPlayerPosition, GlobalGameInfo.CurrentPlayerCharacter);
            player.Update(new GameTime());
            inventoryUpdateBar = new StatusUpdateBar(50);
            questUpdateBar = new StatusUpdateBar(100);

            dialogManager.LoadContent(this);
            partyManager.LoadContent(this);
            GameClass.SoundManager.LoadSound("Audio/jump");
            GameClass.SoundManager.LoadSound("Audio/land");
            GameClass.SoundManager.LoadSound("Audio/walkStep");
            GameClass.SoundManager.LoadSound("Audio/toBattleEffect");
            GameClass.SoundManager.LoadSound("Audio/battle");

            changeStageSection(GlobalGameInfo.CurrentMapStageSection);
        }
        public void changeStageSection(MapStageSectionID newSection)
        {
            currentMapStageSection = MapStageSection.LoadStageSection(newSection);
            ScreenManager.SetFade(ScreenFadeState.FadeFromBlack, 0.05f);

            MapCharacters.Clear();
            mapItemObjects.Clear();
            MapCharacters.Add(player);


            foreach (QuestEnemyData enemyData in GlobalGameInfo.CurrentQuestEnemies)
            {
                if (enemyData.stageSection == currentMapStageSection.CurrentSectionID)
                {
                    EnemyMapCharacter newEnemy = new EnemyMapCharacter();
                    newEnemy.LoadContent(enemyData);
                    MapCharacters.Add(newEnemy);
                }
            }

            foreach (QuestNPCData NPCData in GlobalGameInfo.CurrentQuestNPCs)
            {
                if (NPCData.stageSection == currentMapStageSection.CurrentSectionID)
                {
                    NPCMapCharacter newNPC = new NPCMapCharacter();
                    newNPC.LoadContent(NPCData);
                    MapCharacters.Add(newNPC);
                }
            }

            foreach(QuestItemData itemData in GlobalGameInfo.CurrentQuestItems)
            {
                if (itemData.stageSection == currentMapStageSection.CurrentSectionID)
                {
                    MapItemObject newItem = new MapItemObject(itemData);
                    mapItemObjects.Add(newItem);
                }
            }

            if (currentMapStageSection.BackgroundMusicFileName != GameClass.SoundManager.CurrentBackgroundMusic)
            {
                GameClass.SoundManager.StopBackgroundMusic();
                GameClass.SoundManager.SetBackgroundMusic(currentMapStageSection.BackgroundMusicFileName, true);
                GameClass.SoundManager.PlayBackgroundMusic();
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (!battleTriggered && !questCompleteTriggered && changingSection == false)
            {
                questUpdateBar.Update(gameTime);
                inventoryUpdateBar.Update(gameTime);
                dialogManager.Update(gameTime);
                partyManager.Update(gameTime);

                GameClass.CurrentGameCamera.Position = new Vector2(player.Position.X - GameClass.ViewPortBounds.Width / 2, player.Position.Y - GameClass.ViewPortBounds.Height / 2);

                if (GameClass.CurrentGameCamera.Position.X < 0 || GameClass.CurrentGameCamera.Position.X == 0)
                {
                    GameClass.CurrentGameCamera.Position = new Vector2(0, GameClass.CurrentGameCamera.Position.Y);
                }
                else if (GameClass.CurrentGameCamera.Position.X > currentMapStageSection.StageSectionBounds.Width - GameClass.CurrentGameCamera.Size.X)
                {
                    GameClass.CurrentGameCamera.Position = new Vector2(currentMapStageSection.StageSectionBounds.Width - GameClass.CurrentGameCamera.Size.X, GameClass.CurrentGameCamera.Position.Y);
                }

                if (GameClass.CurrentGameCamera.Position.Y < 0 || GameClass.CurrentGameCamera.Position.Y == 0)
                {
                    GameClass.CurrentGameCamera.Position = new Vector2(GameClass.CurrentGameCamera.Position.X, 0);
                }
                else if (GameClass.CurrentGameCamera.Position.Y > currentMapStageSection.StageSectionBounds.Height - GameClass.CurrentGameCamera.Size.Y)
                {
                    GameClass.CurrentGameCamera.Position = new Vector2(GameClass.CurrentGameCamera.Position.X, currentMapStageSection.StageSectionBounds.Height - GameClass.CurrentGameCamera.Size.Y);
                }


                if (dialogManager.IsHidden() && partyManager.IsHidden())
                {
                    foreach (StageSectionDoor door in currentMapStageSection.SectionDoorObjects)
                    {
                        door.Update(gameTime);
                    }

                    if (changingSection == false)
                    {
                        foreach (QuestDialogData dialogData in GlobalGameInfo.CurrentQuestAutoDialogs)
                        {
                            if (dialogData.flagsToTrigger != null)
                            {
                                    if (GlobalGameInfo.CheckFlagTrigger(dialogData.flagsToTrigger) == true)
                                    {
                                        dialogManager.LoadConversation(player, dialogData);
                                    }
                            }
                            else
                            {
                                dialogManager.LoadConversation(player, dialogData);
                            }
                        }

                        foreach (SingleSpriteAnimatingObject backgroundObject in currentMapStageSection.SectionBackgroundObjects)
                        {
                            backgroundObject.Update(gameTime);
                        }

                        foreach (SingleSpriteAnimatingObject foregroundObject in currentMapStageSection.SectionForegroundObjects)
                        {
                            foregroundObject.Update(gameTime);
                        }

 

                        foreach (StageSectionDoor door in currentMapStageSection.SectionDoorObjects)
                        {
                            if (player.CurrentCollisionBox.Rectangle.Intersects(door.CollisionBox.Rectangle))
                            {
                                if (door.DoorType == DoorType.Boundary && door.CurrentAction == DoorState.ClosedUnlocked)
                                {
                                    changingSection = true;
                                    sectionIDChangingTo = door.DestinationSectionID;
                                    playerPositionChangingTo = door.PlayerDestinationPosition;


                                    if (door.FlagChangesOnEnter != null)
                                    {
                                        foreach (QuestFlagChangeData flagChange in door.FlagChangesOnEnter)
                                        {
                                            GlobalGameInfo.SetQuestFlag(flagChange.flagNameToChange, flagChange.changeValue, flagChange.flagValueChangeType);
                                        }
                                    }
                                }

                                if ((player.ControlsActive && player.Controls.ButtonPressed(Buttons.DPadUp, false, false)) || (player.DebugControlsActive && player.DebugControls.KeyPressed(Keys.Up, false, false)) && (player.CurrentAction == MapCharacterAction.Idle || player.CurrentAction == MapCharacterAction.Run))
                                {
                                    if (door.DoorType == DoorType.ManualDoor && door.CurrentAction == DoorState.ClosedUnlocked)
                                    {
                                        player.ControlsActive = false;
                                        player.DebugControlsActive = false;
                                        door.CurrentAction = DoorState.Opening;
                                        if (door.OpeningSound != null)
                                        {
                                            GameClass.SoundManager.PlaySoundEffect(door.OpeningSound);
                                        }
                                        changingSection = true;
                                        sectionIDChangingTo = door.DestinationSectionID;
                                        playerPositionChangingTo = door.PlayerDestinationPosition;

                                        if (door.FlagChangesOnEnter != null)
                                        {
                                            foreach (QuestFlagChangeData flagChange in door.FlagChangesOnEnter)
                                            {
                                                GlobalGameInfo.SetQuestFlag(flagChange.flagNameToChange, flagChange.changeValue, flagChange.flagValueChangeType);
                                            }

                                        }
                                    }
                                }
                            }
                        }


                        foreach (MapObstacleObject obstacle in currentMapStageSection.SectionObstacles)
                        {
                            obstacle.Update(gameTime);
                        }


                        for (int count = 0; count < mapItemObjects.Count; count++)
                        {
                            mapItemObjects[count].Update(gameTime);
                            if (mapItemObjects[count].CurrentAction == MapItemState.Collected && mapItemObjects[count].CurrentSprite.SingleAnimationFinished)
                            {
                                mapItemObjects.Remove(mapItemObjects[count]);
                            }

                        }

                        foreach (MapCharacter character in MapCharacters)
                        {
                            character.InAir = true;
                            foreach (MapObstacleObject obstacle in currentMapStageSection.SectionObstacles)
                            {
                                if (CollisionSensor.CheckSensorCollision(character.CollisionSensors[SensorType.Bottom], obstacle.CollisionBox.Rectangle))
                                {
                                    character.InAir = false;
                                }

                                obstacle.HandleCollisions(character);
                            }


                            foreach (MapItemObject item in mapItemObjects)
                            {
                                if (item.CurrentAction == MapItemState.NotCollected)
                                {
                                    if (player.CurrentCollisionBox.Rectangle.Intersects(item.CollisionBox.Rectangle))
                                    {
                                        GameClass.SoundManager.PlaySoundEffect(item.SoundOnCollection);
                                        item.CurrentAction = MapItemState.Collected;
                                        foreach (QuestFlagChangeData flagChange in item.FlagChangesOnCollection)
                                        {
                                            GlobalGameInfo.SetQuestFlag(flagChange.flagNameToChange, flagChange.changeValue, flagChange.flagValueChangeType);
                                        }
                                        foreach (InventoryChangeData inventoryChange in item.InventoryChangesOnCollection)
                                        {
                                            GlobalGameInfo.ChangeInventory(inventoryChange.inventoryChangeType, inventoryChange.itemIDToChange, inventoryChange.QTYToChangeBy);
                                        }
                                        GlobalGameInfo.CurrentQuestItems.Remove(item.ItemData);
                                    }
                                }
                            }

   


                            if ((player.ControlsActive && player.Controls.ButtonPressed(Buttons.B, true, false)) || (player.DebugControlsActive && player.DebugControls.KeyPressed(Keys.RightShift, true, false)) && (player.CurrentAction == MapCharacterAction.Idle || player.CurrentAction == MapCharacterAction.Run))
                            {
                                if (character is NPCMapCharacter)
                                {

                                    switch (player.Facing)
                                    {
                                        case CharacterFacing.Left:
                                            if (CollisionSensor.CheckSensorCollision(player.CollisionSensors[SensorType.Left], character.CollisionSensors[SensorType.Right]))
                                            {
                                                player.Facing = CharacterFacing.Left;
                                                player.Velocity = Vector2.Zero;
                                                character.Facing = CharacterFacing.Right;
                                                player.ControlsActive = false;
                                                player.DebugControlsActive = false;
                                                dialogManager.LoadConversation(player, (NPCMapCharacter)character);
                                            }
                                            break;

                                        case CharacterFacing.Right:
                                            if (CollisionSensor.CheckSensorCollision(player.CollisionSensors[SensorType.Right], character.CollisionSensors[SensorType.Left]))
                                            {
                                                player.Facing = CharacterFacing.Right;
                                                player.Velocity = Vector2.Zero;
                                                character.Facing = CharacterFacing.Left;
                                                player.ControlsActive = false;
                                                player.DebugControlsActive = false;
                                                dialogManager.LoadConversation(player, (NPCMapCharacter)character);
                                            }
                                            break;
                                    }
                                }
                            }


                            if (character is EnemyMapCharacter)
                            {
                                if (character.CurrentAction != MapCharacterAction.Death)
                                {
                                    if (player.CurrentCollisionBox.Rectangle.Intersects(character.CurrentCollisionBox.Rectangle))
                                    {
                                        GlobalGameInfo.CurrentEnemyInBattle = (EnemyMapCharacter)character;
                                        battleTriggered = true;
                                        GameClass.SoundManager.PlaySoundEffect("Audio/toBattleEffect");
                                        GlobalGameInfo.PreviousMapCameraPosition = GameClass.CurrentGameCamera.Position;
                                        GameClass.SoundManager.StopBackgroundMusic();
                                        GameClass.SoundManager.SetBackgroundMusic("Audio/battle", true);
                                        GameClass.SoundManager.PlayBackgroundMusic();
                                        GameClass.ScreenManager.ScreenTransition(ScreenIdentity.Battle);
                                    }
                                }
                            }

                            character.Update(gameTime);
                        }
                    }

                    if (player.ControlsActive && player.Controls.ButtonPressed(Buttons.Start, false, false) || player.DebugControlsActive && player.DebugControls.KeyPressed(Keys.LeftShift, false, false))
                    {
                        player.ControlsActive = false;
                        player.DebugControlsActive = false;
                        partyManager.Show();
                    }


                    if (player.CurrentCollisionBox.Rectangle.Right > currentMapStageSection.StageSectionBounds.Right)
                    {
                        player.Position = new Vector2(player.Position.X - (player.Velocity.X + 2), player.Position.Y);
                        player.Velocity = new Vector2(0, player.Velocity.Y);
                    }

                    if (player.CurrentCollisionBox.Rectangle.Left < currentMapStageSection.StageSectionBounds.Left)
                    {
                        player.Position = new Vector2(player.Position.X + (-player.Velocity.X + 2), player.Position.Y);
                        player.Velocity = new Vector2(0, player.Velocity.Y);
                    }

                  

                    for (int count = 0; count < MapCharacters.Count; count++)
                    {
                        if (MapCharacters[count] is EnemyMapCharacter)
                        {
                            if (MapCharacters[count].CurrentAction == MapCharacterAction.Death && MapCharacters[count].CurrentSprite.SingleAnimationFinished)
                            {

                                EnemyMapCharacter tempEnemyCast = (EnemyMapCharacter)MapCharacters[count];
                                foreach (QuestFlagChangeData flagChange in tempEnemyCast.LinkedQuestEnemyData.flagChangesOnDefeat)
                                {
                                    GlobalGameInfo.SetQuestFlag(flagChange.flagNameToChange, flagChange.changeValue, flagChange.flagValueChangeType);
                                }

                                GlobalGameInfo.CurrentQuestEnemies.Remove(tempEnemyCast.LinkedQuestEnemyData);
                                MapCharacters.Remove(MapCharacters[count]);
                            }
                        }
                    }
                    base.Update(gameTime);
                }

                int requirementsCompleted = 0;
                foreach (QuestRequirementData requirement in GlobalGameInfo.QuestCompletionRequirements)
                {
                    if (GlobalGameInfo.QuestRequirementCompleted(requirement))
                    {
                        requirementsCompleted += 1;
                    }
                }

                if (requirementsCompleted == GlobalGameInfo.QuestCompletionRequirements.Count)
                {
                    //questCompleteTriggered = true;
                    //GameClass.SoundManager.StopBackgroundMusic();
                    //GameClass.ScreenManager.ScreenTransition(ScreenIdentity.ThanksForPlaying);
                }
            }

            if (changingSection == true)
            {
                player.ControlsActive = false;

                if (ScreenManager.CurrentFadeState != ScreenFadeState.FadeToBlack && ScreenManager.CurrentFadeState != ScreenFadeState.IdleBlack)
                {
                    ScreenManager.SetFade(ScreenFadeState.FadeToBlack, 0.05f);
                }

                if (ScreenManager.CurrentFadeState == ScreenFadeState.IdleBlack)
                {
                    changeStageSection(sectionIDChangingTo);
                    player.Position = playerPositionChangingTo;
                    player.Update(gameTime);
                    if (player.CurrentAction == MapCharacterAction.Run)
                    {
                        player.CurrentAction = MapCharacterAction.Idle;
                    }
                    player.ControlsActive = true;
                    player.DebugControlsActive = true;

                    changingSection = false;
                }
            }
        }
        public override void Draw()
        {
            GameClass.SpriteBatch.Draw(currentMapStageSection.MapBG, GameClass.CurrentGameCamera.Position, Color.White);

            foreach (MapObstacleObject obstacle in currentMapStageSection.SectionObstacles)
            {
                if (obstacle.BehindScenery == true)
                {
                    obstacle.Draw();
                }
            }

            foreach (SingleSpriteAnimatingObject backgroundObject in currentMapStageSection.SectionBackgroundObjects)
            {
                backgroundObject.Draw();
            }

            foreach (StageSectionDoor door in currentMapStageSection.SectionDoorObjects)
            {
                door.Draw();
            }
            foreach (MapObstacleObject obstacle in currentMapStageSection.SectionObstacles)
            {
                if (obstacle.BehindScenery == false)
                {
                    obstacle.Draw();
                }
            }

            foreach (MapItemObject item in mapItemObjects)
            {
                item.Draw();
            }

            foreach (MapCharacter character in MapCharacters)
            {
                character.Draw();
            }

            foreach (SingleSpriteAnimatingObject foregroundObject in currentMapStageSection.SectionForegroundObjects)
            {
                foregroundObject.Draw();
            }

            dialogManager.Draw();
            partyManager.Draw();
            inventoryUpdateBar.Draw();
            questUpdateBar.Draw();

            if (GameClass.DrawDebugInfo == true)
            {
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Current Quest: " + GlobalGameInfo.CurrentQuestName, new Vector2(GameClass.CurrentGameCamera.Position.X + 50, GameClass.CurrentGameCamera.Position.Y + 60), Color.White);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Player Position: " + player.Position, new Vector2(GameClass.CurrentGameCamera.Position.X + 50, GameClass.CurrentGameCamera.Position.Y + 70), Color.White);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Player In Air: " + player.InAir, new Vector2(GameClass.CurrentGameCamera.Position.X + 50, GameClass.CurrentGameCamera.Position.Y + 80), Color.White);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Player Controls Active: " + player.ControlsActive, new Vector2(GameClass.CurrentGameCamera.Position.X + 50, GameClass.CurrentGameCamera.Position.Y + 90), Color.White);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Player Debug Controls Active: " + player.DebugControlsActive, new Vector2(GameClass.CurrentGameCamera.Position.X + 50, GameClass.CurrentGameCamera.Position.Y + 100), Color.White);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Screen Manager State: " + ScreenManager.CurrentFadeState, new Vector2(GameClass.CurrentGameCamera.Position.X + 50, GameClass.CurrentGameCamera.Position.Y + 110), Color.White);
                GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Current Stage Section: " + currentMapStageSection.CurrentSectionID, new Vector2(GameClass.CurrentGameCamera.Position.X + 50, GameClass.CurrentGameCamera.Position.Y + 120), Color.White);
                if (GlobalGameInfo.CurrentEnemyInBattle != null)
                {
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Enemy In Battle: " + GlobalGameInfo.CurrentEnemyInBattle.CharacterID, new Vector2(GameClass.CurrentGameCamera.Position.X + 50, GameClass.CurrentGameCamera.Position.Y + 130), Color.White);
                }
                int drawPos = 150;
                foreach (QuestFlagData flag in GlobalGameInfo.CurrentQuestProgress)
                {
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, flag.flagName + ": " + flag.flagValue, new Vector2(GameClass.CurrentGameCamera.Position.X + 50, GameClass.CurrentGameCamera.Position.Y + drawPos), Color.WhiteSmoke);
                    drawPos += 10;
                }
                drawPos = 20;
                foreach (KeyValuePair<InventoryItemIdentity, int> inventoryItem in GlobalGameInfo.Inventory)
                {
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + inventoryItem.Key + ": " + inventoryItem.Value, new Vector2(GameClass.CurrentGameCamera.Position.X + 400, GameClass.CurrentGameCamera.Position.Y + drawPos), Color.White);
                    drawPos += 10;
                }
            }
            base.Draw();
        }
        public void DisplayInventoryUpdateBar()
        {
            inventoryUpdateBar.Show("Inventory Update!");
        }
        public void DisplayQuestUpdateBar()
        {
            questUpdateBar.Show("Quest Update!");
        }
    }
}
