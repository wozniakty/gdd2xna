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
    public struct float2
    {
        public float x;
        public float y;

        public float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float2(float2 other)
        {
            this.x = other.x;
            this.y = other.y;
        }
        public static float2 operator+(float2 thing, float2 other)
        {
            return new float2(thing.x + other.x, thing.y + other.y);
        }
        public static bool operator==(float2 thing, float2 other)
        {
            return thing.x == other.x && thing.y == other.y;
        }
        public static bool operator!=(float2 thing, float2 other)
        {
            return thing.x != other.x || thing.y != other.y;
        }
        
    }
    public class Tile
    {
        public float2 SPEED;
        private bool animating;
        private float2 realPosition;
        private Point targetPosition;
        public TileType type;
        public Grid container;

        public void update()
        {
            //gtfo
            if(animating == false) return;
            //get dist info
            var yDist = realPosition.y - targetPosition.Y;
            var xDist = realPosition.x - targetPosition.X;
            var tDistSq = (yDist * yDist) + (xDist * xDist);
            //if close, we're done
            if (tDistSq < (SPEED.x * SPEED.x) + (SPEED.y * SPEED.y))
            {
                animating = false;
                realPosition.y = targetPosition.Y;
                realPosition.x = targetPosition.X;
                return;
            }
            //move a bitch
            else
            {
                var vec = new float2(SPEED);
                if (yDist > 0) vec.y *= -1;
                if (xDist > 0) vec.x *= -1;
                if (yDist == 0) vec.y = 0;
                if (xDist == 0) vec.x = 0;
                realPosition = realPosition + vec;
            }

        }

        //accessors and shit

        public Tile(int x, int y, TileType t, Grid c)
        {
            container = c;
            type = t;
            animating = false;
            realPosition = new float2(x * Grid.TILE_SIZE + container.rect.Left, y * Grid.TILE_SIZE + container.rect.Top );
            targetPosition = new Point(x * Grid.TILE_SIZE + container.rect.Left, y * Grid.TILE_SIZE + container.rect.Top);
            SPEED = new float2(2f, 2f);
        }

        public Point Position
        {
            get { return targetPosition; }
            set
            {
                animating = true;
                targetPosition = value;
            }
        }

        public float2 ScreenPosition
        {
            get { return realPosition; }
        }

        public bool Animating
        {
            get { return animating; }
        }

        public static bool operator==(Tile thing, Tile other)
        {
            return thing.realPosition == other.realPosition && thing.targetPosition == other.targetPosition;
        }

        public static bool operator !=(Tile thing, Tile other)
        {
            return !(thing == other);
        }
    }
}
