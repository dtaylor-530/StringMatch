using F23.StringSimilarity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyString;
using DuoVia.FuzzyStrings;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace StringMatch
{ 
    // 11/08/2017
    // 1. split ApproximatelyEquals class into two methods
    // broadening the choice of results type to include list of metrics
    // 2. changed name to better reflect usage

    public static partial class StringComparison
    {


        public static Dictionary<string,double> GetQuality(string source, string target, params Algorithm[] options)
        {
            Dictionary<string,double> comparisonResults = new Dictionary<string, double>();
            //Stopwatch sw = new Stopwatch();

            if (!options.Contains(Algorithm.CaseSensitive))
            {
                source = source.Capitalize();
                target = target.Capitalize();
            }

            // Min: 0    Max: source.Length = target.Length
            if (options.Contains(Algorithm.HammingDistance))
            {
                if (source.Length == target.Length)
                {
                    //sw.Restart();
                    comparisonResults[nameof(Algorithm.HammingDistance)]=(source.HammingDistance(target) / target.Length);
                    //sw.Stop();
                }
                else
                    comparisonResults[nameof(Algorithm.HammingDistance)] = (0.01);
                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.HammingDistance);
            }


            if (options.Contains(Algorithm.FuzzyString))
            {
                comparisonResults[nameof(Algorithm.FuzzyString)] = source.FuzzyMatch(target);
            }

            if (options.Contains(Algorithm.FuzzySharp))
            {
                //var ratio = ScorerCache.Get<DefaultRatioScorer>();
                //var partialRatio = ScorerCache.Get<PartialRatioScorer>();
                //var tokenSet = ScorerCache.Get<TokenSetScorer>();
                //var partialTokenSet = ScorerCache.Get<PartialTokenSetScorer>();
                //var tokenSort = ScorerCache.Get<TokenSortScorer>();
                //var partialTokenSort = ScorerCache.Get<PartialTokenSortScorer>();
                //var tokenAbbreviation = ScorerCache.Get<TokenAbbreviationScorer>();
                //var partialTokenAbbreviation = ScorerCache.Get<PartialTokenAbbreviationScorer>();
                //var weighted = ScorerCache.Get<WeightedRatioScorer>();
                //ratio.Score(source, target);
                comparisonResults[nameof(Algorithm.FuzzySharp)] = Fuzz.Ratio(source,target);
            }

            // Min: 0    Max: 1
            if (options.Contains(Algorithm.JaccardDistance))
            {
                var j = new Jaccard();
                comparisonResults[nameof(Algorithm.JaccardDistance)] = (j.Distance(source,target));
            }

            if (options.Contains(Algorithm.JaroWinklerDistance))
            {
                var jw = new JaroWinkler();       
                comparisonResults[nameof(Algorithm.JaroWinklerDistance)] = (jw.Similarity(source,target));
            }

            // Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
            // Min: LevenshteinDistanceLowerBounds    Max: LevenshteinDistanceUpperBounds
            //if (options.Contains(Algorithm.NormalizedLevenshteinDistance))
            //{
            //    comparisonResults.Add(Convert.ToDouble(source.NormalizedLevenshteinDistance(target)) / Convert.ToDouble((Math.Max(source.Length, target.Length) - source.LevenshteinDistanceLowerBounds(target))));
            //}
            //else if (options.Contains(Algorithm.LevenshteinDistance))
            //{
            //    comparisonResults.Add(Convert.ToDouble(source.LevenshteinDistance(target)) / Convert.ToDouble(source.LevenshteinDistanceUpperBounds(target)));
            //}
            //sw.Stop();

            //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.NormalizedLevenshteinDistance);

            //Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
            // Min: LevenshteinDistanceLowerBounds Max: LevenshteinDistanceUpperBounds
            if (options.Contains(Algorithm.NormalizedLevenshteinDistance))
            {
                //sw.Restart();
                var l = new NormalizedLevenshtein();
                //comparisonResults.Add(Convert.ToDouble(source.NormalizedLevenshteinDistance(target)) / Convert.ToDouble((Math.Max(source.Length, target.Length) - source.LevenshteinDistanceLowerBounds(target))));
                comparisonResults[nameof(Algorithm.NormalizedLevenshteinDistance)] = (l.Distance(source,target));
                //sw.Stop();
                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.NormalizedLevenshteinDistance);
            }
            //else if (options.Contains(Algorithm.LevenshteinDistance))
            //{
            //    comparisonResults.Add(Convert.ToDouble(source.LevenshteinDistance(target)) / Convert.ToDouble(source.LevenshteinDistanceUpperBounds(target)));
            //}

            if (options.Contains(Algorithm.MetricLongestCommonSubsequence))
            {
                var mlcs = new MetricLCS();
                comparisonResults[nameof(Algorithm.MetricLongestCommonSubsequence)] = (1 - Convert.ToDouble((mlcs.Distance(source, target) / Convert.ToDouble(Math.Min(source.Length, target.Length)))));
            }
            
            if (options.Contains(Algorithm.LongestCommonSubstring))
            {
                comparisonResults[nameof(Algorithm.LongestCommonSubstring)] = (1 - Convert.ToDouble((source.LongestCommonSubstring(target).Length) / Convert.ToDouble(Math.Min(source.Length, target.Length))));
            }

            // Min: 0    Max: 1
            if (options.Contains(Algorithm.SorensenDiceDistance))
            {
                var sor = new SorensenDice();
                comparisonResults[nameof(Algorithm.SorensenDiceDistance)] = (sor.Distance(source,target));
            }

            // Min: 0    Max: 1
            if (options.Contains(Algorithm.OverlapCoefficient))
            {
                comparisonResults[nameof(Algorithm.OverlapCoefficient)] = (1 - source.OverlapCoefficient(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(Algorithm.RatcliffObershelpSimilarity))
            {
                comparisonResults[nameof(Algorithm.RatcliffObershelpSimilarity)] = (1 - source.RatcliffObershelpSimilarity(target));
            }

            return comparisonResults;
        }
    }


}
