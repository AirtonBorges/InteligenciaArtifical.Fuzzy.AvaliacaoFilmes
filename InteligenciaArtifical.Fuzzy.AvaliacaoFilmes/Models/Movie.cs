using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace InteligenciaArtifical.Fuzzy.AvaliacaoFilmes.Models;

public class Movie
{
    [Name("index")]
    public int Index { get; set; }
    [Name("original_title")]
    public required string OriginalTitle { get; set; }
    [Name("genres")]
    public Genre Genre { get; set; }
    [Name("runtime")]
    public float? Runtime { get; set; }
    [Name("vote_average")]
    public float? VoteAverage { get; set; }
    [Name("budget")]
    public float? Budget { get; set; }
    [Name("vote_count")]
    public float? VoteCount { get; set; }
    
    public float? Score { get; set; }
}

public sealed class MovieMap: ClassMap<Movie>
{
    public MovieMap()
    {
        Map(p => p.Genre).Convert(p =>
        {
            var field = p.Row.GetField("genres");
            
            if (string.IsNullOrWhiteSpace(field))
            {
                return Genre.Unknown;
            }

            var genre = Enum.Parse<Genre>(
                field!.Split(' ').First().Trim()
            );
            
            return genre;
        });
        Map(p => p.Index).Name("index");
        Map(p => p.OriginalTitle).Name("original_title");
        Map(p => p.Runtime).Name("runtime");
        Map(p => p.VoteAverage).Name("vote_average");
        Map(p => p.Budget).Name("budget");
        Map(p => p.VoteCount).Name("vote_count");
    }
}