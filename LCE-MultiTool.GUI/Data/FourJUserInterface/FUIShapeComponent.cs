using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIShapeComponent
    {
        public FUIFillStyle FillInfo = FUIFillStyle.Empty;
        public int VertIndex;
        public int VertCount;

        public FUIShapeComponent(ByteReader reader)
        {
            FillInfo = new FUIFillStyle(reader);
            VertIndex = reader.ReadInt();
            VertCount = reader.ReadInt();
        }
    }
}
