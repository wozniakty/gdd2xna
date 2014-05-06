using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Text;

namespace gdd2xna
{
    public class Packet
    {
        /// <summary>
        /// The ID of the packet.
        /// </summary>
        private int id;

        /// <summary>
        /// The bytes of the packet.
        /// </summary>
        private byte[] buf;

        /// <summary>
        /// The current buffer offset.
        /// </summary>
        private int offset;

        /// <summary>
        /// Creates a new Packet.
        /// </summary>
        /// <param name="id">The ID of the packet.</param>
        /// <param name="buf">The byte buffer of the packet.</param>
        public Packet(int id, byte[] buf)
        {
            this.id = id;
            this.buf = buf;
        }

        /// <summary>
        /// Creates a new Packet.
        /// </summary>
        /// <param name="id">The ID of the packet.</param>
        public Packet(int id)
        {
            this.id = id;
            this.buf = new byte[256];
        }

        /// <summary>
        /// Write the packet to the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        public void WriteTo(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            // Write the packet ID
            stream.WriteByte((byte)id);

            // Write the size of the packet as a word
            stream.WriteByte((byte)(offset >> 8));
            stream.WriteByte((byte)(offset));

            // Write the data
            stream.Write(buf, 0, offset);

            // Flush!
            stream.Flush();
        }

        /// <summary>
        /// Get the ID of the packet.
        /// </summary>
        /// <returns>The ID.</returns>
        public int getId()
        {
            return id;
        }

        /// <summary>
        /// Get the underlying buffer.
        /// </summary>
        /// <returns>The buffer.</returns>
        public byte[] getBuffer()
        {
            return buf;
        }

        /// <summary>
        /// Get the current offset.
        /// </summary>
        /// <returns>The current offset.</returns>
        public int getOffset()
        {
            return offset;
        }

        /// <summary>
        /// Read a signed byte from the stream.
        /// </summary>
        /// <returns>The value.</returns>
        public byte readSignedByte()
        {
            return buf[offset++];
        }

        /// <summary>
        /// Read an unsigned byte from the stream.
        /// </summary>
        /// <returns>The value.</returns>
        public int readUnsignedByte()
        {
            return buf[offset++] & 0xff;
        }

        /// <summary>
        /// Write a byte to the stream.
        /// </summary>
        /// <param name="i">The value.</param>
        /// <returns>The packet.</returns>
        public Packet writeByte(int i)
        {
            buf[offset++] = (byte)i;
            return this;
        }

        /// <summary>
        /// Read an unsigned word from the stream.
        /// </summary>
        /// <returns>The value.</returns>
        public int readUnsignedWord()
        {
            offset += 2;
            return ((buf[offset - 2] & 0xff) << 8) + (buf[offset - 1] & 0xff);
        }

        /// <summary>
        /// Write a word to the stream.
        /// </summary>
        /// <param name="i">The value.</param>
        /// <returns>The packet.</returns>
        public Packet writeWord(int i)
        {
            writeByte((byte)(i >> 8));
            writeByte((byte)(i));
            return this;
        }

        /// <summary>
        /// Read a double word.
        /// </summary>
        /// <returns>The value.</returns>
        public int readDWord()
        {
            offset += 4;
            return ((buf[offset - 4] & 0xff) << 24) + ((buf[offset - 3] & 0xff) << 16) + ((buf[offset - 2] & 0xff) << 8) + (buf[offset - 1] & 0xff);
        }

        /// <summary>
        /// Write a double word.
        /// </summary>
        /// <param name="i">The value.</param>
        /// <returns>The packet.</returns>
        public Packet writeDWord(int i)
        {
            writeByte((byte)(i >> 24));
            writeByte((byte)(i >> 16));
            writeByte((byte)(i >> 8));
            writeByte((byte)(i));
            return this;
        }

        /// <summary>
        /// Read a byte array.
        /// </summary>
        /// <returns>The array.</returns>
        public byte[] readByteArray()
        {
            int size = readDWord();
            byte[] array = new byte[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = readSignedByte();
            }
            return array;
        }

        /// <summary>
        /// Write a byte array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>The packet.</returns>
        public Packet writeByteArray(byte[] array)
        {
            writeDWord(array.Length);
            foreach (byte i in array)
            {
                writeByte(i);
            }
            return this;
        }

        /// <summary>
        /// Read a string.
        /// </summary>
        /// <returns>The string.</returns>
        public string readString()
        {
            byte[] bytes = readByteArray();
            return Encoding.UTF32.GetString(bytes);
        }

        /// <summary>
        /// Write a string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The packet.</returns>
        public Packet writeString(string s)
        {
            byte[] bytes = Encoding.UTF32.GetBytes(s);
            writeByteArray(bytes);
            return this;
        }

        /// <summary>
        /// Read a boolean.
        /// </summary>
        /// <returns>The value.</returns>
        public bool readBoolean()
        {
            return readUnsignedByte() == 1;
        }

        /// <summary>
        /// Write a boolean.
        /// </summary>
        /// <param name="b">The value.</param>
        /// <returns>The packet.</returns>
        public Packet writeBoolean(bool b)
        {
            writeByte(b ? 1 : 0);
            return this;
        }
    }
}
