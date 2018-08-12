using System.Collections.Generic;

namespace Yahtzee.Core
{
    internal abstract class Scorer
    {
        internal abstract int CalculateCategoryScore(Category category, int[] rollResult);
        internal abstract int? CalculateBonusScore(Dictionary<Category, int?> gameStatus);
        internal abstract int? CalculatePartialScore(Dictionary<Category, int?> gameStatus);
        internal abstract int? CalculateTotalScore(Dictionary<Category, int?> gameStatus);
    }
}
