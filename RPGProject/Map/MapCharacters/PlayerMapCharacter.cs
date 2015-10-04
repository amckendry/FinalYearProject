using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPGProjectLibrary;

namespace RPGProject
{//this class creates an instance of a character controllable by the player on the Map Screen.

    public class PlayerMapCharacter : MapCharacter
    {
       GamePadController controls;
       public GamePadController Controls
       {
           get
           {
               return controls;
           }
       }

       KeyboardController debugControls;
       public KeyboardController DebugControls
           {
               get
               {
                   return debugControls;
               }
           }

       float walkSoundTimer = 0.0f;

       public bool ControlsActive
       {
           get
           {
               return controls.ControlsActive;
           }
           set
           {
               controls.ControlsActive = value;
           }
       }
       public bool DebugControlsActive
       {
           get
           {
               return debugControls.ControlsActive;
           }
           set
           {
               debugControls.ControlsActive = value;
           }
       }

        public PlayerMapCharacter()
            : base(CharacterFacing.Left)
        {
        }
        public void LoadContent(Vector2 position, CharacterIdentity playerID)
        {
            this.characterID = playerID;
            this.position = position;
            this.CollisionBoxes = new Dictionary<MapCharacterAction, CollisionBox>();

            controls = new GamePadController(PlayerIndex.One);
            debugControls = new KeyboardController();

            MapCharacterData characterData = GameClass.ContentManager.Load<MapCharacterData>(@"MapCharacters/" + playerID.ToString() + "/" + playerID.ToString() + "Map");

            Sprites.Add(MapCharacterAction.Idle, AnimatedSprite.CreateFromData(characterData.idleSpriteData));
            Sprites.Add(MapCharacterAction.Run, AnimatedSprite.CreateFromData(characterData.runSpriteData));
            Sprites.Add(MapCharacterAction.Jump, AnimatedSprite.CreateFromData(characterData.jumpSpriteData));
            Sprites.Add(MapCharacterAction.Fall, AnimatedSprite.CreateFromData(characterData.fallSpriteData));
            Sprites.Add(MapCharacterAction.Land, AnimatedSprite.CreateFromData(characterData.landSpriteData));

            CollisionBoxes = new Dictionary<MapCharacterAction, CollisionBox>();
            CollisionBoxes.Add(MapCharacterAction.Idle, CollisionBox.CreateFromData(characterData.idleSpriteData.collisionBoxData));
            CollisionBoxes.Add(MapCharacterAction.Run, CollisionBox.CreateFromData(characterData.runSpriteData.collisionBoxData));
            CollisionBoxes.Add(MapCharacterAction.Jump, CollisionBox.CreateFromData(characterData.jumpSpriteData.collisionBoxData));
            CollisionBoxes.Add(MapCharacterAction.Fall, CollisionBox.CreateFromData(characterData.fallSpriteData.collisionBoxData));
            CollisionBoxes.Add(MapCharacterAction.Land, CollisionBox.CreateFromData(characterData.landSpriteData.collisionBoxData));

            CollisionSensors.Add(SensorType.Top, new CollisionSensor(CollisionBoxes[MapCharacterAction.Idle], SensorType.Top, 4));
            CollisionSensors.Add(SensorType.Bottom, new CollisionSensor(CollisionBoxes[MapCharacterAction.Idle], SensorType.Bottom, 4));
            CollisionSensors.Add(SensorType.Left, new CollisionSensor(CollisionBoxes[MapCharacterAction.Idle], SensorType.Left, 4));
            CollisionSensors.Add(SensorType.Right, new CollisionSensor(CollisionBoxes[MapCharacterAction.Idle], SensorType.Right, 4));

            portraitTexture = GameClass.ContentManager.Load<Texture2D>(characterData.portraitTextureName);
            walkSpeed = characterData.walkSpeed;
            airInfluence = characterData.airInfluence;
            currentFrictionFactor = characterData.frictionFactor;
            jumpStrength = characterData.jumpStrength;
            mass = characterData.mass;

            CurrentAction = MapCharacterAction.Idle;
            currentCollisionBox = CollisionBoxes[CurrentAction];
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            controls.Update(gameTime);
            debugControls.Update(gameTime);

            if (ControlsActive && DebugControlsActive)
            {
                if (controls.ButtonPressed(Buttons.DPadLeft, true, false) || debugControls.KeyPressed(Keys.Left, true, false))
                {

                    Facing = CharacterFacing.Left;
                    if (InAir == false)
                    {
                        velocity.X -= walkSpeed;
                        if (walkSoundTimer > 1 / CurrentSprite.FPS * 2.5)
                        {
                            walkSoundTimer = 0.0f;
                            GameClass.SoundManager.PlaySoundEffect("Audio/walkStep");
                        }
                        else
                        {
                            walkSoundTimer += GameClass.Elapsed;
                        }
                    }
                    else
                    {
                        velocity.X -= airInfluence;
                    }

                    if (CurrentAction == MapCharacterAction.Idle)
                    {
                        CurrentAction = MapCharacterAction.Run;
                    }
                }


                if (controls.ButtonPressed(Buttons.DPadRight, true, false) || debugControls.KeyPressed(Keys.Right, true, false))
                {

                    Facing = CharacterFacing.Right;

                    if (InAir == false)
                    {
                        velocity.X += walkSpeed;
                        if (walkSoundTimer > 1 / CurrentSprite.FPS * 2.5)
                        {
                            walkSoundTimer = 0.0f;
                            GameClass.SoundManager.PlaySoundEffect("Audio/walkStep");
                        }
                        else
                        {
                            walkSoundTimer += GameClass.Elapsed;
                        }
                    }
                    else
                    {
                        velocity.X += airInfluence;
                    }

                    if (CurrentAction == MapCharacterAction.Idle)
                    {
                        CurrentAction = MapCharacterAction.Run;
                    }


                   
                }

                if (controls.ButtonPressed(Buttons.A, false, false) || debugControls.KeyPressed(Keys.Space, false, false))
                {
                    if (jumpStrength > 0.0f)
                    {
                        if (InAir == false)
                        {
                            walkSoundTimer = 0.0f;
                            if (CurrentAction == MapCharacterAction.Idle || CurrentAction == MapCharacterAction.Run)
                            {
                                CurrentAction = MapCharacterAction.Jump;
                                GameClass.SoundManager.PlaySoundEffect("Audio/jump");
                                velocity.Y -= jumpStrength;
                                InAir = true;
                            }
                        }
                    }
                }


                if ((controls.ButtonReleased(Buttons.DPadLeft) && controls.ButtonReleased(Buttons.DPadRight)) && (debugControls.KeyReleased(Keys.Left) && debugControls.KeyReleased(Keys.Right)))
                {
                    if (CurrentAction == MapCharacterAction.Run)
                    {
                        walkSoundTimer = 0.0f;
                        CurrentAction = MapCharacterAction.Idle;
                    }
                }
            }
        }
    }
}
