using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using RPGProjectLibrary;

namespace RPGProject
{
    /// This is the main type for the game
    public class TextureData
    {
       public string textureFileName;
       public Texture2D texture;
       public int noOfObjectsUsingTexture;
       public TextureData(string textureFileName)
       {
           this.textureFileName = textureFileName;
           texture = GameClass.ContentManager.Load<Texture2D>(textureFileName);
           noOfObjectsUsingTexture = 1;
       }
    }
    public enum FadeState
    {
        IdleFaded,
        FadingIn,
        FadingOut,
        IdleOpaque
    }
    public class GameClass : Microsoft.Xna.Framework.Game
    {
        static GraphicsDeviceManager graphics;
        static SpriteBatch spriteBatch;
        static ContentManager contentManager;
        static Camera currentGameCamera;
        static bool paused;
        static float elapsed;
        static List<TextureData> loadedTextureData;
        static SpriteFont size8Font;
        static SpriteFont size6Font;
        static Random random;
        static bool exit;

        public static GraphicsDeviceManager GraphicsManager
        {
            get
            {
                return graphics;
            }
        }
        public static SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
            }
        }
        public static ContentManager ContentManager
        {
            get
            {
                return contentManager;
            }
        }
        public static bool Paused
        {
            get
            {
                return paused;
            }
            set
            {
                paused = value;
            }
        }
        public static Rectangle ViewPortBounds
        {
            get
            {
                return new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            }

        }
        public static Camera CurrentGameCamera
        {
            get
            {
                return currentGameCamera;
            }
        }
        public static SpriteFont Size8Font
        {
            get
            {
                return size8Font;
            }
        }
        public static SpriteFont Size6Font
        {
            get
            {
                return size6Font;
            }
        }
        public static float Elapsed
        {
            get
            {
                return elapsed;
            }

        }
        public static Random Random
        {
            get
            {
                return random;
            }
        }

        static ScreenManager screenManager;
        public static ScreenManager ScreenManager
        {
            get
            {
                return screenManager;
            }
            set
            {
                screenManager = value;
            }
        }

        static SoundManager soundManager;
        public static SoundManager SoundManager
        {
            get
            {
                return soundManager;
            }
        }

        static bool drawDebugInfo = false;
        public static bool DrawDebugInfo
        {
            get
            {
                return drawDebugInfo;
            }
        }

        public GameClass()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 400;

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            contentManager = Content;
            Content.RootDirectory = "Content";
            paused = false;
        }

        protected override void Initialize()
        {
            screenManager = new ScreenManager();
            screenManager.Initialise();
            random = new Random();
            soundManager = new SoundManager();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            loadedTextureData = new List<TextureData>();
            size8Font = Content.Load<SpriteFont>("BookmanOldStyleSize8");
            size6Font = Content.Load<SpriteFont>("BookmanOldStyleSize6");

            currentGameCamera = new Camera(new Vector2(0, 0), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

            screenManager.LoadContent();
        }
        protected override void UnloadContent()
        {
            
        }
        protected override void Update(GameTime gameTime)
        {
            if (exit == false)
            {
                KeyboardState keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.D))
                {
                    GameClass.drawDebugInfo = true;
                }

                if (keyboardState.IsKeyDown(Keys.S))
                {
                    GameClass.drawDebugInfo = false;
                }

                elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                screenManager.Update(gameTime);
                base.Update(gameTime);
            }
            else if (exit == true)
            {
                this.Exit();
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, currentGameCamera.TransformationMatrix);

            screenManager.Draw();

            if (drawDebugInfo == true)
            {

               // int drawPos = 20;
                //foreach (TextureData textureData in loadedTextureData)
                //{

                    //spriteBatch.DrawString(size8Font, textureData.textureFileName, new Vector2(GameClass.CurrentGameCamera.Position.X + 400, GameClass.CurrentGameCamera.Position.Y + drawPos), Color.White);

         
                //spriteBatch.DrawString(size8Font, "" + GlobalGameInfo.CurrentEnemyInBattle.CurrentAction, new Vector2(GameClass.CurrentGameCamera.Position.X + 400, GameClass.CurrentGameCamera.Position.Y + 300), Color.White);
                //    drawPos += 10;
                //}
           }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Texture2D LoadTextureData(string textureFileName)
        {
            foreach (TextureData loadedTexture in loadedTextureData)
            {
                if (textureFileName == loadedTexture.textureFileName)
                {
                    loadedTexture.noOfObjectsUsingTexture += 1;
                    return loadedTexture.texture;
                }
            }

            TextureData newTextureData = new TextureData(textureFileName);
            loadedTextureData.Add(newTextureData);
            return newTextureData.texture;
        }
        public static void UnloadTextureData(Texture2D texture)
        {
            foreach (TextureData loadedTexture in loadedTextureData)
            {
                if (texture == loadedTexture.texture)
                {
                    loadedTexture.noOfObjectsUsingTexture -= 1;
                    if (loadedTexture.noOfObjectsUsingTexture == 0)
                    {
                        loadedTextureData.Remove(loadedTexture);
                    }
                }
            }
        }
        public static void StartExit()
        {
            exit = true;
        }
    }
}