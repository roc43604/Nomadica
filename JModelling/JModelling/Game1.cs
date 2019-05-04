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

        private Mesh cube;
        private Mesh meshWater;

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
            manager = new JManager(this, Width, Height, graphics, spriteBatch);

            cube = Load.Mesh(@"Content/Models/cube.obj");

            generator = new ChunkGenerator(43545544, 20, 20, 4, manager, Load.Mesh(@"Content/Models/cube.obj"));


            Triangle a = new Triangle(
                new Vec4(1, 0, 1), new Vec4(0, 0, 1), new Vec4(1, 0, 0)
            );
            a.Normal = new Vec4(0, 1, 0);
            a.NormalLength = 1;
            Triangle b = new Triangle(
                  new Vec4(0, 0, 0), new Vec4(0, 0, 1), new Vec4(1, 0, 0)
             );
            b.Normal = new Vec4(0, 1, 0);
            b.NormalLength = 1;

            meshWater = new Mesh(new Triangle[] { a, b });
            meshWater.Scale(1000f, 1000f, 1000f);
            meshWater.Translate(0, 1, 0);
            meshWater.SetColor(Color.LightSteelBlue);

            cube.SetColor(Color.Yellow);
            cube.Scale(20f, 20f, 20f);

            manager.AddMesh(cube);
            //manager.AddMesh(meshWater);
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
            Vec4 pos = manager.camera.loc;
            //Vec4 sizeWater = meshWater.Size;

            // cube.MoveTo(pos.X - 50, generator.GetHeightAt(pos.X - 50, pos.Z), pos.Z);

            //Console.WriteLine("[" + pos.X + ":" + pos.Z + "] , [" + generator.GetHeightAt(pos.X - 100, pos.Z) + "]");

            // meshWater.MoveTo(pos.X, 1, pos.Z);
            manager.camera.loc.Y = generator.GetHeightAt(pos.X, pos.Z) + 100;


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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.Green); 
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            {
                spriteBatch.Draw(manager.GetSkyboxTexture(), new Rectangle(0, 0, Width, Height), Color.White);
                spriteBatch.Draw(manager.GetWorldTexture(), new Rectangle(0, 0, Width, Height), Color.White);
            }
            spriteBatch.End();

            //Console.WriteLine(1f / (float)(gameTime.ElapsedGameTime.TotalSeconds)); 

            base.Draw(gameTime);
        }
    }
}
