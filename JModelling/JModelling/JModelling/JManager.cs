﻿using GraphicsEngine;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace JModelling.JModelling
{
    /// <summary>
    /// The manager-class that will deal with managing all of the other
    /// JModelling classes. 
    /// </summary>
    public class JManager
    {
        private const float PITimesTwo = (float)(Math.PI * 2d);

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
        private int centerX, centerY;

        private bool isMouseFocused = true;

        /// <summary>
        /// The extent of the observable game world that is seen on display. 
        /// </summary>
        private const float FOV = (float)(Math.PI / 2d);

        /// <summary>
        /// What will be dealing with drawing to the screen. 
        /// </summary>
        private Painter painter;

        /// <summary>
        /// The thing the user will be seeing through. 
        /// </summary>
        private Camera camera;

        /// <summary>
        /// The last known location of the mouse. 
        /// </summary>
        private int lastMouseX, lastMouseY;

        /// <summary>
        /// All of the meshes/models being used in this scene currently. 
        /// </summary>
        private Mesh[] meshes;

        /// <summary>
        /// How far away a polygon has to be before we
        /// draw its average color instead of its image.
        /// </summary>
        private float AverageColorDrawDistance = 1f / 10f;

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
        /// Creates a manager that will construct necessary fields for use
        /// with the methods. 
        /// </summary>
        public JManager(Game host, int width, int height, GraphicsDeviceManager graphicsDeviceManager, SpriteBatch spriteBatch)
        {
            // Assigns the host
            this.host = host; 

            // Set the correct dimensions of the screen.
            Width = width;
            Height = height;

            DrawWidth = width / 4;
            DrawHeight = height / 4;

            centerX = width / 2;
            centerY = height / 2;

            painter = new Painter(width, height, graphicsDeviceManager.GraphicsDevice, spriteBatch);

            // Create the models
            meshes = new Mesh[] { Load.Mesh(@"Content/Models/mountains.obj") };

            numTriangles = 0;
            foreach (Mesh mesh in meshes)
            {
                numTriangles += mesh.Triangles.Length;
            }

            // Projection matrix
            matProj = Matrix.MakeProjection(FOV, (float)height / width, 0.1f, 1000);

            // Create the camera
            camera = new Camera(0, 0, 0);
            camera.yaw = 0;
            camera.pitch = 0;

            // Set the last mouse's position. 
            lastMouseX = -1;
            lastMouseY = -1;
        }

        /// <summary>
        /// Will be called every time the program needs to update. 
        /// </summary>
        public void Update()
        {
            UpdateInputs();

            painter.Begin(DrawWidth, DrawHeight);

            float[,] depthBuffer = new float[DrawWidth, DrawHeight];

            Vec4 target = new Vec4(0, -camera.pitch, 1);
            Vec4 lookDir = camera.GetLookDir(target);
            target = camera.loc + lookDir;
            camera.lookDir = lookDir;

            Matrix matView = Matrix.PointAt(camera.loc, target, new Vec4(0, 1, 0));
            matView.QuickInverse();

            Triangle[] triangles = new Triangle[numTriangles];
            int triIndex = 0;

            foreach (Mesh mesh in meshes)
            {
                foreach (Triangle tri in mesh.Triangles)
                {
                    // Get ray from triangle to camera
                    Vec4 cameraRay = tri.Points[0] - camera.loc;

                    // If ray is aligned with normal, then triangle is visible
                    if (Vec4.DotProduct(tri.Normal, cameraRay) < 0)
                    {
                        // Convert World Space to View Space
                        Triangle triViewed = new Triangle(
                            matView * tri.Points[0],
                            matView * tri.Points[1],
                            matView * tri.Points[2],
                            tri.Texels[0],
                            tri.Texels[1],
                            tri.Texels[2],
                            tri.Color,
                            tri.Image);

                        // Clip viewed triangle against near plane (this could
                        // form two additional triangles)
                        Triangle[] clipped = Vec4.TriangleClipAgainstPlane(
                            new Vec4(0, 0, 0.1f),
                            new Vec4(0, 0, 1),
                            triViewed);

                        foreach (Triangle clippedTri in clipped)
                        {
                            // Project triangle from 3D to 2D
                            Vec4 a = clippedTri.Points[0] * matProj;
                            Vec4 b = clippedTri.Points[1] * matProj;
                            Vec4 c = clippedTri.Points[2] * matProj;
                            Triangle triProjected = new Triangle(
                                a, b, c,
                                new Vec3(
                                    clippedTri.Texels[0].U / a.W,
                                    clippedTri.Texels[0].V / a.W,
                                    1f / a.W),
                                new Vec3(
                                    clippedTri.Texels[1].U / b.W,
                                    clippedTri.Texels[1].V / b.W,
                                    1f / b.W),
                                new Vec3(
                                    clippedTri.Texels[2].U / c.W,
                                    clippedTri.Texels[2].V / c.W,
                                    1f / c.W),
                                clippedTri.Color,
                                clippedTri.Image);

                            // Scale into view 
                            triProjected.Points[0] = triProjected.Points[0] / triProjected.Points[0].W;
                            triProjected.Points[1] = triProjected.Points[1] / triProjected.Points[1].W;
                            triProjected.Points[2] = triProjected.Points[2] / triProjected.Points[2].W;

                            // X/Y are inverted so put them back 
                            triProjected.Points[0].X *= -1;
                            triProjected.Points[1].X *= -1;
                            triProjected.Points[2].X *= -1;
                            triProjected.Points[0].Y *= -1;
                            triProjected.Points[1].Y *= -1;
                            triProjected.Points[2].Y *= -1;

                            // Offset verts into visible normalized space 
                            Vec4 offsetView = new Vec4(1, 1, 0);
                            triProjected.Points[0] = triProjected.Points[0] + offsetView;
                            triProjected.Points[1] = triProjected.Points[1] + offsetView;
                            triProjected.Points[2] = triProjected.Points[2] + offsetView;

                            triProjected.Points[0].X *= 0.5f * DrawWidth; triProjected.Points[0].Y *= 0.5f * DrawHeight;
                            triProjected.Points[1].X *= 0.5f * DrawWidth; triProjected.Points[1].Y *= 0.5f * DrawHeight;
                            triProjected.Points[2].X *= 0.5f * DrawWidth; triProjected.Points[2].Y *= 0.5f * DrawHeight;

                            triProjected.ClosestDist = triProjected.Texels[0].W;
                            if (triProjected.Texels[1].W < triProjected.ClosestDist)
                            {
                                triProjected.ClosestDist = triProjected.Texels[1].W;
                            }
                            if (triProjected.Texels[2].W < triProjected.ClosestDist)
                            {
                                triProjected.ClosestDist = triProjected.Texels[2].W;
                            }

                            // Illumination
                            float alpha = Math.Max(0, (float)(1 - Dist(camera.loc, tri.Points[0]) / 160));
                            triProjected.Alpha = alpha;

                            triangles[triIndex++] = triProjected;
                        }
                    }
                }
            }

            for (int index = 0; index < triIndex; index++)
            {
                Queue<Triangle> listTriangles = new Queue<Triangle>();
                int nNewTriangles = 1;

                listTriangles.Enqueue(triangles[index]);
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

                foreach (Triangle t in listTriangles)
                {
                    if(t.ClosestDist < AverageColorDrawDistance)
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

            painter.DrawToScreen(painter.GetCanvas());
        }

        private double Dist(Vec4 a, Vec4 b)
        {
            return Math.Sqrt(
                Math.Pow(a.X - b.X, 2) +
                Math.Pow(a.Y - b.Y, 2) +
                Math.Pow(a.Z - b.Z, 2));
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
        public void UpdateInputs()
        {
            // Get the current state of the keyboard and mouse. 
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            // Perform any special actions with any special keys. 
            ProcessSpecialKeyInputs(kb);

            // Get where the player should move 
            Vec4 moveDir = GetMoveDirectionFromKeyboard(kb);

            // If the player has actually moved, then move them
            if (moveDir != Vec4.Zero) 
            {
                // If shift is pressed, camera moves quicker.
                float speed = (kb.IsKeyDown(Controls.Sprint)) ? Camera.FastSpeed : Camera.NormalSpeed;

                camera.Move(speed, moveDir);
            }

            // If the mouse is focused and has moved since the last frame...
            if (isMouseFocused && (ms.X != lastMouseX || ms.Y != lastMouseY))
            {
                // Update last known positions.
                lastMouseX = ms.X;
                lastMouseY = ms.Y;

                // Process the mouse. 
                ProcessMouseLoc(ms.X, ms.Y); 
            }
        }

        /// <summary>
        /// Performs special actions if any important keys are pressed (for
        /// example, if ESC is pressed, then the program will do something
        /// like pause or close). 
        /// </summary>
        private void ProcessSpecialKeyInputs(KeyboardState kb)
        {
            // If Escape is pressed, close the program. 
            if (kb.IsKeyDown(Controls.QuitProgram))
            {
                System.Environment.Exit(0); 
            }

            // If Tab is pressed, unfocus/focus the mouse. 
            else if (kb.IsKeyDown(Controls.FocusOrUnfocusMouse))
            {
                isMouseFocused = !isMouseFocused;
                host.IsMouseVisible = !isMouseFocused;
            }
        }

        /// <summary>
        /// Gets where the player will be moving, as well as how fast they
        /// will move, from the keyboard. 
        /// </summary>
        private Vec4 GetMoveDirectionFromKeyboard(KeyboardState kb)
        {
            // Will add each movement vector to this base vector if the 
            // associated movement key is pressed. 
            Vec4 direction = new Vec4(); 

            // Forward/Backwards
            if (kb.IsKeyDown(Controls.Forward))
            {
                direction += new Vec4(0, 0, 1); 
            }    
            if (kb.IsKeyDown(Controls.Backwards))
            {
                direction += new Vec4(0, 0, -1); 
            }
            
            // Left/Right
            if (kb.IsKeyDown(Controls.Left))
            {
                direction += new Vec4(1, 0, 0); 
            }
            if (kb.IsKeyDown(Controls.Right))
            {
                direction += new Vec4(-1, 0, 0); 
            }

            // Up/Down
            if (kb.IsKeyDown(Controls.Up))
            {
                direction += new Vec4(0, 1, 0); 
            }
            if (kb.IsKeyDown(Controls.Down))
            {
                direction += new Vec4(0, -1, 0);
            }

            return direction; 
        }

        public void ProcessMouseLoc(int xLoc, int yLoc)
        {
            float x = (float)xLoc;
            float y = (float)yLoc; 

            x -= centerX;
            y -= centerY; 

            x /= Controls.MouseSensitivity;
            y /= Controls.MouseSensitivity;

            camera.yaw += x;
            camera.pitch += y; 

            if (camera.yaw > PITimesTwo)
            {
                camera.yaw = 0; 
            }
            else if (camera.yaw < 0)
            {
                camera.yaw = PITimesTwo; 
            }

            Mouse.SetPosition(centerX, centerY); 
        }
    }
}
