using LCE_MultiTool.GUI.Memory;

namespace LCE_MultiTool.Data.Archive
{
    public class ArchiveFile
    {
        public ArchiveEntry[] Entries = [];

        private ArchiveFile() { }

        public static ArchiveFile Load(ByteReader reader)
        {
            ArchiveFile arc = new ArchiveFile();
            reader.SetEndian(true);

            uint fileCount = reader.ReadUInt();
            arc.Entries = new ArchiveEntry[fileCount];
            for (uint i = 0; i < fileCount; i++)
            {
                string name = reader.ReadAutoUTF8_16b();
                uint offset = reader.ReadUInt();
                uint size = reader.ReadUInt();

                long ret = reader.Tell();
                reader.Seek(offset);
                byte[] data = reader.ReadBytes((int)size);
                reader.Seek(ret);

                arc.Entries[i] = new ArchiveEntry
                {
                    Name = name,
                    Data = data
                };
            }

            return arc;
        }
    }
}
