namespace CodeBase.Domain.Models
{
    public class FileData
    {
        public string AbsolutePath { get; set; }
        public long Length { get; set; }
        public string Extension { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
