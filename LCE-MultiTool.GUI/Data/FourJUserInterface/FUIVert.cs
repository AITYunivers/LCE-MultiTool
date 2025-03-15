using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIVert
    {
        public float X;
        public float Y;

        public FUIVert(ByteReader reader)
        {
            X = reader.ReadFloat();
            Y = reader.ReadFloat();
        }
    }
}
