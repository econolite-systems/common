// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Text;

namespace Econolite.Ode.Serialization
{
    public static class Deserializers
    {
        public static string UTF8(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data.ToArray());
        }

        public static long Long(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException($"Argument [{nameof(data)}] is null");
            }

            if (data.Length != 8)
            {
                throw new ArgumentException($"Size of {nameof(data)} received by Deserializer<Long> is not 8");
            }

            // network byte order -> big endian -> most significant byte in the smallest address.
            long result = ((long)data[0]) << 56 |
                ((long)(data[1])) << 48 |
                ((long)(data[2])) << 40 |
                ((long)(data[3])) << 32 |
                ((long)(data[4])) << 24 |
                ((long)(data[5])) << 16 |
                ((long)(data[6])) << 8 |
                (data[7]);
            return result;
        }

        public static ulong ULong(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException($"Argument [{nameof(data)}] is null");
            }

            if (data.Length != 8)
            {
                throw new ArgumentException($"Size of {nameof(data)} received by Deserializer<Long> is not 8");
            }

            // network byte order -> big endian -> most significant byte in the smallest address.
            ulong result = ((ulong)data[0]) << 56 |
                ((ulong)(data[1])) << 48 |
                ((ulong)(data[2])) << 40 |
                ((ulong)(data[3])) << 32 |
                ((ulong)(data[4])) << 24 |
                ((ulong)(data[5])) << 16 |
                ((ulong)(data[6])) << 8 |
                (data[7]);
            return result;
        }

        public static int Int32(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException($"Argument {nameof(data)} is null");
            }

            if (data.Length != 4)
            {
                throw new ArgumentException($"Size of {nameof(data)} received by Deserializer<Int32> is not 4");
            }

            // network byte order -> big endian -> most significant byte in the smallest address.
            return
                (((int)data[0]) << 24) |
                (((int)data[1]) << 16) |
                (((int)data[2]) << 8) |
                (int)data[3];
        }

        public static uint UInt32(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException($"Argument {nameof(data)} is null");
            }

            if (data.Length != 4)
            {
                throw new ArgumentException($"Size of {nameof(data)} received by Deserializer<UInt32> is not 4");
            }

            // network byte order -> big endian -> most significant byte in the smallest address.
            return
                (((uint)data[0]) << 24) |
                (((uint)data[1]) << 16) |
                (((uint)data[2]) << 8) |
                (uint)data[3];
        }

        public static float Float(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException($"Argument {nameof(data)} is null");
            }

            if (data.Length != 4)
            {
                throw new ArgumentException($"Size of {nameof(data)} received by Deserializer<Float> is not 4");
            }

            // network byte order -> big endian -> most significant byte in the smallest address.
            if (BitConverter.IsLittleEndian)
            {
                unsafe
                {
                    float result = default(float);
                    byte* p = (byte*)(&result);
                    *p++ = data[3];
                    *p++ = data[2];
                    *p++ = data[1];
                    *p++ = data[0];
                    return result;
                }
            }
            else
            {
                return BitConverter.ToSingle(data.ToArray(), 0);
            }
        }

        public static double Double(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException($"Argument {nameof(data)} is null");
            }

            if (data.Length != 8)
            {
                throw new ArgumentException($"Size of {nameof(data)} received by Deserializer<Double> is not 8");
            }

            // network byte order -> big endian -> most significant byte in the smallest address.
            if (BitConverter.IsLittleEndian)
            {
                unsafe
                {
                    double result = default(double);
                    byte* p = (byte*)(&result);
                    *p++ = data[7];
                    *p++ = data[6];
                    *p++ = data[5];
                    *p++ = data[4];
                    *p++ = data[3];
                    *p++ = data[2];
                    *p++ = data[1];
                    *p++ = data[0];
                    return result;
                }
            }
            else
            {
#if NETCOREAPP2_1
                return BitConverter.ToDouble(data);
#else
                return BitConverter.ToDouble(data.ToArray(), 0);
#endif
            }
        }
        public static byte[] ByteArray(ReadOnlySpan<byte> data)
        {
            if (data == null) return null;
            return data.ToArray();
        }

        public static short Short(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException($"Argument {nameof(data)} is null");
            }

            if (data.Length != 2)
            {
                throw new ArgumentException($"Size of {nameof(data)} received by Deserializer<Short> is not 2");
            }

            // network byte order -> big endian -> most significant byte in the smallest address.
            return
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
                (short)((((short)data[0]) << 8) |
                (short)data[1]);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
        }

        public static ushort UShort(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException($"Argument {nameof(data)} is null");
            }

            if (data.Length != 2)
            {
                throw new ArgumentException($"Size of {nameof(data)} received by Deserializer<UShort> is not 2");
            }

            // network byte order -> big endian -> most significant byte in the smallest address.
            return
                (ushort)((((ushort)data[0]) << 8) |
                (ushort)data[1]);
        }

        public static Guid Guid(ReadOnlySpan<byte> data)
        {
            return new Guid(ByteArray(data));
        }

        public static DateTime DateTime(ReadOnlySpan<byte> data)
        {
            return System.DateTime.SpecifyKind(System.DateTime.FromBinary(Long(data)), DateTimeKind.Utc);
        }
    }
}
