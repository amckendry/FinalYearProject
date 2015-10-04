using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RPGProjectLibrary;

namespace RPGProject
{// This class creates an instance of an NPC (Non-Player Character) on the Map Screen.
    public class NPCMapCharacter : MapCharacter
    {

        QuestNPCData linkedNPCData;
        public QuestNPCData LinkedNPCData
        {
            get
            {
                return linkedNPCData;
            }
        }

        public NPCMapCharacter()
            : base(CharacterFacing.Left)
        {
        }

        public void LoadContent(QuestNPCData NPCData)
        {
            linkedNPCData = NPCData;
            this.characterID = NPCData.NPCID;
            this.position = NPCData.position;
            this.CollisionBoxes = new Dictionary<MapCharacterAction, CollisionBox>();

            MapCharacterData characterData = GameClass.ContentManager.Load<MapCharacterData>(@"MapCharacters/" + NPCData.NPCID.ToString() + "/" + NPCData.NPCID.ToString() + "Map");

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
            CollisionSensors.Add(SensorType.Left, new CollisionSensor(CollisionBoxes[MapCharacterAction.Idle], SensorType.Left, 10));
            CollisionSensors.Add(SensorType.Right, new CollisionSensor(CollisionBoxes[MapCharacterAction.Idle], SensorType.Right, 10));

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
