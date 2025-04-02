namespace InteligenciaArtifical.Fuzzy.AvaliacaoFilmes.Models;

public class GrupoVariaveis(GrupoVariaveisTipo pTipo)
{
    public readonly List<VariavelFuzzy> ListaDeVariaveis = [];
    public readonly GrupoVariaveisTipo Tipo = pTipo;

    public void Add(VariavelFuzzy variavel)
    {
        ListaDeVariaveis.Add(variavel);
    }

    public void Fuzzifica(float pValor, Dictionary<(GrupoVariaveisTipo pGrupoTipo, VariavelFuzzyTipo pVariavelTipo), float> pVariaveis)
    {
        foreach (var variavel in ListaDeVariaveis)
        {
            var resultado = variavel.Fuzzifica(pValor);
            pVariaveis[(Tipo, variavel.Tipo)] = resultado;
        }
    }
}