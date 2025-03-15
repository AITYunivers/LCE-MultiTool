using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIShape
    {
        private int _unknown;
        public int ComponentIndex;
        public int ComponentCount;
        public FUIRect Rect = FUIRect.Empty;

        public FUIShape(ByteReader reader)
        {
            _unknown = reader.ReadInt();
            ComponentIndex = reader.ReadInt();
            ComponentCount = reader.ReadInt();
            Rect = new FUIRect(reader);
        }
    }
}
