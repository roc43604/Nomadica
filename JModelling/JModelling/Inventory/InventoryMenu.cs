using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Inventory
{
    /// <summary>
    /// The menu that shows all the player's inventory items. 
    /// </summary>
    class InventoryMenu
    {
        /// <summary>
        /// How much of the screen should be dedicated to empty space. For
        /// instance, if EdgeBufferMultiple.X = 10, then 1/20 of the left 
        /// side of the screen is empty, and 1/20 of the right side of the
        /// screen is empty.
        /// </summary>
        private readonly Vector2 EdgeBufferMultiple = new Vector2(10, 10);

        /// <summary>
        /// Follows the same rules as EdgeBufferMultiple, but this one is
        /// for the edge of the menu and the start of the items. 
        /// </summary>
        private readonly Vector2 EdgeOfMenuAndItemBufferMultiple = new Vector2(10, 10);

        /// <summary>
        /// A number saying what amount of the screen (1 / ItemBufferMultiple) 
        /// is dedicated to the amount of space between two items. 
        /// </summary>
        private const int ItemBufferMultiple = 10;

        private Texture2D WhiteTexture, GrayTexture;

        /// <summary>
        /// The inventory we're drawing to the screen. 
        /// </summary>
        private Inventory inventory; 

        /// <summary>
        /// The bounds of the inventory menu. 
        /// </summary>
        private Rectangle menuBounds;

        /// <summary>
        /// The start drawing area of the items. 
        /// </summary>
        private Vector2 itemBounds; 

        /// <summary>
        /// How big each inventory item will be. The size of an item will
        /// always be a square. 
        /// </summary>
        private float itemSize;

        /// <summary>
        /// An actual pixel-value of how much space is between each item. 
        /// </summary>
        private Vector2 itemBuffer;

        public InventoryMenu(Inventory inventory, int screenWidth, int screenHeight)
        {
            this.inventory = inventory; 

            InitSizes(screenWidth, screenHeight);
        }

        /// <summary>
        /// Decides how big the menu will be, how big each inventory item will
        /// be, etc. 
        /// </summary>
        private void InitSizes(int screenWidth, int screenHeight)
        {
            int x = (int)(screenWidth / (EdgeBufferMultiple.X / 2)),
                y = (int)(screenHeight / (EdgeBufferMultiple.Y / 2));
            menuBounds = new Rectangle(x, y, (int)(screenWidth - x), (int)(screenHeight - y));

            // xSpace == size of each item on x-axis.
            int xSpace = (int)(screenWidth / Inventory.Size.X - screenWidth / ItemBufferMultiple);

            // Need to check if xSpace can also fit on y-plane. We do this to make sure each
            // item picture can be a square like intended. 
            if (xSpace * Inventory.Size.Y < screenHeight)
            {
                // Picture can be a square as-is. 
                itemBuffer = new Vector2(
                    (menuBounds.Width - (menuBounds.Width / EdgeOfMenuAndItemBufferMultiple.X)) / ItemBufferMultiple,
                    (menuBounds.Height - (menuBounds.Height / EdgeOfMenuAndItemBufferMultiple.Y) - Inventory.Size.Y * xSpace) / (Inventory.Size.Y - 1)
                );

                itemBounds = new Vector2(
                    menuBounds.X + menuBounds.Width / EdgeOfMenuAndItemBufferMultiple.X,
                    menuBounds.Y + menuBounds.Height / EdgeOfMenuAndItemBufferMultiple.Y
                ); 
            }
            else
            {
                // TODO
                // We need to adapt item size to the y-axis instead of the x-axis. 
                int ySpace = (int)(screenHeight / Inventory.Size.Y - screenHeight / ItemBufferMultiple);
            }
        }

        private void LoadImages(ContentManager content)
        {
            WhiteTexture = content.Load<Texture2D>("Images/Inventory/white");
            GrayTexture = content.Load<Texture2D>("Images/Inventory/gray box");
        }

        /// <summary>
        /// Draws this inventory to the screen. 
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < inventory.Items.GetLength(0); x++)
            {
                for (int y = 0; y < inventory.Items.GetLength(1); y++)
                {
                    spriteBatch.Draw(
                        GrayTexture,
                        new Rectangle(
                            (int)(itemBounds.X + x * itemSize + x * itemBuffer.X),
                            (int)(itemBounds.Y + y * itemSize + y * itemBuffer.Y),
                            (int)itemSize,
                            (int)itemSize
                        ),
                        Color.White
                    ); 
                }
            }
        }
    }
}
