using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gdd2xna
{
    public enum Tile
    {
        Emp,
        Red,
        Blu,
        Yel,
        Gre,
        Bla,
        Pur
    }

    public class Grid
    {
        public int rows, cols;
        private Tile[,] state;

        public Random rnd;

        public Grid(int r, int c)
        {
            rnd = new Random();
            rows = r;
            cols = c;
            state = new Tile[r, c];
            for( int i = 0; i < r * c; i++ )
            {
                this[i] = RandomTile();
            }

            //Regenerate();
        }

        public Tile this[int r, int c]
        {
            get { return state[r, c]; }
            set { state[r, c] = value; }
        }

        public Tile this[int n]
        {
            get 
            {
                if (n < 0 || n > rows * cols - 1)
                    return Tile.Emp;
                return state[n / cols,n % rows]; 
            }
            set 
            {
                if (n >= 0 && n <= rows * cols - 1)
                    state[n / cols, n % rows] = value;
            }
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
        
        // Regenerates a full board without any matches
        public void Regenerate()
        {
            for (int i = 0; i < rows * cols; i++)
            {
                List<Tile> invalid = new List<Tile>();
                //This checks that we are at least on the third tile to the right and there's a 2-match to the left
                if (i % cols > 1 && this[i - 1] == this[i - 2 ])
                {
                    invalid.Add(this[i - 1]);
                } //and this checks that we are at least on the third tile down
                else if(i / rows > 1 && this[i - cols] == this[i - 2*cols])
                {
                    invalid.Add(this[i - cols]);
                }
                Tile next = RandomTile();
                while(invalid.Contains(next))
                {
                    next = RandomTile();
                }

                this[i] = next;
            }
        }

        // Finds any matches with the current tile
        // Only checks right, and down.  This is to be iterated through from 0 -> rows*cols
        // So the algorithm is naive. DONT SCREW IT UP
        public int[] FindMatch(int n)
        {
            //First we make sure the current node isn't empty
            if (this[n] == Tile.Emp)
                return new int[0];

            List<int[]> matches = new List<int[]>();
            //Now we'll check for horizontal
            int[] hor = FindMatchHorizontal(n);
            //If that has a match, we'll check all those matched nodes to see if they have any vertical matches
            List<int[]> horChains = new List<int[]>();
            if (hor.Length > 0)
            {
                for(int i = 1; i < hor.Length; i++)
                {
                    int[] horChain = FindMatchVertical(i);
                    if (horChain.Length > 0)
                        horChains.Add(horChain);
                }
                // I'm not going to keep recursively looking through this though.
                // Any realistic situation in the game shouldn't need more than this
            }
            matches.Add(hor);
            matches.AddRange(horChains);

            //Do the same stuff for verticals now
            int[] ver = FindMatchVertical(n);
            List<int[]> verChains = new List<int[]>();
            if (ver.Length > 0)
            {
                for(int i = 1; i < ver.Length; i++)
                {
                    int[] verChain = FindMatchHorizontal(i);
                    if (verChain.Length > 0)
                    {
                        verChains.Add(verChain);
                    }
                }
            }
            matches.Add(ver);
            matches.AddRange(verChains);

            //So now we have all of our possible matches
            //Our next goal is to return a non-duplicate list of every tile affected by matches
            //This bool array will help us keep track of duplicates
            bool[] matched = new bool[rows * cols];
            List<int> finalMatch = new List<int>();

            for (int i = 0; i < matches.Count; i++)
            {
                int[] match = matches[i];
                for (int j = 0; j < match.Length; j++)
                {
                    if (!matched[match[j]])
                    {
                        finalMatch.Add(match[j]);
                        matched[match[j]] = true;
                    }
                }
            }

            return finalMatch.ToArray();
        }

        public int[] FindMatchVertical(int n)
        {
            int length = 0;
            // We check here to see if the current tile is Empty, because if it is we ignore it for these checks
            if (this[n] != Tile.Emp)
            {
                length = 1;
                for (int i = 1; i < rows - (n / rows); i++)
                {
                    if (this[n] == this[n + cols*i])
                    {
                        length++;
                    }
                    else
                        break;
                }

                //Matches are at least length 3
                if (length < 3) length = 0;
            }

            int[] match = new int[length];
            for (int i = 0; i < length; i++)
            {
                match[i] = n + cols*i;
            }
            return match;
        }

        public int[] FindMatchHorizontal(int n)
        {
            int length = 0;
            // We check here to see if the current tile is Empty, because if it is we ignore it for these checks
            if (this[n] != Tile.Emp)
            {
                length = 1;
                for (int i = 1; i < cols - (n % cols); i++)
                {
                    if (this[n] == this[n + i])
                    {
                        length++;
                    }
                    else
                        break;
                }

                //Matches are at least length 3
                if (length < 3) length = 0;
            }

            int[] match = new int[length];
            for (int i = 0; i < length; i++)
            {
                match[i] = n + i;
            }
            return match;
        }

        // Helper function for getting a random tile
        public Tile RandomTile()
        {
            return (Tile)rnd.Next(1, Enum.GetNames(typeof(Tile)).Length);
        }

        //Debug function
        public void Print()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(this[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}
