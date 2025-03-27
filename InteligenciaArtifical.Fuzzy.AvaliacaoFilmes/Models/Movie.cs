using CsvHelper.Configuration.Attributes;

namespace InteligenciaArtifical.Fuzzy.AvaliacaoFilmes.Models;

public class Movie
{
    [Name("index")]
    public int Index { get; set; }
    [Name("original_title")]
    public required string OriginalTitle { get; set; }
    [Name("genres")]
    public float Genre { get; set; }
    [Name("runtime")]
    public float Runtime { get; set; }
    [Name("vote_average")]
    public float VoteAverage { get; set; }
    [Name("budget")]
    public float Budget { get; set; }
}