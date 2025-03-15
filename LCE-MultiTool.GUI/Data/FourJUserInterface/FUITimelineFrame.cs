using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUITimelineFrame
    {
        public string FrameName = string.Empty;
        public int EventIndex;
        public int EventCount;

        public FUITimelineFrame(ByteReader reader)
        {
            FrameName = reader.ReadAsciiStop(0x40);
            EventIndex = reader.ReadInt();
            EventCount = reader.ReadInt();
        }
    }
}
