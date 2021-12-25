using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Tracing;
using Cotur.DataMining.Association;
//using System.Diagnostics.Tracing;
namespace Apiriori
{
    class Program
    {
        static void Main(string[] args) 
        {
            //Apriori aa = new Apriori();
            Apriori ap1 = ExampleOne();
            WriteApriori(ap1,.22f);
           
        }

        static  Apriori ExampleOne()
        {
            List<string> fieldNames=new List<string>()
            {
                "I1","I2","I3","I4","I5"
            };

            List<List<int>> Transactions=new List<List<int>>(){

             new List<int>(){0,1,4},
             new List<int>(){1,3},
             new List<int>(){1,2},
             new List<int>(){0,1,3},
             new List<int>(){0,2},
             new List<int>(){1,2},
             new List<int>(){0,2},
             new List<int>(){0,1,2,4},
             new List<int>(){0,1,2},
             new List<int>(){4,0},
             new List<int>(){5,1}

            };
            DataFields d = new DataFields(5, Transactions, fieldNames);

            Apriori myApiriori=new Apriori(d);
            return myApiriori;
        }



        static void WriteApriori(Apriori Ap,float minsupport)
        {
            Ap.CalculateCNodes(minsupport);
            
         
            int table = 1;
            foreach(var Levels in Ap.EachLevelOfNodes)
            {
                Console.WriteLine("\n-- Table{0} --", table++);
                foreach (var node in Levels)
                {
                    Console.WriteLine(Ap.Data);
                }
            }



            Console.WriteLine("\n-- Rules --\n");
            foreach(var rules in Ap.Rules.Where(rule=>rule.Confidence=> 0.7f)) //this for is to show all azsociation rules witch apply this condition
            {
                Console.Write(rules.ToString(Ap.Rules));
            }

        }

    }
}
