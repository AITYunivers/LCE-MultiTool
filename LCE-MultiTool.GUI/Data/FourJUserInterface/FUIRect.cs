using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIRect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public FUIRect(ByteReader reader)
        {
            X = reader.ReadFloat();          // 0x00 + 4 |
            Width = reader.ReadFloat() - X;  // 0x04 + 4 |
            Y = reader.ReadFloat();          // 0x08 + 4 |
            Height = reader.ReadFloat() - Y; // 0x0C + 4 = 0x10
        }

        private FUIRect() {}
        public static FUIRect Empty { get; } = new FUIRect();
    }
}
