using GraphicsEngine;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using JModelling.Creature;
using JModelling.JModelling.Chunk;
using JModelling.InventorySpace;
using JModelling.Pause;
using JModelling.GUI;


namespace JModelling.JModelling
{
    /// <summary>
    /// The manager-class that will deal with managing all of the other
    /// JModelling classes. 
    /// </summary>
    public class JManager
    {
        public const float PITimesTwo = (float)(Math.PI * 2d);

        /// <summary>
        /// The last keyboard state
        /// </summary>
        private KeyboardState lastKb;

        private MouseState lastMs; 

        /// <summary>
        /// The thing that this manager belongs to. 
        /// </summary>
        private Game host; 
        /// <summary>
        /// The screen dimensions. 
        /// </summary>
        public int Width, Height;

        /// <summary>
        /// How many pixels on the screen we're really drawing to. 
        /// </summary>
        public int DrawWidth, DrawHeight;

        /// <summary>
        /// The location of the center of the screen. 
        /// </summary>
        public static int centerX, centerY;

        public bool isMouseFocused = true;

        /// <summary>
        /// The extent of the observable game world that is seen on display. 
        /// </summary>
        private const float FOV = (float)(Math.PI / 2d);

        /// <summary>
        /// What the current state of the game is. 
        /// </summary>
        private GameState gameState;

        /// <summary>
        /// Used for displaying the player's inventory. 
        /// </summary>
        private InventoryMenu inventoryMenu;

        /// <summary>
        /// Used for configuring settings or quitting the game.
        /// </summary>
        private PauseMenu pauseMenu; 

        /// <summary>
        /// What will be dealing with drawing to the screen. 
        /// </summary>
        private Painter painter;

        /// <summary>
        /// The thing generating the terrain. Used for collision detection. 
        /// </summary>
        public ChunkGenerator cg; 

        /// <summary>
        /// Represents the player character. Contains a reference to the 
        /// camera, the thing the player is seeing through. 
        /// </summary>
        public Player player;

        /// <summary>
        /// All of the meshes/models being used in this scene currently. 
        /// </summary>
        // private Mesh[] meshes;
        private ListUtil<Mesh> meshList;
        private Dictionary<Mesh, ListNode<Mesh>> meshListNodeDict;

        /// <summary>
        /// How far away a polygon has to be before we
        /// draw its average color instead of its image.
        /// </summary>
        private float AverageColorDrawDistance = 1f;

        /// <summary>
        /// How many triangles are currently in this scene. 
        /// </summary>
        private int numTriangles;

        /// <summary>
        /// The projection matrix, that will be used for projecting items to
        /// the screen. 
        /// </summary>
        private Matrix matProj;

        /// <summary>
        /// A test for day/night cycles. Represents the sun and the moon. 
        /// </summary>
        private Satellite[] satellites;

        /// <summary>
        /// A test for the skybox. 
        /// </summary>
        private SkyBox skybox;

        private float NormalShadowImportance = 0.25f;
        private float SunShadowImportance = 0.75f;

        public Texture2D lastWorldTexture, lastSkyTexture;

        public Light[] lights;
        private bool turnOnLights;

        public SoundEffect Vil;
        public SoundEffect com;
        public SoundEffect rew;
        /// <summary>
        /// Monster test. 
        /// </summary>
        public static MeleeAttacker[] monsters;

        /// <summary>
        /// A list of all of the items to draw in world-space. If they are
        /// picked up, they are removed from this list. 
        /// </summary>
        private LinkedList<Item> itemsInWorld;

        private GraphicsDevice graphicsDevice;

        private NPC[] npcs;

        private DialogueBox dialogueBox;

        private Compass compass;

