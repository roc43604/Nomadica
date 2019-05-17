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
using JModelling.JModelling;
using GraphicsEngine;
using JModelling.JModelling.Chunk;
using System.Diagnostics;
using JModelling.InventorySpace; 

namespace JModelling
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        JManager manager;

        ChunkGenerator generator;

        public int Width = 1600, Height = 1000;

        int curChunkX = -1;
        int curChunkZ = -1;

        private SpriteFont debugFont;
        public static bool DebugEnabled;
        private int debugUpdateInterval = 250;
        private double debugLastUpdate;
        private double debugFPS;
        private double debugUPS; 

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            DebugEnabled = false; 

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Load.Init(Services);
            debugFont = Content.Load<SpriteFont>("DebugFont");
            InventoryMenu.LoadImages(Content); 

            generator = new ChunkGenerator(43545544, 10, 10, 4, manager, spriteBatch, Load.Mesh(@"Content/Models/cube.obj"));

            manager = new JManager(this, Width, Height, graphics, generator, spriteBatch);

            generator.manager = manager;
        }

        /// <summary>
        /// Hides/shows the mouse. True if the mouse is visible, false if it's hidden. 
        /// </summary>
        /// <param name="shouldYou"></param>
        public void HideMouse(bool isVisible)
        {
            IsMouseVisible = isVisible;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            Stopwatch stopWatch = Stopwatch.StartNew();
            stopWatch.Start(); 

            ////////////////////////////////////////////////////////////////////////////////////
            Vec4 pos = manager.player.Camera.loc;

            int chunkIndexX = generator.GetIndexX((int)pos.X);
            int chunkIndexZ = generator.GetIndexZ((int)pos.Z);

            if (curChunkX != chunkIndexX || chunkIndexZ != curChunkZ)
            {
                generator.GenerateChunks(
                    chunkIndexX,
                    chunkIndexZ,
                    3
                );

                curChunkX = chunkIndexX;
                curChunkZ = chunkIndexZ;
            }

            manager.Update();
            ////////////////////////////////////////////////////////////////////////////////////
            stopWatch.Stop(); 
            if (gameTime.TotalGameTime.TotalMilliseconds - debugLastUpdate > debugUpdateInterval)
            {
                debugUPS = 1000 / (stopWatch.ElapsedMilliseconds + 1);
                debugLastUpdate = gameTime.TotalGameTime.TotalMilliseconds; 
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            Stopwatch stopWatch = Stopwatch.StartNew();
            stopWatch.Start();

            ////////////////////////////////////////////////////////////////////////////////////
            GraphicsDevice.Clear(Color.Green); 
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            manager.Draw(spriteBatch); 
            ////////////////////////////////////////////////////////////////////////////////////
            stopWatch.Stop(); 
            if (gameTime.TotalGameTime.TotalMilliseconds - debugLastUpdate > debugUpdateInterval)
            {
                debugFPS = (1000) / (stopWatch.ElapsedMilliseconds + 1); 
            }

            if (DebugEnabled)
            {
                spriteBatch.DrawString(
                debugFont,
                "FPS   : " + debugFPS,
                new Vector2(5, 5),
                Color.Black
            );
                spriteBatch.DrawString(
                    debugFont,
                    "UPS   : " + debugUPS,
                    new Vector2(5, debugFont.LineSpacing + 5),
                    Color.Black
                );
                //spriteBatch.DrawString(
                //    debugFont,
                //    "Mode  : " + movementMode,
                //    new Vector2(5, debugFont.LineSpacing * 2 + 5),
                //    Color.White
                //);
                spriteBatch.DrawString(
                    debugFont,
                    "Biome : " + generator.BiomeAt(manager.player.Camera.loc).ToString(),
                    new Vector2(5, debugFont.LineSpacing * 3 + 5),
                    Color.Black
                );

                spriteBatch.DrawString(
                    debugFont,
                    "Pos : " + manager.player.Camera.loc.ToString(),
                    new Vector2(5, debugFont.LineSpacing * 5 + 5),
                    Color.Black
                );
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
