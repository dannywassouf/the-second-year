using System;
using System.Collections.Generic;
using System.Linq;
namespace MachineLearning.DecisionTree
{
    public class DecisionTreeID3<T> where T : IEquatable<T>
    {
        T[,] Data;
        string[] Names;
        int Category;
        T[] CategoryLabels;
        DecisionTreeNode<T> Root;
        public DecisionTreeID3()
        {
        }
        public DecisionTreeID3(T[,] data, string[] names, T[] categoryLabels)
        {
            Data = data;
            Names = names;
            Category = data.GetLength(1) - 1;//Categorical variables need to be placed in the last column
            CategoryLabels = categoryLabels;
        }
        public void Learn()
        {
            int nRows = Data.GetLength(0);
            int nCols = Data.GetLength(1);
            int[] rows = new int[nRows];

            int[] cols = new int[nCols];
            for (int i = 0; i < nRows; i++) rows[i] = i;
            for (int i = 0; i < nCols; i++) cols[i] = i;
            Root = new DecisionTreeNode<T>(-1, default(T));
            Learn(rows, cols, Root);
            DisplayNode(Root);
        }
        public void DisplayNode(DecisionTreeNode<T> Node, int depth = 0)
        {
            if (Node.Label != -1)
                Console.WriteLine("{0} {1}: {2}", new string('-', depth * 3), Names[Node.Label], Node.Value);
            foreach (var item in Node.Children)
                DisplayNode(item, depth + 1);
        }
        private void Learn(int[] pnRows, int[] pnCols, DecisionTreeNode<T> Root)
        {
            var categoryValues = GetAttribute(Data, Category, pnRows);
            var categoryCount = categoryValues.Distinct().Count();
            if (categoryCount == 1)
            {
                var node = new DecisionTreeNode<T>(Category, categoryValues.First());
                Root.Children.Add(node);
            }
            else
            {
                if (pnRows.Length == 0) return;
                else if (pnCols.Length == 1)
                {
                    //Vote~
                    //Majority vote
                    var Vote = categoryValues.GroupBy(i => i).OrderBy(i => i.Count()).First();
                    var node = new DecisionTreeNode<T>(Category, Vote.First());
                    Root.Children.Add(node);
                }
                else
                {
                    var maxCol = MaxEntropy(pnRows, pnCols);
                    var attributes = GetAttribute(Data, maxCol, pnRows).Distinct();
                    string currentPrefix = Names[maxCol];
                    foreach (var attr in attributes)
                    {
                        int[] rows = pnRows.Where(irow => Data[irow, maxCol].Equals(attr)).ToArray();
                        int[] cols = pnCols.Where(i => i != maxCol).ToArray();
                        var node = new DecisionTreeNode<T>(maxCol, attr);
                        Root.Children.Add(node);
                        Learn(rows, cols, node);//Recursively generate decision trees
                    }
                }
            }
        }
        public double AttributeInfo(int attrCol, int[] pnRows)
        {
            var tuples = AttributeCount(attrCol, pnRows);
            var sum = (double)pnRows.Length;
            double Entropy = 0.0;
            foreach (var tuple in tuples)
            {
                int[] count = new int[CategoryLabels.Length];
                foreach (var irow in pnRows)
                    if (Data[irow, attrCol].Equals(tuple.Item1))
                    {
                        int index = Array.IndexOf(CategoryLabels, Data[irow, Category]);
                        count[index]++;//Currently only supports categorical variables in the last column
                    }
                double k = 0.0;
                for (int i = 0; i < count.Length; i++)
                {
                    double frequency = count[i] / (double)tuple.Item2;
                    double t = -frequency * Log2(frequency);
                    k += t;
                }
                double freq = tuple.Item2 / sum;
                Entropy += freq * k;
            }
            return Entropy;
        }
        public double CategoryInfo(int[] pnRows)
        {
            var tuples = AttributeCount(Category, pnRows);
            var sum = (double)pnRows.Length;
            double Entropy = 0.0;
            foreach (var tuple in tuples)
            {
                double frequency = tuple.Item2 / sum;
                double t = -frequency * Log2(frequency);
                Entropy += t;
            }
            return Entropy;
        }
        private static IEnumerable<T> GetAttribute(T[,] data, int col, int[] pnRows)
        {
            foreach (var irow in pnRows)
                yield return data[irow, col];
        }
        private static double Log2(double x)
        {
            return x == 0.0 ? 0.0 : Math.Log(x, 2.0);
        }
        public int MaxEntropy(int[] pnRows, int[] pnCols)
        {
            double cateEntropy = CategoryInfo(pnRows);
            int maxAttr = 0;
            double max = double.MinValue;
            foreach (var icol in pnCols)
                if (icol != Category)
                {
                    double Gain = cateEntropy - AttributeInfo(icol, pnRows);
                    if (max < Gain)
                    {
                        max = Gain;
                        maxAttr = icol;
                    }
                }
            return maxAttr;
        }
        public IEnumerable<Tuple<T, int>> AttributeCount(int col, int[] pnRows)
        {
            var tuples = from n in GetAttribute(Data, col, pnRows)
                         group n by n into i
                         select Tuple.Create(i.First(), i.Count());
            return tuples;
        }
    }








    public sealed class DecisionTreeNode<T>
    {
        public int Label { get; set; }
        public T Value { get; set; }
        public List<DecisionTreeNode<T>> Children { get; set; }
        public DecisionTreeNode(int label, T value)
        {
            Label = label;
            Value = value;
            Children = new List<DecisionTreeNode<T>>();
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            var da = new string[,]
            {
                {"sunny","hot","high","false","no"},
                {"sunny","hot","high","true","no"},
                {"overcast","hot","high","false","yes"},
                {"rain","mild","high","false","yes"},
                {"rain","cool","normal","false","yes"},
                {"rain","cool","normal","true","no"},
                {"overcast","cool","normal","true","yes"},
                {"sunny","mild","high","false","no"},
                {"sunny","cool","normal","false","yes"},
                {"rain","mild","normal","false","yes"},
                {"sunny","mild","normal","true","yes"},
                {"overcast","mild","high","true","yes"},
                {"overcast","hot","normal","false","yes"},
                {"rain","mild","high","true","no"}
            };
            var names = new string[] { "outlook", "tempreture", "humidity", "windy", "play" };
            var tree = new DecisionTreeID3<string>(da, names, new string[] { "yes", "no" });
            tree.Learn();
            Console.ReadKey();
        }
    }
}