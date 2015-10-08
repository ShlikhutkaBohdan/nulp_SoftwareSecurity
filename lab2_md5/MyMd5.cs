using System;
using System.IO;
using System.Text;


namespace lab2_md5
{

	public class MyMD5
	{

	    public delegate void OnProgressChangedEvent(ushort persentage);

	    public event OnProgressChangedEvent OnProgressChanged;

	    protected static readonly uint[] T = new uint[64];

	    static MyMD5()
	    {
	        long k = (long)Math.Pow(2, 32);//2 ^ 32
            for (int i = 1; i <= 64; i++)
                T[i-1] = (uint)(long)(k * Math.Abs(Math.Sin(i)));
	    }
		
        public const uint A0 = 0x67452301;
        public const uint B0 = 0xEFCDAB89;
        public const uint C0 = 0x98BADCFE;
        public const uint D0 = 0X10325476;

	    public string GetMd5FromString(string message)
	    {
	        Stream stream = GenerateStreamFromString(message);
            return Md5FromStream(stream);
	    }

        public string GetMd5FromFile(string filename)
	    {
            using (FileStream SourceStream = File.Open(filename, FileMode.Open))
            {
                return Md5FromStream(SourceStream);
            }
	    }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static uint MyCLS(uint number, ushort shift)
        {
            return ((number >> 32 - shift) | (number << shift));
        }

        private string Md5FromStream(Stream stream)
        {

            long oldLength = stream.Length;
            byte[] paddingBytes = CreateTail(oldLength);
            
            uint a = A0;
            uint b = B0;
            uint c = C0;
            uint d = D0;
            
            byte[] buffer = new byte[64];//512 bits
            uint[] block = new uint[16];

            int len;
            bool f = false;
            bool isStreamPadding = false;

            MemoryStream lastBlockStream = new MemoryStream();
            ulong i = 0;
            do
            {
                //begin cycle
                len = stream.Read(buffer, 0, 64);

                if (len == 0 && isStreamPadding)
                    break;
                if (len < 64)
                {
                    lastBlockStream.Write(buffer, 0, len);
                    lastBlockStream.Write(paddingBytes, 0, paddingBytes.Length);
                    stream = lastBlockStream;
                    stream.Position = 0;
                    len = -1;
                    isStreamPadding = true;
                    continue;
                }
                int index = 0;
                for (int j = 0; j < 61; j += 4)
                    block[j >> 2] = (((uint) buffer[(j + 3)]) << 24) |
                                    (((uint) buffer[(j + 2)]) << 16) |
                                    (((uint) buffer[(j + 1)]) << 8) |
                                    (((uint) buffer[(j)]));
                ProcessBlock(block, ref a, ref b, ref c, ref d);

                //for progress update
                i++;
                if (OnProgressChanged != null)
                {
                    ushort persentage = (ushort) ((i*64)/(double) oldLength*100);
                    OnProgressChanged(persentage);
                }
            } while (len != 0);
		    return ReverseByte(a).ToString("X8") +
                ReverseByte(b).ToString("X8") +
                ReverseByte(c).ToString("X8") +
                ReverseByte(d).ToString("X8"); ;
		}

        private static readonly ushort[,] _sS = 
        {
            {7, 12, 17, 22},
            {5, 9, 14, 20},
            {4, 11, 16, 23},
            {6, 10, 15, 21}
        };
		
		private void ProcessBlock(uint[] X, ref uint A,ref uint B,ref uint C, ref uint D)//X - block 512 byte
		{
		    uint a = A;
		    uint b = B;
		    uint c = C;
		    uint d = D;
            ushort[,] sArr = _sS;
		    uint cycleNum = 0;

		    for (uint i = 0; i < 64; ++i)
		    {
		        cycleNum = i/16;
		        uint column = i%4;
		        uint k = i;
                ushort s = sArr[cycleNum, column];
                switch (cycleNum)
		        {
                    case 0:
                        //f function
                        a = b + MyCLS((a + ((b & c) | (~(b) & d)) + X[k] + T[i]), s);
                        break;
                    case 1:
                        k = (k * 5 + 1) % 16;
                        //g function
                        a = b + MyCLS((a + ((b & d) | (c & ~d)) + X[k] + T[i]), s);
                        break;
                    case 2:
		                k = (k*3 + 5)% 16;
                        //h function
                        a = b + MyCLS((a + (b ^ c ^ d) + X[k] + T[i]), s);
                        break;
                    case 3:
                        k = (k * 7) %16;
                        //i function
                        a = b + MyCLS((a + (c ^ (b | ~d)) + X[k] + T[i]), s);
                        break;   
		        }
		        uint temp = d;
                d = c;
                c = b;
                b = a;
                a = temp;
		    }
		    A += a;
		    B += b;
		    C += c;
		    D += d;
		}
       

        private byte[] CreateTail(long length)
        {
            long temp = (448 - ((length * 8) % 512));
            uint pad = (uint)((temp + 512) % 512);
            if (pad == 0)
                pad = 512;
            uint sizeMsgBuff = ((pad / 8) + 8);//58
            ulong sizeMsg = (ulong)length * 8;
            byte[] tail = new byte[sizeMsgBuff];
            tail[0] |= 0x80;
            for (int i = 8; i > 0; i--)
                tail[sizeMsgBuff - i] = (byte)(sizeMsg >> ((8 - i) * 8) & 0x00000000000000ff);
            return tail;
        }

        private static uint ReverseByte(uint number)
        {
            return (((number & 0x000000ff) << 24) |
                        (number >> 24) |
                    ((number & 0x00ff0000) >> 8) |
                    ((number & 0x0000ff00) << 8));
        }
	}

}
