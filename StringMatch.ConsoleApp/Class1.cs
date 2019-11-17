using FuzzyString;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMatch.ConsoleApp
{
    class Program
    {
        // 11/08/2017 
        // Enabled user to select their own combination of words
        // Refactored
        // removed FuzzyString prefix from enums

        static void Main(string[] args)
        {

            Sample();

            string input = "Y";

            do
            {
                Console.WriteLine("Try with another pair of words? Y");

                input = Console.ReadLine();


                Console.WriteLine("Name1:");
                string name1 = Console.ReadLine();
                Console.WriteLine("Name2:");
                string name2 = Console.ReadLine();
                Sample(name1, name2);


            } while (input.ToLower() == "y");

        }


        static void Sample(string name1 = "kevin", string name2 = "kevjyn")
        {

            List<Algorithm> options = new List<Algorithm>();
            options.Add(Algorithm.JaccardDistance);
            options.Add(Algorithm.CaseSensitive);

            string line = "{0} Match: {1} with {2} is {3}";

            foreach (ComparisonTolerance tolerance in Enum.GetValues(typeof(ComparisonTolerance)))
            {
                if (tolerance != ComparisonTolerance.Manual)
                {
                    bool isMatch = name1.IsMatch(name2, tolerance, options.ToArray());
                    Console.WriteLine(string.Format(line, tolerance.ToString(), name1, name2, isMatch.ToString()));

                }
            }




        }

    }
}