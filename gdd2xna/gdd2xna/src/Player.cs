﻿using System;
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
    public class Player
    {
        /// <summary>
        /// The locations to draw the grid at for each player.
        /// </summary>
        private static readonly int[][] GRID_LOCATIONS = {new int[] {100, 100}, new int[] {800, 100}};

        /// <summary>
        /// The locations to draw the grid at for each player in small window mode.
        /// </summary>
        private static readonly int[][] GRID_LOCATIONS_SMALL = { new int[] { 0, 0 }, new int[] { 600, 0 } };
        
        /// <summary>
        /// The points per tile.
        /// </summary>
        private static readonly int POINTS_PER_TILE = 13; // Need a good value for this (13) (32 for testing)

        /// <summary>
        /// The amount of shuffles per game.
        /// </summary>
        private static readonly int SHUFFLES_PER_GAME = 3;

        /// <summary>
        /// The size of the board in pixels.
        /// </summary>
        private static readonly int BOARD_SIZE = 400;

        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// The player index.
        /// </summary>
        private readonly int index;

        /// <summary>
        /// The grid of the player.
        /// </summary>
        private readonly Grid grid;

        /// <summary>
        /// The sound manager instance.
        /// </summary>
        private readonly SoundManager soundManager;

        /// <summary>
        /// The scores instance.
        /// </summary>
        private readonly Scores scoreBars;

        /// <summary>
        /// The current stage of the player in the game.
        /// </summary>
        public GameStep step  { get; set; }

        /// <summary>
        /// The index of the player.
        /// </summary>
        public int Index { get { return index; } }

        /// <summary>
        /// The draw location of the board.
        /// </summary>
        private int[] location;

        /// <summary>
        /// ?
        /// </summary>
        private int[] prevSwap;

        /// <summary>
        /// ?
        /// </summary>
        private int lowestEmp = 0;

        /// <summary>
        /// The shuffle button.
        /// </summary>
        private Button shuffleButton;

        /// <summary>
        /// The amount of shuffles that the player has left.
        /// </summary>
        private int shufflesRemaining = SHUFFLES_PER_GAME;

        /// <summary>
        /// Creates a new Player.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="index">The index of the player.</param>
        /// <param name="soundManager">The sound manager.</param>
        /// <param name="scores">The scores instance.</param>
        public Player(ViaGame game, int index, SoundManager soundManager, Scores scoreBars)
        {
            this.game = game;
            this.index = index;
            this.soundManager = soundManager;
            this.scoreBars = scoreBars;
            if (game.SizeMode == ViaGame.SIZE_SMALL)
            {
                location = GRID_LOCATIONS_SMALL[index];
            }
            else
            {
                location = GRID_LOCATIONS[index];
            }

            this.grid = new Grid(index, 8, 8, location[0], location[1], game);

            Reset();
        }

        /// <summary>
        /// Reset the player.
        /// </summary>
        public void Reset()
        {
            grid.Reset();

            step = GameStep.Waiting;
            prevSwap = new int[2] { 0, 0 };
            shufflesRemaining = SHUFFLES_PER_GAME;
            updateShuffleText();
        }

        /// <summary>
        /// Called when asset loading is completed.
        /// </summary>
        /// <param name="defaultFont">The default font.</param>
        public void LoadingFinished(SpriteFont defaultFont)
        {
            int offset = (BOARD_SIZE / 2) - (game.buttonTexture.Width / 2);
            shuffleButton = new Button(
                game.buttonTexture,
                new Vector2(location[0] + offset, BOARD_SIZE + 25),
                "Shuffle",
                defaultFont,
                delegate(Button button)
                {
                    if (step != GameStep.Input)
                        return;

                    HandleShuffle();

                    if (game.State == GameState.NetworkPlay)
                    {
                        // Send the move to the server
                        Packet p = new Packet(OutgoingPackets.SHUFFLE);
                        game.Client.WritePacket(p);
                    }
                },
                delegate(Button button)
                {
                    return step == GameStep.Input && shufflesRemaining > 0;
                },
                delegate(Button button)
                {
                    if (game.State == GameState.NetworkPlay)
                    {
                        return index == 0;
                    }
                    return true;
                });

            updateShuffleText();
        }

        /// <summary>
        /// Handle a swap.
        /// </summary>
        /// <param name="first">The first grid index.</param>
        /// <param name="second">The second grid index.</param>
        public void HandleSwap(int first, int second)
        {
            grid.Swap(first, second);
            prevSwap[0] = first;
            prevSwap[1] = second;
            grid.ClearSelection();
            step = GameStep.CheckMatch;
        }

        /// <summary>
        /// Update the text of the shuffle button.
        /// </summary>
        private void updateShuffleText()
        {
            if (shuffleButton != null)
            {
                shuffleButton.Text = "Shuffle (" + shufflesRemaining + ")";
            }
        }

        /// <summary>
        /// Handle a shuffle.
        /// </summary>
        public void HandleShuffle()
        {
            if (shufflesRemaining <= 0)
            {
                // Someone is probably trying to cheat.
                return;
            }

            shufflesRemaining--;
            updateShuffleText();
            grid.ShuffleBoard();
            step = GameStep.CheckMatchShuffle;
        }

        /// <summary>
        /// Update the player.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            // Update the shuffle button
            shuffleButton.Update(gameTime);

            if (!grid.Animating())
            {
                if (step == GameStep.Input)
                {
                    if (Input.LeftClick())
                    {
                        var mousePos = Input.MousePos();
                        var gridPos = grid.ScreenToGrid(mousePos.X, mousePos.Y);
                        var gridNum = grid.RCtoN(gridPos[0], gridPos[1]);
                        //example of how to play a soundEffect in the code
                        //soundManager.Play(SoundEffectName.Match3);
                        if (!grid.HasActiveSelection())
                        {
                            grid.UpdateSelection(gridPos[0], gridPos[1]);
                        }
                        else
                        {
                            var selectNum = grid.RCtoN(grid.selection[0], grid.selection[1]);
                            var swaps = grid.GetSwaps(selectNum);
                            if (swaps.Contains(gridNum))
                            {
                                HandleSwap(selectNum, gridNum);

                                // Check if we are online
                                if (game.State == GameState.NetworkPlay)
                                {
                                    // Send the move to the server
                                    Packet p = new Packet(OutgoingPackets.SWAP_TILES);
                                    p.writeWord(prevSwap[0]);
                                    p.writeWord(prevSwap[1]);

                                    game.Client.WritePacket(p);
                                }
                            }
                            else
                            {
                                grid.UpdateSelection(gridPos[0], gridPos[1]);
                            }
                        }
                    }
                }
                else if (step == GameStep.CheckMatch || step == GameStep.CheckMatchShuffle)
                {
                    var matches = grid.FindMatches();
                    GameStep noMatchStep = GameStep.Input;
                    GameStep noMatchNetworkStep = GameStep.NetworkInput;
                    if (step == GameStep.CheckMatchShuffle)
                    {
                        noMatchStep = GameStep.CheckDeadlock;
                        noMatchNetworkStep = GameStep.CheckDeadlock;
                    }
                    if (matches.Count == 0)
                    {
                        // Don't unswap tiles on a shuffle.
                        if (step != GameStep.CheckMatchShuffle)
                        {
                            grid.Swap(prevSwap[0], prevSwap[1]);
                        }

                        if (game.State == GameState.LocalPlay)
                        {
                            step = noMatchStep;
                        }
                        else if (game.State == GameState.NetworkPlay)
                        {
                            if (index == 0)
                            {
                                step = noMatchStep;
                            }
                            else
                            {
                                step = noMatchNetworkStep;
                            }
                        }
                    }
                    else
                    {

                        int winIndex = -1;
                        foreach (var match in matches)
                        {
                            int points = POINTS_PER_TILE;
                            int count = match.Count();
                            // For each amount over 3 times, give 2 extra points per tile.
                            points += 2 * (count - 3);
                            foreach (var tile in match)
                            {
                                TileType type = grid[tile].type;
                                if (game.State == GameState.NetworkPlay)
                                {
                                    // Tell the server we scored, but only for our player
                                    // as the server will tell the other play we scored.
                                    if (index == 0)
                                    {
                                        Packet p = new Packet(OutgoingPackets.INCREASE_SCORE);
                                        p.writeWord((int)type);
                                        p.writeWord(points);
                                        game.Client.WritePacket(p);
                                    }
                                }
                                else
                                {
                                    winIndex = scoreBars.add(type, index, points);
                                }
                            }
                                
                            grid.EmptyTiles(match);
                        }

                        lowestEmp = grid.DropEmpties();
                        grid.RefillBoard(lowestEmp);
                        if (winIndex != -1)
                        {
                            game.SetWinner(winIndex);

                            if (game.State == GameState.NetworkPlay)
                            {
                                // Tell the server who we think won!
                                Packet p = new Packet(OutgoingPackets.GAME_OVER);
                                p.writeByte(winIndex);
                                game.Client.WritePacket(p);
                            }
                        }
                        else
                        {
                            if (grid.FindMatches().Count > 0)
                                step = GameStep.CheckMatch;
                            else
                                step = GameStep.CheckDeadlock;
                        }
                    }
                }
                else if (step == GameStep.CheckDeadlock)
                {
                    if (grid.Deadlocked())
                        grid.ShuffleBoard();
                    else
                    {
                        if (game.Mode == GameMode.TurnBased)
                        {
                            step = GameStep.Complete;
                        }
                        else if (game.Mode == GameMode.RealTime)
                        {
                            if (index == 0)
                            {
                                step = GameStep.Input;
                            }
                            else
                            {
                                step = GameStep.Waiting;
                            }
                        }
                        grid.ClearSelection();
                    }
                }
                else if (step == GameStep.Waiting)
                {

                }
            }
            else
            {
                for (int i = 0; i < grid.rows * grid.cols; i++)
                {
                    grid[i].update();
                }
            }
        }

        /// <summary>
        /// Draw the player (their grid).
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            // Draw the grid
            grid.Draw(spriteBatch, step == GameStep.Waiting && game.Mode != GameMode.RealTime);

            // Check if we should draw any text.
            string text = null;
            Color? color = null;

            // Check the current step of the player.
            if (step == GameStep.Win)
            {
                text = "You win!";
                color = Color.Green;
            }
            else if (step == GameStep.Lose)
            {
                text = "Better luck next time...";
                color = Color.Red;
            }
            else
            {
                /*text = "Placeholder";
                color = Color.Blue;*/
            }

            // Draw text if needed
            if (text != null)
            {
                int x = location[0];
                int y = location[1] + BOARD_SIZE;
                Vector2 size = font.MeasureString(text);
                spriteBatch.DrawString(font, text, new Vector2(x + (BOARD_SIZE / 2) - (size.X / 2), y), color.Value);
            }

            // Draw the shuffle button for the player
            shuffleButton.Draw(gameTime, spriteBatch);
        }
    }
}
