using LCE_MultiTool.GUI.Memory;
using System;
using System.IO;

namespace LCE_MultiTool.Data.SoundBank
{
    internal class SoundBankFile
    {
        public SoundBankEntry[] Entries = [];

        private SoundBankFile() { }

        public static SoundBankFile Load(ByteReader reader)
        {
            SoundBankFile msscmp = new SoundBankFile();
            string header = reader.ReadAscii(4);
            reader.SetEndian(header == "BANK");

            if (header != "BANK" && header != "KNAB")
                throw new InvalidDataException("Invalid Sound Bank header");

            bool is64Bit = reader.Seek(0x10).ReadLong() == 0;
            Func<long> ReadBitSpecificLong = new Func<long>(() => is64Bit ? reader.ReadLong() : reader.ReadInt());

            reader.Seek(8);
            long fileDataOffset = ReadBitSpecificLong();
            reader.Skip(8);
            long index1Offset = ReadBitSpecificLong();
            long index2Offset = ReadBitSpecificLong();
            reader.Skip(is64Bit ? 0x18 : 0xC);
            long index1EntryCount = ReadBitSpecificLong();
            reader.Skip(is64Bit ? 0x4 : 0x8);
            uint index2EntryCount = reader.ReadUInt();

            // Useless
            /*reader.Seek(index1Offset);
            uint[] index1Offets = new uint[index1EntryCount];
            for (int i = 0; i < index1Offets.Length; i++)
            {
                index1Offets[i] = reader.ReadUInt();
                reader.Skip(4);
            }*/

            reader.Seek(index2Offset);
            uint[] index2PathOffsets = new uint[index2EntryCount];
            uint[] index2InfoOffsets = new uint[index2EntryCount];
            for (int i = 0; i < index2EntryCount; i++)
            {
                index2PathOffsets[i] = reader.ReadUInt();
                index2InfoOffsets[i] = reader.ReadUInt();
            }

            msscmp.Entries = new SoundBankEntry[index2EntryCount];
            for (int i = 0; i < index2EntryCount; i++)
            {
                SoundBankEntry entry = new SoundBankEntry();
                
                reader.Seek(index2PathOffsets[i]);
                entry.FilePath = reader.ReadAscii();

                reader.Seek(index2InfoOffsets[i]);
                reader.Skip(4); // Path Offset
                reader.Skip(4); // Unknown
                uint dataOffset = reader.ReadUInt();
                reader.Skip(12); // Unknown
                int dataSize = reader.ReadInt();

                reader.Seek(dataOffset);
                entry.Data = reader.ReadBytes(dataSize);

                msscmp.Entries[i] = entry;
            }

            return msscmp;
        }
    }
}
