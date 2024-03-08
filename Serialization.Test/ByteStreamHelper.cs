// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Econolite.Ode.Serialization.Test
{
    public class BadByteStreamFormatException : Exception
    {
        public BadByteStreamFormatException()
        {
        }

        public BadByteStreamFormatException(string message) : base(message)
        {
        }

        public BadByteStreamFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadByteStreamFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class ByteStreamHelper
    {
        private static char[] hexchars = { '0', '1', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static byte[] Convert(string data)
        {
            var tmp = data.ToUpper();
            var chararray = tmp.ToCharArray();

            var currentcount = 0;
            var currentlyinnum = false;
            byte currentbyte = 0;
            var stream = new MemoryStream();
            var onebyte = new byte[1];
            for (var i = 0; i < chararray.Length; ++i)
                switch (chararray[i])
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case ',':
                    case '\n':
                        if (currentlyinnum)
                            throw new BadByteStreamFormatException(
                                string.Format("Invalid formatted number at index: {0}", i));
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                        if (!currentlyinnum && currentcount == 0)
                        {
                            currentbyte = GetByte(chararray[i]);
                            currentlyinnum = true;
                            currentcount = 1;
                        }
                        else if (currentcount == 1)
                        {
                            currentbyte = (byte)(currentbyte << 4 | GetByte(chararray[i]));
                            onebyte[0] = currentbyte;
                            stream.Write(onebyte, 0, 1);
                            currentlyinnum = false;
                            currentcount = 0;
                        }
                        else
                        {
                            throw new BadByteStreamFormatException(
                                string.Format("Invalid formatted number at index: {0}", i));
                        }

                        break;
                }

            if (currentlyinnum)
                throw new BadByteStreamFormatException(string.Format("Invalid formatted number at index: {0}",
                    chararray.Length));
            return stream.ToArray();
        }

        private static byte GetByte(char character)
        {
            byte result;
            if ('0' <= character && character <= '9')
                result = (byte)(character & 0x0F);
            else if ('A' <= character && character <= 'F')
                result = (byte)(character - 55);
            else
                throw new BadByteStreamFormatException(string.Format("Invalid character: {0}", character));
            return (byte)(result & 0x0F);
        }

        public static string Convert(byte[] data)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < data.Length; ++i)
            {
                sb.Append(data[i].ToString("X2"));

                if (i < data.Length - 1) sb.Append(' ');
            }

            return sb.ToString();
        }

        public static byte[] Extract(ref MemoryStream stream)
        {
            var result = new byte[stream.Length];
            var oldpos = stream.Position;
            stream.Position = 0;

            stream.Read(result, 0, result.Length);
            stream.Position = oldpos;

            return result;
        }
    }
}
