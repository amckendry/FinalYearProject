using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    //Enum to represent the different states the battle can be in.
    public enum BattleState
    {
        AwaitingBattleStart,
        GeneralTechniqueChoice,
        PlayerTechniqueChoice,
        PlayerItemChoice,
        TargetChoice,
        TechniqueResolve,
        DamageResolve,
        TurnResolve,
        Victory,
        Loss
    }
    //Enum to represent the different states the resolution of an action can be in.
    public enum ActionResolveState
    {
        DetermineTarget,
        MoveToTarget,
        PerformTechnique,
        ReturnToOriginalPosition
    }
    //Enum to hold the state of the flashing Current Turn indicator.
    public enum AlternatingFadeState
    {
        FadingIn,
        FadingOut
    }

    class BattleScreen : Screen
    {//An extension of Screen. Manages battles that take place between a player's party and a generated enemy group.

        #region Fields

        //static textures for the battle background, HUD, and turn chart.
        static Texture2D battleBG;
        static Texture2D battleHUD;
        static Texture2D turnChartBG;

        //static textures reused by the characters for their HUD.
        static Texture2D statusBarTexture;
        static Texture2D turnChartSelection;
        public static Texture2D StatusBarTexture
        {
            get
            {
                return statusBarTexture;
            }
        }
        static Texture2D hpBarTexture;
        public static Texture2D HPBarTexture
        {
            get
            {
                return hpBarTexture;
            }
        }
        static Texture2D mpBarTexture;
        public static Texture2D MPBarTexture
        {
            get
            {
                return mpBarTexture;
            }
        }
        static Texture2D buffArrowTexture;
        public static Texture2D BuffArrowTexture
        {
            get
            {
                return buffArrowTexture;
            }
        }

        //boolean indicating if the current battle is a retry of a battle that already took place.
        static bool retryingBattle = false;
        //A record list of the enemy group that the player lost to in order to prompt a retry.
        static List<CharacterIdentity> enemyGroupLostTo = new List<CharacterIdentity>();

        //The various menus and an info bar to display helpful text for ease of action selection by the player.
        Menu playerTurnMenu;
        Menu gameOverMenu;
        CharacterTechniqueMenu playerTechniqueMenu;
        BattleInventoryMenu battleInventoryMenu;
        BattleTargetSelector battleTargetSelector;
        InfoBar infoBar;

        //States indicating the status of the battle.
        BattleState battleState;
        ActionResolveState actionResolveState;
        AlternatingFadeState turnChartSelectorFadeState;

        //int to hold what turn in the battle it is.
        int currentTurnCount;

        //int to hold how far into the current battle turn it is.
        int currentTurnIndex;

        //the draw list for the battle characters. Sorted using a BattleCharacterDrawPositionSorter instance at runtime.
        List<BattleCharacter> drawList;

        //the list of characters that have been KOd.
        List<BattleCharacter> inactiveCharacters;

        //the list of characters that are not KOd. Sorted using a BattleCharacterSpeedSorter instance to get the character turn order.
        List<BattleCharacter> activeCharacters;

        //float to modify the alpha of the turn selection indicator to allow it to flash.
        float turnChartSelectionAlpha;

        //Controller objects to handle player input.
        GamePadController controls;
        KeyboardController debugControls;

        //Debug controller to handle restarting of battle for test purposes.
        KeyboardController restartScreenControls;


        //boolean set at appropriate intervals to stop input registering instantly between two Menus when one deactivates and another activates.
        public static bool inputDelay;

        //Gets the position of the centre of the screen. Characters move to this position when a Direct technique targets multiple targets.
        Vector2 CentrePosition
        {
            get
            {
                return new Vector2(325, 295 - TurnCharacter.YDrawOffset);
            }
        }

        //the name of the technique selected by the player. The selected technique to resolve is derived from this.
        string selectedTechniqueName;

        //The selected technique to resolve.
        BattleTechnique selectedTechnique;

        List<BattleTechniqueAction> resolvedActions;
        Vector2 characterOriginalPosition;
        Vector2 techniqueMoveTarget;
        List<BattleCharacter> availableTargets;
        List<BattleCharacter> selectedTargets;
        int deathResolveCount;

        //List to hold effects that need resolved for the selected technique.
        List<BattleTechniqueEffect> activeEffects;

        //List to hold stat buffs that are actively affecting the battle. 
        List<BattleTechniqueBuff> activeBuffs;

        //List to hold effects which have been resolved for the selected technique.
        List<BattleTechniqueEffect> resolvedEffects;

        //List to hold stat buffs that have been resolved for the selected technique.
        List<BattleTechniqueBuffData> resolvedBuffs;

        //List to hold the sounds that have been resolved for the selected technique.
        List<BattleTechniqueSoundData> resolvedSounds;

        //The players inventory of items with battle effects.
        List<BattleItem> battleItemInventory;
        //Boolean indicating if the technique name is an item name. This is required to ensure that item effects are dealt with appropriately.
        bool selectedTechniqueNameIsItem;

        //Boolean indicating if the dashing sound has played when a character moves to a target for a Direct technique.
        bool dashSoundPlayed;


        //List of player status boxes to manage EXP earning when the player wins the battle.
        List<PlayerCharacterStatusBox> playerStatusBoxes;

        //Property to return the character whose turn it is.
        public BattleCharacter TurnCharacter
        {
            get
            {
                return activeCharacters[currentTurnIndex];
            }
        }
        #endregion
        #region Methods
        public override void Initialise()
        {//Initialises the BattleScreen so content can be load for a battle.
            screenID = ScreenIdentity.Battle;
            drawList = new List<BattleCharacter>();
            activeCharacters = new List<BattleCharacter>();
            inactiveCharacters = new List<BattleCharacter>();
            activeEffects = new List<BattleTechniqueEffect>();
            resolvedEffects = new List<BattleTechniqueEffect>();
            selectedTechniqueName = null;
            selectedTechnique = null;
            resolvedActions = new List<BattleTechniqueAction>();
            activeBuffs = new List<BattleTechniqueBuff>();
            resolvedBuffs = new List<BattleTechniqueBuffData>();
            resolvedSounds = new List<BattleTechniqueSoundData>();
            battleItemInventory = new List<BattleItem>();
            selectedTechniqueNameIsItem = false;

            availableTargets = new List<BattleCharacter>();
            selectedTargets = new List<BattleCharacter>();
            battleState = BattleState.AwaitingBattleStart;
            actionResolveState = ActionResolveState.DetermineTarget;
            deathResolveCount = 0;
            base.Initialise();
        }
        public override void LoadContent()
        {//Loads the content for a battle.

            //load the static and resued textures.
            battleBG = GlobalGameInfo.CurrentBattleBG;
            battleHUD = GameClass.LoadTextureData("General/BattleHUD");
            turnChartBG = GameClass.LoadTextureData("General/TurnChartBG");
            statusBarTexture = GameClass.LoadTextureData("General/StatusBar");
            hpBarTexture = GameClass.LoadTextureData("General/HPBar");
            mpBarTexture = GameClass.LoadTextureData("General/MPBar");
            buffArrowTexture = GameClass.LoadTextureData("General/buffArrow");
            turnChartSelection = GameClass.LoadTextureData("General/turnChartSelection");

            //Load in the sound effects used by the BattleScreen.
            GameClass.SoundManager.LoadSound("Audio/dash");
            GameClass.SoundManager.LoadSound("Audio/victory");
            GameClass.SoundManager.LoadSound("Audio/loss");

            //reset the games camera to zero as the BattleScreen does not use the camera.
            GameClass.CurrentGameCamera.Position = Vector2.Zero;

            //Initialise the controllers.
            controls = new GamePadController(PlayerIndex.One);
            controls.ControlsActive = false;
            debugControls = new KeyboardController();
            debugControls.ControlsActive = false;
            restartScreenControls = new KeyboardController();

            //Set up the player turn menu.
            playerTurnMenu = new Menu("Player Turn Menu", new Vector2(138, 172), 8, "General/playerTurnMenuBG", "General/playerTurnMenuCursor", Vector2.Zero, Vector2.Zero, true);
            playerTechniqueMenu = new CharacterTechniqueMenu(new Vector2(150, 172), "General/playerTurnMenuCursor", "General/playerTechniqueMenuBG", new Vector2(59, 12));
            battleInventoryMenu = new BattleInventoryMenu(new Vector2(150, 172), "General/playerTurnMenuCursor", "General/battleInventoryMenuBG", new Vector2(59, 12));
            battleTargetSelector = new BattleTargetSelector("General/playerTurnMenuCursor");

            //Set up and add the options for the player turn menu.
            MenuOption attackOption = new MenuOption("Attack", "Perform a normal attack.", Color.Black, Color.White, "General/playerTurnMenuButtonSelected", "General/playerTurnMenuButtonNotSelected");
            playerTurnMenu.AddMenuOption(attackOption, true);
            MenuOption defendOption = new MenuOption("Defend", "Defend yourself.", Color.Black, Color.White, "General/playerTurnMenuButtonSelected", "General/playerTurnMenuButtonNotSelected");
            playerTurnMenu.AddMenuOption(defendOption, false);
            MenuOption skillsOption = new MenuOption("Skills", "Use a special skill.", Color.Black, Color.White, "General/playerTurnMenuButtonSelected", "General/playerTurnMenuButtonNotSelected");
            playerTurnMenu.AddMenuOption(skillsOption, false);
            MenuOption itemsOption = new MenuOption("Items", "Use an Item.", Color.Black,  Color.White, "General/playerTurnMenuButtonSelected", "General/playerTurnMenuButtonNotSelected");
            playerTurnMenu.AddMenuOption(itemsOption, false);

            //Set up the game over menu.
            gameOverMenu = new Menu("Game Over Menu", new Vector2(GameClass.CurrentGameCamera.Position.X + GameClass.CurrentGameCamera.Size.X / 2, GameClass.CurrentGameCamera.Position.Y + GameClass.CurrentGameCamera.Size.Y / 2), 8, "General/gameOverMenuBG", "General/playerTurnMenuCursor", Vector2.Zero, new Vector2(0, 50), true);
            MenuOption retryOption = new MenuOption("Retry", "Retry this battle from the beginning.", Color.Black, Color.White, "General/playerTurnMenuButtonSelected", "General/playerTurnMenuButtonNotSelected");
            gameOverMenu.AddMenuOption(retryOption, true);
            MenuOption quitOption = new MenuOption("Quit", "Quit the game.", Color.Black, Color.White, "General/playerTurnMenuButtonSelected", "General/playerTurnMenuButtonNotSelected");
            gameOverMenu.AddMenuOption(quitOption, false);

            //Set up the info bar.
            infoBar = new InfoBar("General/InfoBarBG", new Vector2(230, 10), GameClass.Size8Font, Color.White, new Vector2(-225, -10), 0.1f);

            //For all the items in the players general inventory, check which are battle items and set them up as new BattleItem objects, loading their effect data from BattleCharacters/BattleItemList.xml into the battle inventory.
            List<BattleItemData> battleItemDataList = GameClass.ContentManager.Load<List<BattleItemData>>("BattleCharacters/BattleItemDataList");
            foreach (KeyValuePair<InventoryItemIdentity, int> inventoryItem in GlobalGameInfo.Inventory)
            {
                foreach (BattleItemData battleItemData in battleItemDataList)
                {
                    if (battleItemData.itemID == inventoryItem.Key)
                    {
                        BattleItem newBattleItem = new BattleItem();
                        newBattleItem.LoadContent(battleItemData, inventoryItem.Value);
                        battleItemInventory.Add(newBattleItem);
                    }
                }
            }

            //Initialise the status box list.
            playerStatusBoxes = new List<PlayerCharacterStatusBox>();

            //For the first three characters in the players party, load the characters sprites and techniques from <characterName>Battle.xml and load their stats from the PartyCharacter into a BattleCharacter
            //The players party takes up Battle Orders 0-2.
            int battleOrder = 0;
            int statusBoxDrawPos = 110;
            for (int count = 0; count < 3; count++)
            {
                if (GlobalGameInfo.CurrentPlayerParty[count] != null)
                {
                    PlayerBattleCharacter newPlayerCharacter = new PlayerBattleCharacter();
                    newPlayerCharacter.LoadContent(battleOrder, GlobalGameInfo.CurrentPlayerParty[count]);
                    activeCharacters.Add(newPlayerCharacter);
                    battleOrder ++;

                    //Create a new status box for each loaded character.
                    PlayerCharacterStatusBox newStatusBox = new PlayerCharacterStatusBox(new Vector2(statusBoxDrawPos, 170), PlayerCharacterStatusBoxMode.EndOfBattle);
                    newStatusBox.LoadContent(GlobalGameInfo.CurrentPlayerParty[count]);
                    playerStatusBoxes.Add(newStatusBox);
                    statusBoxDrawPos += 210;
                }
            }


            //Load the enemies.
            battleOrder = 3;
            if (retryingBattle == false)
            {//If this battle is a fresh battle....

                //load the 'mandatory' enemy (i.e. The map enemy that the player collided with) into the first enemy slot (Battle Order 3)
                EnemyBattleCharacter mandatoryEnemyCharacter = new EnemyBattleCharacter();
                mandatoryEnemyCharacter.LoadContent(battleOrder, GlobalGameInfo.CurrentEnemyInBattle.CharacterID);
                activeCharacters.Add(mandatoryEnemyCharacter);
                enemyGroupLostTo.Add(GlobalGameInfo.CurrentEnemyInBattle.CharacterID);
                battleOrder++;


                //Load in BattleCharacters/EnemyDifficultyList.xml and find the 'mandatory' enemy's difficulty rating.
                List<EnemyDifficultyData> enemyDifficultyList  = GameClass.ContentManager.Load<List<EnemyDifficultyData>>("BattleCharacters/EnemyDifficultyList");
                int currentEnemyDifficulty = -1;
                int spawnEnemy = 0;
                foreach (EnemyDifficultyData difficultyData in enemyDifficultyList)
                {
                    if (difficultyData.enemyID == GlobalGameInfo.CurrentEnemyInBattle.CharacterID)
                    {
                        currentEnemyDifficulty = difficultyData.difficultyLevel;
                    }
                }
                //Iterate through BattleCharacters/EnemyDifficultyList.xml and retrieve all enemies of a similar difficulty rating (i.e. within a range of -1 to 1 from it)
                List<CharacterIdentity> similarDifficultyEnemies = new List<CharacterIdentity>();
                foreach (EnemyDifficultyData difficultyData in enemyDifficultyList)
                {
                    if (Math.Abs(difficultyData.difficultyLevel - currentEnemyDifficulty) < 2)
                    {
                        similarDifficultyEnemies.Add(difficultyData.enemyID);
                    }
                }

                if (similarDifficultyEnemies.Count > 0)
                {//if there are more enemies of a similar difficulty....
                    for (int count = 0; count < 2; count++)
                    {//for the rest of the enemy slots (Battle Orders 4 and 5)....

                        //Decide randomly whether to spawn another enemy.
                        spawnEnemy = GameClass.Random.Next(0, 2);
                        if (spawnEnemy == 1)
                        {//if decided to spawn another enemy...
                            //Decide randomly which enemy to spawn, and load it in.
                            CharacterIdentity enemyToSpawn = similarDifficultyEnemies[GameClass.Random.Next(0, similarDifficultyEnemies.Count)];
                            EnemyBattleCharacter optionalEnemyCharacter = new EnemyBattleCharacter();
                            optionalEnemyCharacter.LoadContent(battleOrder, enemyToSpawn);
                            activeCharacters.Add(optionalEnemyCharacter);
                            enemyGroupLostTo.Add(enemyToSpawn);
                            battleOrder++;
                        }
                    }
                }
            }
            else if(retryingBattle == true)
            {//Otherwise if this battle is a retry, load the previous enemy group back in.
                foreach (CharacterIdentity enemyID in enemyGroupLostTo)
                {
                    EnemyBattleCharacter reloadedEnemyCharacter = new EnemyBattleCharacter();
                    reloadedEnemyCharacter.LoadContent(battleOrder, enemyID);
                    activeCharacters.Add(reloadedEnemyCharacter);
                    battleOrder++;
                }
                retryingBattle = false;
            }

            //Add all the loaded characters to the draw list.
            foreach (BattleCharacter character in activeCharacters)
            {
                drawList.Add(character);
            }

            //Initialise the turn chart and its indicator.
            turnChartSelectorFadeState = AlternatingFadeState.FadingOut;
            turnChartSelectionAlpha = 1.0f;
            currentTurnCount = 1;
            currentTurnIndex = 0;

            //Calculate the first turns order.
            CalculateTurnOrder();
        }
        public override void Update(GameTime gameTime)
        {
            //Update the controllers.
            controls.Update(gameTime);
            debugControls.Update(gameTime);
            restartScreenControls.Update(gameTime);

            if (ScreenManager.CurrentFadeState == ScreenFadeState.IdleFaded)
            {//If the screen isn't fading in or out...

                //Update all the Menus and the info bar.
                playerTurnMenu.Update(gameTime);
                playerTechniqueMenu.Update(gameTime);
                battleTargetSelector.Update(gameTime);
                battleInventoryMenu.Update(gameTime);
                gameOverMenu.Update(gameTime);
                infoBar.Update(gameTime);

                //Update all the player status boxes.
                foreach (PlayerCharacterStatusBox statusBox in playerStatusBoxes)
                {
                    statusBox.Update(gameTime);
                }
                //Update everyone in the draw list.
                foreach (BattleCharacter character in drawList)
                {
                    character.Update(gameTime);
                }

                switch (battleState)
                {//At what stage is this turn...?
                    #region Start of Battle
                      //The battle is only starting. All characters if introductory animations perform them, and then the first turn starts.
                    case BattleState.AwaitingBattleStart:
                        bool battleShouldStart = true;
                        foreach (BattleCharacter character in activeCharacters)
                        {
                            if (character.CurrentAction == "BattleStart")
                            {
                                battleShouldStart = false;
                            }
                        }

                        if (battleShouldStart == true)
                        {
                            battleState = BattleState.GeneralTechniqueChoice;
                        }
                        break;
                    #endregion
                    #region Beginning of Turn; Technique Selection
                    case BattleState.GeneralTechniqueChoice:

                        if (TurnCharacter.CurrentAction != "Defend")
                        {//If this turns character wasn't defending from last turn....

                            if (characterOriginalPosition != TurnCharacter.Position)
                            {//If there is a discrepency between the characters current position and where it should be, correct it.
                                characterOriginalPosition = TurnCharacter.Position;
                            }


                            if (TurnCharacter is PlayerBattleCharacter)
                            {//If it is a player characters turn...

                                switch (playerTurnMenu.MenuState)
                                {//What state is the general menu in?
                                    case MenuState.IdleFaded:
                                        //Its hidden; Show it and the info bar.
                                        GameClass.SoundManager.PlaySoundEffect("Audio/menuPopUp");
                                        playerTurnMenu.Show();
                                        infoBar.Show();
                                        infoBar.InfoText = playerTurnMenu.GetSelectionDescription();
                                        break;

                                    case MenuState.IdleAwaitingInput:
                                        //Its waiting for player input; update the info bar to show information for what the player is currently selecting.
                                        infoBar.InfoText = playerTurnMenu.GetSelectionDescription();
                                        break;

                                    case MenuState.IdleValueSelected:
                                        //Its received input from the player to perform a technique.

                                        //get the technique name from the Menu.
                                        selectedTechniqueName = playerTurnMenu.GetSelection();


                                        if (selectedTechniqueName == "Skills")
                                        {//If the player selected 'Skills'...

                                            if (TurnCharacter.Techniques.Count > 2)
                                            {//If this player character has more than 'Attack' and 'Defend' defined for it....

                                                //Deactivate the general menu, load the characters techniques into the technique menu and change the battle state.
                                                playerTurnMenu.HalfHide();
                                                playerTechniqueMenu.LoadTechniques(TurnCharacter);
                                                battleState = BattleState.PlayerTechniqueChoice;
                                            }
                                            else
                                            {//Otherwise this character has no 'Skills'; continue listening for different player input.
                                                playerTurnMenu.ListenForNewSelection();
                                            }
                                        }
                                        else if (selectedTechniqueName == "Items")
                                        {//If the player selected 'Items'...

                                            int items = 0;
                                            foreach (BattleItem item in battleItemInventory)
                                            {

                                                if (item.QTY > 0)
                                                {
                                                    items++;
                                                }
                                            }

                                            if (items > 0)
                                            {//If the player has any items...

                                                //Deactivate the general menu, load the players items into the Battle Inventory menu and change the battle state.
                                                playerTurnMenu.HalfHide();
                                                battleInventoryMenu.LoadBattleItems(battleItemInventory);
                                                battleState = BattleState.PlayerItemChoice;
                                            }
                                            else
                                            {//Otherwise player has no items; continue listening for different player input.
                                                playerTurnMenu.ListenForNewSelection();
                                            }
                                        }
                                        else
                                        {//Otherwise, player selected 'Attack' or 'Defend'; Deactivate general menu; Load the technique data for the selection and advance to target selection.
                                            playerTurnMenu.HalfHide();
                                            selectedTechnique = getTechniqueFromAction(selectedTechniqueName);
                                            battleState = BattleState.TargetChoice;
                                        }
                                        break;
                                }
                            }
                            else if (TurnCharacter is EnemyBattleCharacter)
                            {//Otherwise, character is an enemy; Determine action and targets from AI method and advance to resolving the selection.

                                AIActionSelection();
                                battleState = BattleState.TechniqueResolve;
                            }
                        }
                        else
                        {//Otherwise character was defending last turn; set character back to idle.
                            TurnCharacter.CurrentSprite.AnimationType = SpriteAnimationType.RepeatingReverse;
                            if (TurnCharacter.CurrentSprite.CurrentFrame < 1)
                            {
                                TurnCharacter.CurrentSprite.AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
                                TurnCharacter.CurrentAction = "Idle";
                            }
                        }
                        break;
                    #endregion
                    #region Player selected 'Skills'; Technique choice
                    case BattleState.PlayerTechniqueChoice:
                        //Player is selecting a skill to use.
                        if (controls.ButtonPressed(Buttons.A, false, false) || debugControls.KeyPressed(Keys.Back, false, false))
                        {//Player chose not to use a skill and cancel; Hide Technique Menu, Show General Menu, Return state to general technique selection.
                            GameClass.SoundManager.PlaySoundEffect("Audio/menuCancel");
                            controls.ControlsActive = false;
                            debugControls.ControlsActive = false;
                            playerTechniqueMenu.Hide();
                            playerTurnMenu.Show();
                            battleState = BattleState.GeneralTechniqueChoice;
                        }

                        switch (playerTechniqueMenu.MenuState)
                        {//What state is the Skills Menu in?
                            case MenuState.IdleFaded:
                                //Its hidden; show it and set the info bar to display info for it.
                                playerTechniqueMenu.Show();
                                infoBar.InfoText = playerTechniqueMenu.GetSelectionDescription();
                                controls.ControlsActive = true;
                                debugControls.ControlsActive = true;
                                break;

                            case MenuState.IdleAwaitingInput:
                                //It's listening for player input; set info bar to display information about players current selection.
                                infoBar.InfoText = playerTechniqueMenu.GetSelectionDescription();
                                break;

                            case MenuState.IdleValueSelected:
                                //Player selected a skill to use; Get the technique name for the skill, identify possible targets.
                                selectedTechniqueName = playerTechniqueMenu.GetSelection();
                                selectedTechnique = getTechniqueFromAction(selectedTechniqueName);
                                int possibleTargetCount = getAvailableTargets(selectedTechnique.techniqueTargeting).Count;

                                if (selectedTechnique.techniqueCost > TurnCharacter.CurrentMP || possibleTargetCount == 0)
                                {//Character hasn't enough MP to perform the skill or theres no-one to target with it; Reset selection and listen for new input.
                                    ResetTurnParameters();
                                    playerTechniqueMenu.ListenForNewSelection();
                                }
                                else
                                {//Otherwise Skill is usable; Deactivate technique Menu and advance to target selection.
                                    playerTechniqueMenu.HalfHide();
                                    battleState = BattleState.TargetChoice;
                                }
                                break;
                        }
                        break;
                    #endregion
                    #region Player selected 'Items'; Item choice
                    case BattleState.PlayerItemChoice:
                    //Player is selected an Item to use.
                        if (controls.ButtonPressed(Buttons.A, false, false) || debugControls.KeyPressed(Keys.Back, false, false))
                        {//Player decided not to use an item and cancel; Hide Item Menu, Show General Menu, and return to general technique selection.
                            GameClass.SoundManager.PlaySoundEffect("Audio/menuCancel");
                            controls.ControlsActive = false;
                            debugControls.ControlsActive = false;
                            battleInventoryMenu.Hide();
                            playerTurnMenu.Show();
                            battleState = BattleState.GeneralTechniqueChoice;
                            selectedTechniqueNameIsItem = false;
                        }

                        switch (battleInventoryMenu.MenuState)
                        {//what state is the Item Menu in?
                            case MenuState.IdleFaded:
                                //Its hidden; Show it and set the info bar to display information about it.
                                battleInventoryMenu.Show();
                                infoBar.InfoText = battleInventoryMenu.GetSelectionDescription();
                                controls.ControlsActive = true;
                                debugControls.ControlsActive = true;
                                break;

                            case MenuState.IdleAwaitingInput:
                                //It's listening for player input; set info bar to display information about players current selection. 
                                infoBar.InfoText = battleInventoryMenu.GetSelectionDescription();
                                break;

                            case MenuState.IdleValueSelected:
                                //Player selected an item to use; Get the technique name for the item, identify possible targets.
                                selectedTechniqueName = battleInventoryMenu.GetSelection();
                                selectedTechnique = getTechniqueFromAction(selectedTechniqueName);

                                int possibleTargetCount = getAvailableTargets(selectedTechnique.techniqueTargeting).Count;

                                if (possibleTargetCount == 0)
                                {//Theres no-one to use the item on; Reset selection and listen for new input.
                                    ResetTurnParameters();
                                    battleInventoryMenu.ListenForNewSelection();
                                }
                                else
                                {//Otherwise item is usable; Deactivate item Menu and advance to target selection.
                                    battleInventoryMenu.HalfHide();
                                    battleState = BattleState.TargetChoice;
                                    selectedTechniqueNameIsItem = true;
                                }

                                break;
                        }
                        break;
                    #endregion
                    #region Technique selected; Target for Technique Selection
                    case BattleState.TargetChoice:
                        //Player is selecting targets for their selected technique.
                        switch (battleTargetSelector.CurrentState)
                        {//What state is the battle target selector in?
                            case BattleTargetSelectorState.Inactive:
                                //Its not activated; Load the possible targets into it and activate it.
                                availableTargets = getAvailableTargets(selectedTechnique.techniqueTargeting);
                                battleTargetSelector.BeginTargetSelection(selectedTechnique.techniqueTargeting, availableTargets);
                                controls.ControlsActive = true;
                                debugControls.ControlsActive = true;
                                break;

                            case BattleTargetSelectorState.AwaitingInput:
                            case BattleTargetSelectorState.AllTargetsSelected:
                                //Its listening for the player to make a selection or to confirm their selection...

                                if (controls.ButtonPressed(Buttons.A, false, false) || debugControls.KeyPressed(Keys.Back, false, false))
                                {//Player decided to cancel target selection and head back; Deactivate Target Selector.
                                    GameClass.SoundManager.PlaySoundEffect("Audio/menuCancel");
                                    battleTargetSelector.Reset();
                                    selectedTechniqueNameIsItem = false;

                                    if (selectedTechniqueName == "Attack" || selectedTechniqueName == "Defend")
                                    {//Selected technique was General; show General Menu.
                                        playerTurnMenu.Show();
                                        battleState = BattleState.GeneralTechniqueChoice;
                                    }
                                    else
                                    {//Otherwise selected technique was not General...

                                        if (playerTechniqueMenu.MenuState == MenuState.IdleHalfFaded)
                                        {//Selected technique was a character skill; Show Technique Menu and return to skill selection.
                                            playerTechniqueMenu.Show();
                                            battleState = BattleState.PlayerTechniqueChoice;
                                        }
                                        else if (battleInventoryMenu.MenuState == MenuState.IdleHalfFaded)
                                        {//Selected technique was an item; Show Battle Inventory Menu and return to item selection.
                                            battleInventoryMenu.Show();
                                            battleState = BattleState.PlayerItemChoice;
                                        }
                                    }
                                }

                                if (battleTargetSelector.CurrentState == BattleTargetSelectorState.AllTargetsSelected)
                                {//Player made a target selection...

                                    if (!inputDelay)
                                    {//Player input was not carried over from last menu(i.e. Button was pressed again rather than held.)

                                        if (controls.ButtonPressed(Buttons.B, false, false) || debugControls.KeyPressed(Keys.Space, false, false))
                                        {//Player confirmed target selection; Hide all menus and info bar, advance to technique resolution.
                                            selectedTargets = battleTargetSelector.GetSelections();
                                            battleTargetSelector.Reset();
                                            playerTurnMenu.Hide();
                                            infoBar.Hide();
                                            playerTechniqueMenu.Hide();
                                            battleInventoryMenu.Hide();
                                            battleState = BattleState.TechniqueResolve;

                                            //Take characters MP away equal to the selected techniques cost.
                                            TurnCharacter.ChangeMP(selectedTechnique.techniqueCost, BattleTechniqueActionType.Damage);
                                        }
                                    }
                                    else if (inputDelay)
                                    {//Button is being held down.
                                        if (controls.ButtonReleased(Buttons.B) || debugControls.KeyReleased(Keys.Space))
                                        {//Player released button.
                                            inputDelay = false;
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    #endregion
                    #region Target and Technique Confirmed; Technique Resolution
                    case BattleState.TechniqueResolve:
                        //Technique and targets are selected; selected technique is being resolved.
                        switch (actionResolveState)
                        {//What state of resolution is the selected technique?
                            #region Determine Target position
                            case ActionResolveState.DetermineTarget:
                                //It is being determined whether and where to move the character.
                                switch (selectedTechnique.techniqueRange)
                                {//What is the selected techniques range?

                                    case BattleTechniqueRange.Direct:
                                        //It is a Direct technique.

                                        switch (selectedTechnique.techniqueTargeting)
                                        {//What form of targeting does the selected technique have?

                                            case BattleTechniqueTargetType.SingleEnemy:
                                                //It targets a single enemy; calculate movement target as position 40 pixels in front of enemy and advance to character movement resolution.
                                                if (TurnCharacter is PlayerBattleCharacter)
                                                {
                                                    techniqueMoveTarget = new Vector2(selectedTargets[0].Position.X - 40, selectedTargets[0].Position.Y + selectedTargets[0].YDrawOffset - TurnCharacter.YDrawOffset);
                                                }
                                                else if (TurnCharacter is EnemyBattleCharacter)
                                                {
                                                    techniqueMoveTarget = new Vector2(selectedTargets[0].Position.X + 40, selectedTargets[0].Position.Y + selectedTargets[0].YDrawOffset - TurnCharacter.YDrawOffset);
                                                }
                                                actionResolveState = ActionResolveState.MoveToTarget;
                                                break;

                                            case BattleTechniqueTargetType.Self:
                                            case BattleTechniqueTargetType.SingleActiveAlly:
                                            case BattleTechniqueTargetType.SingleKOdAlly:
                                                //It targets an ally; skip movement resolution and go straight to performing the technique.
                                                actionResolveState = ActionResolveState.PerformTechnique;
                                                break;

                                            case BattleTechniqueTargetType.Everyone:
                                            case BattleTechniqueTargetType.AllActiveAllies:
                                            case BattleTechniqueTargetType.AllEnemies:
                                                //It targets more than one target; set movement target to he centre of the battlefield and advance to character movement resolution.
                                                techniqueMoveTarget = CentrePosition;
                                                actionResolveState = ActionResolveState.MoveToTarget;
                                                break;
                                        }
                                        break;

                                    case BattleTechniqueRange.Ranged:
                                        //It is a Ranged technique; skip movement resolution and go straight to performing the technique.
                                        actionResolveState = ActionResolveState.PerformTechnique;
                                        break;
                                }
                                break;
                            #endregion
                            #region Resolution of movement to target
                                //Character is moving before performing the selected technique.
                            case ActionResolveState.MoveToTarget:

                                if (dashSoundPlayed == false)
                                {//The dash sound hasn't played; play the dash sound.
                                    GameClass.SoundManager.PlaySoundEffect("Audio/dash");
                                    dashSoundPlayed = true;
                                }

                                if (TurnCharacter.CurrentAction != "Run")
                                {//The character isn't running; make it start running.
                                    TurnCharacter.CurrentAction = "Run";
                                }
                                //set the velocity to the appropriate direction and magnitude, and apply it to the character while they are not at the target position.
                                Vector2 velocity = techniqueMoveTarget - TurnCharacter.Position;
                                velocity.Normalize();

                                if (velocity.X > 0)
                                {
                                    if (TurnCharacter.Position.X < techniqueMoveTarget.X)
                                    {
                                        TurnCharacter.Position = new Vector2(TurnCharacter.Position.X + (velocity.X * TurnCharacter.RunSpeed), TurnCharacter.Position.Y);
                                    }
                                    else
                                    {
                                        TurnCharacter.Position = new Vector2(techniqueMoveTarget.X, TurnCharacter.Position.Y);
                                    }
                                }
                                else if (velocity.X < 0)
                                {
                                    if (TurnCharacter.Position.X > techniqueMoveTarget.X)
                                    {
                                        TurnCharacter.Position = new Vector2(TurnCharacter.Position.X + (velocity.X * TurnCharacter.RunSpeed), TurnCharacter.Position.Y);
                                    }
                                    else
                                    {
                                        TurnCharacter.Position = new Vector2(techniqueMoveTarget.X, TurnCharacter.Position.Y);
                                    }
                                }

                                if (velocity.Y > 0)
                                {
                                    if (TurnCharacter.Position.Y < techniqueMoveTarget.Y)
                                    {
                                        TurnCharacter.Position = new Vector2(TurnCharacter.Position.X, TurnCharacter.Position.Y + (velocity.Y * TurnCharacter.RunSpeed));
                                    }
                                    else
                                    {
                                        TurnCharacter.Position = new Vector2(TurnCharacter.Position.X, techniqueMoveTarget.Y);
                                    }
                                }
                                else if (velocity.Y < 0)
                                {
                                    if (TurnCharacter.Position.Y > techniqueMoveTarget.Y)
                                    {
                                        TurnCharacter.Position = new Vector2(TurnCharacter.Position.X, TurnCharacter.Position.Y + (velocity.Y * TurnCharacter.RunSpeed));
                                    }
                                    else
                                    {
                                        TurnCharacter.Position = new Vector2(TurnCharacter.Position.X, techniqueMoveTarget.Y);
                                    }
                                }

                                if (Math.Abs(TurnCharacter.Position.X - techniqueMoveTarget.X) < 5.0f && Math.Abs(TurnCharacter.Position.Y - techniqueMoveTarget.Y) < 5.0f)
                                {//character is close enough to the target position; advance to performing the technique.
                                    actionResolveState = ActionResolveState.PerformTechnique;
                                }
                                break;
                            #endregion
                            #region Character at Target Location; Performing of Technique and Resolution of Actions and Effects
                            case ActionResolveState.PerformTechnique:

                                if (selectedTechniqueNameIsItem)
                                {//Selected technique is an item....

                                    if (TurnCharacter.CurrentAction != "Item")
                                    {//character isn't performing its 'Item' animation; make it perform its 'Item' animation.
                                        TurnCharacter.CurrentAction = "Item";
                                    }
                                }
                                else
                                {//Otherwise, seelcted technique isn't an item...
                                    if (TurnCharacter.CurrentAction != selectedTechniqueName)
                                    {//character isn't performing the animation for the selected technique; make it perform the technique animation.
                                        TurnCharacter.CurrentAction = selectedTechniqueName;
                                    }
                                }

                                if (selectedTechniqueName != "Defend")
                                {//selected technique is 'Defend'...

                                    foreach (BattleTechniqueBuffData buffData in selectedTechnique.buffs)
                                    {//For every stat buff in the selected tedhnique's data...
                                        if (TurnCharacter.CurrentSprite.CurrentFrame == buffData.buffTime || TurnCharacter.CurrentSprite.CurrentFrame > buffData.buffTime)
                                        {//if it's time to resolve the stat buff...

                                            if(resolvedBuffs.IndexOf(buffData) == -1)
                                            {//if it hasn't already been resolved...

                                                foreach (BattleCharacter target in selectedTargets)
                                                {//for every chosen target...

                                                    //Assume the target isn't already buffed in the same way.
                                                    bool matchingBuffExists = false;
                                                    foreach (BattleTechniqueBuff buff in activeBuffs)
                                                    {//for every stat buff thats active...

                                                        if (buff.Target == target && buff.BuffType == buffData.buffType)
                                                        {//if the active buff has the same target and type as the one we're resolving...

                                                            //The target is already buffed in the same way.
                                                            matchingBuffExists = true;

                                                            //Remove the active buff from the target and apply the new one.
                                                            buff.RemoveStatChange();
                                                            buff.LoadContent(target, buffData);
                                                            buff.ApplyStatChange();

                                                            //mark the new buff as resolved.
                                                            resolvedBuffs.Add(buffData);
                                                        }
                                                    }

                                                    if (matchingBuffExists == false)
                                                    {//if the target isn't already buffed in the same way, apply the new buff and amark it as resolved. 
                                                        BattleTechniqueBuff newBuff = new BattleTechniqueBuff();
                                                        newBuff.LoadContent(target, buffData);
                                                        newBuff.ApplyStatChange();
                                                        activeBuffs.Add(newBuff);
                                                        resolvedBuffs.Add(buffData);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    foreach (BattleTechniqueAction action in selectedTechnique.actions)
                                    {//for every damage action in the selected technique's data...
                                        if (TurnCharacter.CurrentSprite.CurrentFrame == action.ActionTime || TurnCharacter.CurrentSprite.CurrentFrame > action.ActionTime)
                                        {//if its time to resolve the action...
                                            if (resolvedActions.IndexOf(action) == -1)
                                            {//if it hasn't already been resolved, resolve it and mark it as resolved.
                                                ResolveDamageAction(action);
                                                resolvedActions.Add(action);
                                            }
                                        }
                                    }

                                    foreach (BattleTechniqueSoundData sound in selectedTechnique.sounds)
                                    {//for every sound effect in the selected technique's data...
                                        if (TurnCharacter.CurrentSprite.CurrentFrame == sound.soundTime || TurnCharacter.CurrentSprite.CurrentFrame > sound.soundTime)
                                        {//if its time to resolve the sound...
                                            if (resolvedSounds.IndexOf(sound) == -1)
                                            {//if it hasn't already been resolved, play it and mark it as resolved.
                                                GameClass.SoundManager.PlaySoundEffect(sound.soundFileName);
                                                resolvedSounds.Add(sound);
                                            }
                                        }
                                    }

                                    foreach (BattleTechniqueEffect activeEffect in activeEffects)
                                    {//For every effect animation object that is active...

                                        //Resolution code for Damage Actions, Stat Buffs and Sound Effect for this effect(Identical in structure to previous technique resolution code)
                                        foreach (BattleTechniqueAction effectAction in activeEffect.EffectActions)
                                        {

                                            if (activeEffect.CurrentTimer == effectAction.ActionTime || activeEffect.CurrentTimer > effectAction.ActionTime)
                                            {
                                                if (resolvedActions.IndexOf(effectAction) == -1)
                                                {
                                                    ResolveDamageAction(effectAction);
                                                    resolvedActions.Add(effectAction);
                                                }
                                            }
                                        }

                                        foreach (BattleTechniqueBuffData effectbuffData in activeEffect.EffectBuffs)
                                        {
                                            if (activeEffect.CurrentTimer == effectbuffData.buffTime || activeEffect.CurrentTimer > effectbuffData.buffTime)
                                            {
                                                if (resolvedBuffs.IndexOf(effectbuffData) == -1)
                                                {
                                                    foreach (BattleCharacter target in selectedTargets)
                                                    {
                                                        bool matchingBuffExists = false;
                                                        foreach (BattleTechniqueBuff buff in activeBuffs)
                                                        {
                                                            if (buff.Target == target && buff.BuffType == effectbuffData.buffType)
                                                            {
                                                                matchingBuffExists = true;
                                                                buff.RemoveStatChange();
                                                                buff.LoadContent(target, effectbuffData);
                                                                buff.ApplyStatChange();
                                                                resolvedBuffs.Add(effectbuffData);
                                                            }
                                                        }

                                                        if (matchingBuffExists == false)
                                                        {
                                                            BattleTechniqueBuff newBuff = new BattleTechniqueBuff();
                                                            newBuff.LoadContent(target, effectbuffData);
                                                            newBuff.ApplyStatChange();
                                                            activeBuffs.Add(newBuff);
                                                            resolvedBuffs.Add(effectbuffData);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        foreach (BattleTechniqueSoundData sound in activeEffect.EffectSounds)
                                        {
                                            if (activeEffect.CurrentTimer == sound.soundTime || activeEffect.CurrentTimer > sound.soundTime)
                                            {
                                                if (resolvedSounds.IndexOf(sound) == -1)
                                                {
                                                    GameClass.SoundManager.PlaySoundEffect(sound.soundFileName);
                                                    resolvedSounds.Add(sound);
                                                }
                                            }
                                        }

                                        //end of identical resolution code.
                                    }

                                    if (TurnCharacter.CurrentSprite.CurrentFrame > TurnCharacter.CurrentSprite.TotalFrames - 2)
                                    {//if the character has finished the technique animation...

                                        if (TurnCharacter.Position != characterOriginalPosition)
                                        {//if the character had to move to perform the technique, change the state to return movement resolution.
                                            actionResolveState = ActionResolveState.ReturnToOriginalPosition;
                                        }
                                        else
                                        {//otherwise, the character didn't have to move...
                                            if (activeEffects.Count == 0)
                                            {//All active effects have been resolved, return character to idle animation, change the state to damage effect resolution.
                                                TurnCharacter.CurrentAction = "Idle";
                                                battleState = BattleState.DamageResolve;
                                            }
                                        }
                                    }

                                    foreach (BattleTechniqueEffect effect in selectedTechnique.effects)
                                    {//for every effect animation object in selected technique data...

                                        if (resolvedEffects.IndexOf(effect) == -1)
                                        {//if the effect hasn't been resolved.

                                            if (TurnCharacter.CurrentSprite.CurrentFrame == effect.StartTime || TurnCharacter.CurrentSprite.CurrentFrame > effect.StartTime)
                                            {//if it's time to resolve the effect...

                                                //initialise and activate the effect.
                                                effect.Initialise(TurnCharacter.Position);
                                                effect.EffectState = BattleEffectState.Active;
                                                if (effect.EffectType == EffectType.ProjectileTargeted)
                                                {//if the effect is a targeted projectile...

                                                        //Set the direction and magnitude of the velocity of the projectile effect.
                                                        Vector2 targetVelocity = new Vector2(selectedTargets[0].Position.X - TurnCharacter.Position.X, selectedTargets[0].Position.Y - TurnCharacter.Position.Y);
                                                        targetVelocity.Normalize();
                                                        effect.Velocity = targetVelocity;
                                                        effect.Velocity *= effect.Speed;

                                                        if (selectedTargets.Count > 1)
                                                        {//if the projectile effect is supposed to hit multiple targets...
                                                            for (int count = 1; count < selectedTargets.Count; count++)
                                                            {//for every extra target, create a copy of the projectile effect  and set it in the same way as if they were the lone target.
                                                                BattleTechniqueEffect effectCopy = new BattleTechniqueEffect();
                                                                effectCopy.LoadContent(effect);
                                                                Vector2 copyVelocity = new Vector2(selectedTargets[count].Position.X - TurnCharacter.Position.X, selectedTargets[count].Position.Y - TurnCharacter.Position.Y);
                                                                copyVelocity.Normalize();
                                                                effectCopy.Velocity = copyVelocity;
                                                                effectCopy.Velocity *= effectCopy.Speed;
                                                                activeEffects.Add(effectCopy);  
                                                            }
                                                        }
                                                }
                                                else if (effect.EffectType == EffectType.ProjectileIndirect)
                                                {//effect is an indirect projectile, set it to fire forward into the enemy.
                                                    if (TurnCharacter is PlayerBattleCharacter)
                                                    {
                                                        effect.Velocity = new Vector2(effect.Speed, 0);
                                                    }
                                                    else if (TurnCharacter is EnemyBattleCharacter)
                                                    {
                                                        effect.Velocity = new Vector2(-effect.Speed, 0);
                                                    }
                                                }
                                                else if (effect.EffectType == EffectType.SpellTargeted)
                                                {//effect is a targeted spell effect...

                                                        //set the effect to appear on the target.
                                                        effect.Position = new Vector2(selectedTargets[0].Position.X + effect.RelativePosition.X, selectedTargets[0].Position.Y + effect.RelativePosition.Y);

                                                        if (selectedTargets.Count > 1)
                                                        {//if the spell is supposed to affect multiple targets...
                                                            for (int count = 1; count < selectedTargets.Count; count++)
                                                            {//for every extra target, create a copy of the spell effect ands set as if they were the lone target.
                                                                BattleTechniqueEffect effectCopy = new BattleTechniqueEffect();
                                                                effectCopy.LoadContent(effect);
                                                                effect.Position = new Vector2(selectedTargets[count].Position.X + effect.RelativePosition.X, selectedTargets[count].Position.Y + effect.RelativePosition.Y);
                                                                activeEffects.Add(effectCopy);
                                                            }
                                                        }
                                                }

                                                //activate the effect and mark it as resolved.
                                                activeEffects.Add(effect);
                                                resolvedEffects.Add(effect);
                                            }
                                        }
                                    }
                                }
                                else
                                {//Otherwise, character is defending, ensure they stop animating at the last frame of the animtion and advance straight to resolving the turn.
                                    TurnCharacter.CurrentSprite.AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;
                                    battleState = BattleState.TurnResolve;
                                }
                                break;
                            #endregion
                            #region Technique completed; Resolution of movement back to characters original position
                                //selected Direct technique is completely resolved; character must return to their original position.
                            case ActionResolveState.ReturnToOriginalPosition:
                                if (TurnCharacter is PlayerBattleCharacter)
                                {
                                    if (TurnCharacter.CurrentAction != "Run")
                                    {
                                        TurnCharacter.CurrentAction = "Run";
                                        TurnCharacter.Facing = CharacterFacing.Left;
                                    }

                                    if (TurnCharacter.Position.X > characterOriginalPosition.X || TurnCharacter.Position.Y > characterOriginalPosition.Y)
                                    {
                                        Vector2 velocity2 = characterOriginalPosition - TurnCharacter.Position;
                                        velocity2.Normalize();
                                        TurnCharacter.Position += velocity2 * TurnCharacter.RunSpeed;
                                    }
                                    else
                                    {
                                        TurnCharacter.Position = characterOriginalPosition;
                                        TurnCharacter.CurrentAction = "Idle";
                                        TurnCharacter.Facing = CharacterFacing.Right;
                                        battleState = BattleState.DamageResolve;
                                    }
                                }
                                else if (TurnCharacter is EnemyBattleCharacter)
                                {
                                    if (TurnCharacter.CurrentAction != "Run")
                                    {
                                        TurnCharacter.CurrentAction = "Run";
                                        TurnCharacter.Facing = CharacterFacing.Right;
                                    }

                                    if (TurnCharacter.Position.X < characterOriginalPosition.X || TurnCharacter.Position.Y < characterOriginalPosition.Y)
                                    {
                                        Vector2 velocity2 = characterOriginalPosition - TurnCharacter.Position;
                                        velocity2.Normalize();
                                        TurnCharacter.Position += velocity2 * TurnCharacter.RunSpeed;
                                    }
                                    else
                                    {
                                        TurnCharacter.Position = characterOriginalPosition;
                                        TurnCharacter.CurrentAction = "Idle";
                                        TurnCharacter.Facing = CharacterFacing.Left;
                                        battleState = BattleState.DamageResolve;
                                    }
                                }
                                break;
                        }
                        break;
                            #endregion
                    #endregion
                    #region Technique resolution completed; Resolution of KOd characters etc
                        //technique is resolved completely; characters are in correct positions; resolve damage effects...
                    case BattleState.DamageResolve:

                        int deathsToResolve = 0;
                        foreach (BattleCharacter character in activeCharacters)
                        {//for every active character...
                            if (character.CurrentAction == "Death")
                            {//if they died, add one to the death resolve count.
                                deathsToResolve++;
                            }
                        }

                        for (int count = 0; count < activeCharacters.Count; count++)
                        {//for every active character...

                            if (activeCharacters[count].CurrentAction == "Death" && activeCharacters[count].CurrentSprite.CurrentFrame > activeCharacters[count].CurrentSprite.TotalFrames - 2)
                            {//if they died and their animation is complete...

                                //add them to the inactive characters list and shift the turn chart if hadn't had their turn yet.
                                inactiveCharacters.Add(activeCharacters[count]);
                                if (activeCharacters.IndexOf(activeCharacters[count]) < currentTurnIndex)
                                {
                                    currentTurnIndex--;
                                }

                                //remove all the buffs they had on them.
                                for (int count2 = 0; count2 < activeBuffs.Count; count2++)
                                {
                                    if (activeBuffs[count2].Target == activeCharacters[count])
                                    {
                                        activeBuffs[count2].RemoveStatChange();
                                        activeBuffs.Remove(activeBuffs[count2]);
                                    }
                                }

                                //remove them from the active characters list.
                                activeCharacters.Remove(activeCharacters[count]);
                                deathResolveCount++;
                            }
                        }


                        if (deathResolveCount == deathsToResolve && TurnCharacter.CurrentAction == "Idle" && activeEffects.Count == 0)
                        {//if everything is resolved this far...

                            for (int count = 0; count < battleItemInventory.Count; count++)
                            {//for every item the player has...
                                if (battleItemInventory[count].ItemID.ToString() == selectedTechniqueName)
                                {//if the technique that was used was that item, remove one from its QTY.
                                    battleItemInventory[count].QTY--;
                                    break;
                                }
                            }
                            //advance to resolving the turn.
                            battleState = BattleState.TurnResolve;
                        }

                        break;
                    #endregion
                    #region Resolution of next turn
                    case BattleState.TurnResolve:
                    //everything else is resolved; the next turn must be resolved.

                        //determine if a victory or loss was achieved as a result of this turn.
                        bool triggerVictory = true;
                        bool triggerLoss = true;
                        foreach (BattleCharacter character in activeCharacters)
                        {
                            if (character is PlayerBattleCharacter)
                            {
                                triggerLoss = false;
                            }
                            if (character is EnemyBattleCharacter)
                            {
                                triggerVictory = false;
                            }
                        }

                        if (triggerVictory == true && triggerLoss == true)
                        {//int the case of a draw; the player loses by default.
                            triggerVictory = false;
                        }

                        if (triggerVictory == true)
                        {//if victory was achieved...

                            //play the victory jingle.
                            GameClass.SoundManager.StopBackgroundMusic();
                            GameClass.SoundManager.SetBackgroundMusic("Audio/victory", false);
                            GameClass.SoundManager.PlayBackgroundMusic();

                            //determine and add EXP to character statuses.
                            int EXPEarned = 0;
                            foreach(BattleCharacter character in inactiveCharacters)
                            {
                                if(character is EnemyBattleCharacter)
                                {
                                    EnemyBattleCharacter enemy = (EnemyBattleCharacter)character;
                                    EXPEarned += enemy.EXPWorth;
                                }
                            }
                            //show character statuses.
                            foreach (PlayerCharacterStatusBox statusBox in playerStatusBoxes)
                            {
                                statusBox.Show();
                                statusBox.AddEXP(EXPEarned);
                            }
                            //advance to victory state.
                            battleState = BattleState.Victory;

                        }
                        else if (triggerLoss == true)
                        {//if loss was achieved...
                            //play loss jingle.
                            GameClass.SoundManager.StopBackgroundMusic();
                            GameClass.SoundManager.SetBackgroundMusic("Audio/loss", false);
                            GameClass.SoundManager.PlayBackgroundMusic();

                            //advance to loss state.
                            battleState = BattleState.Loss;
                        }
                        else
                        {//if neither victory or loss was achieved...

                            if (currentTurnIndex < activeCharacters.Count - 1)
                            {//if there are still character turns in this battle turn to go, move onto the next character.
                                currentTurnIndex++;
                            }
                            else
                            {//Otherwise, this battle turn is over...

                                //resolve if buffs should run out and be removed, recalculate the turn order, and start a new battle turn.
                                currentTurnIndex = 0;
                                currentTurnCount += 1;
                                for (int count = 0; count < activeBuffs.Count; count++)
                                {
                                    activeBuffs[count].Duration--;
                                    if (activeBuffs[count].Duration == 0)
                                    {
                                        activeBuffs[count].RemoveStatChange();
                                        activeBuffs.Remove(activeBuffs[count]);
                                    }
                                }
                                CalculateTurnOrder();
                            }

                            //reset the turn, and begin another one.
                            ResetTurnParameters();
                            battleState = BattleState.GeneralTechniqueChoice;

                        }
                        break;
                    #endregion
                    #region Victory
                    case BattleState.Victory:
                        //player has won.

                        //set all character to perform their victory animations.
                        foreach (BattleCharacter character in activeCharacters)
                        {
                            if (character is PlayerBattleCharacter)
                            {
                                if (character.CurrentAction != "Victory")
                                {
                                    character.CurrentAction = "Victory";
                                }
                            }
                        }

                        //determine if EXP has all been added.
                        int statusBoxesFinishedAddingEXP = 0;
                        foreach (PlayerCharacterStatusBox statusBox in playerStatusBoxes)
                        {
                            if (statusBox.State == PlayerCharacterStatusBoxState.AwaitingInput)
                            {
                                statusBoxesFinishedAddingEXP += 1;
                            }
                        }

                        //if EXP has all been added and player hits confirm button, refresh inventory, and return to the map screen.
                        if (statusBoxesFinishedAddingEXP == playerStatusBoxes.Count)
                        {
                            if (controls.ButtonPressed(Buttons.B, false, false) || debugControls.KeyPressed(Keys.Space, false, false))
                            {
                                    foreach(BattleItem item in battleItemInventory)
                                    {
                                        if (item.QTY > 0)
                                        {
                                            GlobalGameInfo.Inventory[item.ItemID] = item.QTY;
                                        }
                                        else
                                        {
                                            GlobalGameInfo.Inventory.Remove(item.ItemID);
                                        }
                                    }


                                GameClass.ScreenManager.ScreenTransition(ScreenIdentity.Map);
                            }
                        }
                        break;
                    #endregion
                    #region Loss
                    case BattleState.Loss:
                        switch (gameOverMenu.MenuState)
                        {
                            case MenuState.IdleFaded:
                                gameOverMenu.Show();
                                infoBar.Show();
                                infoBar.InfoText = gameOverMenu.GetSelectionDescription();
                                break;

                            case MenuState.IdleAwaitingInput:
                                infoBar.InfoText = gameOverMenu.GetSelectionDescription();
                                break;

                            case MenuState.IdleValueSelected:
                                string selectedGameOverAction = gameOverMenu.GetSelection();
                                if (selectedGameOverAction == "Retry")
                                {
                                    gameOverMenu.Hide();
                                    GameClass.SoundManager.StopBackgroundMusic();
                                    GameClass.SoundManager.SetBackgroundMusic("Audio/battle", true);
                                    GameClass.SoundManager.PlayBackgroundMusic();
                                    retryingBattle = true;
                                    activeCharacters.Clear();
                                    GameClass.ScreenManager.ScreenTransition(ScreenIdentity.Battle);
                                }
                                else if (selectedGameOverAction == "Quit")
                                {
                                    GameClass.StartExit();
                                }
                                break;
                        }
                        break;
                    #endregion
                }

                #region Alternate flashing turn chart selection
                //set the alpha of the turn chart selection indicator to alternate between 0 and 1 linearly.
                switch (turnChartSelectorFadeState)
                {
                    case AlternatingFadeState.FadingIn:
                        if (turnChartSelectionAlpha > 0.0f)
                        {
                            turnChartSelectionAlpha -= 0.1f;
                        }
                        else
                        {
                            turnChartSelectionAlpha = 0.0f;
                            turnChartSelectorFadeState = AlternatingFadeState.FadingOut;
                        }
                        break;

                    case AlternatingFadeState.FadingOut:
                        if (turnChartSelectionAlpha < 1.0f)
                        {
                            turnChartSelectionAlpha += 0.1f;
                        }
                        else
                        {
                            turnChartSelectionAlpha = 1.0f;
                            turnChartSelectorFadeState = AlternatingFadeState.FadingIn;
                        }
                        break;
                #endregion
                }

                #region Resolve Technique Effect Animations
                for (int count = 0; count < activeEffects.Count; count++)
                {
                    if (activeEffects[count].EffectState == BattleEffectState.Inactive)
                    {
                        activeEffects.Remove(activeEffects[count]);
                    }
                    else
                    {
                        activeEffects[count].Update(gameTime);
                    }
                }
                #endregion
                foreach (BattleTechniqueBuff buff in activeBuffs)
                {
                    buff.Update(gameTime);
                }

                if (restartScreenControls.KeyPressed(Keys.A, false, false))
                {
                    GameClass.SoundManager.StopBackgroundMusic();
                    GameClass.SoundManager.SetBackgroundMusic("Audio/battle", true);
                    GameClass.SoundManager.PlayBackgroundMusic();
                    GameClass.ScreenManager.ScreenTransition(ScreenIdentity.Battle);
                }
                if (restartScreenControls.KeyPressed(Keys.V, false, false))
                {
                    GameClass.SoundManager.StopBackgroundMusic();
                    GameClass.SoundManager.SetBackgroundMusic("Audio/victory", false);
                    GameClass.SoundManager.PlayBackgroundMusic();

                    foreach (PlayerCharacterStatusBox statusBox in playerStatusBoxes)
                    {
                        statusBox.Show();
                    }
                    battleState = BattleState.Victory;
                }
            }
            base.Update(gameTime);
        }
        public override void Draw()
        {
            GameClass.SpriteBatch.Draw(battleBG, Vector2.Zero, Color.White);
            GameClass.SpriteBatch.Draw(battleHUD, new Vector2(0, 320), Color.White);
            GameClass.SpriteBatch.Draw(turnChartBG, new Vector2(562, 4), Color.White);
            GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "Turn: " + currentTurnCount, new Vector2(573, 4), Color.White);

            int drawPos = 20;
            drawList.Sort(new BattleCharacterDrawPositionSorter());
            foreach (BattleCharacter character in activeCharacters)
            {
                GameClass.SpriteBatch.Draw(character.BattlePortrait, new Vector2(571, drawPos), Color.White);
                drawPos += 12;
            }


            foreach (BattleCharacter character in drawList)
            {
                character.Draw();
            }

            foreach (BattleTechniqueBuff buff in activeBuffs)
            {
                buff.Draw();
            }

            foreach (BattleTechniqueEffect effect in activeEffects)
            {      
                effect.Draw();
            }

            playerTurnMenu.Draw();
            playerTechniqueMenu.Draw();
            battleInventoryMenu.Draw();
            battleTargetSelector.Draw();
            infoBar.Draw();

            if (currentTurnIndex > -1)
            {
                GameClass.SpriteBatch.Draw(turnChartSelection, new Vector2(571, 20 + (12 * currentTurnIndex)), new Color(1.0f, 1.0f, 1.0f, turnChartSelectionAlpha));
            }

            if (GameClass.DrawDebugInfo == true)
            {
                drawPos = 20;
                foreach (BattleItem battleItem in battleItemInventory)
                {
                    GameClass.SpriteBatch.DrawString(GameClass.Size8Font, "" + battleItem.ItemID + ": " + battleItem.QTY, new Vector2(GameClass.CurrentGameCamera.Position.X + 400, GameClass.CurrentGameCamera.Position.Y + drawPos), Color.White);
                    drawPos += 10;
                }
            }

            gameOverMenu.Draw();
            foreach (PlayerCharacterStatusBox statusBox in playerStatusBoxes)
            {
                statusBox.Draw();
            }
            base.Draw();
        }

        void CalculateTurnOrder()
        {//sorts the active characters list using a BattleCharacterSpeedSorter.
            activeCharacters.Sort(new BattleCharacterSpeedSorter());
        }
        void ResetTurnParameters()
        {//resets all the paramters set during a character turn.
            selectedTechniqueName = null;
            selectedTechnique = null;
            resolvedActions = new List<BattleTechniqueAction>();
            resolvedEffects = new List<BattleTechniqueEffect>();
            resolvedBuffs = new List<BattleTechniqueBuffData>();
            resolvedSounds = new List<BattleTechniqueSoundData>();
            techniqueMoveTarget = Vector2.Zero;
            availableTargets = new List<BattleCharacter>();
            selectedTargets = new List<BattleCharacter>();
            actionResolveState = ActionResolveState.DetermineTarget;
            selectedTechniqueNameIsItem = false;
            deathResolveCount = 0;
            dashSoundPlayed = false;
        }
        BattleTechnique getTechniqueFromAction(string action)
        {
            BattleTechnique returnTechnique = null;

            if (battleState != BattleState.PlayerItemChoice)
            {
                foreach (BattleTechnique battleTechnique in TurnCharacter.Techniques)
                {
                    if (battleTechnique.techniqueName == action)
                    {
                        if (returnTechnique == null)
                        {
                            returnTechnique = battleTechnique;
                        }
                        else
                        {
                            throw new Exception("Duplicate names exist");
                        }
                    }
                }
            }
            else
            {
                foreach (BattleItem battleItem in battleItemInventory)
                {
                    if (battleItem.ItemID.ToString() == action)
                    {
                        returnTechnique = BattleTechnique.GenerateItemTechnique(TurnCharacter, battleItem);
                        break;
                    }
                }
            }

            if (returnTechnique == null)
            {
                throw new Exception("No technique found for this action.");
            }
            else
            {
                return returnTechnique;
            }
        }
        List<BattleCharacter> getAvailableTargets(BattleTechniqueTargetType targetType)
        {
            List<BattleCharacter> targets = new List<BattleCharacter>();

            if(targetType == BattleTechniqueTargetType.Everyone)
            {
                targets = activeCharacters;
            }
            else if(targetType == BattleTechniqueTargetType.Self)
            {
                targets.Add(TurnCharacter);
            }
            else if (TurnCharacter is PlayerBattleCharacter)
            {
                switch (targetType)
                {
                    case BattleTechniqueTargetType.AllActiveAllies:
                    case BattleTechniqueTargetType.SingleActiveAlly:
                        foreach (BattleCharacter character in activeCharacters)
                        {
                            if (character is PlayerBattleCharacter)
                            {
                                targets.Add(character);
                            }
                        }
                        break;

                    case BattleTechniqueTargetType.AllEnemies:
                    case BattleTechniqueTargetType.SingleEnemy:
                        foreach (BattleCharacter character in activeCharacters)
                        {
                            if (character is EnemyBattleCharacter)
                            {
                                targets.Add(character);
                            }
                        }
                        break;

                    case BattleTechniqueTargetType.SingleKOdAlly:
                        foreach (BattleCharacter character in inactiveCharacters)
                        {
                            if (character is PlayerBattleCharacter)
                            {
                                targets.Add(character);
                            }
                        }
                        break;
                }
            }
            else if (TurnCharacter is EnemyBattleCharacter)
            {
                switch (targetType)
                {
                    case BattleTechniqueTargetType.AllActiveAllies:
                    case BattleTechniqueTargetType.SingleActiveAlly:
                        foreach (BattleCharacter character in activeCharacters)
                        {
                            if (character is EnemyBattleCharacter)
                            {
                                targets.Add(character);
                            }
                        }
                        break;

                    case BattleTechniqueTargetType.AllEnemies:
                    case BattleTechniqueTargetType.SingleEnemy:
                        foreach (BattleCharacter character in activeCharacters)
                        {
                            if (character is PlayerBattleCharacter)
                            {
                                targets.Add(character);
                            }
                        }
                        break;
                }
            }

            return targets;
        }
        int CalculateActionDamage(BattleCharacter attacker, BattleTechniqueAction action, BattleCharacter defender)
        {
            int damage = 1;

            switch (action.ActionType)
            {
                case BattleTechniqueActionType.Damage:
                    switch (action.ActionStatType)
                    {
                        case ActionStatType.Physical:
                            if (defender.CurrentAction == "Defend")
                            {
                                damage = attacker.CurrentATK + action.Strength - (defender.CurrentDEF * 2);
                            }
                            else
                            {
                                damage = attacker.CurrentATK + action.Strength - defender.CurrentDEF;
                            }
                            break;

                        case ActionStatType.Magical:
                            if (defender.CurrentAction == "Defend")
                            {
                                damage = attacker.CurrentMAG_ATK + action.Strength - (defender.CurrentMAG_DEF * 2);
                            }
                            else
                            {
                                damage = attacker.CurrentMAG_ATK + action.Strength - defender.CurrentMAG_DEF;
                            }
                            break;
                    }
                    break;
                case BattleTechniqueActionType.Healing:
                    switch (action.ActionStatType)
                    {
                        case ActionStatType.Physical:
                            damage = (attacker.CurrentMAG_ATK + action.Strength);
                            break;

                        case ActionStatType.Magical:
                            damage = (attacker.CurrentMAG_ATK + action.Strength) * (GameClass.Random.Next(7, 11) / 100);
                            break;
                    }
                    break;
            }

            if (damage < 0 || damage == 0)
            {
                damage = 1;
            }

            return damage;
        }
        void ResolveDamageAction(BattleTechniqueAction action)
        {
            if (action.ActionType == BattleTechniqueActionType.Revive)
            {
                foreach (BattleCharacter target in selectedTargets)
                {
                    int damage = action.Strength;
                    target.ChangeHP(damage, action.ActionType);
                    activeCharacters.Add(target);
                    inactiveCharacters.Remove(target);
                }
            }
            else
            {
                if (selectedTechniqueNameIsItem)
                {
                    foreach (BattleCharacter target in selectedTargets)
                    {
                        int damage = action.Strength;
                        target.ChangeHP(damage, action.ActionType);
                    }
                }
                else
                {
                    switch (action.ActionStatType)
                    {
                        case ActionStatType.Physical:
                            foreach (BattleCharacter target in selectedTargets)
                            {
                                int damage = CalculateActionDamage(TurnCharacter, action, target);
                                target.ChangeHP(damage, action.ActionType);
                            }
                            break;

                        case ActionStatType.Magical:
                            foreach (BattleCharacter target in selectedTargets)
                            {
                                int damage = CalculateActionDamage(TurnCharacter, action, target);
                                target.ChangeMP(damage, action.ActionType);
                            }
                            break;
                    }
                }
            }
        }
        void AIActionSelection()
        {
            List<PlayerBattleCharacter> playerTeam = new List<PlayerBattleCharacter>();
            List<EnemyBattleCharacter> enemyTeam = new List<EnemyBattleCharacter>();


            foreach (BattleCharacter character in activeCharacters)
            {
                if (character is EnemyBattleCharacter)
                {
                    enemyTeam.Add((EnemyBattleCharacter)character);
                }
                else if (character is PlayerBattleCharacter)
                {
                    playerTeam.Add((PlayerBattleCharacter)character);
                }
            }

            EnemyBattleCharacter me = (EnemyBattleCharacter)TurnCharacter;

            List<string> viableActions = new List<string>();

            if (me.Techniques.Count == 0)
            {
                throw new Exception("This enemy cannot perform any actions.");
            }
            else if (me.Techniques.Count == 1)
            {
                if (me.Techniques[0].techniqueCost < me.CurrentMP || me.Techniques[0].techniqueCost == 0)
                {
                    viableActions.Add(me.Techniques[0].techniqueName);
                }
            }
            else
            {
                if (me.CurrentHP < me.MaxHP / 20)
                {
                    foreach (BattleTechnique technique in me.Techniques)
                    {
                        if (technique.techniqueCost < me.CurrentMP)
                        {
                            if (!viableActions.Contains(technique.techniqueName))
                            {
                                if (technique.AICategories.Contains(BattleTechniqueAICategory.Healing))
                                {
                                    viableActions.Add(technique.techniqueName);
                                }
                            }
                        }
                    }
                }

                foreach (EnemyBattleCharacter enemy in enemyTeam)
                {
                    if (enemy.CurrentHP < enemy.MaxHP / 20)
                    {
                        foreach (BattleTechnique technique in me.Techniques)
                        {
                            if (technique.techniqueCost < me.CurrentMP || technique.techniqueCost == 0)
                            {
                                if (!viableActions.Contains(technique.techniqueName))
                                {
                                    if (technique.AICategories.Contains(BattleTechniqueAICategory.Healing) && technique.techniqueTargeting != BattleTechniqueTargetType.Self)
                                    {
                                        viableActions.Add(technique.techniqueName);
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (BattleTechnique technique in me.Techniques)
                {
                    if (technique.techniqueCost < me.CurrentMP || technique.techniqueCost == 0 && technique.AICategories.Contains(BattleTechniqueAICategory.Damaging) || technique.AICategories.Contains(BattleTechniqueAICategory.Offensive))
                    {
                        viableActions.Add(technique.techniqueName);
                    }
                }

                foreach (BattleTechnique technique in me.Techniques)
                {
                    if (technique.techniqueCost < me.CurrentMP || technique.techniqueCost == 0)
                    {
                        if (!viableActions.Contains(technique.techniqueName))
                        {
                            if (technique.AICategories.Contains(BattleTechniqueAICategory.Buff) && technique.techniqueTargeting != BattleTechniqueTargetType.Self)
                            {
                                foreach (BattleTechniqueBuffData techniqueBuffData in technique.buffs)
                                {
                                    if (techniqueBuffData.buffStrength < 0)
                                    {
                                        if (!viableActions.Contains(technique.techniqueName))
                                        {
                                            viableActions.Add(technique.techniqueName);
                                            break;
                                        }
                                    }
                                }

                                foreach (BattleTechniqueEffect effect in technique.effects)
                                {
                                    foreach (BattleTechniqueBuffData effectBuffData in effect.EffectBuffs)
                                    {
                                        if (effectBuffData.buffStrength < 0)
                                        {
                                            if (!viableActions.Contains(technique.techniqueName))
                                            {
                                                viableActions.Add(technique.techniqueName);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                foreach (BattleTechniqueBuff activeBuff in activeBuffs)
                {
                    if (activeBuff.Target == me && activeBuff.BuffStrength < 0)
                    {
                        foreach (BattleTechnique technique in me.Techniques)
                        {
                            if (technique.techniqueCost < me.CurrentMP || technique.techniqueCost == 0)
                            {
                                if (!viableActions.Contains(technique.techniqueName))
                                {
                                    if (technique.AICategories.Contains(BattleTechniqueAICategory.Buff))
                                    {
                                        foreach (BattleTechniqueBuffData techniqueBuffData in technique.buffs)
                                        {
                                            if (techniqueBuffData.buffType == activeBuff.BuffType && techniqueBuffData.buffStrength > 0)
                                            {
                                                if (!viableActions.Contains(technique.techniqueName))
                                                {
                                                    viableActions.Add(technique.techniqueName);
                                                    break;
                                                }
                                            }
                                        }

                                        foreach (BattleTechniqueEffect effect in technique.effects)
                                        {
                                            foreach (BattleTechniqueBuffData effectBuffData in effect.EffectBuffs)
                                            {
                                                if (effectBuffData.buffType == activeBuff.BuffType && effectBuffData.buffStrength > 0)
                                                {
                                                    if (!viableActions.Contains(technique.techniqueName))
                                                    {
                                                        viableActions.Add(technique.techniqueName);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }                  
                }
            }

            if (viableActions.Count == 0)
            {
                throw new Exception("AI didn't consider any actions viable.");
            }

            selectedTechniqueName = viableActions[GameClass.Random.Next(0, viableActions.Count)];
            selectedTechnique = getTechniqueFromAction(selectedTechniqueName);


            switch (selectedTechnique.techniqueTargeting)
            {
                case BattleTechniqueTargetType.Self:
                    selectedTargets.Add(me);
                    break;
                case BattleTechniqueTargetType.AllActiveAllies:
                    foreach (EnemyBattleCharacter character in enemyTeam)
                    {
                        selectedTargets.Add(character);
                    }
                    break;
                case BattleTechniqueTargetType.AllEnemies:
                    foreach (PlayerBattleCharacter character in playerTeam)
                    {
                        selectedTargets.Add(character);
                    }
                    break;
                case BattleTechniqueTargetType.Everyone:
                    foreach (BattleCharacter character in activeCharacters)
                    {
                        selectedTargets.Add(character);
                    }
                    break;
                case BattleTechniqueTargetType.SingleActiveAlly:
                    if(selectedTechnique.AICategories.Contains(BattleTechniqueAICategory.Buff))
                    {
                        selectedTargets.Add(enemyTeam[GameClass.Random.Next(0, enemyTeam.Count)]);
                    }
                    break;
                case BattleTechniqueTargetType.SingleEnemy:
                    selectedTargets.Add(playerTeam[GameClass.Random.Next(0, playerTeam.Count)]);
                    break;
            }


            if (selectedTargets.Count == 0)
            {
                throw new Exception("AI didn't select any targets for its technique.");
            }
        }
        #endregion
    }
}
