using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    public class GlobalGameInfo
    {//This non-instance class holds global information about the state of the game, as loaded by the ScreenManager.
        public static int StatLimit
        {
            get
            {
                return 100;
            }
        }
        public static int LevelLimit
        {
            get
            {
                return 30;
            }
        }
        
        static MapStageSectionID currentMapStageSection;
        public static MapStageSectionID CurrentMapStageSection
        {
            get
            {
                return currentMapStageSection;
            }
            set
            {
                currentMapStageSection = value;
            }
        }

        static CharacterIdentity currentPlayerCharacter;
        public static CharacterIdentity CurrentPlayerCharacter
        {
            get
            {
                return currentPlayerCharacter;
            }
            set
            {
                currentPlayerCharacter = value;
            }
        }

        static Vector2 currentPlayerPosition;
        public static Vector2 CurrentPlayerPosition
        {
            get
            {
                return currentPlayerPosition;
            }
            set
            {
                currentPlayerPosition = value;
            }
        }

        static string currentQuestName;
        public static string CurrentQuestName
        {
            get
            {
                return currentQuestName;
            }
            set
            {
                currentQuestName = value;
            }
        }

        static List<QuestFlagData> currentQuestProgress;
        public static List<QuestFlagData> CurrentQuestProgress
        {
            get
            {
                return currentQuestProgress;
            }
            set
            {
                currentQuestProgress = value;
            }
        }

        static List<QuestRequirementData> questCompletionRequirements;
        public static List<QuestRequirementData> QuestCompletionRequirements
        {
            get
            {
                return questCompletionRequirements;
            }
            set
            {
                questCompletionRequirements = value;
            }
        }

        static List<QuestRequirementData> currentQuestRequirements;
        public static List<QuestRequirementData> CurrentQuestRequirements
        {
            get
            {
                return currentQuestRequirements;
            }
            set
            {
                currentQuestRequirements = value;
            }
        }

        static List<QuestDialogData> currentQuestAutoDialogs;
        public static List<QuestDialogData> CurrentQuestAutoDialogs
        {
            get
            {
                return currentQuestAutoDialogs;
            }
            set
            {
                currentQuestAutoDialogs = value;
            }
        }

        static List<QuestEnemyData> currentQuestEnemies;
        public static List<QuestEnemyData> CurrentQuestEnemies
        {
            get
            {
                return currentQuestEnemies;
            }
            set
            {
                currentQuestEnemies = value;
            }
        }

        static List<QuestNPCData> currentQuestNPCs;
        public static List<QuestNPCData> CurrentQuestNPCs
        {
            get
            {
                return currentQuestNPCs;
            }
            set
            {
                currentQuestNPCs = value;
            }
        }

        static List<QuestItemData> currentQuestItems;
        public static List<QuestItemData> CurrentQuestItems
        {
            get
            {
                return currentQuestItems;
            }
            set
            {
                currentQuestItems = value;
            }
        }

        static Texture2D currentBattleBG;
        public static Texture2D CurrentBattleBG
            {
                get
                {
                    return currentBattleBG;
                }
                set
                {
                    currentBattleBG = value;
                }
            }

        static EnemyMapCharacter currentEnemyInBattle;
        public static EnemyMapCharacter CurrentEnemyInBattle
        {
            get
            {
                return currentEnemyInBattle;
            }
            set
            {
                currentEnemyInBattle = value;
            }
        }

        static PartyCharacter[] currentPlayerParty;
        public static PartyCharacter[] CurrentPlayerParty
        {
            get
            {
                return currentPlayerParty;
            }
            set
            {
                currentPlayerParty = value;
            }
        }

        static Vector2 previousMapCameraPosition;
        public static Vector2 PreviousMapCameraPosition
        {
            get
            {
               return previousMapCameraPosition;
            }
            set
            {
                previousMapCameraPosition = value;
            }
        }

        static Dictionary<InventoryItemIdentity, int> inventory;
        public static Dictionary<InventoryItemIdentity, int> Inventory
        {
            get
            {
                return inventory;
            }
            set
            {
                inventory = value;
            }
        }

        static List<PlayerStatsData> playerStatsList;
        public static List<PlayerStatsData> PlayerStatsList
        {
            get
            {
                return playerStatsList;
            }
            set
            {
                playerStatsList = value;
            }
        }

        public static void SetQuestFlag(string flagName, int value, QuestFlagChangeType valueChangeType)
        {
            int indexOfFoundFlag = -1;
            int currentQuestRequirementsCompleted = 0;
            int newQuestRequirementsCompleted = 0;
            foreach (QuestFlagData flag in currentQuestProgress)
            {
               if (flag.flagName == flagName)
               {
                  indexOfFoundFlag = currentQuestProgress.IndexOf(flag);
               }
            }
            QuestFlagData flagToChange;

            foreach (QuestRequirementData requirement in GlobalGameInfo.CurrentQuestRequirements)
            {
                if (GlobalGameInfo.QuestRequirementCompleted(requirement))
                {
                    currentQuestRequirementsCompleted++;
                }
            }
            //if (indexOfFoundFlag == -1)
            //{
            //   flagToChange  = new QuestFlagData();
            //   flagToChange.flagName = flagName;

            //   if (valueChangeType == QuestFlagChangeType.Absolute)
            //    {
            //        flagToChange.flagValue = value;
            //    }
            //   else if (valueChangeType == QuestFlagChangeType.Increment)
            //    {
            //        flagToChange.flagValue = 0;
            //        flagToChange.flagValue += value;
            //    }

            //   GlobalGameInfo.CurrentQuestProgress.Add(flagToChange);
            //}
            //else
            if(indexOfFoundFlag != -1)
            {
                flagToChange = currentQuestProgress[indexOfFoundFlag];

                if (valueChangeType == QuestFlagChangeType.Absolute)
                {
                    flagToChange.flagValue = value;
                }
                else if (valueChangeType == QuestFlagChangeType.Increment)
                {
                    flagToChange.flagValue += value;
                }
                currentQuestProgress[indexOfFoundFlag] = flagToChange;

                foreach (QuestRequirementData requirement in GlobalGameInfo.CurrentQuestRequirements)
                {
                    if (GlobalGameInfo.QuestRequirementCompleted(requirement))
                    {
                        newQuestRequirementsCompleted++;
                    }
                }

                if (newQuestRequirementsCompleted > currentQuestRequirementsCompleted)
                {
                    GameClass.ScreenManager.MapScreen.DisplayQuestUpdateBar();
                }
            }
        }
        public static int GetQuestFlagValue(string flagName)
        {
            foreach (QuestFlagData flag in currentQuestProgress)
            {
                if (flag.flagName == flagName)
                {
                    return flag.flagValue;
                }
            }

            return -1;
        }
        public static QuestFlagData GetFlagByName(string flagName)
        {
            foreach (QuestFlagData flag in GlobalGameInfo.CurrentQuestProgress)
            {
                if (flag.flagName == flagName)
                {
                    return flag;
                }
            }
            throw new Exception("Reference to flag that doesn't exist");
        }
        public static bool CheckFlagTrigger(List<QuestFlagData> checkFlags)
        {
            int flagsTriggered = 0;

            foreach (QuestFlagData setflag in currentQuestProgress)
            {
                foreach (QuestFlagData checkFlag in checkFlags)
                {
                    if (setflag.flagName == checkFlag.flagName)
                    {
                        if (setflag.flagValue == checkFlag.flagValue)
                        {
                            flagsTriggered++;
                        }
                    }
                }
            }

            if (flagsTriggered == checkFlags.Count && checkFlags.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void ChangeInventory(InventoryChangeType inventoryChangeType, InventoryItemIdentity itemIDToChange, int QTYToChangeBy)
        {
            switch (inventoryChangeType)
            {
                case InventoryChangeType.Add:
                    if (inventory.ContainsKey(itemIDToChange))
                    {
                        inventory[itemIDToChange] += QTYToChangeBy;
                    }
                    else
                    {
                        inventory.Add(itemIDToChange, QTYToChangeBy);
                    }

                    if (inventory[itemIDToChange] > 100)
                    {
                        inventory[itemIDToChange] = 100;
                    }
                    break;

                case InventoryChangeType.Subtract:
                    if (inventory.ContainsKey(itemIDToChange))
                    {
                        inventory[itemIDToChange] -= QTYToChangeBy;
                        if (!(inventory[itemIDToChange] > 0))
                        {
                            inventory.Remove(itemIDToChange);
                        }
                    }
                    break;

            }
        }

        public static PlayerStatsData GetPlayerStatsData(CharacterIdentity characterID)
        {
            foreach (PlayerStatsData playerStatsData in playerStatsList)
            {
                if (playerStatsData.characterID == characterID)
                {
                    return playerStatsData;
                }
            }
            throw new Exception("No Player Stats Found for " + characterID.ToString() + ". Please update PlayerStatsList.xml");
        }

        public static bool QuestRequirementCompleted(QuestRequirementData requirementData)
        {
            int requirementFlagsMet = 0;

            foreach (QuestFlagData requirementFlag in requirementData.completionFlagValues)
            {
                foreach (QuestFlagData progressFlag in currentQuestProgress)
                {
                    if (progressFlag.flagName == requirementFlag.flagName)
                    {
                        if (progressFlag.flagValue == requirementFlag.flagValue)
                        {
                            requirementFlagsMet += 1;
                        }
                    }
                }
            }

            return (requirementFlagsMet == requirementData.completionFlagValues.Count);
        }
    }
}