        /// <summary>
        /// Creates a manager that will construct necessary fields for use
        /// with the methods. 
        /// </summary>
        public JManager(Game host, int width, int height, GraphicsDeviceManager graphicsDeviceManager, ChunkGenerator cg, SpriteBatch spriteBatch)
        {
            // Assigns the last keyboard state
            lastKb = Keyboard.GetState();
            lastMs = Mouse.GetState(); 

            // Assigns the host
            this.host = host; 

            // Set the correct dimensions of the screen.
            Width = width;
            Height = height;

            DrawWidth = (int)(width / 4);
            DrawHeight = (int)(height / 4);

            centerX = width / 2;
            centerY = height / 2;

            this.cg = cg;

            gameState = GameState.Playing;

            graphicsDevice = graphicsDeviceManager.GraphicsDevice; 

            painter = new Painter(width, height, graphicsDevice, spriteBatch);

            // Create the skybox test
            skybox = new SkyBox(
                new WeatherAndTime[] { WeatherAndTime.Day, WeatherAndTime.Night },
                new string[] { @"Images/skybox", @"Images/night" },
                2048,
                1536);

            // Create the models
            meshList = new ListUtil<Mesh>();
            meshListNodeDict = new Dictionary<Mesh, ListNode<Mesh>>();

            numTriangles = 0;
            ListNode<Mesh> meshNode = meshList.list;
            while (meshNode != null)
            {
                numTriangles += meshNode.dat.Triangles.Length;
                meshNode = meshNode.next;
            }

            // Projection matrix
            matProj = Matrix.MakeProjection(FOV, (float)height / width, 0.1f, 1000);

            // Create the player and their camera 
            Camera camera = new Camera(0, 0, 0);
            camera.yaw = 0;
            camera.pitch = 0;
            player = new Player(this, camera);

            // Create the satellite test.
            satellites = new Satellite[2]; 
            satellites[0] = new Satellite((float)(Math.PI / 60 / 120), Load.OneDimImage(@"Images/sun"), 2000, 2000, (float)(Math.PI * 2), 1500, 1500, 800);
            satellites[1] = new Satellite((float)(Math.PI / 60 / 120), Load.OneDimImage(@"Images/moon"), 1859, 1897, (float)(Math.PI / 2 * 3), 500, 500, 800);

            lights = new Light[1];
            lights[0] = new Light(100f, null);
            turnOnLights = false;

            Random random = new Random();
            monsters = new MeleeAttacker[1];
            for (int k = 0; k < monsters.Length; k++)
            {
                Vec4 monsterLoc = camera.loc.Clone();
                monsterLoc.X += 200 + random.Next(-100, 100);
                monsterLoc.Z += 200 + random.Next(-100, 100);
                monsters[k] = new MeleeAttacker(Load.Mesh(@"Content/Models/cube.obj", 20, 0, 0, 0), monsterLoc, Camera.NormalSpeed * 0.666f, 5, 100, 100, cg);
                monsters[k].Mesh.SetColor(Color.LightBlue);
                monsters[k].noise = com;
                AddMesh(monsters[k].Mesh);
            }

            itemsInWorld = new LinkedList<Item>();
            Vec4 itemLoc = player.Camera.loc.Clone();
            itemLoc.X += 100; 
            itemsInWorld.AddLast(new DefaultItem(itemLoc, cg));

            inventoryMenu = new InventoryMenu(player.Inventory, graphicsDevice, Width, Height);
            pauseMenu = new PauseMenu(Width, Height);

            //float h = cg.GetHeightAt(itemLoc.X, itemLoc.Z); 
            //cube = Load.Mesh(@"Content/Models/cube.obj", 25, itemLoc.X, h, itemLoc.Z);
            //AddMesh(cube); 

            npcs = new NPC[5]; 

            for (int k = 0; k < npcs.Length; k++)
            {
                npcs[k] = new NPC(new JModelling.Vec4(-100 + random.Next(0,100), 0, -100 + random.Next(0,100)), new string[] { "Thanks for dealing with those Zombies!", "Here is a new sword as a reward!"}, -1, -1, -1, new int[] { 1 });
                npcs[k].Mesh.SetColor(Color.LightBlue);
                dialogueBox = null;
                DialogueBox.Init(Width, Height);
            }

            Compass.Init(camera, Width, Height); 
            compass = new Compass(monsters[0].Loc); 
        }

        /// <summary>
        /// Adds a mesh to the render engine. The method will return
        /// a <see cref="ListNode{T}"/> which is used to store this mesh
        /// in a linked list.
        /// </summary>
        /// <param name="mesh"> Mesh to add </param>
        /// <returns>The list node of this mesh</returns>
        public ListNode<Mesh> AddMesh(Mesh mesh)
        {
            ListNode<Mesh> node = new ListNode<Mesh>(mesh);
            meshList.Add(node);
            numTriangles += mesh.Triangles.Length;

            meshListNodeDict.Add(mesh, node);

            return node;
        }

        /// <summary>
        /// Removes a <see cref="ListNode{Mesh}"/> from the render engine
        /// </summary>
        /// <param name="mesh"> <see cref="ListNode"/> to remove </param>
        public void RemoveMesh(ListNode<Mesh> mesh)
        {
            mesh.Remove();
        }

        /// <summary>
        /// Removes a mesh from the render engine
        /// </summary>
        /// <param name="mesh"> <see cref="Mesh"/> to remove </param>
        public void RemoveMesh(Mesh mesh)
        {
            ListNode<Mesh> node = meshListNodeDict[mesh];

            if (node != null)
                node.Remove();
        }

        /// <summary>
        /// Will be called every time the program needs to update. 
        /// </summary>
        public void Update()
        {
            switch (gameState)
            {
                case GameState.Playing:
                    UpdatePlaying();
                    break;

                case GameState.Inventory:
                    UpdateInventory();
                    break;

                case GameState.Paused:
                    UpdatePaused();
                    break;

                case GameState.Talking:
                    UpdateTalking();
                    break; 
            }
            
            lastKb = Keyboard.GetState(); 
        }

