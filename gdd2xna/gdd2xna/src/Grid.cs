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

    public enum TileType
    {
        Emp,
        Red,
        Ora,
        Yel,
        Gre,
        Pur,
        Pnk,
        Wht
    }

    public enum Direction
    {
        Up, Right, Down, Left
    }

    public class Grid
    {
        
        private Rectangle gridRect;
        public static readonly int TILE_SIZE = 50;
        private Point position;
        public int[] selection;
        public int rows, cols;
        private Tile[,] state;
        public Random rnd;
        private Game1 main;

        public Grid(int r, int c, int x, int y, Game1 m)
        {
            main = m;
#if DEBUG
            rnd = new Random(3);
#else
            rnd = new Random();
#endif

            rows = r;
            cols = c;
            gridRect.Width = c * (TILE_SIZE);
            gridRect.Height = r * (TILE_SIZE);
            gridRect.X = x;
            gridRect.Y = y;
            

            state = new Tile[r, c];
            for( int i = 0; i < r; i++ )
                for (int j = 0; j < c; ++j)
                {
                    var t = RandomTile();
                    this[i,j] = new Tile(i,j,t, this);
                }
            selection = new int[2] { -1, -1 };

            Regenerate();
        }
        #region accessors
        public Tile this[int r, int c]
        {
            get 
            {
                if (r < 0 || c < 0 || r >= rows || c >= cols)
                    return new Tile(-1, -1, TileType.Emp, this);
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
                    return new Tile(-1, -1, TileType.Emp, this);
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

        public Rectangle rect
        {
            get{return gridRect;}
        }

        public void UpdateSelection( int row, int col )
        {
            selection[0] = row;
            selection[1] = col;
        }

        public bool HasActiveSelection()
        {
            return selection[0] >= 0 && selection[0] < rows && selection[1] >= 0 && selection[1] < cols;
        }

        public void ClearSelection()
        {
            selection[0] = -1;
            selection[1] = -1;
        }
        #endregion

        #region logic
        
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

        public int[] GetSwaps(int row, int col)
        {
            return GetSwaps(RCtoN(row,col));
        }
        
        /// <summary>
        /// Regenerates a full board without any matches
        /// </summary>
        public void Regenerate()
        {
            int y = 0;
            for (int i = 0; i < rows * cols; i++)
            {
                TileType invalid1 = TileType.Emp;
                TileType invalid2 = TileType.Emp;
                //This checks that we are at least on the third tile to the right and there's a 2-match to the left
                if (i % cols > 1 && this[i - 1].type == this[i - 2 ].type)
                {
                    invalid1 = this[i - 1].type;
                } //and this checks that we are at least on the third tile down
                if(i / cols > 1 && this[i - cols].type == this[i - 2*cols].type)
                {
                    invalid2 = this[i - cols].type;
                }
                TileType next = RandomTile();
                while(invalid1 == next || invalid2 == next)
                {
                    next = RandomTile();
                }

                if (i % cols == 0) ++y;

                var t = new Tile(i % cols, y, next, this);
                this[i] = t;
            }
        }

        /// <summary>
        /// Swaps the value of the tiles at n1 and n2
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        public void Swap(int n1, int n2)
        {
            var t1 = this[n1];
            var t2 = this[n2];
            var p1 = t1.Position;
            t1.Position = t2.Position;
            t2.Position = p1;

            this[n2] = t1;
            this[n1] = t2;
        }

        /// <summary>
        /// Converts all tiles in the Indeces array to be empty
        /// </summary>
        /// <param name="indeces"></param>
        public void EmptyTiles(int[] indeces)
        {
            for (int i = 0; i < indeces.Length; i++)
            {
                this[indeces[i]].type = TileType.Emp;
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
            if (this[n].type != TileType.Emp)
            {
                length = 1;
                for (int i = 1; i < rows - (n / cols); i++)
                {
                    if (this[n].type == this[n + cols * i].type)
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
            if (this[n].type != TileType.Emp)
            {
                length = 1;
                for (int i = 1; i < cols - (n % cols); i++)
                {
                    if (this[n].type == this[n + i].type)
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

        public void RefillBoard(int lowestEmp = -1)
        {
            //Store the lowest empty tile, which will help us optimize at the end
            if (lowestEmp < 0)
                lowestEmp = DropEmpties();            
            
            int[] spot = new int[cols];
            int y = lowestEmp / cols;
            for (int i = lowestEmp; i >= 0; i--)
            {
                if (this[i].type == TileType.Emp)
                {
                    var t = RandomTile();
                    this[i] = new Tile(i % cols, spot[i % cols], t, this);
                    spot[i % cols]--;
                    this[i].Position = new Point(((i % cols) * Grid.TILE_SIZE) + rect.Left, ((y + 1) * Grid.TILE_SIZE) + rect.Top);
                }

                if (i % cols == 0) --y;
            }
        }

        public int DropEmpties()
        {
            var lowestEmp = 0;
            for (int i = rows * cols - 1; i >= 0; i--)
            {
                //We'll do a check to make sure we aren't at the very top of the column
                bool top = false;
                while (this[i].type == TileType.Emp && !top)
                {
                    top = true;
                    var cur = i;
                    var above = cur - cols;
                    while (above > 0 - cols)
                    {
                        if( this[above].type != TileType.Emp )
                        {
                            top = false;
                            break;
                        }
                        else
                        {
                            above -= cols;
                        }
                    }

                    if (top == true)
                    {
                        if(i > lowestEmp)
                            lowestEmp = i;
                        break;
                    }
                    else
                    {
                        while(this[above].type != TileType.Emp)
                        {
                            Swap(cur, above);
                            cur -= cols;
                            above -= cols;
                        }
                    }                    
                }
            }

            return lowestEmp;
        }

        public bool Animating()
        {
            for (int i = 0; i < rows * cols; i++)
            {
                if (this[i].Animating)
                    return true;
            }

            return false;
        }

        // Returns true if there is no possible match
        // Assumes there are no spaces where there are already 3 or more matched
        public bool Deadlocked()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (this[i, j].type == this[i, j + 2].type &&
                        (this[i, j].type == this[i - 1, j + 1].type ||
                        this[i, j].type == this[i + 1, j + 1].type))
                            return false;
                    if (this[i, j].type == this[i + 2, j].type &&
                        (this[i,j].type == this[i + 1,j-1].type ||
                        this[i,j].type == this[i+1,j+1].type))
                            return false;
                    if (this[i, j].type == this[i, j + 1].type &&
                        (this[i, j].type == this[i - 1, j - 1].type ||
                        this[i, j].type == this[i, j - 2].type ||
                        this[i, j].type == this[i + 1, j - 1].type ||
                        this[i, j].type == this[i + 1, j + 2].type ||
                        this[i, j].type == this[i, j + 3].type ||
                        this[i, j].type == this[i - 1, j + 2].type))
                            return false;
                    if(this[i,j].type == this[i+1,j].type &&
                        (this[i,j].type == this[i-2,j].type ||
                        this[i,j].type == this[i-1,j-1].type ||
                        this[i,j].type == this[i+2,j-1].type ||
                        this[i,j].type == this[i+3,j].type ||
                        this[i,j].type == this[i+2,j+1].type ||
                        this[i,j].type == this[i-1,j+1].type))
                            return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Shuffle dat board
        /// </summary>
        public void ShuffleBoard()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int r = rnd.Next(0,cols);
                    Swap(RCtoN(i, j), RCtoN(i, r));
                }
            }

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    int r = rnd.Next(0, rows);
                    Swap(RCtoN(j, i), RCtoN(r, i));
                }
            }
        }

        // Helper function for getting a random tile
        public TileType RandomTile()
        {
            return (TileType)rnd.Next(1, Enum.GetNames(typeof(TileType)).Length);
        }

        /// <summary>
        /// Converts screen space X and Y to a grid space row, and column
        /// </summary>
        /// <param name="X">Screen X</param>
        /// <param name="Y">Screen Y</param>
        /// <returns>An array containing the Row and Column in grid space</returns>
        public int[] ScreenToGrid(int X, int Y)
        {
            if( gridRect.Contains( X, Y ) )
            {
                return new int[2]{ (Y - gridRect.Y) / TILE_SIZE, (X - gridRect.X) / TILE_SIZE };
            }
            return new int[2] { -1, -1 };
        }

        public int[] GridToScreen(int X, int Y)
        {
            return new int[2]{ ( Y * TILE_SIZE ) + gridRect.Y, ( X * TILE_SIZE ) + gridRect.X };
        }

        public int RCtoN(int row, int col)
        {
            return (row * cols) + col;
        }

        #endregion

        #region Draw code

        public void Draw(SpriteBatch sb)
        {
            //draw the board
            sb.Draw(main.Grid_Art, gridRect, Color.White);
            //draw the tiles
            var tileTexture = new Texture2D(main.GraphicsDevice, Grid.TILE_SIZE, Grid.TILE_SIZE);
            var tileColor = new Color();
            tileColor = Color.White;
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    tileTexture = createTileTexture(this[i, j].type);
                    var tileRect = new Rectangle((int)this[i,j].ScreenPosition.x,(int)this[i,j].ScreenPosition.y - Grid.TILE_SIZE, Grid.TILE_SIZE, Grid.TILE_SIZE); 
                    if (this[i, j].type != TileType.Emp)
                        sb.Draw(tileTexture, tileRect, tileColor);
                    else
                        sb.Draw(tileTexture, tileRect, new Color(0, 0, 0, 0));

                    if (selection[0] == i && selection[1]== j)
                    {
                        main.DrawBorder(sb, tileRect, 3, Color.White);
                    }

                }
            }
        }

        private Texture2D createTileTexture(TileType type)
        {
            Texture2D t;

            /**************************
             * set texture to an asset*
             * in switch statement    *
             * ************************/

            switch (type)
            {
                case TileType.Emp:
                    t = main.HamSandwich;
                    break;
                case TileType.Ora:
                    t = main.Carrot;
                    break;
                case TileType.Gre:
                    t = main.Broccoli;
                    break;
                case TileType.Pur:
                    t = main.Eggplant;
                    break;
                case TileType.Red:
                    t = main.Tomato;
                    break;
                case TileType.Yel:
                    t = main.Corn;
                    break;
                case TileType.Pnk:
                    t = main.Radish;
                    break;
                case TileType.Wht:
                    t = main.Onion;
                    break;
                default:
                    throw new Exception("Could not assign texture");
            }

            return t;
        }
        #endregion

        #region Debug utilities
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

        #endregion
    }
}
