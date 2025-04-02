using System.Globalization;
using CsvHelper;
using InteligenciaArtifical.Fuzzy.AvaliacaoFilmes.Models;
using InteligenciaArtifical.Fuzzy.AvaliacaoFilmes.Rules;

var xGeneros = new Dictionary<Genre, float>
{
    { Genre.Drama, 0.24f }
    , { Genre.Music, 0.2f }
    , { Genre.Family, 0.3f }
    , { Genre.Documentary, 0.4f }
    , { Genre.History, 0.45f }
    , { Genre.Adventure, 0.57f }
    , { Genre.Western, 0.5f }
    , { Genre.Action, 0.68f }
    , { Genre.Horror, 0.6f }
    , { Genre.Foreign, 0.65f }
    , { Genre.Crime, 0.72f }
    , { Genre.Fantasy, 0.75f }
    , { Genre.War, 0.7f }
    , { Genre.TV, 0.75f }
    , { Genre.Animation, 0.87f }
    , { Genre.Romance, 0.8f }
    , { Genre.Mystery, 0.84f }
    , { Genre.Comedy, 0.9f }
    , { Genre.Thriller, 0.95f }
    , { Genre.Science, 0.99f }
    , { Genre.Unknown, 0 }
};

var xGrupoGenero = new GrupoVariaveis(GrupoVariaveisTipo.Genre);
xGrupoGenero.Add(new VariavelFuzzy(VariavelFuzzyTipo.Pouco, 0, 0, 0.25f, 0.50f));
xGrupoGenero.Add(new VariavelFuzzy(VariavelFuzzyTipo.Mediano, 0.499f, 0.5199999f, 0.52f, 0.60f));
xGrupoGenero.Add(new VariavelFuzzy(VariavelFuzzyTipo.Bastante, 0.55f, 0.719999999f, 0.72f, 0.80f));
xGrupoGenero.Add(new VariavelFuzzy(VariavelFuzzyTipo.Muito, 0.75f, 0.999f, 0.1f, 1.0f));

var xGrupoTempoDeFilme = new GrupoVariaveis(GrupoVariaveisTipo.Runtime);
xGrupoTempoDeFilme.Add(new VariavelFuzzy(VariavelFuzzyTipo.Pouco, 0, 0, 60, 90));
xGrupoTempoDeFilme.Add(new VariavelFuzzy(VariavelFuzzyTipo.Mediano, 85, 119.9999f, 120, 150));
xGrupoTempoDeFilme.Add(new VariavelFuzzy(VariavelFuzzyTipo.Bastante, 140, 199.999f, 200, 240));
xGrupoTempoDeFilme.Add(new VariavelFuzzy(VariavelFuzzyTipo.Muito, 210, 359.9999f, 360, 360));

var xGrupoMediaVotos = new GrupoVariaveis(GrupoVariaveisTipo.VoteAverage);
xGrupoMediaVotos.Add(new VariavelFuzzy(VariavelFuzzyTipo.Pouco, 0, 0, 2, 4));
xGrupoMediaVotos.Add(new VariavelFuzzy(VariavelFuzzyTipo.Mediano, 3, 5.9999f, 6f, 6.55f));
xGrupoMediaVotos.Add(new VariavelFuzzy(VariavelFuzzyTipo.Bastante, 6.45f, 7.5999f, 7.6f, 7.8f));
xGrupoMediaVotos.Add(new VariavelFuzzy(VariavelFuzzyTipo.Muito, 7.7f, 9.99999f, 10, 10));

var xGrupoCusto = new GrupoVariaveis(GrupoVariaveisTipo.Budget);
xGrupoCusto.Add(new VariavelFuzzy(VariavelFuzzyTipo.Pouco, 0, 0, 40_000_000, 50_000_000));
xGrupoCusto.Add(new VariavelFuzzy(VariavelFuzzyTipo.Mediano, 100_000_000, 199_999_999, 200_000_000, 250_000_000));
xGrupoCusto.Add(new VariavelFuzzy(VariavelFuzzyTipo.Bastante, 200_000_000, 299_999_999, 300_000_000, 350_000_000));
xGrupoCusto.Add(new VariavelFuzzy(VariavelFuzzyTipo.Muito, 300_000_000, 399_999_999, 400_000_000, 400_000_000));

