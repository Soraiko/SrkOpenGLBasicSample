using System;
using System.Collections.Generic;
using System.Text;

namespace SrkAlternatives
{
    public class BitConverter : IDisposable
    {
        public void Dispose()
        {

        }

        public BitConverter()
        {

        }
        public BitConverter(ref byte[] data)
        {
            this.Data = data;
        }

        public byte[] Data;
        public Int32 Int32(int position)
        {
            return global::System.BitConverter.ToInt32(this.Data, position);
        }

        public Int16 Int16(int position)
        {
            return global::System.BitConverter.ToInt16(this.Data, position);
        }
        public UInt16 UInt16(int position)
        {
            return global::System.BitConverter.ToUInt16(this.Data, position);
        }

        public Single Single(int position)
        {
            return global::System.BitConverter.ToSingle(this.Data, position);
        }
    }
}
