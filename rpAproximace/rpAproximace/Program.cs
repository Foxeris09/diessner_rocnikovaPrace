using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace rpAproximace
{
    public class Program
    {
        static void Main(string[] args)
        {
            //možnost vložit matici do terminálu v horním schodovitém tvaru
            
            Console.WriteLine("Kolik vrcholů má graf?");
            int pocetVrcholu = Convert.ToInt16(Console.ReadLine());
            int[,] maticeSousednosti = new int[pocetVrcholu,pocetVrcholu];
            Console.WriteLine("Teď na dalších (počet vrcholů mínus jedna) řádků vypiš matici sousednosti v horním schodovitém tvaru. Čísla odděluj mezerou.");
            input(pocetVrcholu,  maticeSousednosti);

            //menchmark pro měření složitostí
            //BenchmarkRunner.Run<MujBenchmark>(); //pro spuštění benchmarku v mainu zakomentuj vše, ktromě tohoto řádku. Následně odkomentuj classu MujBenchmark, nakoře přepni spuštění z Debug na Release a dej Ctrl + F5

            //možnost vypsat vlastní matici
            /*  
            int[,] maticeSousednosti = new int[,] {
                    { 0, 15, 42, 54, 21, 63, 33, 48, 12 },
                    { 15, 0, 31, 45, 18, 55, 24, 39, 10 },
                    { 42, 31, 0, 18, 25, 33, 14, 21, 38 },
                    { 54, 45, 18, 0, 39, 21, 28, 15, 50 },
                    { 21, 18, 25, 39, 0, 48, 15, 30, 24 },
                    { 63, 55, 33, 21, 48, 0, 38, 22, 58 },
                    { 33, 24, 14, 28, 15, 38, 0, 18, 32 },
                    { 48, 39, 21, 15, 30, 22, 18, 0, 45 },
                    { 12, 10, 38, 50, 24, 58, 32, 45, 0 }
                };
            */

            // možnost vygenerování matice
            /* 
            int pocetVrcholu = 5;
            int[,] maticeSousednosti = GeneraceMatice(pocetVrcholu);
            */

            Console.WriteLine("Zde je sled vrcholů, který tvoří nejhůře 2krát delší cestu, než by byla ta optimální. Graf ale musí splňovat trojúhelníkovou nerovnost.");
            dvaAproximace aproxmacniAlgoritmus = new dvaAproximace(pocetVrcholu, maticeSousednosti);
            aproxmacniAlgoritmus.Jarnik(pocetVrcholu);
            aproxmacniAlgoritmus.DFS();
            Console.WriteLine(aproxmacniAlgoritmus.Cesta);

            Console.WriteLine("Toto je délka cesty, kterou vyhodila 2-aproximace:");
            Console.WriteLine(aproxmacniAlgoritmus.Delka);

            

            Console.WriteLine("Toto je nejkratší hamiltonovská kružnice získaná pomocí Held-Karpova algoritmu.");

            HeldKarp heldKarp = new HeldKarp(maticeSousednosti, pocetVrcholu);
            Console.WriteLine(heldKarp.ProhledaniVsech(maticeSousednosti));

            Console.WriteLine("Řešení hrubou silou.");

            BruteForce bruteForce = new BruteForce(pocetVrcholu);
            bruteForce.Permutace(maticeSousednosti, 0);
            Console.WriteLine(bruteForce.Tisk());

            Console.WriteLine("Toto je délka optimální cesty:");
            Console.WriteLine(bruteForce.NejkratsiCesta);

            Console.ReadLine();
            
        }

        
        
        


        /// <summary>
        /// dodělá a uloží matici sousednosti
        /// </summary>
        /// <param name="n">počet vrcholů</param>
        /// <param name="matice">matice sousednosti</param>
        static void input(int n, int[,] matice) // vstup je matice sousednosti v horním schodovitém tvaru
        {
            int k = 1;
            
            for (int i = 0; i < n-1; i++)
            {
                string[] radek = Console.ReadLine().Split(' ');

                for (int j = 0; j < radek.Length; j++)
                {
                    matice[i, k + j] = Convert.ToInt16(radek[j]);
                    matice[k+j,i] = Convert.ToInt16(radek[j]);
                }
                k += 1;
            }
        }

        /// <summary>
        /// Funkce pro vygenerování matice sousednosti grafu, ktarý splňuje trojůhelníkovou nerovnost
        /// </summary>
        /// <param name="m">počet vrcholů</param>
        /// <returns></returns>
        public static int[,] GeneraceMatice(int m)
        {
            Random random = new Random();
            int[,] vyslednaMatice = new int[m, m];
            int maxSouradnice = 500;
            List<(int X, int Y)> mesta = new List<(int X, int Y)>();

            for (int i = 0; i < m; i++)
            {
                mesta.Add((random.Next(0, maxSouradnice), random.Next(0, maxSouradnice)));
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (i != j)
                    {
                        int vektorX = mesta[i].X - mesta[j].X;
                        int vektorY = mesta[i].Y - mesta[j].Y;

                        double velikostVektoru = Math.Sqrt((vektorX * vektorX) + (vektorY * vektorY));
                        vyslednaMatice[i, j] = Convert.ToInt32(Math.Ceiling(velikostVektoru)); //ceiling vrací hodnotu zaokrouhlenou nahoru
                    }
                }
            }
            return vyslednaMatice;
        }
    }
    /*
    [MemoryDiagnoser]
    public class MujBenchmark
    {
        private int[,] matice;
        private int n;

        [GlobalSetup]
        public void Setup()
        {
            n = 5;
            matice = Program.GeneraceMatice(n);
        }

        


        [Benchmark]
        public string Aproximace()
        {
            dvaAproximace alg = new dvaAproximace(n, matice);
            alg.Jarnik(n);
            alg.DFS();
            return alg.Cesta;
        }

        [Benchmark]
        public string HeldKarp()
        {
            HeldKarp heldKarp = new HeldKarp(matice, n);
            return heldKarp.ProhledaniVsech(matice);
        }

        [Benchmark]
        public string HrubaSila()
        {
            BruteForce bruteForce = new BruteForce(n);
            bruteForce.Permutace(matice, 0);
            return bruteForce.Tisk();
        }
    }
    */

    public class dvaAproximace
    {

        private List<int>[] sousedi {  get; set; }
        private int[] vzdalenost {  get; set; }
        private bool[] vKostre { get; set; }
        private int[,] _matice { get; }
        public int Delka {  get; set; }
        public string Cesta { get; set; }

        public dvaAproximace(int n, int[,] matice) // kvadratický čas, ale kružnice může být nejhůře 2krát dlouhá
        {
            vzdalenost = new int[n];
            vKostre = new bool[n];
            sousedi = new List<int>[n];
            Delka = 0;
            _matice = matice;
            for (int i = 0; i < n; i++)
            {
                vzdalenost[i] = int.MaxValue;
                vKostre[i] = false;
                sousedi[i] = new List<int>();
            }
        }
        private int najdiMin(int k, int[] ints, bool[] bools)
        {
            int min = int.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < k; i++)
            {
                if (!bools[i] && ints[i] < min)
                {
                    min = ints[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }

        public void Jarnik(int n)
        {
            int[] rodic = new int[n];
            rodic[0] = -1;
            vzdalenost[0] = 0;
            for (int i = 0; i < n; i++)
            {
                int m = najdiMin(n, vzdalenost, vKostre);
                vKostre[m] = true;

                for (int j = 0; j < n; j++)
                {
                    if (_matice[m,j] != 0 && !vKostre[j] && _matice[m,j] < vzdalenost[j])
                    {
                        vzdalenost[j] = _matice[m, j];
                        rodic[j] = m;
                    }
                }
            }
            for (int i = 0;i < n; i++)
            {
                if (rodic[i] != -1)
                {
                    sousedi[i].Add(rodic[i]);
                    sousedi[rodic[i]].Add(i);
                }
            }
        }
        public void DFS()
        {
            StringBuilder sb = new StringBuilder();
            int[] stav = new int[sousedi.Length]; // 0 - nenalezený, 1 - nalezený,
            int posledni = -1;
            void _dfs(int n)
            {
                stav[n] = 1;
                sb.Append(n + " ");
                if (posledni != -1)
                    Delka += _matice[posledni, n];
                posledni = n;
                foreach (int i in sousedi[n])
                {
                    if (stav[i] == 0)
                        _dfs(i);
                }
            }
            _dfs(0);
            sb.Append(0);
            Delka += _matice[posledni, 0];
            Cesta = sb.ToString();
        }
    }

    public class HeldKarp  // úplné řešení v exponenciálním čase 
    {
        private int[,] cesty { get; set; }
        private int[,] rodic { get; set; }
        private int velikost { get; }
        private int n { get; }
        public HeldKarp(int[,] matice, int m) 
        {
            n = m;
            velikost = (1 << n); //Navstivena mesta budu reprezentovat pomocí bitmasky 0-nenavstiveny 1-navstiveny 
            cesty = new int[velikost, n]; // tabulka 2^n * n ve které je uložena délka cesty; pozice x označuje čislo té bitmasky a y je vrchol, ve kterém cesta končí
            rodic = new int[velikost, n];

            for (int i = 0; i < velikost; i++)
                for (int j = 0; j < n; j++)
                {
                    cesty[i, j] = int.MaxValue/2;
                    rodic[i, j] = -1;
                }

            for (int i = 1; i < n; i++)
            {
                int k = (1 << i) + 1;   //pro cislo 2 to bude 0101 to znamená že byl navstiven vrchol 0 a 2
                cesty[k, i] = matice[0, i]; // nula to bude vždy protože začíná
                rodic[k, i] = 0;
            }

        }

        public string ProhledaniVsech(int[,] matice)
        {
            for (int maska = 1; maska < velikost; maska++)
            {
                if ((maska & (1<<0)) == 0) // když bude na nulté pozici nula(nultý vrchol není součástí cesty) přeskočím
                    continue;

                for (int i = 1; i < n; i++)
                {
                    if((maska & (1<<i)) == 0)  // ještě potřebuji aby byl v masce druhý vrchol i
                        continue;
                    int predchoziMaska = maska ^ (1<<i); // udělá masku přechozí této(smaže z ní 1 an i-té pozici)
                    for (int j = 0; j < n; j++)
                    {
                        if ((predchoziMaska & (1<<j)) == 0)
                            continue;
                        int novaVzdalenost = cesty[predchoziMaska, j] + matice[j,i]; //postupně přidávám cesty, pokud se do nichh dá nově dostat
                        if (novaVzdalenost < cesty[maska, i])
                        {
                            cesty[maska, i] = novaVzdalenost;
                            rodic[maska, i] = j;
                        }

                    }
                }

            }

            int minCesta = int.MaxValue;
            int aktualniVrchol = -1;
            int vyslednaMaska = velikost - 1;

            for (int i = 1; i < n; i++)  // tady se vrátím cestou zpátky do vrcholu nula a najdu nejlepší variantu
            {
                int plnaCesta = cesty[vyslednaMaska, i] + matice[i,0];
                if (plnaCesta < minCesta)
                {
                    minCesta = plnaCesta;
                    aktualniVrchol = i;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(0 + " ");
            while (aktualniVrchol != 0)   // tady rekonstruuji cestu, což povede na optimální řešení
            {
                sb.Append(aktualniVrchol + " ");
                int dalsiVrchol = rodic[vyslednaMaska, aktualniVrchol];
                vyslednaMaska = vyslednaMaska ^ (1 << aktualniVrchol); //vždy beru z aktualní masky, jdu 'pozpátku' a vždy smažu vrchol který jsem zapsal a přesunu se na jeho rodiče
                aktualniVrchol = dalsiVrchol;
            }
            sb.Append(0);
            return sb.ToString();
        }

    }

    public class BruteForce
    {
        public int NejkratsiCesta = int.MaxValue;
        private int[] vyslednaCesta { get; set; }
        private int[] mesta { get; set; }

        public BruteForce(int n )
        {
            mesta = new int[n - 1];
            for ( int i = 1; i < n ; i++ )
                mesta[i - 1] = i;
            vyslednaCesta = new int[n + 1];
        }
        private void prohod(int n, int m)
        {
            int l = mesta[n];
            mesta[n] = mesta[m];
            mesta[m] = l;
        }
        
        private void vypocitaniCesty(int[,] ints)
        {
            int cena = 0;
            int aktualniVrchol = 0;
            foreach (int vrchol in mesta) //spočítám délku jedné dané cesty, začnu v nule
            {
                cena += ints[aktualniVrchol, vrchol];
                aktualniVrchol = vrchol;
            }

            cena += ints[aktualniVrchol, 0]; // a skončím v nule

            if (cena < NejkratsiCesta) // pokud je cesta kratší než jakákoliv dřív, přepíšu ji
            {
                NejkratsiCesta = cena; 
                for (int i = 0; i < mesta.Length; i++)  //potřebuji mít na první i na poslední pozici nulu
                    vyslednaCesta[i + 1] = mesta[i];
            }

        }

        public void Permutace(int[,] matice, int start)
        {
            if (start == mesta.Length)
            {
                vypocitaniCesty(matice);
                return;
            }
            for (int i = start; i < mesta.Length; i++) // vždy mám zafixovaný start
            {
                if (start != i)
                    prohod(start, i); 
                Permutace(matice, start + 1);
                if (start != i)
                    prohod(start, i);
            }
            
        }
        public string Tisk()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int vrchol in vyslednaCesta)
                sb.Append(vrchol + " ");
            return sb.ToString();
        }

    }



}
