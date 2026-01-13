using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace RpAproximace
{
    public class Program
    {
        static void Main(string[] args)
        {

            //možnost vložit matici do terminálu v horním schodovitém tvaru
            /*
            Console.WriteLine("Kolik vrcholů má graf?");
            int pocetVrcholu = Convert.ToInt16(Console.ReadLine());
            int[,] maticeSousednosti = new int[pocetVrcholu,pocetVrcholu];
            Console.WriteLine("Teď na dalších (počet vrcholů mínus jedna) řádků vypiš matici sousednosti v horním schodovitém tvaru. Čísla odděluj mezerou.");
            NactiVstup(pocetVrcholu,  maticeSousednosti);
            */

            //menchmark pro měření složitostí
            //BenchmarkRunner.Run<MujBenchmark>(); //pro spuštění benchmarku v mainu zakomentuj vše, ktromě tohoto řádku. Následně odkomentuj classu MujBenchmark, nakoře přepni spuštění z Debug na Release a dej spustit bez ladění(tlačítko nahoře vedle klasického spustit)

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
            
            int pocetVrcholu = 10;
            int[,] maticeSousednosti = GenerovatMatici(pocetVrcholu);
            
            
            Console.WriteLine("Zde je sled vrcholů, který tvoří nejhůře 2krát delší cestu, než by byla ta optimální. Graf ale musí splňovat trojúhelníkovou nerovnost.");
            DvaAproximace aproxmacniAlgoritmus = new DvaAproximace(pocetVrcholu, maticeSousednosti);
            aproxmacniAlgoritmus.AlgoritmusJarnik(pocetVrcholu);
            aproxmacniAlgoritmus.SpustitDfs();
            Console.WriteLine(aproxmacniAlgoritmus.cesta);

            Console.WriteLine("Toto je délka cesty, kterou vyhodila 2-aproximace:");
            Console.WriteLine(aproxmacniAlgoritmus.delka);

            

            Console.WriteLine("Toto je nejkratší hamiltonovská kružnice získaná pomocí Held-Karpova algoritmu.");

            HeldKarp heldKarp = new HeldKarp(maticeSousednosti, pocetVrcholu);
            Console.WriteLine(heldKarp.ProhledatVse(maticeSousednosti));

            Console.WriteLine("Řešení hrubou silou.");

            BruteForce bruteForce = new BruteForce(pocetVrcholu);
            bruteForce.GenerovatPermutace(maticeSousednosti, 0);
            Console.WriteLine(bruteForce.Tisk());

            Console.WriteLine("Toto je délka optimální cesty:");
            Console.WriteLine(bruteForce.nejkratsiCesta);

            Console.ReadLine();
            
        }

        
        
        


        /// <summary>
        /// dodělá a uloží matici sousednosti
        /// </summary>
        /// <param name="n">počet vrcholů</param>
        /// <param name="matice">matice sousednosti</param>
        static void NactiVstup(int n, int[,] matice) // vstup je matice sousednosti v horním schodovitém tvaru
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
        public static int[,] GenerovatMatici(int m)
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

    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class MujBenchmark
    {
        private int[,] matice { get; set; }
        private int n {  get; set; }

        [GlobalSetup]
        public void Setup()
        {
            n = 20;
            matice = Program.GenerovatMatici(n);
        }

        
        [Benchmark]
        public string Aproximace()
        {
            DvaAproximace alg = new DvaAproximace(n, matice);
            alg.AlgoritmusJarnik(n);
            alg.SpustitDfs();
            return alg.cesta;
        }
        
        [Benchmark]
        public string HeldKarp()
        {
            HeldKarp heldKarp = new HeldKarp(matice, n);
            return heldKarp.ProhledatVse(matice);
        }
        
        
        [Benchmark]
        public string HrubaSila()
        {
            BruteForce bruteForce = new BruteForce(n);
            bruteForce.GenerovatPermutace(matice, 0);
            return bruteForce.Tisk();
        }
        
        
    }
    

    public class DvaAproximace
    {

        private List<int>[] sousedi {  get; set; }
        private int[] vzdalenost {  get; set; }
        private bool[] vostre { get; set; }
        private int[,] matice { get; }
        public int delka {  get; set; }
        public string cesta { get; set; }

        public DvaAproximace(int n, int[,] matice) // kvadratický čas, ale kružnice může být nejhůře 2krát dlouhá
        {
            vzdalenost = new int[n];
            vostre = new bool[n];
            sousedi = new List<int>[n];
            delka = 0;
            this.matice = matice;
            for (int i = 0; i < n; i++)
            {
                vzdalenost[i] = int.MaxValue;
                vostre[i] = false;
                sousedi[i] = new List<int>();
            }
        }
        private int najdiMin(int k, int[] vzdalenosti, bool[] navstiveno)
        {
            int min = int.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < k; i++)
            {
                if (!navstiveno[i] && vzdalenosti[i] < min)
                {
                    min = vzdalenosti[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }

        public void AlgoritmusJarnik(int n)
        {
            int[] rodic = new int[n];
            rodic[0] = -1;
            vzdalenost[0] = 0;
            for (int i = 0; i < n; i++)
            {
                int m = najdiMin(n, vzdalenost, vostre);
                vostre[m] = true;

                for (int j = 0; j < n; j++)
                {
                    if (matice[m,j] != 0 && !vostre[j] && matice[m,j] < vzdalenost[j])
                    {
                        vzdalenost[j] = matice[m, j];
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
        public void SpustitDfs()
        {
            StringBuilder sb = new StringBuilder();
            int[] stav = new int[sousedi.Length]; // 0 - nenalezený, 1 - nalezený,
            int posledniVrchol = -1;
            void _rekurzeDfs(int n)
            {
                stav[n] = 1;
                sb.Append(n + " ");
                if (posledniVrchol != -1)
                    delka += matice[posledniVrchol, n];
                posledniVrchol = n;
                foreach (int i in sousedi[n])
                {
                    if (stav[i] == 0)
                        _rekurzeDfs(i);
                }
            }
            _rekurzeDfs(0);
            sb.Append(0);
            delka += matice[posledniVrchol, 0];
            cesta = sb.ToString();
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

        public string ProhledatVse(int[,] matice)
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
        public int nejkratsiCesta { get; private set; }
        private int[] vyslednaCesta { get; set; }
        private int[] mesta { get; set; }

        public BruteForce(int n )
        {
            mesta = new int[n - 1];
            nejkratsiCesta = int.MaxValue;
            for ( int i = 1; i < n ; i++ )
                mesta[i - 1] = i;
            vyslednaCesta = new int[n + 1];
        }
        private void prohod(int n, int m)
        {
            int docasna = mesta[n];
            mesta[n] = mesta[m];
            mesta[m] = docasna;
        }
        
        private void vypocitejCestu(int[,] matice)
        {
            int cena = 0;
            int aktualniVrchol = 0;
            foreach (int vrchol in mesta) //spočítám délku jedné dané cesty, začnu v nule
            {
                cena += matice[aktualniVrchol, vrchol];
                aktualniVrchol = vrchol;
            }

            cena += matice[aktualniVrchol, 0]; // a skončím v nule

            if (cena < nejkratsiCesta) // pokud je cesta kratší než jakákoliv dřív, přepíšu ji
            {
                nejkratsiCesta = cena; 
                for (int i = 0; i < mesta.Length; i++)  //potřebuji mít na první i na poslední pozici nulu
                    vyslednaCesta[i + 1] = mesta[i];
            }

        }

        public void GenerovatPermutace(int[,] matice, int start)
        {
            if (start == mesta.Length)
            {
                vypocitejCestu(matice);
                return;
            }
            for (int i = start; i < mesta.Length; i++) // vždy mám zafixovaný start
            {
                if (start != i)
                    prohod(start, i); 
                GenerovatPermutace(matice, start + 1);
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
