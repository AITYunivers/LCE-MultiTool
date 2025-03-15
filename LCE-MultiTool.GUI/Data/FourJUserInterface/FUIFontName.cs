using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIFontName
    {
        public int ID;
        public string Name = string.Empty;
        private int _unknown1;
        private string _unknown2 = string.Empty;
        private int _unknown3;
        private int _unknown4;
        private string _unknown5 = string.Empty;
        private int _unknown6;
        private int _unknown7;
        private string _unknown8 = string.Empty;

        public FUIFontName(ByteReader reader)
        {
            ID = reader.ReadInt();                  // 0x00 + 4  |
            Name = reader.ReadAsciiStop(0x40);      // 0x04 + 64 |
            _unknown1 = reader.ReadInt();           // 0x44 + 4  |
            _unknown2 = reader.ReadAsciiStop(0x40); // 0x48 + 64 |
            _unknown3 = reader.ReadInt();           // 0x88 + 4  |
            _unknown4 = reader.ReadInt();           // 0x8C + 4  |
            _unknown5 = reader.ReadAsciiStop(0x40); // 0x90 + 64 |
            _unknown6 = reader.ReadInt();           // 0xD0 + 4  |
            _unknown7 = reader.ReadInt();           // 0xD4 + 4  |
            _unknown8 = reader.ReadAsciiStop(0x2C); // 0xD8 + 44 = 0x104
        }
    }
}
