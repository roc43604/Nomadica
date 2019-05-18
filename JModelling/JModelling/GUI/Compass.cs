using JModelling.JModelling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.GUI
{
    /// <summary>
    /// Points to the next objective 
    /// </summary>
    public class Compass
    {
        private const int LookLength = 10; 

        private static Texture2D BaseImage, NeedleImage; 

        private static Camera camera;

        private static Rectangle drawLoc, needleLoc; 

        private Vec4 goal;

        private double theta; 

        public Compass(Vec4 goal)
        {
            this.goal = goal; 
        }

        public static void Load(ContentManager content)
        {
            BaseImage = content.Load<Texture2D>("Images/Menu/Compass");
            NeedleImage = content.Load<Texture2D>("Images/Menu/Needle");
        }

        public static void Init(Camera camera, int screenWidth, int screenHeight)
        {
            Compass.camera = camera;
            int width = screenWidth / 8; 
            drawLoc = new Rectangle(screenWidth - width, screenHeight - width, width, width);
            needleLoc = new Rectangle(drawLoc.X + drawLoc.Width / 2, drawLoc.Y + drawLoc.Height / 2, drawLoc.Width, drawLoc.Height); 
        }

        public void Update()
        {
            Vec4 lookPoint = new Vec4(
                (float)Math.Cos(camera.yaw) * LookLength + camera.loc.X,
                camera.loc.Y,
                (float)Math.Sin(camera.yaw) * LookLength + camera.loc.Z
            );
            float deltaX = goal.X - lookPoint.X,
                  deltaZ = goal.Z - lookPoint.Z;

            theta = Math.Atan2(deltaZ, deltaX) - camera.yaw + Math.PI;  
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BaseImage, drawLoc, Color.White); 
            Rectangle source = new Rectangle(0, 0, BaseImage.Width, BaseImage.Height);
            spriteBatch.Draw(NeedleImage, needleLoc, source, Color.White, (float)theta, new Vector2(source.Center.X, source.Center.Y), SpriteEffects.None, 0);    
        }
    }
}
