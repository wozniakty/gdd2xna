using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;

namespace gdd2xna
{
    /// <summary>
    /// The client to the game server.
    /// </summary>
    public class GameClient
    {
        /// <summary>
        /// The default port that the server runs on.
        /// </summary>
        public static readonly int DEFAULT_PORT = 10532;

        /// <summary>
        /// The minimum amount of numbers to keep in the random number buffer.
        /// </summary>
        public static readonly int MIN_RANDOM_BUFFER = 4096;

        /// <summary>
        /// Should debug messages be printed.
        /// </summary>
        public static readonly bool DEBUG = false;

        /// <summary>
        /// The network protocol version of the client.
        /// </summary>
        public static readonly int NETWORK_PROTOCOL_VERSION = 2;

        /// <summary>
        /// The network error message.
        /// </summary>
        private static readonly string ERROR_MESSAGE = "Sorry, a network error occurred.";

        /// <summary>
        /// The game instance.
        /// </summary>
        private readonly ViaGame game;

        /// <summary>
        /// The write queue for packets.
        /// </summary>
        private BlockingCollection<Packet> writeQueue = new BlockingCollection<Packet>();

        /// <summary>
        /// The queue of random numbers.
        /// </summary>
        private BlockingCollection<int>[] randomQueues = new BlockingCollection<int>[2];

        /// <summary>
        /// The cancel button.
        /// </summary>
        private Button cancelButton;

        /// <summary>
        /// The current status.
        /// </summary>
        private string status = "Connecting to server...";

        /// <summary>
        /// The socket to the server.
        /// </summary>
        private TcpClient socket = null;

        /// <summary>
        /// The packet reading thread.
        /// </summary>
        private Thread reader = null;

        /// <summary>
        /// Is the reader thread running.
        /// </summary>
        private bool readerRunning = false;

        /// <summary>
        /// The packet writing thread.
        /// </summary>
        private Thread writer = null;

        /// <summary>
        /// Is the write thread running.
        /// </summary>
        private bool writerRunning = false;

        /// <summary>
        /// Is the client shutting down.
        /// </summary>
        private bool shuttingDown = false;

        /// <summary>
        /// Creates a new GameClient.
        /// </summary>
        /// <param name="game">The game instance.</param>
        public GameClient(ViaGame game)
        {
            this.game = game;
            for (int i = 0; i < 2; i++)
            {
                randomQueues[i] = new BlockingCollection<int>();
            }
        }

        /// <summary>
        /// Get the next random number for the specified player.
        /// </summary>
        /// <param name="playerIndex">The player index.</param>
        /// <returns>The next random number.</returns>
        public int GetNextRandom(int playerIndex)
        {
            if (randomQueues[playerIndex].Count <= MIN_RANDOM_BUFFER)
            {
                Packet p = new Packet(OutgoingPackets.REQUEST_RANDOM);
                p.writeByte(playerIndex);
                WritePacket(p);
            }
            while (randomQueues[playerIndex].Count() == 0)
            {
                // Uhh, panic?
                Thread.Sleep(1000);
            }
            return randomQueues[playerIndex].Take();
        }

        /// <summary>
        /// Add a packet to the write queue.
        /// </summary>
        /// <param name="p">The packet to write.</param>
        public void WritePacket(Packet p)
        {
            writeQueue.Add(p);
        }

        /// <summary>
        /// Start the packet writing loop.
        /// </summary>
        private void WriteLoop()
        {
            writerRunning = true;
            try
            {
                Packet next;
                while ((next = writeQueue.Take()) != null)
                {
                    // Make sure the client is still active.
                    if (socket == null || !socket.Connected || next.getId() == -1)
                    {
                        break;
                    }

                    if (DEBUG)
                    {
                        Console.WriteLine("Writing packet ID " + next.getId());
                    }
                    next.WriteTo(socket);
                }
            }
            catch (Exception ex)
            {
                if (DEBUG)
                {
                    Console.WriteLine(ex);
                }
                if (!shuttingDown)
                    Shutdown(true);
            }
            writerRunning = false;
        }

        /// <summary>
        /// Connect to the game server.
        /// </summary>
        private void Connect()
        {
            readerRunning = true;
            try
            {
                socket = new TcpClient();
                socket.Connect("via.allgofree.org", DEFAULT_PORT);

                // Write the login packet
                Packet handshake = new Packet(13); // 13 is lucky right? ;)
                handshake.writeDWord(ViaGame.GAME_BUILD);
                handshake.writeDWord(NETWORK_PROTOCOL_VERSION);
                handshake.writeString("test");
                handshake.WriteTo(socket);

                // Request a game
                Packet gameRequest = new Packet(OutgoingPackets.REQUEST_GAME);
                gameRequest.WriteTo(socket);

                // Update the status
                status = "Searching for an opponent...";

                // Get the stream
                NetworkStream stream = socket.GetStream();

                // Start the thread that writes packets
                writer = new Thread(new ThreadStart(WriteLoop));
                writer.Start();

                // Start reading packets
                while (socket != null && socket.Connected)
                {
                    // Read the next packet ID
                    int id = stream.ReadByte();

                    if (id == -1)
                    {
                        return;
                    }

                    // Read the length of the packet
                    byte[] sizePayload = new byte[2];
                    stream.Read(sizePayload, 0, sizePayload.Length);

                    // Parse the length
                    Packet sizePacket = new Packet(-1, sizePayload);
                    int size = sizePacket.readUnsignedWord();

                    // Read the payload
                    byte[] payload = new byte[size];
                    if (size != 0)
                    {
                        int read = 0;
                        while (read < payload.Length)
                        {
                            int n = stream.Read(payload, read, payload.Length - read);
                            read += n;
                        }
                    }

                    // Create the packet object
                    Packet packet = new Packet(id, payload);
                    ReadPacket(packet);
                }
            }
            catch (Exception ex)
            {
                if (DEBUG)
                {
                    Console.WriteLine(ex);
                }
                if (!shuttingDown)
                    Shutdown(true);
            }
            readerRunning = false;
        }

