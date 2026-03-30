using MatriculasService.Domain.Exceptions;

namespace MatriculasService.Domain.ValueObjects;

public sealed class PeriodoLetivo : IEquatable<PeriodoLetivo>
{
    public int Ano { get; }
    public int Semestre { get; }

    public PeriodoLetivo(int ano, int semestre)
    {
        if (semestre is not 1 and not 2)
            throw new DomainException("Semestre deve ser 1 ou 2.");

        if (ano < 2000 || ano > 2100)
            throw new DomainException($"Ano letivo inválido: {ano}. Deve estar entre 2000 e 2100.");

        Ano = ano;
        Semestre = semestre;
    }

    public bool Equals(PeriodoLetivo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Ano == other.Ano && Semestre == other.Semestre;
    }

    public override bool Equals(object? obj) => Equals(obj as PeriodoLetivo);

    public override int GetHashCode() => HashCode.Combine(Ano, Semestre);

    public override string ToString() => $"{Ano}/{Semestre}";

    public static bool operator ==(PeriodoLetivo? left, PeriodoLetivo? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(PeriodoLetivo? left, PeriodoLetivo? right) =>
        !(left == right);
}
