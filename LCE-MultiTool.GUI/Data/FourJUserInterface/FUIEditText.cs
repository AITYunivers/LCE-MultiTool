using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIEditText
    {
        private int _unknown1;
        public FUIRect Rect = FUIRect.Empty;
        public int FontID;
        private float _unknown2;
        public FUIRGBA Color = FUIRGBA.Empty;
        private int _unknown3;
        private int _unknown4;
        private int _unknown5;
        private int _unknown6;
        private int _unknown7;
        private int _unknown8;
        public string HTMLText = string.Empty;

        public FUIEditText(ByteReader reader)
        {
            _unknown1 = reader.ReadInt();           // 0x00 + 4   |
            Rect = new FUIRect(reader);             // 0x04 + 16  |
            FontID = reader.ReadInt();              // 0x14 + 4   |
            _unknown2 = reader.ReadFloat();         // 0x18 + 4   |
            Color = new FUIRGBA(reader);            // 0x1C + 4   |
            _unknown3 = reader.ReadInt();           // 0x20 + 4   |
            _unknown4 = reader.ReadInt();           // 0x24 + 4   |
            _unknown5 = reader.ReadInt();           // 0x28 + 4   |
            _unknown6 = reader.ReadInt();           // 0x2C + 4   |
            _unknown7 = reader.ReadInt();           // 0x30 + 4   |
            _unknown8 = reader.ReadInt();           // 0x34 + 4   |
            HTMLText = reader.ReadAsciiStop(0x100); // 0x38 + 256 = 0x138
        }
    }
}
