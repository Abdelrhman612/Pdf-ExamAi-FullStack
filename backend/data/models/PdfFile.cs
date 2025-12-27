public class PdfFile
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = null!;
    public string StoredFilePath { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
