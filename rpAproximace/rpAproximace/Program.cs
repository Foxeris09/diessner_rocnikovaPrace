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
            Console.WriteLine(aproxmacniAlgoritmus.Cesta);

            Console.WriteLine("Toto je délka cesty, kterou vyhodila 2-aproximace:");
            Console.WriteLine(aproxmacniAlgoritmus.Delka);

            

            Console.WriteLine("Toto je nejkratší hamiltonovská kružnice získaná pomocí Held-Karpova algoritmu.");

            HeldKarp heldKarp = new HeldKarp(maticeSousednosti, pocetVrcholu);
            Console.WriteLine(heldKarp.ProhledatVse(maticeSousednosti));

            Console.WriteLine("Řešení hrubou silou.");

            BruteForce bruteForce = new BruteForce(pocetVrcholu);
            bruteForce.GenerovatPermutace(maticeSousednosti, 0);
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
            int MaxSouradnice = 500;
            List<(int X, int Y)> mesta = new List<(int X, int Y)>();

            for (int i = 0; i < m; i++)
            {
                mesta.Add((random.Next(0, MaxSouradnice), random.Next(0, MaxSouradnice)));
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
        private int[,] _matice;
        private int _n;

        [GlobalSetup]
        public void Setup()
        {
            _n = 20;
            _matice = Program.GenerovatMatici(_n);
        }

        
        [Benchmark]
        public string Aproximace()
        {
            DvaAproximace alg = new DvaAproximace(_n, _matice);
            alg.AlgoritmusJarnik(_n);
            alg.SpustitDfs();
            return alg.Cesta;
        }
        
        [Benchmark]
        public string HeldKarp()
        {
            HeldKarp heldKarp = new HeldKarp(_matice, _n);
            return heldKarp.ProhledatVse(_matice);
        }
        
        
        [Benchmark]
        public string HrubaSila()
        {
            BruteForce bruteForce = new BruteForce(_n);
            bruteForce.GenerovatPermutace(_matice, 0);
            return bruteForce.Tisk();
        }
        
        
    }
    

    public class DvaAproximace
    {

        private List<int>[] Sousedi {  get; set; }
        private int[] Vzdalenost {  get; set; }
        private bool[] VKostre { get; set; }
        private int[,] Matice { get; }
        public int Delka {  get; set; }
        public string Cesta { get; set; }

        public DvaAproximace(int n, int[,] matice) // kvadratický čas, ale kružnice může být nejhůře 2krát dlouhá
        {
            Vzdalenost = new int[n];
            VKostre = new bool[n];
            Sousedi = new List<int>[n];
            Delka = 0;
            Matice = matice;
            for (int i = 0; i < n; i++)
            {
                Vzdalenost[i] = int.MaxValue;
                VKostre[i] = false;
                Sousedi[i] = new List<int>();
            }
        }
        private int NajdiMin(int k, int[] vzdalenosti, bool[] navstiveno)
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
            Vzdalenost[0] = 0;
            for (int i = 0; i < n; i++)
            {
                int m = NajdiMin(n, Vzdalenost, VKostre);
                VKostre[m] = true;

                for (int j = 0; j < n; j++)
                {
                    if (Matice[m,j] != 0 && !VKostre[j] && Matice[m,j] < Vzdalenost[j])
                    {
                        Vzdalenost[j] = Matice[m, j];
                        rodic[j] = m;
                    }
                }
            }
            for (int i = 0;i < n; i++)
            {
                if (rodic[i] != -1)
                {
                    Sousedi[i].Add(rodic[i]);
                    Sousedi[rodic[i]].Add(i);
                }
            }
        }
        public void SpustitDfs()
        {
            StringBuilder sb = new StringBuilder();
            int[] stav = new int[Sousedi.Length]; // 0 - nenalezený, 1 - nalezený,
            int posledniVrchol = -1;
            void RekurzeDfs(int n)
            {
                stav[n] = 1;
                sb.Append(n + " ");
                if (posledniVrchol != -1)
                    Delka += Matice[posledniVrchol, n];
                posledniVrchol = n;
                foreach (int i in Sousedi[n])
                {
                    if (stav[i] == 0)
                        RekurzeDfs(i);
                }
            }
            RekurzeDfs(0);
            sb.Append(0);
            Delka += Matice[posledniVrchol, 0];
            Cesta = sb.ToString();
        }
    }

    public class HeldKarp  // úplné řešení v exponenciálním čase 
    {
        private int[,] Cesty { get; set; }
        private int[,] Rodic { get; set; }
        private int Velikost { get; }
        private int N { get; }
        public HeldKarp(int[,] matice, int m) 
        {
            N = m;
            Velikost = (1 << N); //Navstivena mesta budu reprezentovat pomocí bitmasky 0-nenavstiveny 1-navstiveny 
            Cesty = new int[Velikost, N]; // tabulka 2^n * n ve které je uložena délka cesty; pozice x označuje čislo té bitmasky a y je vrchol, ve kterém cesta končí
            Rodic = new int[Velikost, N];

            for (int i = 0; i < Velikost; i++)
                for (int j = 0; j < N; j++)
                {
                    Cesty[i, j] = int.MaxValue/2;
                    Rodic[i, j] = -1;
                }

            for (int i = 1; i < N; i++)
            {
                int k = (1 << i) + 1;   //pro cislo 2 to bude 0101 to znamená že byl navstiven vrchol 0 a 2
                Cesty[k, i] = matice[0, i]; // nula to bude vždy protože začíná
                Rodic[k, i] = 0;
            }

        }

        public string ProhledatVse(int[,] matice)
        {
            for (int maska = 1; maska < Velikost; maska++)
            {
                if ((maska & (1<<0)) == 0) // když bude na nulté pozici nula(nultý vrchol není součástí cesty) přeskočím
                    continue;

                for (int i = 1; i < N; i++)
                {
                    if((maska & (1<<i)) == 0)  // ještě potřebuji aby byl v masce druhý vrchol i
                        continue;
                    int predchoziMaska = maska ^ (1<<i); // udělá masku přechozí této(smaže z ní 1 an i-té pozici)
                    for (int j = 0; j < N; j++)
                    {
                        if ((predchoziMaska & (1<<j)) == 0)
                            continue;
                        int novaVzdalenost = Cesty[predchoziMaska, j] + matice[j,i]; //postupně přidávám cesty, pokud se do nichh dá nově dostat
                        if (novaVzdalenost < Cesty[maska, i])
                        {
                            Cesty[maska, i] = novaVzdalenost;
                            Rodic[maska, i] = j;
                        }

                    }
                }

            }

            int minCesta = int.MaxValue;
            int aktualniVrchol = -1;
            int vyslednaMaska = Velikost - 1;

            for (int i = 1; i < N; i++)  // tady se vrátím cestou zpátky do vrcholu nula a najdu nejlepší variantu
            {
                int plnaCesta = Cesty[vyslednaMaska, i] + matice[i,0];
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
                int dalsiVrchol = Rodic[vyslednaMaska, aktualniVrchol];
                vyslednaMaska = vyslednaMaska ^ (1 << aktualniVrchol); //vždy beru z aktualní masky, jdu 'pozpátku' a vždy smažu vrchol který jsem zapsal a přesunu se na jeho rodiče
                aktualniVrchol = dalsiVrchol;
            }
            sb.Append(0);
            return sb.ToString();
        }

    }

    public class BruteForce
    {
        public int NejkratsiCesta { get; private set; }
        private int[] VyslednaCesta { get; set; }
        private int[] Mesta { get; set; }

        public BruteForce(int n )
        {
            Mesta = new int[n - 1];
            NejkratsiCesta = int.MaxValue;
            for ( int i = 1; i < n ; i++ )
                Mesta[i - 1] = i;
            VyslednaCesta = new int[n + 1];
        }
        private void Prohod(int n, int m)
        {
            int docasna = Mesta[n];
            Mesta[n] = Mesta[m];
            Mesta[m] = docasna;
        }
        
        private void VypocitejCestu(int[,] matice)
        {
            int cena = 0;
            int aktualniVrchol = 0;
            foreach (int vrchol in Mesta) //spočítám délku jedné dané cesty, začnu v nule
            {
                cena += matice[aktualniVrchol, vrchol];
                aktualniVrchol = vrchol;
            }

            cena += matice[aktualniVrchol, 0]; // a skončím v nule

            if (cena < NejkratsiCesta) // pokud je cesta kratší než jakákoliv dřív, přepíšu ji
            {
                NejkratsiCesta = cena; 
                for (int i = 0; i < Mesta.Length; i++)  //potřebuji mít na první i na poslední pozici nulu
                    VyslednaCesta[i + 1] = Mesta[i];
            }

        }

        public void GenerovatPermutace(int[,] matice, int start)
        {
            if (start == Mesta.Length)
            {
                VypocitejCestu(matice);
                return;
            }
            for (int i = start; i < Mesta.Length; i++) // vždy mám zafixovaný start
            {
                if (start != i)
                    Prohod(start, i); 
                GenerovatPermutace(matice, start + 1);
                if (start != i)
                    Prohod(start, i);
            }
            
        }
        public string Tisk()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int vrchol in VyslednaCesta)
                sb.Append(vrchol + " ");
            return sb.ToString();
        }

    }
}
