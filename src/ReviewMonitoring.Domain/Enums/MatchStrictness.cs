
namespace ReviewMonitoring.Domain.Enums;
public enum MatchStrictness
{
    Loose,    // похожие товары (другие цвета, объёмы)
    Medium,   // тот же товар, разные продавцы
    Strict    // точное совпадение
}