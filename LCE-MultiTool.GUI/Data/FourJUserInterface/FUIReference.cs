using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIReference
    {
        public int SymbolIndex;
        public string Name = string.Empty;
        public int Index;

        public FUIReference(ByteReader reader)
        {
            SymbolIndex = reader.ReadInt();     // 0x00 + 4  |
            Name = reader.ReadAsciiStop(0x40);  // 0x04 + 64 |
            Index = reader.ReadInt();           // 0x44 + 4  = 0x48
        }
    }
}
