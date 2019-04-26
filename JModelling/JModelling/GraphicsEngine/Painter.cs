﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsEngine
{
    /// <summary>
    /// A giant bundle of methods involved in drawing items to the screen.
    /// When drawing something, call Begin(), perform all of the tasks you 
    /// want (i.e. DrawImage() or DrawString()), and call End().   
    /// </summary>
    class Painter
    {
        /// <summary>
        /// The dimensions of the screen this Painter is drawing
        /// on. This can be changed through the UpdateSize() method. 
        /// </summary>
        private int width, height;

        /// <summary>
        /// The dimensions of the canvas. Should be smaller than the screen
        /// if you want to be drawing at a different resolution.
        /// </summary>
        private int drawWidth, drawHeight; 

        /// <summary>
        /// What we're drawing on/with. 
        /// </summary>
        private Color[] canvas;

        /// <summary>
        /// Performs rendering, creates resources, handles variables, etc.
        /// We use it to create Texture2D's, which the SpriteBatch can 
        /// draw. 
        /// </summary>
        private GraphicsDevice graphicsDevice;

        /// <summary>
        /// What we're drawing to. This class essentially creates images
        /// on a per-pixel basis, and then puts the image onto the default
        /// SpriteBatch to draw. 
        /// </summary>
        private SpriteBatch spriteBatch;

        Texture2D image;
        int curDW = 0;
        int curDH = 0;

        /// <summary>
        /// Creates a default Painter object.
        /// </summary>
        public Painter(int width, int height, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            this.width = width;
            this.height = height;
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
            
        }

        /// <summary>
        /// Tells the program to start accepting what you want to draw. 
        /// This must be called before you begin drawing. 
        /// </summary>
        public void Begin(int drawWidth, int drawHeight)
        {
            this.drawWidth = drawWidth;
            this.drawHeight = drawHeight;

            canvas = new Color[drawWidth * drawHeight];

            if (curDH != drawHeight || curDW != drawWidth)
            {
                canvas = new Color[drawWidth * drawHeight];
               
                curDH = drawHeight;
                curDW = drawWidth;
            }
            
        }

        /// <summary>
        /// Returns a Texture2D of the drawing you've created. 
        /// </summary>
        public Texture2D GetCanvas()
        {
            graphicsDevice.Textures[0] = null;
            Texture2D texture = new Texture2D(graphicsDevice, drawWidth, drawHeight);
            texture.SetData<Color>(canvas);

            return texture; 
        }

        /// <summary>
        /// Draws the texture directly to the screen. 
        /// </summary>
        public void DrawToScreen(Texture2D texture)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone); 
            {
                spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            }
            spriteBatch.End(); 
        }

        /// <summary>
        /// Draws an image at the specified location. The image can be any
        /// size at any location. 
        /// </summary>
        public void DrawImage(Texture2D image, Rectangle location)
        {
            // Gets the data of the image into an array
            Color[] data = new Color[image.Width * image.Height];
            image.GetData<Color>(data);

            DrawImage(data, image.Width, image.Height, location); 
        }

        /// <summary>
        /// Draws an image at the specified location. The image can be
        /// any size at any location. 
        /// </summary>
        public void DrawImage(Color[] data, int imageWidth, int imageHeight, Rectangle location)
        {
            // Gets the bounds of the area we're drawing to. Sometimes, the programmer
            // may supply an area that is out-of-bounds; as such, we would need to crop
            // it to fit on the screen. 
            float imageY = 0, imageStartX = 0; // texture coords
            int endRow = location.Y + location.Height,
                endCol = location.X + location.Width;

            int startRow = location.Y;
            if (startRow < 0) // Above the top of the screen
            {
                // Gets a percentage of how much of the location we cropped off, 
                // and applys it to the image rectangle as well. 
                float diff = 100f / (float)location.Height * (0 - startRow);
                imageY = imageHeight / 100 * diff;
                startRow = 0;
            }

            if (endRow > height-1)
            {
                float diff = 100f / (float)location.Height * (endRow - height);
                imageY = imageHeight / 100 * diff;
                endRow = height-1;
            }

            int startCol = location.X;
            if (startCol < 0)
            {
                float diff = 100f / (float)location.Width * (0 - startCol);
                imageStartX = imageWidth / 100 * diff;
                startCol = 0;
            }

            if (endCol > width-1)
            {
                float diff = 100f / (float)location.Width * (endCol - width);
                imageStartX = imageWidth / 100 * diff;
                endCol = width-1;
            }

            // These represent how many image pixels pass for every screen pixel. 
            float mx = (float)imageWidth / (float)location.Width, my = (float)imageHeight / (float)location.Height;

            // Goes through each pixel we would draw to, and finds out what texture
            // color we would place there.
            float imageX;
            for (int row = startRow; row < endRow; row++)
            {
                imageX = imageStartX;
                for (int col = startCol; col < endCol; col++)
                {
                    int canvasIndex = row * drawWidth + col;
                    int dataIndex = (int)imageY * imageWidth + (int)imageX; 

                    // If the indices are in bounds, we can set the pixels. 
                    if (canvasIndex >= 0 && canvasIndex < canvas.Length && 
                        dataIndex >= 0 && dataIndex < data.Length)
                    {
                        canvas[canvasIndex] = data[dataIndex];
                    }
                    
                    imageX += mx;
                }
                imageY += my;
            }
        }

        /// <summary>
        /// Draws the image with a supplied depth buffer 
        /// </summary>
        public void DrawImage(Color[] data, int imageWidth, int imageHeight, Rectangle location, float[,] depthBuffer, float depth)
        {
            // Gets the bounds of the area we're drawing to. Sometimes, the programmer
            // may supply an area that is out-of-bounds; as such, we would need to crop
            // it to fit on the screen. 
            float imageY = 0, imageStartX = 0; // texture coords
            int endRow = location.Y + location.Height,
                endCol = location.X + location.Width;

            int startRow = location.Y;
            if (startRow < 0) // Above the top of the screen
            {
                // Gets a percentage of how much of the location we cropped off, 
                // and applys it to the image rectangle as well. 
                float diff = 100f / (float)location.Height * (0 - startRow);
                imageY = imageHeight / 100 * diff;
                startRow = 0;
            }

            if (endRow > height - 1)
            {
                float diff = 100f / (float)location.Height * (endRow - height);
                imageY = imageHeight / 100 * diff;
                endRow = height - 1;
            }

            int startCol = location.X;
            if (startCol < 0)
            {
                float diff = 100f / (float)location.Width * (0 - startCol);
                imageStartX = imageWidth / 100 * diff;
                startCol = 0;
            }

            if (endCol > width - 1)
            {
                float diff = 100f / (float)location.Width * (endCol - width);
                imageStartX = imageWidth / 100 * diff;
                endCol = width - 1;
            }

            // These represent how many image pixels pass for every screen pixel. 
            float mx = (float)imageWidth / (float)location.Width, my = (float)imageHeight / (float)location.Height;

            // Goes through each pixel we would draw to, and finds out what texture
            // color we would place there.
            float imageX;
            for (int row = startRow; row < endRow; row++)
            {
                imageX = imageStartX;
                for (int col = startCol; col < endCol; col++)
                {
                    int canvasIndex = row * drawWidth + col;
                    int dataIndex = (int)imageY * imageWidth + (int)imageX;

                    // If the indices are in bounds, we can set the pixels. 
                    if (canvasIndex >= 0 && canvasIndex < canvas.Length &&
                        dataIndex >= 0 && dataIndex < data.Length &&
                        col >= 0 && col < depthBuffer.GetLength(0) &&
                        row >= 0 && row < depthBuffer.GetLength(1))
                    {
                        // Checks if the depth makes this pixel visible
                        if (depth > depthBuffer[col, row])
                        {
                            // Checks the transparency
                            if (data[dataIndex].A == 255)
                            {
                                canvas[canvasIndex] = data[dataIndex];
                                depthBuffer[col, row] = depth;
                            }
                        }
                    }

                    imageX += mx;
                }
                imageY += my;
            }
        }

        /// <summary>
        /// Draws an image at the specified location, but given the reference
        /// parameter. The image will be placed in reference to the parameter,
        /// meaning the (x,y) coords of the location will be placed differently.
        /// 
        /// For example, if you wanted to draw an image in the middle of the 
        /// screen, you would do: 
        /// DrawImage(image, 
        ///     new Rectangle(
        ///         screenWidth / 2,
        ///         screenHeight / 2, 
        ///         screenWidth, 
        ///         screenHeight),
        ///     ImageReference.Center); 
        ///     
        /// If you wanted to draw an image in the bottom-right corner, you would do:
        /// DrawImage(image, 
        ///     new Rectangle(
        ///         screenWidth,
        ///         screenHeight, 
        ///         screenWidth,
        ///         screenHeight), 
        ///     ImageReference.BottomRight);
        ///     
        /// This method alleviates the need for extra-dividing throughout the program,
        /// tidy-ing it up into a single method. 
        /// </summary>
        public void DrawImage(Texture2D image, Rectangle location, ImageReference reference)
        {
            Rectangle newLoc; 
            switch (reference)
            {
                case (ImageReference.TopCenter):
                    newLoc = new Rectangle(location.X - location.Width / 2, location.Y, location.Width, location.Height);
                    break;

                case (ImageReference.TopRight):
                    newLoc = new Rectangle(location.X - location.Width, location.Y, location.Width, location.Height); 
                    break;

                case (ImageReference.CenterLeft):
                    newLoc = new Rectangle(location.X, location.Y - location.Height / 2, location.Width, location.Height);
                    break;

                case (ImageReference.Center):
                    newLoc = new Rectangle(location.X - location.Width / 2, location.Y - location.Height / 2, location.Width, location.Height);
                    break;

                case (ImageReference.CenterRight):
                    newLoc = new Rectangle(location.X - location.Width, location.Y - location.Height / 2, location.Width, location.Height);
                    break;

                case (ImageReference.BottomLeft):
                    newLoc = new Rectangle(location.X, location.Y - location.Height, location.Width, location.Height);
                    break;

                case (ImageReference.BottomCenter):
                    newLoc = new Rectangle(location.X - location.Width / 2, location.Y - location.Height, location.Width, location.Height);
                    break;

                case (ImageReference.BottomRight):
                    newLoc = new Rectangle(location.X - location.Width, location.Y - location.Height, location.Width, location.Height);
                    break;

                default:
                    newLoc = location;
                    break; 
            }

            DrawImage(image, newLoc); 
        }

        /// <summary>
        /// Sets the pixel at (x,y) to the specified color. This method will
        /// never return an IndexOutOfBoundsException, because it will always
        /// fall within the range of the array. 
        /// </summary>
        public void SetSafePixel(int x, int y, Color color)
        {
            if (x >= 0 && x < drawWidth && y >= 0 && y < drawHeight)
            {
                SetPixel(x, y, color); 
            }
        }
        
        /// <summary>
        /// Sets the pixel at (x,y) to the specified color.
        /// </summary>
        public void SetPixel(int x, int y, Color color)
        {
            canvas[y * drawWidth + x] = color;
        }

        //public void DrawLine(Line line, Color color)
        //{
        //    DrawLine(line.A.X, line.A.Y, line.B.X, line.B.Y, color); 
        //}

        /// <summary>
        /// Draws a line from (x1,y1) to (x2,y2) 
        /// </summary>
        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            float x, y, dx, dy, dx1, dy1, px, py, xe, ye, i; 

            // Calculate line deltas
            dx = x2 - x1;
            dy = y2 - y1;

            // Create a positive copy of deltas (makes iterating easier)
            dx1 = Math.Abs(dx);
            dy1 = Math.Abs(dy);

            // Calculate error intervals for both axis
            px = 2 * dy1 - dx1;
            py = 2 * dx1 - dy1; 

            // The line is X-axis dominant
            if(dy1 <= dx1)
            {
                // Line is drawn left to right
                if(dx >= 0)
                {
                    x = x1;
                    y = y1;
                    xe = x2;  
                }
                else // Line is drawn right to left (swap ends)
                {
                    x = x2;
                    y = y2;
                    xe = x1;
                }

                SetSafePixel((int)x, (int)y, color); 

                // Rasterize the line
                for(i = 0; x < xe; i++)
                {
                    x++; 

                    // Deal with octants
                    if(px < 0)
                    {
                        px += 2 * dy1; 
                    }
                    else
                    {
                        if((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            y++; 
                        }
                        else
                        {
                            y--; 
                        }
                        px += 2 * (dy1 - dx1); 
                    }

                    SetSafePixel((int)x, (int)y, color); 
                }
            }
            else // The line is y-axis dominant
            {
                // Line is drawn from bottom to dop
                if(dy >= 0)
                {
                    x = x1;
                    y = y1;
                    ye = y2;
                }
                else // Line is drawn from top to bottom
                {
                    x = x2;
                    y = y2;
                    ye = y1; 
                }

                SetSafePixel((int)x, (int)y, color); 

                // Rasterize the line
                for(i = 0; y < ye; i++)
                {
                    y++; 

                    // Deal with octants...
                    if(py <= 0)
                    {
                        py += 2 * dx1; 
                    }
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            x++;
                        }
                        else
                        {
                            x--; 
                        }
                        py += 2 * (dx1 - dy1); 
                    }

                    SetSafePixel((int)x, (int)y, color); 
                }
            }
        }
    }
}
