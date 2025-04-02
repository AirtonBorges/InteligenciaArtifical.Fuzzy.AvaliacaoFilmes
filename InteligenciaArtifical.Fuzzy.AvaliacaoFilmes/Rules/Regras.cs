using InteligenciaArtifical.Fuzzy.AvaliacaoFilmes.Models;

namespace InteligenciaArtifical.Fuzzy.AvaliacaoFilmes.Rules;

public static class Regras
{
    public static void RodaRegraE(
        Dictionary<(GrupoVariaveisTipo pGrupoTipo, VariavelFuzzyTipo pVariavelTipo), float> pVariaveis
        , (GrupoVariaveisTipo pGrupoTipo, VariavelFuzzyTipo pVariavelTipo)[] pAntecedente
        , (GrupoVariaveisTipo pGrupoTipo, VariavelFuzzyTipo variavelFuzzyTipo) pPrecedente
    )
    {
        var xResultado = pAntecedente.Select(p => pVariaveis[p]).Min();

        if (pVariaveis.ContainsKey(pPrecedente))
        {
            pVariaveis[pPrecedente] = float.Max(pVariaveis[pPrecedente], xResultado);
        }
        else
        {
            pVariaveis.Add(pPrecedente, xResultado);
        }
    }
}