using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIRGBA
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A = 255;

        public FUIRGBA(ByteReader reader)
        {
            R = reader.ReadByte(); // 0x00 + 1 |
            G = reader.ReadByte(); // 0x01 + 1 |
            B = reader.ReadByte(); // 0x02 + 1 |
            A = reader.ReadByte(); // 0x03 + 1 = 0x04
        }

        private FUIRGBA() {}
        public static FUIRGBA Empty { get; } = new FUIRGBA();
    }
}
