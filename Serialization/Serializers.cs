// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Text;

namespace Econolite.Ode.Serialization
{
    public static class Serializers
    {
        public static byte[] UTF8(string data)
        {
            if (data == null)
            {
                return null;
            }

            return Encoding.UTF8.GetBytes(data);
        }

        public static byte[] Long(long data)
        {
            var result = new byte[8];
            result[0] = (byte)(data >> 56);
            result[1] = (byte)(data >> 48);
            result[2] = (byte)(data >> 40);
            result[3] = (byte)(data >> 32);
            result[4] = (byte)(data >> 24);
            result[5] = (byte)(data >> 16);
            result[6] = (byte)(data >> 8);
            result[7] = (byte)data;
            return result;
        }

        public static byte[] ULong(ulong data)
        {
            var result = new byte[8];
            result[0] = (byte)(data >> 56);
            result[1] = (byte)(data >> 48);
            result[2] = (byte)(data >> 40);
            result[3] = (byte)(data >> 32);
            result[4] = (byte)(data >> 24);
            result[5] = (byte)(data >> 16);
            result[6] = (byte)(data >> 8);
            result[7] = (byte)data;
            return result;
        }

        public static byte[] Int32(int data)
        {
            var result = new byte[4]; // int is always 32 bits on .NET.
            // network byte order -> big endian -> most significant byte in the smallest address.
            // Note: At the IL level, the conv.u1 operator is used to cast int to byte which truncates
            // the high order bits if overflow occurs.
            // https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.conv_u1.aspx
            result[0] = (byte)(data >> 24);
            result[1] = (byte)(data >> 16); // & 0xff;
            result[2] = (byte)(data >> 8); // & 0xff;
            result[3] = (byte)data; // & 0xff;
            return result;
        }

        public static byte[] UInt32(uint data)
        {
            var result = new byte[4];
            result[0] = (byte)(data >> 24);
            result[1] = (byte)(data >> 16); // & 0xff;
            result[2] = (byte)(data >> 8); // & 0xff;
            result[3] = (byte)data; // & 0xff;
            return result;
        }

        public static byte[] Float(float data)
        {
            if (BitConverter.IsLittleEndian)
            {
                unsafe
                {
                    byte[] result = new byte[4];
                    byte* p = (byte*)(&data);
                    result[3] = *p++;
                    result[2] = *p++;
                    result[1] = *p++;
                    result[0] = *p++;
                    return result;
                }
            }
            else
            {
                return BitConverter.GetBytes(data);
            }
        }

        public static byte[] Double(double data)
        {
            if (BitConverter.IsLittleEndian)
            {
                unsafe
                {
                    byte[] result = new byte[8];
                    byte* p = (byte*)(&data);
                    result[7] = *p++;
                    result[6] = *p++;
                    result[5] = *p++;
                    result[4] = *p++;
                    result[3] = *p++;
                    result[2] = *p++;
                    result[1] = *p++;
                    result[0] = *p++;
                    return result;
                }
            }
            else
            {
                return BitConverter.GetBytes(data);
            }
        }

        public static byte[] ByteArray(byte[] data)
        {
            return data;
        }

        public static byte[] Short(short data)
        {
            var result = new byte[2];
            result[0] = (byte)(data >> 8); // & 0xff;
            result[1] = (byte)data; // & 0xff;
            return result;
        }

        public static byte[] UShort(ushort data)
        {
            var result = new byte[2];
            result[0] = (byte)(data >> 8); // & 0xff;
            result[1] = (byte)data; // & 0xff;
            return result;
        }

        public static byte[] Guid(Guid data)
        {
            return ByteArray(data.ToByteArray());
        }

        public static byte[] DateTime(DateTime data)
        {
            return Long(System.DateTime.SpecifyKind(data, DateTimeKind.Utc).ToBinary());
        }
    }
}