var xGrupoQuantidadeVotos = new GrupoVariaveis(GrupoVariaveisTipo.VoteCount);
xGrupoQuantidadeVotos.Add(new VariavelFuzzy(VariavelFuzzyTipo.Pouco, 0, 0, 100, 200));
xGrupoQuantidadeVotos.Add(new VariavelFuzzy(VariavelFuzzyTipo.Mediano, 150, 299.9999f, 300, 400));
xGrupoQuantidadeVotos.Add(new VariavelFuzzy(VariavelFuzzyTipo.Bastante, 350, 499.9999f, 500, 600));
xGrupoQuantidadeVotos.Add(new VariavelFuzzy(VariavelFuzzyTipo.Muito, 550, 999.9999f, 1000, 1000));

try
{
    using var xReader = new StreamReader("movie_dataset.csv");
    using var xCsv = new CsvReader(xReader, CultureInfo.InvariantCulture);
    xCsv.Context.RegisterClassMap<MovieMap>();

    var xItens = xCsv.GetRecords<Movie>()
        .ToList()
        .Where(p => p.VoteAverage > 0)
        .Where(p => p.Budget > 0)
        .Where(p => p.Runtime > 0)
        .Where(p => p.VoteCount > 10)
        .ToList()
    ;

    foreach (var xItem in xItens)
    {
        var xVariaveis = new Dictionary<(GrupoVariaveisTipo pGrupoTipo, VariavelFuzzyTipo pVariavelTipo), float>();

        xGrupoGenero.Fuzzifica(xGeneros[xItem.Genre], xVariaveis);
        xGrupoTempoDeFilme.Fuzzifica(xItem.Runtime ?? 0, xVariaveis);
        xGrupoMediaVotos.Fuzzifica(xItem.VoteAverage ?? 0, xVariaveis);
        xGrupoCusto.Fuzzifica(xItem.Budget ?? 0, xVariaveis);
        xGrupoQuantidadeVotos.Fuzzifica(xItem.VoteCount ?? 0, xVariaveis);

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Pouco)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Pouco));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Pouco)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Pouco));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Mediano)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Bastante));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Bastante),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Mediano)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Muito));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Bastante)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Muito));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Bastante)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Mediano));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Pouco)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Pouco));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Bastante),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Bastante)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Bastante));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Bastante),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Mediano)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Muito));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Mediano)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Muito));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.Genre, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Runtime, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.VoteAverage, VariavelFuzzyTipo.Mediano),
            (GrupoVariaveisTipo.Budget, VariavelFuzzyTipo.Muito)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Muito));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.VoteCount, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Muito)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Muito));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.VoteCount, VariavelFuzzyTipo.Muito),
            (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Mediano)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Mediano));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.VoteCount, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Bastante)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Pouco));

        Regras.RodaRegraE(xVariaveis, [
            (GrupoVariaveisTipo.VoteCount, VariavelFuzzyTipo.Pouco),
            (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Muito)
        ], (GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Pouco));

        xVariaveis.TryGetValue((GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Pouco), out var xValorNaoGosto);
        xVariaveis.TryGetValue((GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Mediano), out var xValorMediano);
        xVariaveis.TryGetValue((GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Bastante), out var xValorGosto);
        xVariaveis.TryGetValue((GrupoVariaveisTipo.Score, VariavelFuzzyTipo.Muito), out var xValorGostoMuito);
        
        var xDivisor = (xValorNaoGosto + xValorMediano + xValorGosto + xValorGostoMuito);
        var xResultado = (xValorNaoGosto * 4
                          + xValorMediano * 6
                          + xValorGosto * 8
                          + xValorGostoMuito * 10)
            / xDivisor == 0
                ? 1
                : xDivisor
            ;

        xItem.Score = (float)(xResultado + (0.05 * xItem.VoteAverage ?? 0)) * 10;
    }

    var xTopFilmes = xItens.OrderByDescending(p => p.Score);
    foreach (var xItem in xTopFilmes.Select((pFilme, pIndice) => (pFilme, pIndice)))
    {
        Console.WriteLine($"{xItem.pIndice + 1}° Lugar: {xItem.pFilme.OriginalTitle}"
          + $"; Score: {xItem.pFilme.Score}"
          + $"; Genre: {xItem.pFilme.Genre}"
          + $"; Budget: {xItem.pFilme.Budget}"
          + $"; VoteAverage: {xItem.pFilme.VoteAverage}"
          + $"; VoteCount: {xItem.pFilme.VoteCount}" 
          + $"; Runtime: {xItem.pFilme.Runtime}")
        ;
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
