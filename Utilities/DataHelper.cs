using System;
using System.IO;
using MinishMaker.Utilities;
using static MinishMaker.Core.RoomMetaData;
using System.Linq;
using System.Text;

namespace MinishMaker.Core
{
    public class DataHelper
    {
        public static byte[] GetData(AddrData addrData)
        {
            if (addrData.compressed)
            {
                return GetFromCompressed(addrData.src);
            }
            else
            {
                return GetUncompressed(addrData.src, addrData.size);
            }
        }

        public static byte[] GetFromSavedData(string path, bool compressed, int size = 0x2000)
        {
            byte[] data = null;
            if (File.Exists(path))
            {
                data = new byte[size];
                byte[] savedData = File.ReadAllBytes(path);
                if (compressed)
                {
                    using (MemoryStream os = new MemoryStream(data))
                    {
                        using (MemoryStream ms = new MemoryStream(savedData))
                        {
                            Reader r = new Reader(ms);
                            var decompSize = Lz77Decompress(r, os);
                            if (decompSize < size && decompSize < int.MaxValue) //TODO: check again
                            {
                                Array.Resize(ref data, (int)decompSize);
                            }
                        }
                    }
                }
            }
            return data;
        }

        public static byte[] GetByteArrayFromJSON(string path, int bytesPerValue)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            if(bytesPerValue > 8)
            {
                throw new ArgumentException("Max bytesPerValue is 8, use GetByteArrayFromJSON2 for infinite");
            }

            var json = File.ReadAllText(path);
            var stringArray = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(json);
            var byteArray = new byte[stringArray.Length * bytesPerValue];

            for (int i = 0; i < stringArray.Length; i++)
            {
                var longVal = Convert.ToInt64(stringArray[i], 16);
                for (int j = 0; j < bytesPerValue; j++)
                {
                    byteArray[i * bytesPerValue + j] = (byte)((longVal >> (8 * j)) & 0xff);
                }
            }

            return byteArray;
        }

        public static byte[] GetByteArrayFromJSON2(string path, int bytesPerValue)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var json = File.ReadAllText(path);
            var stringArray = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(json);
            var byteArray = new byte[stringArray.Length * bytesPerValue];

            for (int i = 0; i < stringArray.Length; i++)
            {
                var stringVal = stringArray[i];
                for (int j = 0; j < bytesPerValue; j++)
                {
                    var byteString = stringVal.Substring(2+2*j, 2); //skip first 2 chars as it should be 0x
                    var byteVal = byte.Parse(byteString, System.Globalization.NumberStyles.HexNumber);
                    byteArray[(i+1) * bytesPerValue - (j+1)] = byteVal; //count down instead as the largest value is in front
                }
            }

