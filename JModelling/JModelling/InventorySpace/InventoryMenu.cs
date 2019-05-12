using GraphicsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.InventorySpace
{
    /// <summary>
    /// The menu that shows all the player's inventory items. 
    /// </summary>
    public class InventoryMenu
    {
        /// <summary>
        /// The image behind the menu. 
        /// </summary>
        private Texture2D World, Sky;  

        /// <summary>
        /// Used for drawing solid colors to the screen. 
        /// </summary>
        private static Texture2D WhiteTexture;

        /// <summary>
        /// Used for drawing a border on images to the screen. 
        /// </summary>
        private static Texture2D RoundBox;

        /// <summary>
        /// The thing that's drawing the images to the screen. 
        /// </summary>
        private Painter painter; 

        /// <summary>
        /// The bounds of the entire viewing rectangle. 
        /// </summary>
        private Rectangle windowBounds; 

        /// <summary>
        /// The inventory we're drawing to the screen. 
        /// </summary>
        private Inventory inventory;

        /// <summary>
        /// How much space on the x-axis should be dedicated to empty space. 
        /// </summary>
        private const int MenuStartBufferX = 10;

        /// <summary>
        /// How much space is between each item. 
        /// </summary>
        private const int ItemBuffer = 10; 

        /// <summary>
        /// The draw-area of the inventory menu. 
        /// </summary>
        private Rectangle menuBounds;

        /// <summary>
        /// The length and width of each item icon.
        /// </summary>
        private int itemSize; 

        public InventoryMenu(Painter painter, Inventory inventory, int screenWidth, int screenHeight, Texture2D World, Texture2D Sky)
        {
            this.painter = painter; 
            this.inventory = inventory;
            this.World = World;
            this.Sky = Sky; 
            InitSizes(screenWidth, screenHeight);
            windowBounds = new Rectangle(0, 0, screenWidth, screenHeight); 
        }

        /// <summary>
        /// Decides how big the menu will be, how big each inventory item will
        /// be, etc. 
        /// </summary>
        private void InitSizes(int screenWidth, int screenHeight)
        {
            // The bounds of the menu. The Y and Height components will be figured
            // out later. 
            menuBounds = new Rectangle(
                screenWidth / MenuStartBufferX, 
                0, 
                screenWidth - screenWidth / MenuStartBufferX * 2, 
                0
            );

            itemSize = menuBounds.Width / (int)Inventory.Size.X - ItemBuffer + ItemBuffer / ((int)Inventory.Size.X - 1);

            menuBounds.Height = itemSize * (int)Inventory.Size.Y + ItemBuffer * ((int)Inventory.Size.Y - 1);
            menuBounds.Y = (screenHeight - menuBounds.Height) / 2; 
        }

        public static void LoadImages(ContentManager content)
        {
            WhiteTexture = content.Load<Texture2D>("Images/Inventory/white");
            RoundBox = content.Load<Texture2D>("Images/Inventory/round box"); 
        }

        /// <summary>
        /// Draws this inventory to the screen. 
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sky, windowBounds, Color.Gray);
            spriteBatch.Draw(World, windowBounds, Color.Gray);
            for (int x = 0; x < inventory.Items.GetLength(0); x++)
            {
                for (int y = 0; y < inventory.Items.GetLength(1); y++)
                {
                    Rectangle loc = new Rectangle(
                            (menuBounds.X + x * itemSize + x * ItemBuffer),
                            (menuBounds.Y + y * itemSize + y * ItemBuffer),
                            itemSize,
                            itemSize
                    );

                    // Border
                    spriteBatch.Draw(RoundBox, loc, Color.White);

                    Rectangle itemLoc = new Rectangle(loc.X + 5, loc.Y + 5, loc.Width - 10, loc.Height - 10);

                    // Item image
                    Item item = inventory.Items[x, y]; 
                    if (item != null)
                    {
                        Texture2D tex = new Texture2D(painter.graphicsDevice, item.TextureWidth, item.TextureHeight);
                        tex.SetData<Color>(item.Texture);
                        spriteBatch.Draw(tex, itemLoc, Color.White); 
                        //painter.DrawImage(item.Texture, item.TextureWidth, item.TextureHeight, loc); 
                    }
                }
            }
        }
    }
}
