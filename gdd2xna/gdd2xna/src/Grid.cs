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

    public enum Direction
    {
        Up, Right, Down, Left
    }

    public class Grid
    {
        public int rows, cols;
        private Tile[,] state;

        public Random rnd;

        public Grid(int r, int c)
        {
#if DEBUG
            rnd = new Random(5);
#else
            rnd = new Random();
#endif

            rows = r;
            cols = c;
            state = new Tile[r, c];
            for( int i = 0; i < r * c; i++ )
            {
                this[i] = RandomTile();
            }

            Regenerate();
        }

        public Tile this[int r, int c]
        {
            get 
            {
                if (r < 0 || c < 0 || r >= rows || c >= cols)
                    return Tile.Emp;
                return state[r, c]; 
            }
            set 
            { 
                state[r, c] = value;
            }
        }

        public Tile this[int n]
        {
            get 
            {
                if (n < 0 || n > rows * cols - 1)
                    return Tile.Emp;
                return state[n / cols,n % cols];
            }
            set 
            {
                if (n >= 0 && n <= rows * cols - 1)
                    state[n / cols, n % cols] = value;
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
        
        /// <summary>
        /// Returns an array of the indices for possible swaps,
        /// in order Up, Right, Down, Left (as in the Direction enum).  Any -1 values mean that's not a valid swap location
        /// </summary>
        /// <param name="n">The index of the tile</param>
        /// <returns></returns>
        public int[] GetSwaps(int n)
        {
            return new [] { (n / cols == 0)? -1 : n - cols,
                ( n % cols == cols - 1 )? -1 : n + 1,
                (n / cols == rows - 1)? -1 : n + cols,
                (n % cols == 0)? -1 : n - 1 };
        }
        
        /// <summary>
        /// Regenerates a full board without any matches
        /// </summary>
        public void Regenerate()
        {
            for (int i = 0; i < rows * cols; i++)
            {
                Tile invalid1 = Tile.Emp;
                Tile invalid2 = Tile.Emp;
                //This checks that we are at least on the third tile to the right and there's a 2-match to the left
                if (i % cols > 1 && this[i - 1] == this[i - 2 ])
                {
                    invalid1 = this[i - 1];
                } //and this checks that we are at least on the third tile down
                if(i / rows > 1 && this[i - cols] == this[i - 2*cols])
                {
                    invalid2 = this[i - cols];
                }
                Tile next = RandomTile();
                while(invalid1 == next || invalid2 == next)
                {
                    next = RandomTile();
                }

                this[i] = next;
            }
        }

        /// <summary>
        /// Swaps the value of the tiles at n1 and n2
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        public void Swap(int n1, int n2)
        {
            var temp = this[n1];
            this[n1] = this[n2];
            this[n2] = temp;
        }

        /// <summary>
        /// Converts all tiles in the Indeces array to be empty
        /// </summary>
        /// <param name="indeces"></param>
        public void EmptyTiles(int[] indeces)
        {
            for (int i = 0; i < indeces.Length; i++)
            {
                this[indeces[i]] = Tile.Emp;
            }
        }

        /// <summary>
        /// Converts all tiles in the Indeces array to be empty
        /// </summary>
        /// <param name="indeces"></param>
        public void EmptyTiles(List<int> indeces)
        {
            EmptyTiles(indeces.ToArray());
        }

        /// <summary>
        /// Returns a list of ALL valid matches with no redudancies
        /// for the current state
        /// </summary>
        /// <returns></returns>
        public List<List<int>> FindMatches()
        {
            //Reference to the current match at each tile
            var matchSet = new Dictionary<int, List<int>>();
            //List of all discrete unconnected matches
            var matches = new List<List<int>>();

            // First we're going to get all the horizontal matches
            for (int i = 0; i < rows * cols; i++)
            {
                var hor = FindMatchHorizontal(i);
                if (hor.Length > 0)
                {
                    var match = new List<int>();
                    foreach (int index in hor)
                    {
                        match.Add(index);
                        matchSet.Add(index, match);
                    }
                    matches.Add(match);
                    // And then move our index along to the next unchecked tile
                    i += hor.Length - 1;
                }
            }

            //Now that we have all those, we'll reiterate through the list and handle verticals

            for (int j = 0; j < rows * cols; j++)
            {
                // I make a dummy i that goes down the columns since that check is more efficient
                int i = (j / rows) + ((j % rows) * cols);
                var ver = FindMatchVertical(i);
                if (ver.Length > 0)
                {
                    //We'll get all the Match lists this match collides with
                    var collisions = new List<List<int>>();
                    foreach (int index in ver)
                    {
                        if (matchSet.ContainsKey(index))
                            collisions.Add(matchSet[index]);
                    }

                    //If there is only one list, we'll just add new values from this match to that list
                    if (collisions.Count == 1)
                    {
                        var match = collisions.Last();
                        foreach (int index in ver)
                        {
                            if (!match.Contains(index))
                            {
                                match.Add(index);
                                matchSet.Add(index, match);
                            }
                        }
                    } //If there are more than one collision, we'll get rid of all of those and make a new single discrete match
                    else if (collisions.Count > 1)
                    {
                        var match = new List<int>();
                        foreach (List<int> col in collisions)
                        {
                            foreach (int index in col)
                                matchSet[index] = match;
                            match.AddRange(col);
                            matches.Remove(col);
                        }

                        foreach (int index in ver)
                        {
                            if (!match.Contains(index))
                            {
                                match.Add(index);
                                matchSet.Add(index, match);
                            }
                        }
                    }
                    else
                    {
                        //If there is no collision, we just create the match and add it to the list
                        var match = new List<int>();
                        foreach (int index in ver)
                        {
                            match.Add(index);
                            matchSet.Add(index, match);
                        }
                        matches.Add(match);
                    
                    }

                    // And finally move our index along by the length
                    j += ver.Length - 1;
                }
            }

            // And then return them!
            return matches;
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

            var match = new int[length];
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

            var match = new int[length];
            for (int i = 0; i < length; i++)
            {
                match[i] = n + i;
            }
            return match;
        }

        public void RefillBoard()
        {
            //Store the lowest empty tile, which will help us optimize at the end
            int lowestEmp = 0;
            for (int i = rows * cols - 1; i >= 0; i--)
            {
                //We'll do a check to make sure we aren't at the very top of the column
                bool top = false;
                while (this[i] == Tile.Emp && !top)
                {
                    top = true;
                    var cur = i;
                    var above = cur - cols;
                    while (cur > 0)
                    {
                        if (this[cur] != Tile.Emp)
                            top = false;

                        this[cur] = this[above];
                        cur = above;
                        above = cur - cols;
                    }

                    if (top == true && i > lowestEmp)
                        lowestEmp = i;
                }
            }

            for (int i = 0; i <= lowestEmp; i++)
            {
                if (this[i] == Tile.Emp)
                    this[i] = RandomTile();
            }
        }

        // Returns true if there is no possible match
        // Assumes there are no spaces where there are already 3 or more matched
        public bool Deadlocked()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (this[i, j] == this[i, j + 2] &&
                        (this[i, j] == this[i - 1, j + 1] ||
                        this[i, j] == this[i + 1, j + 1]))
                            return false;
                    if (this[i, j] == this[i + 2, j] &&
                        (this[i,j] == this[i + 1,j-1] ||
                        this[i,j] == this[i+1,j+1]))
                            return false;
                    if (this[i, j] == this[i, j + 1] &&
                        (this[i, j] == this[i - 1, j - 1] ||
                        this[i, j] == this[i, j - 2] ||
                        this[i, j] == this[i + 1, j - 1] ||
                        this[i, j] == this[i + 1, j + 2] ||
                        this[i, j] == this[i, j + 3] ||
                        this[i, j] == this[i - 1, j + 2]))
                            return false;
                    if(this[i,j] == this[i+1,j] &&
                        (this[i,j] == this[i-2,j] ||
                        this[i,j] == this[i-1,j-1] ||
                        this[i,j] == this[i+2,j-1] ||
                        this[i,j] == this[i+3,j] ||
                        this[i,j] == this[i+2,j+1] ||
                        this[i,j] == this[i-1,j+1]))
                            return false;
                }
            }

            return true;
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
