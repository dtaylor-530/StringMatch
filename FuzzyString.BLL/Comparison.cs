using FuzzyString;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMatch
{

    public static partial class Comparison
    {
        public static bool IsMatch(this string source, string target, ComparisonTolerance tolerance, params Algorithm[] options)
        {

            List<double> comparisonResults = GetQualityMetrics(source, target, options);


            if (comparisonResults.Count == 0)
            {
                return false;
            }

            if (tolerance == ComparisonTolerance.Strong)
            {
                if (comparisonResults.Average() < 0.25)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tolerance == ComparisonTolerance.Normal)
            {
                if (comparisonResults.Average() < 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tolerance == ComparisonTolerance.Weak)
            {
                if (comparisonResults.Average() < 0.75)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tolerance == ComparisonTolerance.Manual)
            {
                if (comparisonResults.Average() > 0.6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }


        public static List<double> GetQualityMetrics(string source, string target,params Algorithm[] options)
        {

            List<double> comparisonResults = new List<double>();
            Stopwatch sw = new Stopwatch();

            sw.Start();

            if (!options.Contains(Algorithm.CaseSensitive))
            {
                source = source.Capitalize();
                target = target.Capitalize();
            }


            sw.Stop();

            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.CaseSensitive);





            sw.Start();
            // Min: 0    Max: source.Length = target.Length
            if (options.Contains(Algorithm.HammingDistance))
            {
                if (source.Length == target.Length)
                {
                    comparisonResults.Add(source.HammingDistance(target) / target.Length);
                }
            }
            sw.Stop();

            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.HammingDistance);



            sw.Start();
            // Min: 0    Max: 1
            if (options.Contains(Algorithm.JaccardDistance))
            {
                comparisonResults.Add(source.JaccardDistance(target));
            }
            sw.Stop();

            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.JaccardDistance);



            sw.Start();
            // Min: 0    Max: 1
            if (options.Contains(Algorithm.JaroDistance))
            {
                comparisonResults.Add(source.JaroDistance(target));
            }
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.JaroDistance);



            sw.Restart();
            // Min: 0    Max: 1
            if (options.Contains(Algorithm.JaroWinklerDistance))
            {
                comparisonResults.Add(source.JaroWinklerDistance(target));
            }
            sw.Stop();

            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.JaroWinklerDistance);


            sw.Restart();
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



            sw.Restart();
            if (options.Contains(Algorithm.LongestCommonSubsequence))
            {
                comparisonResults.Add(1 - Convert.ToDouble((source.LongestCommonSubsequence(target).Length) / Convert.ToDouble(Math.Min(source.Length, target.Length))));
            }
            sw.Stop();


            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.LongestCommonSubsequence);



            sw.Restart();
            if (options.Contains(Algorithm.LongestCommonSubstring))
            {
                comparisonResults.Add(1 - Convert.ToDouble((source.LongestCommonSubstring(target).Length) / Convert.ToDouble(Math.Min(source.Length, target.Length))));
            }
            sw.Stop();

            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.LongestCommonSubstring);



            sw.Restart();
            // Min: 0    Max: 1
            if (options.Contains(Algorithm.SorensenDiceDistance))
            {
                comparisonResults.Add(source.SorensenDiceDistance(target));
            }
            sw.Stop();

            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.SorensenDiceDistance);



            sw.Restart();
            // Min: 0    Max: 1
            if (options.Contains(Algorithm.OverlapCoefficient))
            {
                comparisonResults.Add(1 - source.OverlapCoefficient(target));
            }
           sw.Stop(); 

            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.OverlapCoefficient);


            sw.Restart();
            // Min: 0    Max: 1
            if (options.Contains(Algorithm.RatcliffObershelpSimilarity))
            {
                comparisonResults.Add(1 - source.RatcliffObershelpSimilarity(target));
            }
            sw.Stop();

            System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.RatcliffObershelpSimilarity);
            return comparisonResults;
        }
    }
}
