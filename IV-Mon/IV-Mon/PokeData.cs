using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace IV_Mon
{
    struct PokeData
    {
        public string PokemonName;
        public int HP;
        public int Atk;
        public int Def;
        public int PokemonNumber;
        public string Gen;
    }

    struct CPMultiplier
    {
        public double Level;
        public double CP_Multiplier;
    }

    struct PowerUpData
    {
        public int Stardust;
        public double Level;
        public int PowerUps;
        public int SingleCandy;
        public int CumuStardust;
        public int CumuCandy;
    }

    public struct CP_Pos
    {
        public double cpPossibility;
        public int ivHPPossibility;
        public double lvlPossibility;

    }

    public struct PossibleIV
    {
        public int atk;
        public int def;
        public int hp;
        public double cp_multiplier;
        public double IV;
        public double Level;
    }

    class PokemonData
    {
        public List<PokeData> pokemonData = new List<PokeData>();
        public List<CPMultiplier> cpData = new List<CPMultiplier>();
        public List<PowerUpData> powerUpData = new List<PowerUpData>();

        public PokemonData()
        {
            ProcessPokemonDataCSV("PokemonGoIV.csv");
            ProcessCPDataCSV("CPMultiplier.csv");
            ProcessPowerUpDataCSV("Dust.csv");
        }

        public void ProcessPokemonDataCSV(string embeddedResourceFileName)
        {
            pokemonData.Clear();
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(embeddedResourceFileName));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                string[] rows = result.Split(new Char[] { '\n','\r' });

                for(int x = 1; x< rows.Length; x++)
                {
                    string[] columnData = rows[x].Split(',');
                    if (columnData.Length > 1)
                    {
                        PokeData pd = new PokeData();
                        pd.PokemonName = columnData[0];
                        pd.HP = Int32.Parse(columnData[1]);
                        pd.Atk = Int32.Parse(columnData[2]);
                        pd.Def = Int32.Parse(columnData[3]);
                        pd.PokemonNumber = Int32.Parse(columnData[4]);
                        pd.Gen = columnData[5];
                        pokemonData.Add(pd);
                    }
                }

            }


        }

        public void ProcessCPDataCSV(string embeddedResourceFileName)
        {
            cpData.Clear();
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(embeddedResourceFileName));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                string[] rows = result.Split(new Char[] { '\n', '\r' });

                for (int x = 1; x < rows.Length; x++)
                {
                    string[] columnData = rows[x].Split(',');
                    if (columnData.Length > 1)
                    {
                        CPMultiplier cpm = new CPMultiplier();
                        cpm.Level = Double.Parse(columnData[0]);
                        cpm.CP_Multiplier = Double.Parse(columnData[1]);

                        cpData.Add(cpm);
                    }
                }

            }


        }

        public void ProcessPowerUpDataCSV(string embeddedResourceFileName)
        {
            powerUpData.Clear();
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(embeddedResourceFileName));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                string[] rows = result.Split(new Char[] { '\n', '\r' });

                for (int x = 1; x < rows.Length; x++)
                {
                    string[] columnData = rows[x].Split(',');
                    if (columnData.Length > 1)
                    {
                        PowerUpData pud = new PowerUpData();
                        pud.Stardust = Int32.Parse(columnData[0]);
                        pud.Level = Double.Parse(columnData[1]);
                        pud.PowerUps = Int32.Parse(columnData[2]);
                        pud.SingleCandy = Int32.Parse(columnData[3]);
                        pud.CumuStardust = Int32.Parse(columnData[4]);
                        pud.CumuCandy = Int32.Parse(columnData[5]);

                        powerUpData.Add(pud);
                    }
                }

            }


        }
    }

}
