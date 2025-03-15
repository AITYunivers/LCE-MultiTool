using LCE_MultiTool.Data.Archive;
using LCE_MultiTool.GUI.Memory;
using System.IO;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIFile
    {
        public byte Version;
        public string FileName = string.Empty;
        public FUIRect FrameSize = FUIRect.Empty;

        public FUITimeline[] Timelines = [];
        public FUITimelineAction[] TimelineActions = [];
        public FUIShape[] Shapes = [];
        public FUIShapeComponent[] ShapeComponents = [];
        public FUIVert[] Verts = [];
        public FUITimelineFrame[] TimelineFrames = [];
        public FUITimelineEvent[] TimelineEvents = [];
        public string[] TimelineEventNames = [];
        public FUIReference[] References = [];
        public FUIEditText[] EditTexts = [];
        public FUIFontName[] FontNames = [];
        public FUISymbol[] Symbols = [];
        public string[] ImportAssets = [];
        public FUIBitmap[] Bitmaps = [];

        private FUIFile() { }

        public static FUIFile Load(ByteReader reader)
        {
            FUIFile fui = new FUIFile();

            fui.Version = reader.ReadByte();                        // 0x00 + 1  |
            string header = reader.ReadAscii(3);                    // 0x01 + 3  |
            reader.SetEndian(header == "FUI");                      //           |
            if (header != "IUF" && header != "FUI")                 //           |
                throw new InvalidDataException("Invalid FUI file"); //           |
            reader.Skip(4); // Part of signature                    // 0x04 + 4  |
                                                                    //           |
            int dataSize = reader.ReadInt();                        // 0x08 + 4  |
            fui.FileName = reader.ReadAsciiStop(0x40);              // 0x48 + 64 |
                                                                    //           |
            int timelineCount = reader.ReadInt();                   // 0x4C + 4  |
            int timelineEventNameCount = reader.ReadInt();          // 0x50 + 4  |
            int timelineActionCount = reader.ReadInt();             // 0x54 + 4  |
            int shapeCount = reader.ReadInt();                      // 0x58 + 4  |
            int shapeComponentCount = reader.ReadInt();             // 0x5C + 4  |
            int vertCount = reader.ReadInt();                       // 0x60 + 4  |
            int timelineFrameCount = reader.ReadInt();              // 0x64 + 4  |
            int timelineEventCount = reader.ReadInt();              // 0x68 + 4  |
            int referenceCount = reader.ReadInt();                  // 0x6C + 4  |
            int editTextCount = reader.ReadInt();                   // 0x70 + 4  |
            int symbolCount = reader.ReadInt();                     // 0x74 + 4  |
            int bitmapCount = reader.ReadInt();                     // 0x78 + 4  |
            int imageDataSize = reader.ReadInt();                   // 0x7C + 4  |
            int fontNameCount = reader.ReadInt();                   // 0x80 + 4  |
            int importAssetCount = reader.ReadInt();                // 0x84 + 4  |
            fui.FrameSize = new FUIRect(reader);                    // 0x8C + 16 = 0x9C

            fui.Timelines = new FUITimeline[timelineCount];
            for (int i = 0; i < timelineCount; i++)
                fui.Timelines[i] = new FUITimeline(reader);

            fui.TimelineActions = new FUITimelineAction[timelineActionCount];
            for (int i = 0; i < timelineActionCount; i++)
                fui.TimelineActions[i] = new FUITimelineAction(reader);

            fui.Shapes = new FUIShape[shapeCount];
            for (int i = 0; i < shapeCount; i++)
                fui.Shapes[i] = new FUIShape(reader);

            fui.ShapeComponents = new FUIShapeComponent[shapeComponentCount];
            for (int i = 0; i < shapeComponentCount; i++)
                fui.ShapeComponents[i] = new FUIShapeComponent(reader);

            fui.Verts = new FUIVert[vertCount];
            for (int i = 0; i < vertCount; i++)
                fui.Verts[i] = new FUIVert(reader);

            fui.TimelineFrames = new FUITimelineFrame[timelineFrameCount];
            for (int i = 0; i < timelineFrameCount; i++)
                fui.TimelineFrames[i] = new FUITimelineFrame(reader);

            fui.TimelineEvents = new FUITimelineEvent[timelineEventCount];
            for (int i = 0; i < timelineEventCount; i++)
                fui.TimelineEvents[i] = new FUITimelineEvent(reader);

            fui.TimelineEventNames = new string[timelineEventNameCount];
            for (int i = 0; i < timelineEventNameCount; i++)
                fui.TimelineEventNames[i] = reader.ReadAsciiStop(0x40);

            fui.References = new FUIReference[referenceCount];
            for (int i = 0; i < referenceCount; i++)
                fui.References[i] = new FUIReference(reader);

            fui.EditTexts = new FUIEditText[editTextCount];
            for (int i = 0; i < editTextCount; i++)
                fui.EditTexts[i] = new FUIEditText(reader);

            fui.FontNames = new FUIFontName[fontNameCount];
            for (int i = 0; i < fontNameCount; i++)
                fui.FontNames[i] = new FUIFontName(reader);

            fui.Symbols = new FUISymbol[symbolCount];
            for (int i = 0; i < symbolCount; i++)
                fui.Symbols[i] = new FUISymbol(reader);

            fui.ImportAssets = new string[importAssetCount];
            for (int i = 0; i < importAssetCount; i++)
                fui.ImportAssets[i] = reader.ReadAsciiStop(0x40);

            fui.Bitmaps = new FUIBitmap[bitmapCount];
            for (int i = 0; i < bitmapCount; i++)
                fui.Bitmaps[i] = new FUIBitmap(reader);

            long bitmapBaseOffset = reader.Tell();

            foreach (FUIBitmap fuiBitmap in fui.Bitmaps)
                fuiBitmap.LoadData(reader, bitmapBaseOffset);

            return fui;
        }
    }
}
