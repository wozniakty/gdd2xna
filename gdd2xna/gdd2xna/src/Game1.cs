using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace gdd2xna
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MusicManager musicManager;
        Texture2D HamSandwich;
        Texture2D Broccoli;
        Texture2D Carrot;
        Texture2D Corn;
        Texture2D Eggplant;
        Texture2D Tomato;
        Texture2D Grid_Art;
        private bool musicOn;

        Grid g;

        #endregion

        #region Properties
        public bool MusicOn
        {
            get { return musicOn; }
            set { musicOn = value; }
        }
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            musicManager = new MusicManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // load songs to musicManager and play
            musicManager.Initialize();
            musicOn = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            HamSandwich = Content.Load<Texture2D>("Art/Ham_Sandwich");
            Broccoli = Content.Load<Texture2D>("Art/Brocoli_Tile");
            Carrot = Content.Load<Texture2D>("Art/Carrot_Tile");
            Corn = Content.Load<Texture2D>("Art/Corn_Tile");
            Eggplant = Content.Load<Texture2D>("Art/Eggplant_Tile");
            Tomato = Content.Load<Texture2D>("Art/Tomato_Tile");
            Grid_Art = Content.Load<Texture2D>("Art/VIA_Grid_V2");
            g = new Grid(20, 20, 50, 50);
            g.Print();

            var matches = g.FindMatches();
            while (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    foreach (int i in match)
                    {
                        Console.Write(i + ",");
                        g[i] = Tile.Emp;
                    }
                    Console.WriteLine();
                    g.Print();
                }

                Console.WriteLine("\n\nNEW BOARD:");
                g.RefillBoard();
                g.Print();
                matches = g.FindMatches();
            }

            Console.WriteLine(g.Deadlocked());
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            musicManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //draw the board
            spriteBatch.Draw(Grid_Art, g.rect, Color.White);
            //draw the tiles
            var tileRect = new Rectangle(g.rect.X, g.rect.Y, Grid.TILE_SIZE, Grid.TILE_SIZE);
            var tileTexture = new Texture2D(GraphicsDevice, Grid.TILE_SIZE, Grid.TILE_SIZE);
            var tileColor = new Color();
            tileColor = Color.White;
            for (int i = 0; i < g.rows; ++i)
            {
                for (int j = 0; j < g.cols; ++j)
                {
                    tileTexture = createTileTexture(i, j);
                    spriteBatch.Draw(tileTexture, tileRect, tileColor);

                    //move the rectangle
                    var temp = tileRect;
                    temp.X += Grid.TILE_SIZE;
                    tileRect = temp;
                }
                var tempy = tileRect;
                tempy.Y += Grid.TILE_SIZE;
                tempy.X = g.rect.X;
                tileRect = tempy;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private Texture2D createTileTexture(int x, int y)
        {
            Texture2D t;
            
            /**************************
             * set texture to an asset*
             * in switch statement    *
             * ************************/

            switch (g[x, y])
            {
                case Tile.Bla:
                    t = HamSandwich;
                    break;
                case Tile.Ora:
                    t = Carrot;
                    break;
                case Tile.Gre:
                    t = Broccoli;
                    break;
                case Tile.Pur:
                    t = Eggplant;
                    break;
                case Tile.Red:
                    t = Tomato;
                    break;
                case Tile.Yel:
                    t = Corn;
                    break;
                default:
                    throw new Exception("Could not assign texture");
            }

            return t;
        }
    }
}
