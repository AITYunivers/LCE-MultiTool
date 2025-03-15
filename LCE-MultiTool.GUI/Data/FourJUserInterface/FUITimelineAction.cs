using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUITimelineAction
    {
        public byte ActionType;
        private byte _unknown;
        public short FrameIndex;
        public string StringArg0 = string.Empty;
        public string StringArg1 = string.Empty;

        public FUITimelineAction(ByteReader reader)
        {
            ActionType = reader.ReadByte();
            _unknown = reader.ReadByte();
            FrameIndex = reader.ReadShort();
            StringArg0 = reader.ReadAsciiStop(0x40);
            StringArg1 = reader.ReadAsciiStop(0x40);
        }
    }
}
