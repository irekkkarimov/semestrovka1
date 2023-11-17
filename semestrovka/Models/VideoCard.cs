namespace semestrovka.Models;

public class VideoCard
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string PreviewUrl { get; set; }
    public string RedirectUrl { get; set; }
    public int UserId { get; set; }
}