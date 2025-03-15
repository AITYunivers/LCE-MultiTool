using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIMatrix
    {
        public float ScaleX = 1;
        public float ScaleY = 1;
        public float SkewX;
        public float SkewY;
        public float TranslateX;
        public float TranslateY;

        public FUIMatrix(ByteReader reader)
        {
            ScaleX = reader.ReadFloat();        // 0x00 + 4 |
            SkewX = reader.ReadFloat();         // 0x04 + 4 |
            SkewY = reader.ReadFloat();         // 0x08 + 4 |
            ScaleY = reader.ReadFloat();        // 0x0C + 4 |
            TranslateX = reader.ReadFloat();    // 0x10 + 4 |
            TranslateY = reader.ReadFloat();    // 0x14 + 4 = 0x18
        }

        private FUIMatrix() {}
        public static FUIMatrix Empty { get; } = new FUIMatrix();
    }
}