            return byteArray;
        }

        public static string ByteArrayToFormattedJSON(byte[] data, int valuesPerRow, int bytesPerValue)
        {
            if (bytesPerValue > 8)
            {
                throw new ArgumentException("Max bytesPerValue is 8, use ByteArrayToFormattedJSON2 for infinite length");
            }

            if (data.Length % bytesPerValue != 0)
            {
                throw new ArgumentException("Data length is not divisible by bytesPerValue");
            }

            var s = new StringBuilder();
            s.AppendLine("[");
            for (var i = 0; i * valuesPerRow * bytesPerValue < data.Length; i++)
            {
                var lineStart = i * valuesPerRow * bytesPerValue;
                s.Append("  ");
                for (var j = 0; j < valuesPerRow; j++)
                {
                    var pos = lineStart + j * bytesPerValue;
                    long value = 0;
                    for(var k = 0; k < bytesPerValue; k++)
                    { 
                        value += (long)(data[pos + k]) << (8 * k) ;
                    }

                    Console.WriteLine((lineStart + (j * bytesPerValue)) + ": 0x" + value.Hex(bytesPerValue * 2));

                    s.Append($"\"0x{value.Hex(bytesPerValue*2)}\"");
                    //Console.WriteLine($"{i}:{j}:{pos + bytesPerValue} : {data.Length}");
                    if (pos + bytesPerValue == data.Length) //Early out if it isnt a aligned to the valuesPerRow
                    {
                        s.AppendLine("");
                        s.Append(']');
                        return s.ToString();
                    }
                    s.Append(" ,");
                }
                s.AppendLine("");
            }
            s.Append(']');

            return s.ToString();
        }

        public static string ByteArrayToFormattedJSON2(byte[] data, int valuesPerRow, int bytesPerValue)
        {
            Console.WriteLine(data.Length);
            if (data.Length % bytesPerValue != 0)
            {
                throw new ArgumentException("Data length is not divisible by bytesPerValue");
            }

            var s = new StringBuilder();
            s.AppendLine("[");
            for (var i = 0; i * valuesPerRow * bytesPerValue < data.Length; i++)
            {
                var lineStart = i * valuesPerRow * bytesPerValue;
                s.Append("  ");
                for (var j = 0; j < valuesPerRow; j++)
                {
                    var pos = lineStart + j * bytesPerValue;
                    
                    s.Append("\"0x");
                    for (var k = bytesPerValue - 1; k >= 0; k--) //count down instead as the largest number is in front
                    {
                        s.Append($"{data[pos + k].Hex(2)}");
                    }
                    s.Append("\"");
                    Console.WriteLine($"{pos + bytesPerValue} : {data.Length}");
                    if (pos + bytesPerValue == data.Length) //Early out if it isnt a aligned to the valuesPerRow
                    {
                        s.AppendLine("");
                        s.Append(']');
                        return s.ToString();
                    }
                    s.Append(" ,");
                }
                s.AppendLine("");
            }
            s.Append(']');

            return s.ToString();
        }

        private static byte[] GetFromCompressed(int addr)
        {
            var r = ROM.Instance.reader;
            var sizeBytes = r.ReadBytes(3, addr);
            var decompressedSize = Unwrap(sizeBytes) >> 8;

            //return the decompressed data
            byte[] data = new byte[decompressedSize];
            r.SetPosition(addr); //pos was changed from getting size

            using (MemoryStream ms = new MemoryStream(data))
                Lz77Decompress(ROM.Instance.reader, ms);
            return data;
        }

        public static Boolean TestCompressed(byte[] orig, byte[] compressed)
        {
            var data = new byte[0x4000];
            using (MemoryStream os = new MemoryStream(data))
            {
                using (MemoryStream ms = new MemoryStream(compressed))
                {
                    Reader r = new Reader(ms);
                    Lz77Decompress(r, os);
                }
            }

            return orig.SequenceEqual(data);
        }

        public static long CompressData(ref byte[] outData, byte[] uncompressedData)
        {
            var compressed = new byte[uncompressedData.Length];
            MemoryStream ous = new MemoryStream(compressed);
            long totalSize = Compress(uncompressedData, ous, false);

            outData = new byte[totalSize];
            Array.Copy(compressed, outData, totalSize);

            totalSize |= 0x80000000;

            return totalSize;
        }

        private static byte[] GetUncompressed(int addr, int size)
        {
            var r = ROM.Instance.reader;
            return r.ReadBytes(size << 2, addr);
        }

        private static int Unwrap(byte[] data)
        {
            return (data[0] | (data[1] << 8) | (data[2] << 16));
        }

        /// <summary>
        /// Decompress a stream that is compressed in the LZ-10 format.
        /// Doesn't check input length (for ROM hacking projects)
        /// </summary>
        /// <param name="outstream">The output stream, where the decompressed data is written to.</param>
        public static long Lz77Decompress(Reader r, Stream outstream)
        {
            #region format definition from GBATEK/NDSTEK
            /*  Data header (32bit)
				  Bit 0-3   Reserved
				  Bit 4-7   Compressed type (must be 1 for LZ77)
				  Bit 8-31  Size of decompressed data
				Repeat below. Each Flag Byte followed by eight Blocks.
				Flag data (8bit)
				  Bit 0-7   Type Flags for next 8 Blocks, MSB first
				Block Type 0 - Uncompressed - Copy 1 Byte from Source to Dest
				  Bit 0-7   One data byte to be copied to dest
				Block Type 1 - Compressed - Copy N+3 Bytes from Dest-Disp-1 to Dest
				  Bit 0-3   Disp MSBs
				  Bit 4-7   Number of bytes to copy (minus 3)
				  Bit 8-15  Disp LSBs
			 */
            #endregion


            var magicByte = 0x10;

            long readBytes = 0;
            byte type = r.ReadByte();

            if (type != magicByte)
                throw new InvalidDataException("The provided stream is not a valid LZ-0x10 "
                            + "compressed stream (invalid type 0x" + type.ToString("X") + ")");

            byte[] sizeBytes = r.ReadBytes(3);
            int decompressedSize = Unwrap(sizeBytes);
            readBytes += 4;

            if (decompressedSize == 0)
            {
                sizeBytes = r.ReadBytes(4);
                decompressedSize = Unwrap(sizeBytes);
                readBytes += 4;
            }

            // the maximum 'DISP-1' is 0xFFF.
            int bufferLength = 0x1000;
            byte[] buffer = new byte[bufferLength];
            int bufferOffset = 0;


            int currentOutSize = 0;
            int flags = 0, mask = 1;
            while (currentOutSize < decompressedSize)
            {
                // (throws when requested new flags byte is not available)
                #region Update the mask. If all flag bits have been read, get a new set.
                // the current mask is the mask used in the previous run. So if it masks the
                // last flag bit, get a new flags byte.
                if (mask == 1)
                {
                    flags = r.ReadByte();
                    readBytes++;
                    if (flags < 0)
                        throw new Exception("The end of the stream was reached before the given amout of data was read.");
                    mask = 0x80;
                }
                else
                {
                    mask >>= 1;
                }
                #endregion

                // bit = 1 <=> compressed.
                if ((flags & mask) > 0)
                {
                    // (throws when < 2 bytes are available)
                    #region Get length and displacement('disp') values from next 2 bytes
                    // there are < 2 bytes available when the end is at most 1 byte away
                    int byte1 = r.ReadByte();
                    readBytes++;
                    int byte2 = r.ReadByte();
                    readBytes++;
                    if (byte2 < 0)
                        throw new Exception("The end of the stream was reached before the given amout of data was read.");

                    // the number of bytes to copy
                    int length = byte1 >> 4;
                    length += 3;

                    // from where the bytes should be copied (relatively)
                    int disp = ((byte1 & 0x0F) << 8) | byte2;
                    disp += 1;

                    if (disp > currentOutSize)
                        throw new InvalidDataException("Cannot go back more than already written. "
                                + "DISP = 0x" + disp.ToString("X") + ", #written bytes = 0x" + currentOutSize.ToString("X")
                                + " at 0x" + (r.Position - 2).ToString("X"));
                    #endregion

                    int bufIdx = bufferOffset + bufferLength - disp;
                    for (int i = 0; i < length; i++)
                    {
                        byte next = buffer[bufIdx % bufferLength];
                        bufIdx++;
                        outstream.WriteByte(next);
                        buffer[bufferOffset] = next;
                        bufferOffset = (bufferOffset + 1) % bufferLength;
                    }
                    currentOutSize += length;
                }
                else
                {
                    int next = r.ReadByte();
                    readBytes++;
                    if (next < 0)
                        throw new Exception("The end of the stream was reached before the given amout of data was read.");

                    currentOutSize++;
                    outstream.WriteByte((byte)next);
                    buffer[bufferOffset] = (byte)next;
                    bufferOffset = (bufferOffset + 1) % bufferLength;
                }
                outstream.Flush();
            }

            return decompressedSize;
        }

        /// <summary>
        /// Compresses the input using the 'original', unoptimized compression algorithm.
        /// This algorithm should yield files that are the same as those found in the games.
        /// (delegates to the optimized method if LookAhead is set)
        /// </summary>
        public static unsafe int Compress(byte[] indata, Stream outstream, bool lookAhead)
        {
            byte magicByte = 0x10;
            var inLength = indata.Length;

            // make sure the decompressed size fits in 3 bytes.
            // There should be room for four bytes, however I'm not 100% sure if that can be used
            // in every game, as it may not be a built-in function.
            if (inLength > 0xFFFFFF)
                throw new Exception("The compression ratio is not high enough to fit the input in a single compressed file.");

            // use the other method if lookahead is enabled
            if (lookAhead)
            {
                return CompressWithLA(inLength, outstream);
            }


            // write the compression header first
            outstream.WriteByte(magicByte);
            outstream.WriteByte((byte)(inLength & 0xFF));
            outstream.WriteByte((byte)((inLength >> 8) & 0xFF));
            outstream.WriteByte((byte)((inLength >> 16) & 0xFF));

            int compressedLength = 4;

            fixed (byte* instart = &indata[0])
            {
                // we do need to buffer the output, as the first byte indicates which blocks are compressed.
                // this version does not use a look-ahead, so we do not need to buffer more than 8 blocks at a time.
                byte[] outbuffer = new byte[8 * 2 + 1];
                outbuffer[0] = 0;
                int bufferlength = 1, bufferedBlocks = 0;
                int readBytes = 0;
                while (readBytes < inLength)
                {
                    #region If 8 blocks are bufferd, write them and reset the buffer
                    // we can only buffer 8 blocks at a time.
                    if (bufferedBlocks == 8)
                    {
                        outstream.Write(outbuffer, 0, bufferlength);
                        compressedLength += bufferlength;
                        // reset the buffer
                        outbuffer[0] = 0;
                        bufferlength = 1;
                        bufferedBlocks = 0;
                    }
                    #endregion

                    // determine if we're dealing with a compressed or raw block.
                    // it is a compressed block when the next 3 or more bytes can be copied from
                    // somewhere in the set of already compressed bytes.
                    int disp;
                    int oldLength = Math.Min(readBytes, 0x1000);
                    int length = GetOccurrenceLength(instart + readBytes, (int)Math.Min(inLength - readBytes, 0x12),
                                                          instart + readBytes - oldLength, oldLength, out disp, 1);

                    // length not 3 or more? next byte is raw data
                    if (length < 3)
                    {
                        outbuffer[bufferlength++] = *(instart + (readBytes++));
                    }
                    else
                    {
                        // 3 or more bytes can be copied? next (length) bytes will be compressed into 2 bytes
                        readBytes += length;

                        // mark the next block as compressed
                        outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

                        outbuffer[bufferlength] = (byte)(((length - 3) << 4) & 0xF0);
                        outbuffer[bufferlength] |= (byte)(((disp - 1) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disp - 1) & 0xFF);
                        bufferlength++;
                    }
                    bufferedBlocks++;
                }

                // copy the remaining blocks to the output
                if (bufferedBlocks > 0)
                {
                    outstream.Write(outbuffer, 0, bufferlength);
                    compressedLength += bufferlength;
                }
            }

            return compressedLength;
        }

        /// <summary>
        /// Variation of the original compression method, making use of Dynamic Programming to 'look ahead'
        /// and determine the optimal 'length' values for the compressed blocks. Is not 100% optimal,
        /// as the flag-bytes are not taken into account.
        /// </summary>
        private static unsafe int CompressWithLA(long inLength, Stream outstream)
        {
            var r = ROM.Instance.reader;
            byte magicByte = 0x10;
            // save the input data in an array to prevent having to go back and forth in a file
            byte[] indata = new byte[inLength];
            try
            {
                indata = r.ReadBytes((int)inLength);
            }
            catch
            {
                throw new Exception("The end of the stream was reached before the given amout of data was read.");
            }

            // write the compression header first
            outstream.WriteByte(magicByte);
            outstream.WriteByte((byte)(inLength & 0xFF));
            outstream.WriteByte((byte)((inLength >> 8) & 0xFF));
            outstream.WriteByte((byte)((inLength >> 16) & 0xFF));

            int compressedLength = 4;

            fixed (byte* instart = &indata[0])
            {
                // we do need to buffer the output, as the first byte indicates which blocks are compressed.
                // this version does not use a look-ahead, so we do not need to buffer more than 8 blocks at a time.
                byte[] outbuffer = new byte[8 * 2 + 1];
                outbuffer[0] = 0;
                int bufferlength = 1, bufferedBlocks = 0;
                int readBytes = 0;

                // get the optimal choices for len and disp
                int[] lengths, disps;
                GetOptimalCompressionLengths(instart, indata.Length, out lengths, out disps);

                while (readBytes < inLength)
                {
                    // we can only buffer 8 blocks at a time.
                    if (bufferedBlocks == 8)
                    {
                        outstream.Write(outbuffer, 0, bufferlength);
                        compressedLength += bufferlength;
                        // reset the buffer
                        outbuffer[0] = 0;
                        bufferlength = 1;
                        bufferedBlocks = 0;
                    }


                    if (lengths[readBytes] == 1)
                    {
                        outbuffer[bufferlength++] = *(instart + (readBytes++));
                    }
                    else
                    {
                        // mark the next block as compressed
                        outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

                        outbuffer[bufferlength] = (byte)(((lengths[readBytes] - 3) << 4) & 0xF0);
                        outbuffer[bufferlength] |= (byte)(((disps[readBytes] - 1) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disps[readBytes] - 1) & 0xFF);
                        bufferlength++;

                        readBytes += lengths[readBytes];
                    }


                    bufferedBlocks++;
                }

                // copy the remaining blocks to the output
                if (bufferedBlocks > 0)
                {
                    outstream.Write(outbuffer, 0, bufferlength);
                    compressedLength += bufferlength;
                }
            }

            return compressedLength;
        }

        /// <summary>
        /// Gets the optimal compression lengths for each start of a compressed block using Dynamic Programming.
        /// This takes O(n^2) time.
        /// </summary>
        /// <param name="indata">The data to compress.</param>
        /// <param name="inLength">The length of the data to compress.</param>
        /// <param name="lengths">The optimal 'length' of the compressed blocks. For each byte in the input data,
        /// this value is the optimal 'length' value. If it is 1, the block should not be compressed.</param>
        /// <param name="disps">The 'disp' values of the compressed blocks. May be 0, in which case the
        /// corresponding length will never be anything other than 1.</param>
        private static unsafe void GetOptimalCompressionLengths(byte* indata, int inLength, out int[] lengths, out int[] disps)
        {
            lengths = new int[inLength];
            disps = new int[inLength];
            int[] minLengths = new int[inLength];

            for (int i = inLength - 1; i >= 0; i--)
            {
                // first get the compression length when the next byte is not compressed
                minLengths[i] = int.MaxValue;
                lengths[i] = 1;
                if (i + 1 >= inLength)
                    minLengths[i] = 1;
                else
                    minLengths[i] = 1 + minLengths[i + 1];
                // then the optimal compressed length
                int oldLength = Math.Min(0x1000, i);
                // get the appropriate disp while at it. Takes at most O(n) time if oldLength is considered O(n)
                // be sure to bound the input length with 0x12, as that's the maximum length for LZ-10 compressed blocks.
                int maxLen = GetOccurrenceLength(indata + i, Math.Min(inLength - i, 0x12),
                                                 indata + i - oldLength, oldLength, out disps[i], 1);
                if (disps[i] > i)
                    throw new Exception("disp is too large");
                for (int j = 3; j <= maxLen; j++)
                {
                    int newCompLen;
                    if (i + j >= inLength)
                        newCompLen = 2;
                    else
                        newCompLen = 2 + minLengths[i + j];
                    if (newCompLen < minLengths[i])
                    {
                        lengths[i] = j;
                        minLengths[i] = newCompLen;
                    }
                }
            }

            // we could optimize this further to also optimize it with regard to the flag-bytes, but that would require 8 times
            // more space and time (one for each position in the block) for only a potentially tiny increase in compression ratio.
        }

        /// <summary>
        /// Determine the maximum size of a LZ-compressed block starting at newPtr, using the already compressed data
        /// starting at oldPtr. Takes O(inLength * oldLength) = O(n^2) time.
        /// </summary>
        /// <param name="newPtr">The start of the data that needs to be compressed.</param>
        /// <param name="newLength">The number of bytes that still need to be compressed.
        /// (or: the maximum number of bytes that _may_ be compressed into one block)</param>
        /// <param name="oldPtr">The start of the raw file.</param>
        /// <param name="oldLength">The number of bytes already compressed.</param>
        /// <param name="disp">The offset of the start of the longest block to refer to.</param>
        /// <param name="minDisp">The minimum allowed value for 'disp'.</param>
        /// <returns>The length of the longest sequence of bytes that can be copied from the already decompressed data.</returns>
        private static unsafe int GetOccurrenceLength(byte* newPtr, int newLength, byte* oldPtr, int oldLength, out int disp, int minDisp)
        {
            disp = 0;
            if (newLength == 0)
                return 0;
            int maxLength = 0;
            // try every possible 'disp' value (disp = oldLength - i)
            for (int i = 0; i < oldLength - minDisp; i++)
            {
                // work from the start of the old data to the end, to mimic the original implementation's behaviour
                // (and going from start to end or from end to start does not influence the compression ratio anyway)
                byte* currentOldStart = oldPtr + i;
                int currentLength = 0;
                // determine the length we can copy if we go back (oldLength - i) bytes
                // always check the next 'newLength' bytes, and not just the available 'old' bytes,
                // as the copied data can also originate from what we're currently trying to compress.
                for (int j = 0; j < newLength; j++)
                {
                    // stop when the bytes are no longer the same
                    if (*(currentOldStart + j) != *(newPtr + j))
                        break;
                    currentLength++;
                }

                // update the optimal value
                if (currentLength > maxLength)
                {
                    maxLength = currentLength;
                    disp = oldLength - i;

                    // if we cannot do better anyway, stop trying.
                    if (maxLength == newLength)
                        break;
                }
            }
            return maxLength;
        }
    }
}
