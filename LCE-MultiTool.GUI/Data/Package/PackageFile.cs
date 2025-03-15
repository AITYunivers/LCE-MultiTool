using LCE_MultiTool.GUI.Memory;
using System;
using System.Collections.Generic;

namespace LCE_MultiTool.Data.Package
{
    public class PackageFile
    {
        public int Type;
        public PackageEntry[] Entries = [];

        private PackageFile() { }

        public static PackageFile Load(ByteReader reader)
        {
            PackageFile pck = new PackageFile();

            pck.Type = reader.ReadInt();
            switch (pck.Type)
            {
                case 1:
                case 3:
                    {
                        // Lookup Table
                        int lookupCount = reader.ReadInt();
                        Dictionary<int, string> lookupTable = [];
                        for (int i = 0; i < lookupCount; i++)
                        {
                            lookupTable.Add(reader.ReadInt(), reader.ReadAutoYunicode_32b());
                            reader.Skip(4); // Padding
                        }
                        if (lookupTable.ContainsValue("XMLVERSION"))
                            reader.Skip(4); // XML Version

                        // Asset Entries
                        int assetCount = reader.ReadInt();
                        pck.Entries = new PackageEntry[assetCount];
                        for (int i = 0; i < assetCount; i++)
                        {
                            pck.Entries[i] = new PackageEntry()
                            {
                                DataSize = reader.ReadInt(),
                                Type = (EPackageType)reader.ReadInt(),
                                Name = reader.ReadAutoYunicode_32b()
                            };
                            reader.Skip(4); // Padding
                        }

                        // Asset Contents
                        for (int i = 0; i < assetCount; i++)
                        {
                            PackageEntry entry = pck.Entries[i];
                            int propertyCount = reader.ReadInt();
                            entry.Properties = new (string, string)[propertyCount];
                            for (int ii = 0; ii < propertyCount; ii++)
                            {
                                entry.Properties[ii] = (lookupTable[reader.ReadInt()], reader.ReadAutoYunicode_32b());
                                reader.Skip(4); // Padding
                            }
                            entry.Data = reader.ReadBytes(entry.DataSize);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException("Package Type " + pck.Type + " Not Implemented");
            }

            return pck;
        }
    }
}
