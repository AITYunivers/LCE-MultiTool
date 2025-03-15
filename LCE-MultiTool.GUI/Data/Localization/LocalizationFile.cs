using LCE_MultiTool.GUI.Memory;
using System.Collections.Generic;

namespace LCE_MultiTool.GUI.Data.Localization
{
    public class LocalizationFile
    {
        public int Type;
        public int LanguageCount;
        public bool HasUids;
        public List<string> Languages = [];
        public Dictionary<string, Dictionary<string, string>> LanguageKeyValues = [];

        private LocalizationFile() {}

        public static LocalizationFile Load(ByteReader reader)
        {
            LocalizationFile loc = new LocalizationFile();
            reader.SetEndian(true);

            loc.Type = reader.ReadInt();
            loc.LanguageCount = reader.ReadInt();

            if (loc.Type == 2)
            {
                loc.HasUids = reader.ReadBool();
                reader.Skip(-1);
            }

            string[]? keys = loc.Type == 2 ? ReadKeys(reader) : null;
            int[] languageEntryBufferSizes = new int[loc.LanguageCount];
            for (int i = 0; i < loc.LanguageCount; i++)
            {
                string language = reader.ReadAutoUTF8_16b();
                languageEntryBufferSizes[i] = reader.ReadInt();
                loc.Languages.Add(language);
                loc.LanguageKeyValues.Add(language, []);
            }
            for (int i = 0; i < loc.LanguageCount; i++)
            {
                long end = reader.BaseStream.Position + languageEntryBufferSizes[i];
                if (0 < reader.ReadInt())
                    reader.Skip(1);
                string language = reader.ReadAutoUTF8_16b();
                if (!loc.Languages.Contains(language))
                    throw new KeyNotFoundException($"Language '{language}' not found in language list.");
                int count = reader.ReadInt();
                for (int ii = 0; ii < count; ii++)
                {
                    string key = loc.Type == 2 ? keys![ii] : reader.ReadAutoUTF8_16b();
                    string value = reader.ReadAutoUTF8_16b();
                    loc.LanguageKeyValues[language].Add(key, value);
                }
            }

            return loc;
        }

        private static string[] ReadKeys(ByteReader reader)
        {
            bool useUniqueIds = reader.ReadBool();
            int keyCount = reader.ReadInt();
            string[] keys = new string[keyCount];
            for (int i = 0; i < keyCount; i++)
                keys[i] = useUniqueIds ? reader.ReadInt().ToString("X08") : reader.ReadAutoUTF8_16b();
            return keys;
        }
    }
}