        /// <summary>
        /// Read a packet.
        /// </summary>
        /// <param name="p">The packet.</param>
        private void ReadPacket(Packet p)
        {
            const int PING = 0;
            const int START_GAME = 1;
            const int SWAP_TILES = 2;
            const int SHUFFLE = 3;
            const int LOGOUT = 4;
            const int FILL_RANDOM = 5;

            if (DEBUG)
            {
                Console.WriteLine("Reading packet ID " + p.getId());
            }

            switch (p.getId())
            {
                default:
                    // Uhh, panic?
                    break;
                case PING:
                    break;
                case START_GAME:
                    string opponent = p.readString();
                    bool moveFirst = p.readBoolean();
                    game.State = GameState.NetworkPlay;
                    if (!moveFirst)
                    {
                        game.GetPlayer(0).step = GameStep.Waiting;
                        game.GetPlayer(1).step = GameStep.NetworkInput;
                    }
                    
                    break;
                case SWAP_TILES:
                    int first = p.readUnsignedWord();
                    int second = p.readUnsignedWord();

                    game.GetPlayer(1).HandleSwap(first, second);
                    break;
                case SHUFFLE:
                    game.GetPlayer(1).HandleShuffle();
                    break;
                case LOGOUT:
                    game.State = GameState.Error;
                    game.Error.Message = p.readString();
                    break;
                case FILL_RANDOM:
                    int playerIndex = p.readUnsignedByte();
                    bool empty = p.readBoolean();
                    int size = p.readDWord();

                    if (empty)
                    {
                        randomQueues[playerIndex] = new BlockingCollection<int>();
                    }

                    for (int i = 0; i < size; i++)
                    {
                        randomQueues[playerIndex].Add(p.readDWord());
                    }
                    break;
            }
        }

        /// <summary>
        /// Initialize the client.
        /// </summary>
        public void Initialize()
        {
            status = "Connecting to server...";
            Shutdown(false);

            // Wait for the threads to close down
            while (readerRunning || writerRunning)
            {
                Thread.Sleep(500);
            }

            shuttingDown = false;
            reader = new Thread(new ThreadStart(Connect));
            reader.Start();
        }

        /// <summary>
        /// Shutdown the client.
        /// </summary>
        /// <param name="error">Was the shutdown due to an error.</param>
        public void Shutdown(bool error)
        {
            if (error)
            {
                game.Error.Message = ERROR_MESSAGE;
                game.State = GameState.Error;
            }
            else
            {
                shuttingDown = true;
            }
            // Shutdown the connection
            if (socket != null)
            {
                try
                {
                    socket.Close();
                    writeQueue.Add(new Packet(-1));
                }
                catch (Exception ex)
                {
                }
            }

            // Stop all worker threads
            /*if (reader != null)
            {
                try
                {
                    reader.Abort();
                }
                catch (Exception ex)
                {
                }
            }
            if (writer != null)
            {
                try
                {
                    writer.Abort();
                }
                catch (Exception ex)
                {
                }
            }*/

            // Reset everything to null
            socket = null;
            reader = null;
            writer = null;

            // Quick way to "empty" the write queue
            writeQueue = new BlockingCollection<Packet>();
        }

        /// <summary>
        /// Called when loading is complete.
        /// </summary>
        /// <param name="defaultFont">The default font.</param>
        public void LoadingComplete(SpriteFont defaultFont)
        {
            // Do some math for button placement
            int currentY = (game.graphics.PreferredBackBufferHeight) - (2 * game.buttonTexture.Height);
            int halfWidth = (game.graphics.PreferredBackBufferWidth / 2);
            int buttonX = halfWidth - (game.buttonTexture.Width / 2);

            // Create the buttons
            cancelButton = new Button(
                game.buttonTexture,
                new Vector2(buttonX, currentY),
                "Cancel",
                defaultFont,
                delegate(Button button)
                {
                    game.State = GameState.Menu;
                },
                null,
                null
                );
        }

        /// <summary>
        /// Update the client.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            cancelButton.Update(gameTime);
        }

        /// <summary>
        /// Draw the client.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="font">The default font.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            // Draw the current status
            Vector2 size = font.MeasureString(status);
            float x = (game.graphics.PreferredBackBufferWidth / 2) - (size.X / 2);
            float y = (game.graphics.PreferredBackBufferHeight / 2) - (size.Y / 2);
            Vector2 location = new Vector2(x, y);
            spriteBatch.DrawString(font, status, location, Color.Cyan);

            // Draw the cancel button
            cancelButton.Draw(gameTime, spriteBatch);
        }
    }
}
