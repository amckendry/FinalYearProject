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
    public class InventoryChangeDataWriter : ContentTypeWriter<InventoryChangeData>
    {//XML writer for data relating to a change to the players inventory trigged by another given object.
        protected override void Write(ContentWriter output, InventoryChangeData value)
        {
            output.WriteObject<InventoryChangeType>(value.inventoryChangeType);
            output.WriteObject<InventoryItemIdentity>(value.itemIDToChange);
            output.Write(value.QTYToChangeBy);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(InventoryChangeDataReader).AssemblyQualifiedName;//Gets the loaded name of the reader for this writer.
        }
    }
}
