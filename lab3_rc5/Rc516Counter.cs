using System;
using System.Diagnostics;
using System.IO;

namespace lab3_rc5
{
    public class Rc516Counter
    {
        const int k_size = 1024;
        private const ushort k_pw = 0xb7e1;
        private const ushort k_qw = 0x9e37;
        private const ushort k_dwsize = sizeof(uint);

        private int m_raunds;
        private ushort[] m_subKeys;
        private Random m_rand = new Random();

        private uint m_xorArgument;

        public Rc516Counter(int raunds, UInt16[] keys)
        {
            m_raunds = raunds;
            GenerateSubkeys(keys);
        }

        private void GenerateSubkeys(ushort[] keys)
        {
            ushort[] copy = new ushort[keys.Length];
            keys.CopyTo(copy, 0);
            m_subKeys = new ushort[2 * m_raunds  +2];
            for (int i = 0; i < m_subKeys.Length; i++)
            {
                m_subKeys[i] = (ushort)(k_pw + i*k_qw);
            }
            int ii = 0;
            int j = 0, t = Math.Max(keys.Length, m_subKeys.Length);
            ushort a = 0, b = 0;
            for (int s = 0; s < 3*t; s++)
            {
                m_subKeys[ii] = CyclShift(m_subKeys[ii] + a + b, 3);
                a = m_subKeys[ii];
                ii = ii%m_subKeys.Length;
                copy[j] = CyclShift(copy[j] + a + b, (ushort)(a + b));
                b = copy[j];
                j = j%copy.Length;
            }
        }

        public void Crypt(Stream input, Stream output)
        {
            m_xorArgument =(uint) m_rand.Next();
            var cryptXorArgument = CryptValue((ushort)m_xorArgument, (ushort)(m_xorArgument >> 16));
            output.Write(BitConverter.GetBytes(cryptXorArgument), 0, k_dwsize);

            byte[] bytes = new byte[k_dwsize];
            bool endOfBlocks = false;
            while (!endOfBlocks)
            {
                int bytesCount = input.Read(bytes, 0, bytes.Length);
                endOfBlocks = CryptCycle(BitConverter.ToUInt32(bytes, 0), output, bytesCount);
            }
        }

        private bool CryptCycle(uint value, Stream outputStream, int bytesCount)
        {
            var result = bytesCount < k_dwsize;
            if (result)
            {
                value = FillPadding(BitConverter.GetBytes(value), bytesCount);
            }

            value = value ^ m_xorArgument;
            m_xorArgument = CryptValue((ushort) value, (ushort) (value >> 16));
            outputStream.Write(BitConverter.GetBytes(m_xorArgument), 0, sizeof (uint));
            return result;
        }



        private ushort CyclShift(int value, ushort count, bool leftDir = true)
        {
            count = (ushort)(count % 16);
            ushort v = (ushort)value;
            ushort result = 0;
            if (leftDir)
                result = (ushort)((v << count) | (v >> (16 - count)));
            else
            {
                result = (ushort)((v >> count) | (v << (16 - count)));
            }
            return result;
        }

        private uint FillPadding(byte[] bytes, int bytesCount)
        {
            int additional = k_dwsize - bytesCount;
            Debug.Assert(additional >= 0);
            for (int i = 0; i < additional; i++)
            {
                bytes[bytesCount + i] = (byte)additional;
            };
            return BitConverter.ToUInt32(bytes, 0);
        }

        public void Decrypt(Stream input, Stream output)
        {
            var initVector = new byte[4];
            int vectorCount = input.Read(initVector, 0, 4);
            if (vectorCount < 4)
                throw new Exception("File was not crypted");
            var val = BitConverter.ToUInt32(initVector, 0);
            m_xorArgument = DecryptValue(BitConverter.ToUInt16(initVector, 0), BitConverter.ToUInt16(initVector, 2));
            byte[] bytes = new byte[k_dwsize];
            int bytesCount = input.Read(bytes, 0, bytes.Length);
            uint preview = BitConverter.ToUInt32(bytes, 0);
            bool endOfBlock = false;
            while (!endOfBlock)
            {
                bytesCount = input.Read(bytes, 0, bytes.Length);
                endOfBlock = DecryptCycle(preview, output, bytesCount);
                preview = BitConverter.ToUInt32(bytes, 0);
            }
        }

        private bool DecryptCycle(uint preview, Stream output, int bytesCount)
        {
            bool result = bytesCount == 0;
            if (bytesCount != k_dwsize && bytesCount != 0)
                throw new Exception("File was not crypted");
            var value = DecryptValue((ushort)preview, (ushort)(preview >> 16));
            value = value ^ m_xorArgument;
            m_xorArgument = preview;
            var bytes = BitConverter.GetBytes(value);
            if (!result)
            {
                output.Write(bytes, 0, k_dwsize);
            }
            else
            {
                int addit = bytes[bytes.Length - 1];
                output.Write(bytes, 0, k_dwsize - addit);
            }
            return result;
        }

        private uint CryptValue(ushort a, ushort b)
        {
            a = (ushort)(a + m_subKeys[0]);
            b = (ushort)(b + m_subKeys[1]);
            for (int i = 1; i <= m_raunds; i++)
            {
                a = (ushort)(CyclShift(a ^ b, b) + m_subKeys[2 * i]);
                b = (ushort)(CyclShift(b ^ a, a) + m_subKeys[2 * i + 1]);
            }
            return (uint)((b << 16) + a);
        }

        private uint DecryptValue(ushort a, ushort b)
        {
            for (int i = m_raunds; i > 0; i--)
            {
                b = (ushort)(CyclShift(b - m_subKeys[2 * i + 1], a, false) ^ a);
                a = (ushort)(CyclShift(a - m_subKeys[2 * i], b, false) ^ b);
            }
            a = (ushort)(a - m_subKeys[0]);
            b = (ushort)(b - m_subKeys[1]);
            return (uint)((b << 16) + a );
        }
    }
}
