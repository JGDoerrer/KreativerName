﻿using System;
using System.Drawing;
using KreativerName.Grid;
using KreativerName.Rendering;
using KreativerName.Scenes;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace KreativerName
{
    public class MainWindow : GameWindow
    {
        public MainWindow()
            : base(16 * 80, 9 * 80, GraphicsMode.Default, "KreativerName")
        {
            input = new Input(this);

            Textures.LoadTextures(@"Resources\Textures");

            WindowState = Settings.Current.Fullscreen ? WindowState.Fullscreen : WindowState.Normal;

            if (Stats.Current.FirstStart.Ticks == 0)
                Stats.Current.FirstStart = DateTime.Now;

            Scenes.Scenes.SetWindow(this);
            Scenes.Scenes.LoadScene(new MainMenu());

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        Input input;
        static Random random = new Random();
        public int FrameCounter;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (FrameCounter % 10 == 0)
                Title = $"KerativerName {RenderFrequency:N1} fps, {UpdateFrequency:N1} ups";

            if (input.KeyPress(Key.F11))
            {
                if (WindowState != WindowState.Fullscreen)
                    WindowState = WindowState.Fullscreen;
                else
                    WindowState = WindowState.Normal;

                Settings.Current.Fullscreen = WindowState == WindowState.Fullscreen;
            }

            if (input.KeyDown(Key.AltLeft) && input.KeyDown(Key.F4))
                Close();

            Vector2 size = new Vector2(Width, Height);

            Scenes.Scenes.Update(size);

            // Update TimePlaying
            Stats.Current.TimePlaying = Stats.Current.TimePlaying.Add(TimeSpan.FromSeconds(e.Time));

            input.Update();

            FrameCounter++;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.Black);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Vector2 size = new Vector2(Width, Height);

            Scenes.Scenes.Render(size);

            SwapBuffers();
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            if (World.IsValidFile(e.FileName))
            {
                Game game = new Game(World.LoadFromFile(e.FileName, false));
                game.Exit += () =>
                {
                    game.World.SaveToFile(e.FileName, false);
                    Scenes.Scenes.LoadScene(new Transition(new MainMenu(), 10));
                };
                Scenes.Scenes.LoadScene(new Transition(game, 10));
            }
        }
    }
}
