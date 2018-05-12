using F23.StringSimilarity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyString;

namespace StringMatch
{ 
    // 11/08/2017
    // 1. split ApproximatelyEquals class into two methods
    // broadening the choice of results type to include list of metrics
    // 2. changed name to better reflect usage

    public static partial class StringComparison
    {


        public static List<double> GetQuality(string source, string target, params Algorithm[] options)
        {

            List<double> comparisonResults = new List<double>(options.Length);
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

                    comparisonResults.Add(source.HammingDistance(target) / target.Length);

                    //sw.Stop();
                }
                else
                    comparisonResults.Add(0.01);
                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.HammingDistance);
            }

        




      



            // Min: 0    Max: 1
            if (options.Contains(Algorithm.JaccardDistance))
            {

           //     sw.Restart();
                var j = new F23.StringSimilarity.Jaccard();
                comparisonResults.Add(j.Distance(source,target));
                //comparisonResults.Add(source.JaccardDistance(target));
              //  sw.Stop();
             //   System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.JaccardDistance);
            }


          




            //// Min: 0    Max: 1
            //if (options.Contains(Algorithm.JaroDistance))
            //{
            //    //sw.Restart();
            //    comparisonResults.Add(source.JaroDistance(target));
                
            //    //sw.Stop();
            //    //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.JaroDistance);
            //}
 
       




            // Min: 0    Max: 1
            if (options.Contains(Algorithm.JaroWinklerDistance))
            {
         //       sw.Restart();
                var jw = new JaroWinkler();
              
                // substitution of s and t
                comparisonResults.Add(jw.Similarity(source,target));
               // comparisonResults.Add(source.JaroWinklerDistance(target));
           //     sw.Stop();


               // System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.JaroWinklerDistance);


                //sw.Restart();

                //comparisonResults.Add(source.JaroWinklerDistance(target));
                //sw.Stop();

                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1} 2", sw.Elapsed, Algorithm.JaroWinklerDistance);



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
                comparisonResults.Add(l.Distance(source,target));
                //sw.Stop();
                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.NormalizedLevenshteinDistance);
            }
            //else if (options.Contains(Algorithm.LevenshteinDistance))
            //{
            //    comparisonResults.Add(Convert.ToDouble(source.LevenshteinDistance(target)) / Convert.ToDouble(source.LevenshteinDistanceUpperBounds(target)));
            //}


        










            if (options.Contains(Algorithm.MetricLongestCommonSubsequence))
            {
                //sw.Restart();
                var mlcs = new MetricLCS();
                comparisonResults.Add(1 - Convert.ToDouble((mlcs.Distance(source, target) / Convert.ToDouble(Math.Min(source.Length, target.Length)))));
                //sw.Stop();
                ////comparisonResults.Add(1 - Convert.ToDouble((source.LongestCommonSubsequence(target).Length) / Convert.ToDouble(Math.Min(source.Length, target.Length))));
                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.MetricLongestCommonSubsequence);

            }



            //if (options.Contains(Algorithm.LongestCommonSubsequence))
            //{
            //    //sw.Restart();
            //    var lcs = new LongestCommonSubsequence();
      
            //    comparisonResults.Add(1 - Convert.ToDouble((lcs.Distance(source,target) / Convert.ToDouble(Math.Min(source.Length, target.Length)))));
            //    //comparisonResults.Add(1 - Convert.ToDouble((source.LongestCommonSubsequence(target).Length) / Convert.ToDouble(Math.Min(source.Length, target.Length))));
            //    //sw.Stop();
            //    //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.LongestCommonSubsequence);
            //}

            
            if (options.Contains(Algorithm.LongestCommonSubstring))
            {
                //sw.Restart();
                comparisonResults.Add(1 - Convert.ToDouble((source.LongestCommonSubstring(target).Length) / Convert.ToDouble(Math.Min(source.Length, target.Length))));
                //sw.Stop();
                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.LongestCommonSubstring);
            }

      



            // Min: 0    Max: 1
            if (options.Contains(Algorithm.SorensenDiceDistance))
            {

                //sw.Restart();
                var sor = new SorensenDice();
          
                comparisonResults.Add(sor.Distance(source,target));
                //comparisonResults.Add(source.SorensenDiceDistance(target));
                //sw.Stop();
                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.SorensenDiceDistance);
            }


        



            // Min: 0    Max: 1
            if (options.Contains(Algorithm.OverlapCoefficient))
            {

                //sw.Restart();
                comparisonResults.Add(1 - source.OverlapCoefficient(target));
                //sw.Stop();
                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.OverlapCoefficient);
            }






            // Min: 0    Max: 1
            if (options.Contains(Algorithm.RatcliffObershelpSimilarity))
            {
                //sw.Restart();
                comparisonResults.Add(1 - source.RatcliffObershelpSimilarity(target));
                //sw.Stop();
                //System.Diagnostics.Debug.WriteLine("Elapsed={0} {1}", sw.Elapsed, Algorithm.RatcliffObershelpSimilarity);
            }


       



//,


//            MetricLongestCommonSubsequence,

//        NGram,

//        Shingle,

//            QGram,

//        CosineSimilarity,




            return comparisonResults;
        }
    }


    public enum Algorithm
    {
        HammingDistance,

        JaccardDistance,

        //JaroDistance,

        JaroWinklerDistance,

        //LevenshteinDistance,
        NormalizedLevenshteinDistance,

        //LongestCommonSubsequence,
        LongestCommonSubstring,




        OverlapCoefficient,

        RatcliffObershelpSimilarity,

        SorensenDiceDistance,

        //TanimotoCoefficient,


        CaseSensitive,

      

                    //DamerauLevenshtein,

        MetricLongestCommonSubsequence,

        //NGram,

        //Shingle,

        //    QGram,

        //CosineSimilarity,

    }
}
