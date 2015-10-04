using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGProject
{
   public abstract class Screen
    {
       protected ScreenIdentity screenID;
       public ScreenIdentity ScreenID
       {
           get
           {
               return screenID;
           }
       }

       protected bool screenLoading;
       public bool ScreenLoading
       {
           get
           {
               return screenLoading;
           }
           set
           {
               screenLoading = value;
           }
       }


       protected List<GameObject> screenObjects;
       public List<GameObject> ScreenObjects
       {
           get
           {
               return screenObjects;
           }
           set
           {
               screenObjects = value;
           }
       }

       public virtual void Initialise()
       {
           screenObjects = new List<GameObject>();
       }
       public abstract void LoadContent();
       public virtual void Update(GameTime gameTime)
       {
           if (GameClass.Paused == false)
           {
               foreach (GameObject obj in screenObjects)
               {
                   obj.Update(gameTime);
               }
           }
       }
       public virtual void Draw()
       {
           foreach (DrawableGameObject obj in screenObjects)
           {
                   obj.Draw();
           }
       }    
    }
}
