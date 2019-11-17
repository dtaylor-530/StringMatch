using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyString.Model
{
    public enum Algorithm
    {
        HammingDistance,

        JaccardDistance,

        JaroDistance,

        JaroWinklerDistance,

       LevenshteinDistance,

        LongestCommonSubsequence,

        LongestCommonSubstring,

        NormalizedLevenshteinDistance,

        OverlapCoefficient,

        RatcliffObershelpSimilarity,

        SorensenDiceDistance,

        TanimotoCoefficient,

        CaseSensitive,

            MetricLongestCommonSubsequence
    }
}
