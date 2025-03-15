using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUITimelineEvent
    {
        public byte EventType;
        private byte _unknown1;
        public byte ObjectType;
        private byte _unknown2;
        private short _unknown3;
        public short Index;
        private short _unknown4;
        public short NameIndex;
        public FUIMatrix Matrix = FUIMatrix.Empty;
        public FUIColorTransform ColorTransform = FUIColorTransform.Empty;
        public FUIRGBA Color = FUIRGBA.Empty;

        public FUITimelineEvent(ByteReader reader)
        {
            EventType = reader.ReadByte();                  // 0x00 + 1  |
            _unknown1 = reader.ReadByte();                  // 0x01 + 1  |
            ObjectType = reader.ReadByte();                 // 0x02 + 1  |
            _unknown2 = reader.ReadByte();                  // 0x03 + 1  |
            _unknown3 = reader.ReadShort();                 // 0x04 + 2  |
            Index = reader.ReadShort();                     // 0x06 + 2  |
            _unknown4 = reader.ReadShort();                 // 0x08 + 2  |
            NameIndex = reader.ReadShort();                 // 0x0A + 2  |
            Matrix = new FUIMatrix(reader);                 // 0x0C + 24 |
            ColorTransform = new FUIColorTransform(reader); // 0x24 + 32 |
            Color = new FUIRGBA(reader);                    // 0x44 + 4  = 0x48
        }
    }
}
