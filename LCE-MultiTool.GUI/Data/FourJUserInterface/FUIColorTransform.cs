using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIColorTransform
    {
        public float RedMultTerm;
        public float GreenMultTerm;
        public float BlueMultTerm;
        public float AlphaMultTerm;
        public float RedAddTerm;
        public float GreenAddTerm;
        public float BlueAddTerm;
        public float AlphaAddTerm;

        public FUIColorTransform(ByteReader reader)
        {
            RedMultTerm = reader.ReadFloat();   // 0x00 + 4 |
            GreenMultTerm = reader.ReadFloat(); // 0x04 + 4 |
            BlueMultTerm = reader.ReadFloat();  // 0x08 + 4 |
            AlphaMultTerm = reader.ReadFloat(); // 0x0C + 4 |
            RedAddTerm = reader.ReadFloat();    // 0x10 + 4 |
            GreenAddTerm = reader.ReadFloat();  // 0x14 + 4 |
            BlueAddTerm = reader.ReadFloat();   // 0x18 + 4 |
            AlphaAddTerm = reader.ReadFloat();  // 0x1C + 4 = 0x20
        }

        private FUIColorTransform() {}
        public static FUIColorTransform Empty { get; } = new FUIColorTransform();
    }
}
