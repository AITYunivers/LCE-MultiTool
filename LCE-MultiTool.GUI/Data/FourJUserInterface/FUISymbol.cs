using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUISymbol
    {
        public string SymbolName = string.Empty;
        public int ObjectType;
        public int Index;

        public FUISymbol(ByteReader reader)
        {
            SymbolName = reader.ReadAsciiStop(0x40);
            ObjectType = reader.ReadInt();
            Index = reader.ReadInt();
        }
    }
}
