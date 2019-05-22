using JModelling.InventorySpace;
using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using JModelling.Pause;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.GUI
{
    public class DeadMenu
    {
        private JManager source;
        private ChunkGenerator cg;  

        private SpriteFont font;

        private Rectangle screenBounds,
                          buttonBounds;

        private Vector2 textLoc;  

        public DeadMenu(JManager source, ChunkGenerator cg, int width, int height)
        {
            this.source = source;
            this.cg = cg; 
            font = Pause.PauseMenu.questionFont;

            screenBounds = new Rectangle(0, 0, width, height);
            buttonBounds = new Rectangle(width / 5, height / 2 - height / 10, width / 5 * 3, height / 5);

            Vector2 textSize = font.MeasureString("Respawn?");

            textLoc = new Vector2(buttonBounds.X + buttonBounds.Width / 2 - textSize.X / 2,
                buttonBounds.Y + buttonBounds.Height / 2 - textSize.Y / 2
                );
        }

        public void Update(MouseState ms, MouseState lastMs)
        {
            if (ms.LeftButton == ButtonState.Pressed && lastMs.LeftButton == ButtonState.Released)
            {
                if (buttonBounds.Contains(ms.X, ms.Y))
                {
                    source.gameState = GameState.Playing;
                    source.player.Health = 100;

                    Item[,] items = source.player.Inventory.Items; 
                    for (int x = 0; x < items.GetLength(0); x++)
                    {
                        for (int y = 0;y < items.GetLength(1); y++)
                        {
                            if (items[x, y] != null)
                            {
                                items[x, y].Loc = source.player.Camera.loc.Clone();
                                items[x, y].Loc.Y = cg.GetHeightAt(items[x, y].Loc.X, items[x, y].Loc.Z) + 10; 
                                source.itemsInWorld.AddLast(items[x, y]);
                                items[x, y].SetInWorldSpace(items[x,y].Loc); 
                                items[x, y] = null; 
                            }
                        }
                    }

                    source.player.Camera.loc = new Vec4(0, 0, 0);

                    source.isMouseFocused = true;
                    source.host.IsMouseVisible = false; 
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PauseMenu.FilledRoundBox, buttonBounds, Color.Maroon);
            spriteBatch.DrawString(font, "Respawn?", textLoc, Color.White); 
        }
    }
}
