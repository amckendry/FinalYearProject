using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RPGProjectLibrary;

namespace RPGProject
{
   public class EnemyMapCharacter : MapCharacter
    {
       QuestEnemyData linkedQuestEnemyData;
       public QuestEnemyData LinkedQuestEnemyData
       {
           get
           {
               return linkedQuestEnemyData;
           }
       }

       Vector2 initialPosition;

       EnemyAIType enemyAIType;
       public EnemyAIType EnemyAIType
        {
            get
            {
                return enemyAIType;
            }
        }

       public EnemyMapCharacter()
            : base(CharacterFacing.Left)
        {
        }

       public override void Update(GameTime gameTime)
       {
           Vector2 distanceFromPlayer = GameClass.ScreenManager.MapScreen.Player.Position - position;

           if (CurrentAction == MapCharacterAction.Run)
           {
               if (Facing == CharacterFacing.Right)
               {
                   velocity.X += WalkSpeed;
               }
               else if (Facing == CharacterFacing.Left)
               {
                   velocity.X -= WalkSpeed;
               }

               if (enemyAIType == EnemyAIType.PaceLeft || enemyAIType == EnemyAIType.PaceRight)
               {
                   if (initialPosition.X - position.X > 50)
                   {
                           Facing = CharacterFacing.Right;
                   }
                   else if (initialPosition.X - position.X < -50)
                   {
                       Facing = CharacterFacing.Left;
                   }
               }
           }

           switch (enemyAIType)
           {
               case EnemyAIType.PaceRight:

                   if (CurrentAction == MapCharacterAction.Idle)
                   {
                       Facing = CharacterFacing.Right;
                       CurrentAction = MapCharacterAction.Run;
                   }
                   break;
               case EnemyAIType.PaceLeft:

                   if (CurrentAction == MapCharacterAction.Idle)
                   {
                       Facing = CharacterFacing.Left;
                       CurrentAction = MapCharacterAction.Run;
                   }
                   break;

               case EnemyAIType.ChasePlayer:

                   if (Math.Abs(distanceFromPlayer.X) < 200 && Math.Abs(distanceFromPlayer.Y) < 190)
                   {
                       if (CurrentAction == MapCharacterAction.Idle || CurrentAction == MapCharacterAction.Run)
                       {
                           if (distanceFromPlayer.X > 0)
                           {
                               Facing = CharacterFacing.Right;
                               if (CurrentAction == MapCharacterAction.Idle)
                               {
                                   CurrentAction = MapCharacterAction.Run;
                               }
                           }
                           else if (distanceFromPlayer.X < 0)
                           {
                               Facing = CharacterFacing.Left;
                               if (CurrentAction == MapCharacterAction.Idle)
                               {
                                   CurrentAction = MapCharacterAction.Run;
                               }
                           }
                       }
                   }
                   else
                   {
                       if (CurrentAction == MapCharacterAction.Run)
                       {
                           CurrentAction = MapCharacterAction.Idle;
                       }
                   }
                   break;

               case EnemyAIType.AvoidPlayer:


                   if (Math.Abs(distanceFromPlayer.X) < 200 && Math.Abs(distanceFromPlayer.Y) < 150)
                   {
                       if (CurrentAction == MapCharacterAction.Idle || CurrentAction == MapCharacterAction.Run)
                       {
                           if (distanceFromPlayer.X > 0)
                           {
                               Facing = CharacterFacing.Left;
                               if (CurrentAction == MapCharacterAction.Idle)
                               {
                                   CurrentAction = MapCharacterAction.Run;
                               }
                           }
                           else if (distanceFromPlayer.X < 0)
                           {
                               Facing = CharacterFacing.Right;
                               if (CurrentAction == MapCharacterAction.Idle)
                               {
                                   CurrentAction = MapCharacterAction.Run;
                               }
                           }
                       }
                   }
                   else
                   {
                       if (CurrentAction == MapCharacterAction.Run)
                       {
                           CurrentAction = MapCharacterAction.Idle;
                       }
                   }
                   break;
           }
           base.Update(gameTime);
       }

       public void LoadContent(QuestEnemyData enemyData)
       {
           linkedQuestEnemyData = enemyData;

           this.position = enemyData.position;
           this.initialPosition = enemyData.position;
           this.characterID = enemyData.enemyType;
           this.enemyAIType = enemyData.enemyAIType;

           this.CollisionBoxes = new Dictionary<MapCharacterAction, CollisionBox>();

           MapCharacterData characterData = GameClass.ContentManager.Load<MapCharacterData>(@"MapCharacters/" + characterID.ToString() + "/" + characterID.ToString() + "Map");

           Sprites.Add(MapCharacterAction.Idle, AnimatedSprite.CreateFromData(characterData.idleSpriteData));
           Sprites.Add(MapCharacterAction.Run, AnimatedSprite.CreateFromData(characterData.runSpriteData));
           Sprites.Add(MapCharacterAction.Jump, AnimatedSprite.CreateFromData(characterData.jumpSpriteData));
           Sprites.Add(MapCharacterAction.Fall, AnimatedSprite.CreateFromData(characterData.fallSpriteData));
           Sprites.Add(MapCharacterAction.Land, AnimatedSprite.CreateFromData(characterData.landSpriteData));
           Sprites.Add(MapCharacterAction.Death, AnimatedSprite.CreateFromData(characterData.deathSpriteData));
           Sprites[MapCharacterAction.Death].AnimationType = SpriteAnimationType.AnimateOnceMaintainLast;

           CollisionBoxes = new Dictionary<MapCharacterAction, CollisionBox>();
           CollisionBoxes.Add(MapCharacterAction.Idle, CollisionBox.CreateFromData(characterData.idleSpriteData.collisionBoxData));
           CollisionBoxes.Add(MapCharacterAction.Run, CollisionBox.CreateFromData(characterData.runSpriteData.collisionBoxData));
           CollisionBoxes.Add(MapCharacterAction.Jump, CollisionBox.CreateFromData(characterData.jumpSpriteData.collisionBoxData));
           CollisionBoxes.Add(MapCharacterAction.Fall, CollisionBox.CreateFromData(characterData.fallSpriteData.collisionBoxData));
           CollisionBoxes.Add(MapCharacterAction.Land, CollisionBox.CreateFromData(characterData.landSpriteData.collisionBoxData));
           CollisionBoxes.Add(MapCharacterAction.Death, CollisionBox.CreateFromData(characterData.deathSpriteData.collisionBoxData));

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
    }
}
