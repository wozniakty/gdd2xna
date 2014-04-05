using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gdd2xna
{
    public enum Tile
    {
        Empty,
        Red,
        Blue,
        Yellow,
        Green,
        Black,
        Purple
    }

    public class Grid
    {
        public int rows, cols;
        public Tile[,] state;

        public Random rnd;

        public Grid(int r, int c)
        {
            rows = r;
            cols = c;
            state = new Tile[r, c];
            for( int i = 0; i < r * c; i++ )
            {
                this[i] = Tile.Empty;
            }
            rnd = new Random();

            Regenerate();
        }

        public Tile this[int r, int c]
        {
            get { return state[r, c]; }
            set { state[r, c] = value; }
        }

        public Tile this[int n]
        {
            get { return state[n % rows, n / cols]; }
            set { state[n % rows, n / cols] = value; }
        }

        public Tile[] GetRow(int r)
        {
            Tile[] row = new Tile[cols];
            for (int i = 0; i < cols; i++)
            {
                row[i] = state[r, i];
            }
            return row;
        }

        public Tile[] GetCol(int c)
        {
            Tile[] col = new Tile[rows];
            for (int i = 0; i < cols; i++)
            {
                col[i] = state[i, c];
            }
            return col;
        }

        public void Regenerate()
        {
            for (int i = 0; i < rows * cols; i++)
            {
                state[i % rows, i / cols] = (Tile)rnd.Next(1, 7);
            }
        }
    }
}
