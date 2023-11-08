namespace semestrovka.Models;

public class Mission
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = "";
    public string Continent { get; set; } = "";
    public string Country { get; set; } = "";
    public string City { get; set; } = "";
    public string Description { get; set; } = "";
    public string Url { get; set; } = "";
}