namespace ReviewMonitoring.AI.Models;

internal class AnalysisDto
{
    public List<string> Pros { get; set; } = [];
    public List<string> Cons { get; set; } = [];
    public List<string> Keywords { get; set; } = [];
    public List<string> Flags { get; set; } = [];
    public string Summary { get; set; } = string.Empty;
}