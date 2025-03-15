using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUITimeline
    {
        public int SymbolIndex;
        public short FrameIndex;
        public short FrameCount;
        public short ActionIndex;
        public short ActionCount;
        private FUIRect _unknown = FUIRect.Empty;

        public FUITimeline(ByteReader reader)
        {
            SymbolIndex = reader.ReadInt();
            FrameIndex = reader.ReadShort();
            FrameCount = reader.ReadShort();
            ActionIndex = reader.ReadShort();
            ActionCount = reader.ReadShort();
            _unknown = new FUIRect(reader);
        }
    }
}
