namespace InteligenciaArtifical.Fuzzy.AvaliacaoFilmes.Models;

public class VariavelFuzzy
{
    public VariavelFuzzyTipo Tipo { get; set; }
    public float B1 { get; set; }
    public float T1 { get; set; }
    public float T2 { get; set; }
    public float B2 { get; set; }

    public VariavelFuzzy(VariavelFuzzyTipo tipo, float b1, float t1, float t2, float b2)
    {
        Tipo = tipo;
        B1 = b1;
        T1 = t1;
        T2 = t2;
        B2 = b2;
    }

    public float Fuzzifica(float valor)
    {
        if (valor < B1 || valor > B2)
        {
            return 0.0f;
        }
        if (valor >= T1 && valor <= T2)
        {
            return 1.0f;
        }
        if (valor > B1 && valor < T1)
        {
            return (valor - B1) / (T1 - B1);
        }
        if (valor > T2 && valor < B2)
        {
            return 1.0f - (valor - T2) / (B2 - T2);
        }
        return 0.0f;
    }
}