        private void UpdatePlaying()
        {
            // TEST ILLUMINATION
            float shadow = -0.347f * (satellites[0].Angle * satellites[0].Angle) + 1.091f * satellites[0].Angle - 0.028f;
            if (shadow < 0) shadow = 0;
            else if (shadow > 1) shadow = 1;

            // Test hues
            Hue hue = GetHue(satellites[0]);

            Vec4 lightDirection = satellites[0].Loc;
            lightDirection.Normalize();

            // Store camera value for quick accessing
            Camera camera = player.Camera;

            UpdatePlayingInputs();
            skybox.Update(camera.loc);

            compass.Update(); 
       
            for (int k = 0; k < monsters.Length; k++)
            {
                MeleeAttacker monster = monsters[k]; 
                if (monster != null)
                {
                    // If creature died, remove them and drop their items
                    if (monster.Health <= 0)
                    {
                        Vec4 itemLoc = monster.Loc;
                        itemLoc.Y = cg.GetHeightAt(itemLoc.X, itemLoc.Z) + 10;
                        foreach (Item item in monster.DroppedItems)
                        {
                            item.SetInWorldSpace(itemLoc);
                            itemsInWorld.AddLast(item);
                        }
                        RemoveMesh(monster.Mesh);
                        monsters[k] = null;
                    }
                    else // Update monster's movements and actions
                    {
                        monster.Update(player, cg);
                    }
                }
            }

            foreach (NPC npc in npcs)
            {
                npc.Update(player, cg);
            }

            // Update items to bob up and down
            LinkedList<Item> itemsToRemove = new LinkedList<Item>(); 
            foreach (Item item in itemsInWorld)
            {
                // If it's true, the player picked it up
                if (item.Update(player))
                {
                    itemsToRemove.AddLast(item); 
                }
            }
            foreach (Item item in itemsToRemove)
            {
                itemsInWorld.Remove(item); 
            }

            // Set light to player's location
            lights[0].Loc = camera.loc;

            satellites[0].Step(camera.loc);
            satellites[1].Step(camera.loc);

            painter.CreateCanvas(DrawWidth, DrawHeight);

            float[,] depthBuffer = new float[DrawWidth, DrawHeight];

            Vec4 target = new Vec4(0, -camera.pitch, 1);
            Vec4 lookDir = camera.GetLookDir(target);
            target = camera.loc + lookDir;
            camera.lookDir = lookDir;

            Matrix matView = Matrix.PointAt(camera.loc, target, new Vec4(0, 1, 0));
            matView.QuickInverse();

            Light[] activeLights = (turnOnLights) ? lights : new Light[] { };

            foreach (Satellite satellite in satellites)
            {
                satellite.DrawToCanvas(
                    camera, painter,
                    matView, matProj,
                    DrawWidth, DrawHeight);
            }

            ListNode<Mesh> meshNode = meshList.list;
            while (meshNode != null)
            {
                Mesh mesh = meshNode.dat;
                meshNode = meshNode.next;

                if (mesh == null)
                {
                    continue;
                }
                if (mesh.Triangles == null)
                {
                    continue;
                }

                DrawTrianglesToPainterCanvas(painter, depthBuffer, GetDrawableTrianglesFromMesh(mesh, hue, activeLights, matView, lightDirection, shadow));
            }

            foreach (Item item in itemsInWorld)
            {
                item.DrawToCanvas(camera, painter, depthBuffer, matView, matProj, DrawWidth, DrawHeight);
            }

            for (int k = 0; k < monsters.Length; k++)
            {
                MeleeAttacker monster = monsters[k]; 
                if (monster != null)
                {
                    monster.Mesh.MoveTo(monster.Loc.X, monster.Loc.Y, monster.Loc.Z);
                    DrawTrianglesToPainterCanvas(painter, depthBuffer, GetDrawableTrianglesFromMesh(monster.Mesh, hue, activeLights, matView, lightDirection, shadow));
                }
            }
            
            foreach (NPC npc in npcs)
            {
                npc.Mesh.MoveTo(npc.Loc.X, npc.Loc.Y, npc.Loc.Z);
                DrawTrianglesToPainterCanvas(painter, depthBuffer, GetDrawableTrianglesFromMesh(npc.Mesh, hue, activeLights, matView, lightDirection, shadow));
            }

            lastWorldTexture = painter.GetCanvas();
        }

        private void UpdateInventory()
        {
            UpdateInventoryInputs();    
        }

        private void UpdatePaused()
        {
            MouseState ms = Mouse.GetState(); 
            pauseMenu.Update(ms, lastMs);
            UpdatePauseInputs();
            lastMs = ms; 
        }

        public void Talk(TalkingCreature source)
        {
            gameState = GameState.Talking;
            dialogueBox = new DialogueBox(lastWorldTexture, lastSkyTexture, source);
            isMouseFocused = false;
            host.IsMouseVisible = true;
        }

        private void UpdateTalking()
        {
            UpdateTalkingInputs();
        }

        /// <summary>
        /// Draws what the JManager has loaded to the screen. 
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    DrawPlaying(spriteBatch);
                    break;

                case GameState.Inventory:
                    inventoryMenu.Draw(spriteBatch);
                    break;

                case GameState.Paused:
                    pauseMenu.Draw(spriteBatch);
                    break;

