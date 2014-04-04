using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gdd2xna
{
    public enum Token
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
        public Token[,] state;

        public Random rnd;

        public Grid(int r, int c)
        {
            rows = r;
            cols = c;
            state = new Token[r, c];
            for( int i = 0; i < r * c; i++ )
            {
                state[i % r, i / c] = Token.Empty;
            }
            rnd = new Random();

            Regenerate();
        }

        public Token this[int r, int c]
        {
            get { return state[r, c]; }
            set { state[r, c] = value; }
        }

        public Token[] GetRow(int r)
        {
            Token[] row = new Token[cols];
            for (int i = 0; i < cols; i++)
            {
                row[i] = state[r, i];
            }
            return row;
        }

        public Token[] GetCol(int c)
        {
            Token[] col = new Token[rows];
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
                state[i % rows, i / cols] = (Token)rnd.Next(1, 7);
            }
        }
    }
}
