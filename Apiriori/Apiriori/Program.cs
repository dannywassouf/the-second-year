using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Tracing;
using Cotur.DataMining.Association;
using Cotur.DataMining.Association;
using System.Diagnostics.Tracing;
namespace Apiriori
{
    class Program
    {
        static void Main(string[] args)
        {
            Apriori ap1 = new Apriori();

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
            };
            Apriori myApiriori=new Apriori(new DataFields(5,Transactions,fieldNames));
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
                  //  Console.WriteLine(node.ToString(Ap.Data));
                }
            }
        }

    }
}
