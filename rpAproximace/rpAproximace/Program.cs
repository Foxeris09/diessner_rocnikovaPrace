using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpAproximace
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Node> vrcholy = new List<Node>(); //jsou zde uložené všechny vrcholy a jejich indexy v tomto listu odpovídají číslu vrchol

            Console.WriteLine("Kolik vrcholů má graf?");
            int pocerVrcholu = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Teď na dalších (počet vrcholů mínus jedna) řádků vypiš matici sousednosti v horním schodovitém tvaru. Čísla odděluj mezerou.");
            input(pocerVrcholu, vrcholy);
            jarnik(vrcholy);
            Console.WriteLine("Zde je sled vrcholů, který tvoří kružnici. V tomto pořadí by měl obchodní cestující projít svou trasu.");
            Console.WriteLine(DFS(vrcholy));

            Console.ReadLine();
        }

        /// <summary>
        /// zakládá vrcholy a přidává každému do slovníku vzdálenost od ostatních
        /// </summary>
        /// <param name="n">počet vrcholů</param>
        /// <param name="nodes">list, do kterého uložím všechny zložené nody</param>
        static void input(int n, List<Node>nodes) // vstup je matice sousednosti v horním schodovitém tvaru
        {
            int k = 0;
            for (int i = 0; i < n; i++)
            {
                Node node = new Node(i);
                nodes.Add(node);
            }
            for (int i = 0; i < n-1; i++)
            {
                string[] radek = Console.ReadLine().Split(' ');

                for (int j = 0; j < radek.Length; j++)
                {
                    nodes[i].Vzdalenosti.Add(k+j+1, Convert.ToInt16(radek[j]));
                    nodes[k+j+1].Vzdalenosti.Add(i, Convert.ToInt16(radek[j]));
                }
                k += 1;
            }
        }

        static void jarnik(List<Node>nodes)
        {
            int k = 1;
            nodes[0].Otevreny = true;
            int a = 0;
            int b = 0;
            while (k < nodes.Count) //procházím dokud nebudou všechny vrcholy otevřené
            {
                int min = int.MaxValue;
                for (int i = 0; i < nodes.Count; i++) //vždy projdu všechny otevřené vrcholy a najdu jednu nemenší hranu
                {
                    if (nodes[i].Otevreny == true)
                    {
                        foreach (var soused in nodes[i].Vzdalenosti) //projdu všechny sousedy otevřeného vrcholu
                        {
                            if (soused.Value < min) //je vzdálenost kranší než dosavadní?
                            {
                                if (nodes[soused.Key].Otevreny == false) //je to hrana mezi otevřeným a zavřeným vrcholem?
                                {
                                    min = soused.Value;
                                    a = soused.Key;
                                    b = i;
                                }
                            }
                        }
                    }
                }
                nodes[a].Otevreny = true;
                nodes[a].Kostra.Add(b,min);
                nodes[b].Kostra.Add(a, min);
                k++;
            }
        }

        static string DFS(List<Node>nodes) 
        {
            StringBuilder sb = new StringBuilder();

            void _dfs(int n)
            {
                nodes[n].Navstiveny = 1;
                sb.Append(n + " ");
                foreach (int key in nodes[n].Kostra.Keys)
                {
                    if (nodes[key].Navstiveny == 0)
                        _dfs(key);
                }
                nodes[n].Navstiveny = 2;
            }
            _dfs(0);
            sb.Append(0);
            return sb.ToString();
        }

        

    }

    public class Node
    {
        public Node(int id)
        {
            Id = id;
            Otevreny = false;
            Navstiveny = 0;
        }
        public int Id { get; }
        public bool Otevreny { get; set; } // používám při hledání kostry
        public int Navstiveny { get; set; } // používám při DFS; 0 - nenalezený, 1 - otevřený, 2 - uzavřený
        public Dictionary<int, int> Vzdalenosti { get; } = new Dictionary<int, int>(); //key je ID vrcholu o který se jedná a value je délka mezi těmito vrcholy
        public Dictionary<int, int> Kostra { get; } = new Dictionary<int, int>();
    }
}
