#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Particle3DSample
{
    /// <summary>
    /// Sample showing how to implement a particle system entirely
    /// on the GPU, using the vertex shader to animate particles.
    /// </summary>
    public class Particle3DSampleGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;
        SpriteFont font;
        // This sample uses five different particle systems.    
        ParticleSystem smokePlumeParticles;

        TimeSpan timeToNextProjectile = TimeSpan.Zero;

        // Random number generator for the fire effect.
        Random random = new Random();


        // Input state.
        KeyboardState currentKeyboardState;
        GamePadState currentGamePadState;
        KeyboardState lastKeyboardState;
        GamePadState lastGamePadState;


        // Camera state.
        float cameraArc = -5;
        float cameraRotation = 0;
        float cameraDistance = 200;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public Particle3DSampleGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Construct our particle system components.           
            smokePlumeParticles = new SmokePlumeParticleSystem(this, Content);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            // Register the particle system components.           
            Components.Add(smokePlumeParticles);

        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            UpdateCamera(gameTime);


            UpdateSmokePlume();


            base.Update(gameTime);
        }


        /// <summary>
        /// Helper for updating the smoke plume effect.
        /// </summary>
        void UpdateSmokePlume()
        {
            // This is trivial: we just create one new smoke particle per frame.
            smokePlumeParticles.AddParticle(Vector3.Zero, Vector3.Zero);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.Black);

            // Compute camera matrices.
            float aspectRatio = (float)device.Viewport.Width /
                                (float)device.Viewport.Height;

            Matrix view = Matrix.CreateTranslation(0, -25, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                          Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                                              new Vector3(0, 0, 0), Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);

            smokePlumeParticles.SetCamera(view, projection);


            // Draw our background grid and message text.          

            DrawMessage();

            // This will draw the particle system components.
            base.Draw(gameTime);
        }


        /// <summary>
        /// Helper for drawing our message text.
        /// </summary>
        void DrawMessage()
        {
            string message = string.Format("For Nami by Victorem");
            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, new Vector2(10, 10), Color.White);
            spriteBatch.End();
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Handles input for quitting the game and cycling
        /// through the different particle effects.
        /// </summary>
        void HandleInput()
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;

            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for exit.
            if (currentKeyboardState.IsKeyDown(Keys.Escape) ||
                currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

        }


        /// <summary>
        /// Handles input for moving the camera.
        /// </summary>
        void UpdateCamera(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check for input to rotate the camera up and down around the model.
            if (currentKeyboardState.IsKeyDown(Keys.Up) ||
                currentKeyboardState.IsKeyDown(Keys.W))
            {
                cameraArc += time * 0.025f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
                currentKeyboardState.IsKeyDown(Keys.S))
            {
                cameraArc -= time * 0.025f;
            }

            cameraArc += currentGamePadState.ThumbSticks.Right.Y * time * 0.05f;

            // Limit the arc movement.
            if (cameraArc > 90.0f)
                cameraArc = 90.0f;
            else if (cameraArc < -90.0f)
                cameraArc = -90.0f;

            // Check for input to rotate the camera around the model.
            if (currentKeyboardState.IsKeyDown(Keys.Right) ||
                currentKeyboardState.IsKeyDown(Keys.D))
            {
                cameraRotation += time * 0.05f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
                currentKeyboardState.IsKeyDown(Keys.A))
            {
                cameraRotation -= time * 0.05f;
            }

            cameraRotation += currentGamePadState.ThumbSticks.Right.X * time * 0.1f;

            // Check for input to zoom camera in and out.
            if (currentKeyboardState.IsKeyDown(Keys.Z))
                cameraDistance += time * 0.25f;

            if (currentKeyboardState.IsKeyDown(Keys.X))
                cameraDistance -= time * 0.25f;

            cameraDistance += currentGamePadState.Triggers.Left * time * 0.5f;
            cameraDistance -= currentGamePadState.Triggers.Right * time * 0.5f;

            // Limit the camera distance.
            if (cameraDistance > 500)
                cameraDistance = 500;
            else if (cameraDistance < 10)
                cameraDistance = 10;

            if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed ||
                currentKeyboardState.IsKeyDown(Keys.R))
            {
                cameraArc = -5;
                cameraRotation = 0;
                cameraDistance = 200;
            }
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (Particle3DSampleGame game = new Particle3DSampleGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
