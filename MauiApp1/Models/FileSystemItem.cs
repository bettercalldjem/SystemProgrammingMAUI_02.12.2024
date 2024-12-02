namespace MauiApp1.Models
{
    public class FileSystemItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public string Size { get; set; }
        public string LastModified { get; set; }
    }
}
