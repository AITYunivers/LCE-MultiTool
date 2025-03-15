using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIFillStyle
    {
        public int Type;
        public FUIRGBA Color = FUIRGBA.Empty;
        public int BitmapIndex;
        public FUIMatrix Matrix = FUIMatrix.Empty;

        public FUIFillStyle(ByteReader reader)
        {
            Type = reader.ReadInt();        // 0x00 + 4  |
            Color = new FUIRGBA(reader);    // 0x04 + 4  |
            BitmapIndex = reader.ReadInt(); // 0x08 + 4  |
            Matrix = new FUIMatrix(reader); // 0x0C + 24 = 0x36
        }

        private FUIFillStyle() {}
        public static FUIFillStyle Empty { get; } = new FUIFillStyle();
    }
}
