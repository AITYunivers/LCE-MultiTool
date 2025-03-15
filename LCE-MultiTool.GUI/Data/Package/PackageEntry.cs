namespace LCE_MultiTool.Data.Package
{
    public class PackageEntry
    {
        public string Name { get; set; } = string.Empty;
        public EPackageType Type { get; set; }
        public int DataSize { get; set; }
        public (string, string)[] Properties { get; set; } = [];
        public byte[] Data { get; set; } = [];
    }
}
