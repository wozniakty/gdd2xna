using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace gdd2xna
{
    public static class Input
    {
        public static MouseState MousePrevious;
        public static MouseState MouseCurrent;
        private static KeyboardState KeyPrevious;
        private static KeyboardState KeyCurrent;

        public static void Update()
        {
            MousePrevious = MouseCurrent;
            KeyPrevious = KeyCurrent;
            MouseCurrent = Mouse.GetState();
            KeyCurrent = Keyboard.GetState();
        }

        public static Point MousePos()
        {
            return new Point(MouseCurrent.X, MouseCurrent.Y);
        }

        public static Point DeltaMouse()
        {
            return new Point(MouseCurrent.X - MousePrevious.X, MouseCurrent.Y - MousePrevious.Y);
        }

        public static bool LeftClick()
        {
            return MouseCurrent.LeftButton == ButtonState.Pressed && MousePrevious.LeftButton != ButtonState.Pressed;
        }

        public static bool KeyPress( Keys k )
        {
            return KeyCurrent.IsKeyDown(k) && KeyPrevious.IsKeyUp(k);
        }

        public static bool KeyDown(Keys k)
        {
            return KeyCurrent.IsKeyDown(k);
        }
    }
}