                case GameState.Talking:
                    dialogueBox.Draw(spriteBatch);
                    break; 
            }
        }

        private void DrawPlaying(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetSkyboxTexture(player.Camera), new Rectangle(0, 0, Width, Height), Color.White);
            spriteBatch.Draw(GetWorldTexture(), new Rectangle(0, 0, Width, Height), Color.White);
            compass.Draw(spriteBatch); 
        }

        public Texture2D GetWorldTexture()
        {
            return lastWorldTexture; 
        }

        public Texture2D GetSkyboxTexture(Camera camera)
        {
            painter.CreateCanvas(DrawWidth, DrawHeight);

            Vec4 target = new Vec4(0, -camera.pitch, 1);
            Vec4 lookDir = camera.GetLookDir(target);
            target = camera.loc + lookDir;
            camera.lookDir = lookDir;

            Matrix matView = Matrix.PointAt(camera.loc, target, new Vec4(0, 1, 0));
            matView.QuickInverse();

            skybox.DrawToCanvas(painter, camera, matView, matProj, DrawWidth, DrawHeight, GetHue(satellites[0]));

            lastSkyTexture = painter.GetCanvas(); 
            return lastSkyTexture; 
        }

        private List<Triangle> GetDrawableTrianglesFromMesh(Mesh mesh, Hue hue, Light[] activeLights, Matrix matView, Vec4 lightDirection, float shadow)
        {
            Queue<Triangle> triangles = new Queue<Triangle>();
            int triIndex = 0; 
            
            foreach (Triangle original in mesh.Triangles)
            {
                // Get ray from triangle to camera
                Vec4 cameraRay = original.Points[0] - player.Camera.loc;
                
                // If ray is aligned with normal, then triangle is visible
                if (Vec4.DotProduct(original.Normal, cameraRay) < 0)
                {
                    // ILLUMINATION TEST
                    float alpha = 0;

                    // Clone triangle so we can edit and not need to make new
                    // triangle objects.
                    Triangle tri = original.Clone();

                    switch (hue.Status)
                    {
                        case (WeatherAndTime.Day):
                            alpha = Math.Max(0.2f, Vec4.DotProduct(lightDirection, original.Normal));
                            alpha = alpha * SunShadowImportance + shadow * NormalShadowImportance;
                            tri.Color = Color.Lerp(tri.Color, hue.Color, hue.Amount);
                            break;

                        case (WeatherAndTime.Night):
                            alpha = Vec4.DotProduct(new Vec4(0, 1, 0), original.Normal) / 4;
                            tri.Color = Color.Lerp(tri.Color, hue.Color, hue.Amount);
                            break;

                        case (WeatherAndTime.Dusk):
                            float dayAlpha = Math.Max(0.2f, Vec4.DotProduct(lightDirection, original.Normal));
                            dayAlpha = dayAlpha * SunShadowImportance + shadow * NormalShadowImportance;
                            float nightAlpha = Vec4.DotProduct(new Vec4(0, 1, 0), original.Normal) / 4;

                            alpha = (1f - hue.Amount) * dayAlpha + hue.Amount * nightAlpha;

                            break;

                        case (WeatherAndTime.Dawn):
                            dayAlpha = Math.Max(0.2f, Vec4.DotProduct(lightDirection, original.Normal));
                            dayAlpha = dayAlpha * SunShadowImportance + shadow * NormalShadowImportance;
                            nightAlpha = Vec4.DotProduct(new Vec4(0, 1, 0), original.Normal) / 4;

                            alpha = (1f - hue.Amount) * nightAlpha + hue.Amount * dayAlpha;

                            break;
                    }

                    foreach (Light light in activeLights)
                    {
                        float dist = MathExtensions.Dist(lights[0].Loc, Vec4.Average(new Vec4[] { tri.Points[0], tri.Points[1], tri.Points[2] }));
                        if (dist <= light.DistAway + light.DistAway / 5)
                        {
                            // Light polygons depending on distance to light.
                            float lightAlpha = light.CalcAlphaFromDist(dist);

                            if (lightAlpha > 0.2)
                            {
                                // Average light and alpha together. 
                                alpha = (alpha + lightAlpha) / 2;

                                tri.Color = Color.Lerp(tri.Color, Color.Yellow, alpha);
                            }
                        }
                    }

                    // Convert World Space to View Space
                    tri.TimesEquals(matView);

                    // Clip viewed triangle against near plane (this could
                    // form two additional triangles)
                    Triangle[] clipped = Vec4.TriangleClipAgainstPlane(
                        new Vec4(0, 0, 0.1f),
                        new Vec4(0, 0, 1),
                        tri);

                    foreach (Triangle clippedTri in clipped)
                    {
                        // Project triangle from 3D to 2D
                        clippedTri.TimesEquals(matProj);
                        clippedTri.DivideTexel(0, clippedTri.Points[0].W);
                        clippedTri.DivideTexel(1, clippedTri.Points[1].W);
                        clippedTri.DivideTexel(2, clippedTri.Points[2].W);

                        // Scale into view 
                        clippedTri.Points[0].DivideEquals(clippedTri.Points[0].W);
                        clippedTri.Points[1].DivideEquals(clippedTri.Points[1].W);
                        clippedTri.Points[2].DivideEquals(clippedTri.Points[2].W);

                        // X/Y are inverted so put them back 
                        clippedTri.Points[0].X *= -1;
                        clippedTri.Points[1].X *= -1;
                        clippedTri.Points[2].X *= -1;
                        clippedTri.Points[0].Y *= -1;
                        clippedTri.Points[1].Y *= -1;
                        clippedTri.Points[2].Y *= -1;

                        // Offset verts into visible normalized space 
                        Vec4 offsetView = new Vec4(1, 1, 0);
                        clippedTri.Points[0].PlusEquals(offsetView);
                        clippedTri.Points[1].PlusEquals(offsetView);
                        clippedTri.Points[2].PlusEquals(offsetView);

                        clippedTri.Points[0].X *= 0.5f * DrawWidth; clippedTri.Points[0].Y *= 0.5f * DrawHeight;
                        clippedTri.Points[1].X *= 0.5f * DrawWidth; clippedTri.Points[1].Y *= 0.5f * DrawHeight;
                        clippedTri.Points[2].X *= 0.5f * DrawWidth; clippedTri.Points[2].Y *= 0.5f * DrawHeight;

                        clippedTri.ClosestDist = clippedTri.Texels[0].W;
                        if (clippedTri.Texels[1].W < clippedTri.ClosestDist)
                        {
                            clippedTri.ClosestDist = clippedTri.Texels[1].W;
                        }
                        if (clippedTri.Texels[2].W < clippedTri.ClosestDist)
                        {
                            clippedTri.ClosestDist = clippedTri.Texels[2].W;
                        }

                        // Illumination
                        // float alpha = Math.Max(0, (float)(1 - Dist(camera.loc, tri.Points[0]) / 160));
                        clippedTri.Alpha = alpha;

                        triangles.Enqueue(clippedTri);
                        triIndex++; 
                    }
                }
            }

            List<Triangle> drawableTriangles = new List<Triangle>(); 
            while (triangles.Count > 0)
            {
                Queue<Triangle> listTriangles = new Queue<Triangle>();
                int nNewTriangles = 1;

                listTriangles.Enqueue(triangles.Dequeue());
                for (int p = 0; p < 4; p++)
                {
                    while (nNewTriangles > 0)
                    {
                        Triangle[] clipped = null;

                        // Take triangle from front of queue
                        Triangle test = listTriangles.Dequeue();
                        nNewTriangles--;

                        // Clip it against a plane 
                        switch (p)
                        {
                            case 0:
                                clipped = Vec4.TriangleClipAgainstPlane(
                                    new Vec4(0, 0, 0),
                                    new Vec4(0, 1, 0),
                                    test);
                                break;

                            case 1:
                                clipped = Vec4.TriangleClipAgainstPlane(
                                    new Vec4(0, DrawHeight - 1, 0),
                                    new Vec4(0, -1, 0),
                                    test);
                                break;

                            case 2:
                                clipped = Vec4.TriangleClipAgainstPlane(
                                    new Vec4(0, 0, 0),
                                    new Vec4(1, 0, 0),
                                    test);
                                break;

                            case 3:
                                clipped = Vec4.TriangleClipAgainstPlane(
                                    new Vec4(DrawWidth - 1, 0, 0),
                                    new Vec4(-1, 0, 0),
                                    test);
                                break;
                        }

                        // Add clipped triangles to back of queue to 
                        // test again. 
                        for (int w = 0; w < clipped.Length; w++)
                        {
                            listTriangles.Enqueue(clipped[w]);
                        }
                    }
                    nNewTriangles = listTriangles.Count;
                }

                foreach (Triangle tri in listTriangles)
                {
                    // REFACTOR THIS: add queues to the list instead of individual triangles,
                    // Check to see if it flows well with garbage collector. 
                    drawableTriangles.Add(tri);
                }
            }

            return drawableTriangles;
        }

        private void DrawTrianglesToPainterCanvas(Painter painter, float[,] depthBuffer, List<Triangle> triangles)
        {
            foreach (Triangle t in triangles)
            {
                if (t.ClosestDist < AverageColorDrawDistance)
                    UntexturedTriangle(
                        (int)t.Points[0].X, (int)t.Points[0].Y,
                        (int)t.Points[1].X, (int)t.Points[1].Y,
                        (int)t.Points[2].X, (int)t.Points[2].Y,
                        new Color(
                            (int)(t.Color.R * t.Alpha),
                            (int)(t.Color.G * t.Alpha),
                            (int)(t.Color.B * t.Alpha)),
                        painter,
                        t.ClosestDist,
                        depthBuffer);
                else
                    TexturedTriangle(
                        (int)t.Points[0].X, (int)t.Points[0].Y,
                        (int)t.Points[1].X, (int)t.Points[1].Y,
                        (int)t.Points[2].X, (int)t.Points[2].Y,
                        t.Texels[0].U, t.Texels[0].V, t.Texels[0].W,
                        t.Texels[1].U, t.Texels[1].V, t.Texels[1].W,
                        t.Texels[2].U, t.Texels[2].V, t.Texels[2].W,
                        t.Alpha,
                        t.Image,
                        painter,
                        depthBuffer);

                //Polygon((int)t.Points[0].X, (int)t.Points[0].Y,
                //        (int)t.Points[1].X, (int)t.Points[1].Y,
                //        (int)t.Points[2].X, (int)t.Points[2].Y);
            }
        }

        private Hue GetHue(Satellite sun)
        { 
            float sunAngleLoc = sun.Angle;

            // Dawn
            if (sunAngleLoc > Hue.DawnStart)
            {
                return new Hue(new Color(0, 0, 0, 0), WeatherAndTime.Dawn, (sunAngleLoc - Hue.DawnStart) / Hue.DawnLength, false);
            }
            else if (sunAngleLoc < Hue.DawnEndWrapped)
            {
                float sunAngleTemp = sunAngleLoc + PITimesTwo;
                return new Hue(new Color(0, 0, 0, 0), WeatherAndTime.Dawn, (sunAngleTemp - Hue.DawnStart) / Hue.DawnLength, false);
            }

            // Day
            else if (sunAngleLoc > Hue.DayStart && sunAngleLoc < Hue.DayEnd)
            {
                return new Hue(Color.Yellow, WeatherAndTime.Day, 0f, true);
            } 

            // Dusk
            else if (sunAngleLoc < Hue.DuskEnd)
            {
                return new Hue(new Color(0, 0, 0, 0), WeatherAndTime.Dusk, (sunAngleLoc - Hue.DuskStart) / Hue.DuskLength, true); 
            }

            // Night
            else if (sunAngleLoc < Hue.NightEnd)
            {
                float amount = -0.360f * (sunAngleLoc * sunAngleLoc) + 3.395f * sunAngleLoc - 7.5f; 
                return new Hue(Hue.NightColor, WeatherAndTime.Night, amount, false); 
            }

            // Not implemented
            else
            {
                return null; 
            }
        }

        private void Polygon(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            painter.DrawLine(x1, y1, x2, y2, Color.White);
            painter.DrawLine(x2, y2, x3, y3, Color.White);
            painter.DrawLine(x3, y3, x1, y1, Color.White);
        }

        private void UntexturedTriangle(
            int x1, int y1, int x2, int y2, int x3, int y3,
            Color color, Painter painter, float depth, float[,] depthBuffer)
        {
            if (y2 < y1)
            {
                // Swap y1 and y2
                int tempi = y1;
                y1 = y2;
                y2 = tempi;

                // Swap x1 and x2 
                tempi = x1;
                x1 = x2;
                x2 = tempi;
            }
            if (y3 < y1)
            {
                // Swap y1 and y3
                int tempi = y1;
                y1 = y3;
                y3 = tempi;

                // Swap x1 and x3 
                tempi = x1;
                x1 = x3;
                x3 = tempi;
            }
            if (y3 < y2)
            {
                // Swap y2 and y3
                int tempi = y2;
                y2 = y3;
                y3 = tempi;

                // Swap x2 and x3 
                tempi = x2;
                x2 = x3;
                x3 = tempi;
            }

            int dy1 = y2 - y1;
            int dx1 = x2 - x1;

            int dy2 = y3 - y1;
            int dx2 = x3 - x1;

            float daxStep = 0, dbxStep = 0;

            if (dy1 != 0) daxStep = dx1 / (float)Math.Abs(dy1);
            if (dy2 != 0) dbxStep = dx2 / (float)Math.Abs(dy2);

            if (dy1 != 0)
            {
                for (int i = y1; i <= y2; i++)
                {
                    int ax = (int)(x1 + (float)(i - y1) * daxStep);
                    int bx = (int)(x1 + (float)(i - y1) * dbxStep);

                    if (ax > bx)
                    {
                        // Swap ax and bx
                        int tempi = ax;
                        ax = bx;
                        bx = tempi;
                    }

                    for (int j = ax; j < bx; j++)
                    {
                        if (depth > depthBuffer[j, i])
                        {
                            painter.SetPixel(j, i, color);
                            depthBuffer[j, i] = depth;
                        }
                    }
                }
            }

            dy1 = y3 - y2;
            dx1 = x3 - x2;

            if (dy1 != 0) daxStep = dx1 / (float)Math.Abs(dy1);
            if (dy2 != 0) dbxStep = dx2 / (float)Math.Abs(dy2);

            if (dy1 != 0)
            {
                for (int i = y2; i <= y3; i++)
                {
                    int ax = (int)(x2 + (float)(i - y2) * daxStep);
                    int bx = (int)(x1 + (float)(i - y1) * dbxStep);

                    if (ax > bx)
                    {
                        // Swap ax and bx
                        int tempi = ax;
                        ax = bx;
                        bx = tempi;
                    }

                    for (int j = ax; j < bx; j++)
                    {
                        if (depth > depthBuffer[j, i])
                        {
                            painter.SetPixel(j, i, color);
                            depthBuffer[j, i] = depth;
                        }
                    }
                }
            }
        }

        private void TexturedTriangle(
            int x1, int y1, int x2, int y2, int x3, int y3,
            float u1, float v1, float w1,
            float u2, float v2, float w2,
            float u3, float v3, float w3,             
            float alpha, Color[,] sprite, Painter painter, float[,] depthBuffer)
        {
            if (y2 < y1)
            {
                // Swap y1 and y2
                int tempi = y1;
                y1 = y2;
                y2 = tempi;

                // Swap x1 and x2 
                tempi = x1;
                x1 = x2;
                x2 = tempi;

                // Swap u1 and u2
                float tempf = u1;
                u1 = u2;
                u2 = tempf;

                // Swap v1 and v2
                tempf = v1;
                v1 = v2;
                v2 = tempf;

                // Swap w1 and w2
                tempf = w1;
                w1 = w2;
                w2 = tempf; 
            }
            if (y3 < y1)
            {
                // Swap y1 and y3
                int tempi = y1;
                y1 = y3;
                y3 = tempi;

                // Swap x1 and x3 
                tempi = x1;
                x1 = x3;
                x3 = tempi;

                // Swap u1 and u3
                float tempf = u1;
                u1 = u3;
                u3 = tempf;

                // Swap v1 and v3
                tempf = v1;
                v1 = v3;
                v3 = tempf;

                // Swap w1 and w3
                tempf = w1;
                w1 = w3;
                w3 = tempf;
            }
            if (y3 < y2)
            {
                // Swap y2 and y3
                int tempi = y2;
                y2 = y3;
                y3 = tempi;

                // Swap x2 and x3 
                tempi = x2;
                x2 = x3;
                x3 = tempi;

                // Swap u2 and u3
                float tempf = u2;
                u2 = u3;
                u3 = tempf;

                // Swap v2 and v3
                tempf = v2;
                v2 = v3;
                v3 = tempf;

                // Swap w2 and w3
                tempf = w2;
                w2 = w3;
                w3 = tempf;
            }

            int dy1 = y2 - y1;
            int dx1 = x2 - x1;
            float du1 = u2 - u1; 
            float dv1 = v2 - v1;
            float dw1 = w2 - w1; 

            int dy2 = y3 - y1;
            int dx2 = x3 - x1;
            float du2 = u3 - u1;
            float dv2 = v3 - v1;
            float dw2 = w3 - w1; 

            float daxStep = 0, dbxStep = 0,
                  du1Step = 0, dv1Step = 0, 
                  du2Step = 0, dv2Step = 0,
                  dw1Step = 0, dw2Step = 0;

            if (dy2 != 0)
            {
                float dy2Abs = (float)Math.Abs(dy2);
                dbxStep = dx2 / dy2Abs;
                du2Step = du2 / dy2Abs;
                dv2Step = dv2 / dy2Abs;
                dw2Step = dw2 / dy2Abs;
            }

            if (dy1 != 0)
            {
                float dy1Abs = (float)Math.Abs(dy1);
                daxStep = dx1 / dy1Abs;
                du1Step = du1 / dy1Abs;
                dv1Step = dv1 / dy1Abs;
                dw1Step = dw1 / dy1Abs;

                for (int i = y1; i <= y2; i++)
                {
                    float iDiff = (float)(i - y1); 

                    int ax = (int)(x1 + iDiff * daxStep);
                    int bx = (int)(x1 + iDiff * dbxStep);

                    float texSu = u1 + iDiff * du1Step;
                    float texSv = v1 + iDiff * dv1Step;
                    float texSw = w1 + iDiff * dw1Step;

                    float texEu = u1 + iDiff * du2Step;
                    float texEv = v1 + iDiff * dv2Step;
                    float texEw = w1 + iDiff * dw2Step;

                    if (ax > bx)
                    {
                        // Swap ax and bx
                        int tempi = ax;
                        ax = bx;
                        bx = tempi;

                        // Swap texSu and texEu 
                        float tempf = texSu;
                        texSu = texEu;
                        texEu = tempf;

                        // Swap texSv and texEv 
                        tempf = texSv;
                        texSv = texEv;
                        texEv = tempf;

                        // Swap texSw and texEw 
                        tempf = texSw;
                        texSw = texEw;
                        texEw = tempf;
                    }

                    float texU = texSu;
                    float texV = texSv;
                    float texW = texSw;

                    float tStep = 1f / (float)(bx - ax);
                    float t = 0f; 

                    for (int j = ax; j < bx; j++)
                    {
                        texU = (1f - t) * texSu + t * texEu;
                        texV = (1f - t) * texSv + t * texEv;
                        texW = (1f - t) * texSw + t * texEw;

                        if (texW > depthBuffer[j, i])
                        {
                            int spriteLocU = (int)(texU / texW * (sprite.GetLength(0) - 1));
                            int spriteLocV = (int)(texV / texW * (sprite.GetLength(1) - 1));

                            Color baseColor = sprite[spriteLocU, spriteLocV];
                            Color color = new Color(
                                (int)(baseColor.R * alpha),
                                (int)(baseColor.G * alpha),
                                (int)(baseColor.B * alpha));

                            painter.SetPixel(j, i, color);
                            depthBuffer[j, i] = texW;
                        }

                        t += tStep; 
                    }
                }
            }

            dy1 = y3 - y2;
            dx1 = x3 - x2;
            du1 = u3 - u2;
            dv1 = v3 - v2;
            dw1 = w3 - w2; 

            if (dy2 != 0)
            {
                dbxStep = dx2 / (float)(Math.Abs(dy2)); 
            }

            du1Step = 0;
            dv1Step = 0;

            if (dy1 != 0)
            {
                float dy1Abs = (float)(Math.Abs(dy1));
                daxStep = dx1 / dy1Abs;
                du1Step = du1 / dy1Abs;
                dv1Step = dv1 / dy1Abs;
                dw1Step = dw1 / dy1Abs;

                for (int i = y2; i <= y3; i++)
                {
                    float iDiff1 = (float)(i - y1);
                    float iDiff2 = (float)(i - y2);

                    int ax = (int)(x2 + iDiff2 * daxStep);
                    int bx = (int)(x1 + iDiff1 * dbxStep);

                    float texSu = u2 + iDiff2 * du1Step;
                    float texSv = v2 + iDiff2 * dv1Step;
                    float texSw = w2 + iDiff2 * dw1Step;

                    float texEu = u1 + iDiff1 * du2Step;
                    float texEv = v1 + iDiff1 * dv2Step;
                    float texEw = w1 + iDiff1 * dw2Step;

                    if (ax > bx)
                    {
                        // Swap ax and bx
                        int tempi = ax;
                        ax = bx;
                        bx = tempi;

                        // Swap texSu and texEu
                        float tempf = texSu;
                        texSu = texEu;
                        texEu = tempf;

                        // Swap texSv and texEv
                        tempf = texSv;
                        texSv = texEv;
                        texEv = tempf;

                        // Swap texSw and texEw
                        tempf = texSw;
                        texSw = texEw;
                        texEw = tempf;
                    }

                    float texU = texSu;
                    float texV = texSv;
                    float texW = texSw;

                    float tStep = 1f / (float)(bx - ax);
                    float t = 0f;

                    for (int j = ax; j < bx; j++)
                    {
                        texU = (1f - t) * texSu + t * texEu;
                        texV = (1f - t) * texSv + t * texEv;
                        texW = (1f - t) * texSw + t * texEw;
                        
                        if (texW > depthBuffer[j, i])
                        {
                            int spriteLocU = (int)(texU / texW * (sprite.GetLength(0) - 1));
                            int spriteLocV = (int)(texV / texW * (sprite.GetLength(1) - 1));

                            Color baseColor = sprite[spriteLocU, spriteLocV];
                            Color color = new Color(
                                (int)(baseColor.R * alpha),
                                (int)(baseColor.G * alpha),
                                (int)(baseColor.B * alpha)); 

                            painter.SetPixel(j, i, color);
                            depthBuffer[j, i] = texW;
                        }

                        t += tStep; 
                    }
                }
            }
        }

        /// <summary>
        /// Runs through the processes of collecting keyboard/mouse inputs
        /// and dealing with them. 
        /// </summary>
        public void UpdatePlayingInputs()
        {
            // Get the current state of the keyboard and mouse. 
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            // Perform any special actions with any special keys. 
            ProcessSpecialPlayingKeyInputs(kb);

            foreach (NPC npc in npcs)
            {
                if (npc.Talk(player, kb) && lastKb.IsKeyUp(Controls.Interact))
                {
                    Vil.Play();
                    Talk(npc);                   
                }
            }
            player.Update(kb, ms);
        }

        /// <summary>
        /// Performs special actions if any important keys are pressed (for
        /// example, if ESC is pressed, then the program will do something
        /// like pause or close). 
        /// </summary>
        private void ProcessSpecialPlayingKeyInputs(KeyboardState kb)
        {
            // If Tab is pressed, unfocus/focus the mouse. 
            if (kb.IsKeyDown(Controls.FocusOrUnfocusMouse) && lastKb.IsKeyUp(Controls.FocusOrUnfocusMouse))
            {
                isMouseFocused = !isMouseFocused;
                host.IsMouseVisible = !isMouseFocused;
            }

            // If LightButton is pressed, turn on/off the light.
            else if (kb.IsKeyDown(Controls.LightButton) && lastKb.IsKeyUp(Controls.LightButton))
            {
                turnOnLights = !turnOnLights; 
            }

            // If DebugButton is pressed, toggle debug menu. 
            else if (kb.IsKeyDown(Controls.DebugButton) && lastKb.IsKeyUp(Controls.DebugButton))
            {
                Game1.DebugEnabled = !Game1.DebugEnabled; 
            }

            // If inventory is pressed, toggle inventory menu.
            else if (kb.IsKeyDown(Controls.Inventory) && lastKb.IsKeyUp(Controls.Inventory))
            {
                gameState = GameState.Inventory;
                inventoryMenu.Create(lastWorldTexture, lastSkyTexture);              
                isMouseFocused = false;
                host.IsMouseVisible = true; 
            }

            // If Pause is pressed, toggle pause menu. 
            else if (kb.IsKeyDown(Controls.Pause) && lastKb.IsKeyUp(Controls.Pause))
            {
                gameState = GameState.Paused;
                pauseMenu.Create(lastWorldTexture, lastSkyTexture); 
                isMouseFocused = false;
                host.IsMouseVisible = true; 
            }
        }

        /// <summary>
        /// Updates the inputs coming specifically from the inventory screen. 
        /// </summary>
        private void UpdateInventoryInputs()
        {
            // Gets the keyboard and mouse inputs/location 
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            ProcessSpecialInventoryKeyInputs(kb); 
        }

        /// <summary>
        /// Processes any special keys the user presses while on the Inventory 
        /// screen. 
        /// </summary>
        private void ProcessSpecialInventoryKeyInputs(KeyboardState kb)
        {
            // If the inventory button is down, return player to Playing state 
            if (kb.IsKeyDown(Controls.Inventory) && lastKb.IsKeyUp(Controls.Inventory))
            {
                gameState = GameState.Playing;

                isMouseFocused = true; 
                host.IsMouseVisible = false;
                Mouse.SetPosition(centerX, centerY); 
            }
        }

        /// <summary>
        /// Updates the inputs coming specifically from the pause screen. 
        /// </summary>
        private void UpdatePauseInputs()
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            ProcessSpecialPauseKeyInputs(kb); 
        }

        /// <summary>
        /// Processes any special keys the user presses while on the Pause
        /// screen. 
        /// </summary>
        private void ProcessSpecialPauseKeyInputs(KeyboardState kb)
        {
            // If the pause button is down, reset the player back to the playing screen. 
            if (kb.IsKeyDown(Controls.Pause) && lastKb.IsKeyUp(Controls.Pause))
            {
                gameState = GameState.Playing;

                isMouseFocused = true;
                host.IsMouseVisible = false;
                Mouse.SetPosition(centerX, centerY);
            }
        }

        private void UpdateTalkingInputs()
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();                       
            if (dialogueBox.Update(kb, lastKb, ms, lastMs))
            {
                gameState = GameState.Playing;
                Mouse.SetPosition(centerX, centerY);
                isMouseFocused = true;
                host.IsMouseVisible = false;
            }
            ProcessSpecialTalkingKeyInputs(kb); 

            lastKb = kb;
            lastMs = ms; 
        }

        private void ProcessSpecialTalkingKeyInputs(KeyboardState kb)
        {
            if (kb.IsKeyDown(Controls.Quit))
            {
                gameState = GameState.Playing;
                Mouse.SetPosition(centerX, centerY);
                isMouseFocused = true;
                host.IsMouseVisible = false;
            }
        }
    }
}
