using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Elmanager.IO
{
    internal static class FileUtils
    {
        internal static string ReadNullTerminatedString(byte[] data, int startIndex, int initialSize)
        {
            byte[] tempBytes = new byte[initialSize];
            for (int j = startIndex; j < startIndex + initialSize; j++)
            {
                if (data[j] != 0)
                    tempBytes[j - startIndex] = data[j];
                else
                {
                    Array.Resize(ref tempBytes, j - startIndex);
                    break;
                }
            }

            return Encoding.ASCII.GetString(tempBytes);
        }

        internal static string ReadNullTerminatedString(this BinaryReader reader, int count)
        {
            var chars = reader.ReadChars(count);
            return new string(chars.TakeWhile(c => c != '\0').ToArray());
        }

        internal static string ReadString(this BinaryReader reader, int count)
        {
            return new(reader.ReadChars(count));
        }
    }
}