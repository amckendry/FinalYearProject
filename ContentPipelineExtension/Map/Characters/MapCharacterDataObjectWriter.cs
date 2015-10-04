using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using RPGProjectLibrary;

namespace ContentPipelineExtension
{
    [ContentTypeWriter]
    public class MapCharacterDataObjectWriter : ContentTypeWriter<MapCharacterData>
    {//XML writer for data relating to a map character.
        protected override void Write(ContentWriter output, MapCharacterData value)
        {
            output.WriteObject<CharacterAnimatedSpriteData>(value.idleSpriteData);
            output.WriteObject<CharacterAnimatedSpriteData>(value.runSpriteData);
            output.WriteObject<CharacterAnimatedSpriteData>(value.jumpSpriteData);
            output.WriteObject<CharacterAnimatedSpriteData>(value.fallSpriteData);
            output.WriteObject<CharacterAnimatedSpriteData>(value.landSpriteData);
            output.WriteObject<CharacterAnimatedSpriteData>(value.deathSpriteData);
            output.Write(value.portraitTextureName);
            //Values converted to double as XML has no float data type.
            output.Write((double)value.walkSpeed);
            output.Write((double)value.airInfluence);
            output.Write((double)value.frictionFactor);
            output.Write((double)value.jumpStrength);
            output.Write((double)value.mass);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(MapCharacterDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}