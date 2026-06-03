namespace ReviewMonitoring.Application.Models;

public class ProductSearchTerms
{
    public required string Exact { get; set; }
    public List<string> Synonyms { get; set; } = [];
}
