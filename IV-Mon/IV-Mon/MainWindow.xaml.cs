using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IV_Mon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PokemonData Pokemon = new PokemonData();

        int statMin;
        int statMax;
        double IVMin;
        double IVMax;
        int baseStam;
        int baseAtk;
        int baseDef;

        int actualHP;
        int calculatedHPIV;

        
        double PokemonLevelMin;
        double PokemonLevelMax;
        int StarDust;

        double PokemonLevel;
        double CP_Multiplier; //Lookup based on level



        public MainWindow()
        {
            InitializeComponent();
            PopulateComboWithPokemon();
            PopulateTeamChoices();
            PopulateAp1();
            PopulateAp2();

        }

        public void CalculateIV()
        {
            CalculatePokemonLevelRange();
            List<CP_Pos> cppos = new List<CP_Pos>();
            cppos = CalculateCP_Multiplier();

            //Calculate all possibilities to match CP
            List<PossibleIV> piv;
            piv = CalculateCP(cppos);

            //If appraisal -> IV stat range -> Best Stat range
            if(cbA1.SelectedIndex != -1)
            {
                //Filter IV range
                for (int i = piv.Count-1; i >= 0; i--)
                {
                    if(piv[i].IV*100f > IVMax|| piv[i].IV*100f < IVMin)
                    {
                        piv.RemoveAt(i);
                    }
                }
            }

            if (cbA2.SelectedIndex != -1)
            {//Filter stats
                if (ckAtk.IsChecked == true)
                {
                    for (int i = piv.Count - 1; i >= 0; i--)
                    {
                        if (piv[i].atk > statMax || piv[i].atk < statMin)
                        {
                            piv.RemoveAt(i);
                        }
                    }
                }
                if (ckDef.IsChecked == true)
                {
                    for (int i = piv.Count - 1; i >= 0; i--)
                    {
                        if (piv[i].def > statMax || piv[i].def < statMin)
                        {
                            piv.RemoveAt(i);
                        }
                    }
                }
                if (ckHP.IsChecked == true)
                {
                    for (int i = piv.Count - 1; i >= 0; i--)
                    {
                        if (piv[i].hp > statMax || piv[i].hp < statMin)
                        {
                            piv.RemoveAt(i);
                        }
                    }
                }

                for (int i = piv.Count - 1; i >= 0; i--)
                {
                    if((piv[i].atk > statMax) ||
                       (piv[i].def > statMax) ||
                       (piv[i].hp  > statMax))
                    {
                        piv.RemoveAt(i);
                    }
                }

            }



            lbPossibles.Items.Clear();
            for (int i = 0; i < piv.Count; i++)
            {
                lbPossibles.Items.Add("IV:" + (piv[i].IV*100f).ToString("00.00") + "% Atk: " + piv[i].atk + " Def:" + piv[i].def + " HP:" + piv[i].hp);
            }

            if(piv.Count == 1)
            {
                lblPokemonLevel.Content = piv[0].Level.ToString();
                lblIVHP.Content = piv[0].hp.ToString();
                lblIVAtk.Content = piv[0].atk.ToString();
                lblIVDef.Content = piv[0].def.ToString();
                lblCPMultiplier.Content = piv[0].cp_multiplier.ToString();

                lblAttack.Content = ((int)(piv[0].atk + baseAtk * piv[0].cp_multiplier)).ToString();
                lblDefense.Content = ((int)(piv[0].def + baseDef * piv[0].cp_multiplier)).ToString();
                lblHP.Content = ((int)(piv[0].hp + baseStam * piv[0].cp_multiplier)).ToString();

                lblIV.Content = (piv[0].IV*100f).ToString("00.00") + "%";

            }
            else if(piv.Count > 1)
            {//Found multiple entries!
                //Take the range..
                //Level...
                double levelLow = 999;
                double levelHigh = 0;

                for (int i = 0; i < piv.Count; i++)
                {
                    if(piv[i].Level > levelHigh)
                    {
                        levelHigh = piv[i].Level;
                    }
                    if (piv[i].Level < levelLow)
                    {
                        levelLow = piv[i].Level;
                    }
                }
                lblPokemonLevel.Content = (levelLow != levelHigh) ? levelLow.ToString() + " - " + levelHigh.ToString() : levelHigh.ToString();

                //HP...
                double hpLow = 999;
                double hpHigh = 0;

                for (int i = 0; i < piv.Count; i++)
                {
                    if (piv[i].hp > hpHigh)
                    {
                        hpHigh = piv[i].hp;
                    }
                    if (piv[i].hp < hpLow)
                    {
                        hpLow = piv[i].hp;
                    }
                }
                lblIVHP.Content = (hpLow != hpHigh) ? hpLow.ToString() + " - " + hpHigh.ToString() : hpHigh.ToString();

                //Atk
                double atkLow = 999;
                double atkHigh = 0;

                for (int i = 0; i < piv.Count; i++)
                {
                    if (piv[i].atk > atkHigh)
                    {
                        atkHigh = piv[i].atk;
                    }
                    if (piv[i].atk < atkLow)
                    {
                        atkLow = piv[i].atk;
                    }
                }
                lblIVAtk.Content = (atkLow != atkHigh) ? atkLow.ToString() + " - " + atkHigh.ToString() : atkHigh.ToString();

                //Def
                double defLow = 999;
                double defHigh = 0;

                for (int i = 0; i < piv.Count; i++)
                {
                    if (piv[i].def > defHigh)
                    {
                        defHigh = piv[i].def;
                    }
                    if (piv[i].def < defLow)
                    {
                        defLow = piv[i].def;
                    }
                }
                lblIVDef.Content = (defLow != defHigh) ? defLow.ToString() + " - " + defHigh.ToString() : defHigh.ToString();

                //CP
                double cpLow = 999;
                double cpHigh = 0;

                for (int i = 0; i < piv.Count; i++)
                {
                    if (piv[i].cp_multiplier > cpHigh)
                    {
                        cpHigh = piv[i].cp_multiplier;
                    }
                    if (piv[i].cp_multiplier < cpLow)
                    {
                        cpLow = piv[i].cp_multiplier;
                    }
                }
                lblCPMultiplier.Content = (cpLow != cpHigh) ? cpLow.ToString("0.000") + " - " + cpHigh.ToString("0.000") : cpHigh.ToString("0.000");

                //IV
                double ivLow = 999;
                double ivHigh = 0;

                for (int i = 0; i < piv.Count; i++)
                {
                    if (piv[i].IV > ivHigh)
                    {
                        ivHigh = piv[i].IV;
                    }
                    if (piv[i].IV < ivLow)
                    {
                        ivLow = piv[i].IV;
                    }
                }
                lblIV.Content = (ivLow != ivHigh) ? (ivLow * 100f).ToString("00.00") + " - " + (ivHigh * 100f).ToString("00.00") : (ivHigh * 100f).ToString("00.00");

                //Attack
                lblAttack.Content = "...";
                lblDefense.Content = "...";
                lblHP.Content = "...";

                //
            }
            else
            {//No entries found...
                lblPokemonLevel.Content = "...";
                lblIVHP.Content = "...";
                lblIVAtk.Content = "...";
                lblIVDef.Content = "...";
                lblCPMultiplier.Content = "...";

                lblAttack.Content = "...";
                lblDefense.Content = "...";
                lblHP.Content = "...";

                //Attack
                lblAttack.Content = "...";
                lblDefense.Content = "...";
                lblHP.Content = "...";
            }

            //lblIVHP.Content = calculatedHPIV.ToString();
            //lblCPMultiplier.Content = CP_Multiplier.ToString();
        }

        

        public List<PossibleIV> CalculateCP(List<CP_Pos> _cppos)
        {
            List<PossibleIV> piv = new List<PossibleIV>();
            try
            {
                int targetCP = Int32.Parse(txtCP.Text);

                //Try each of the HP IVs
                //For each of those, try Attack IVs and Defense IVs
                for (int i = 0; i < _cppos.Count; i++)
                {//For each HP based cppos

                    //Try each atk and each def
                    for (int a = 1; a <= 15; a++)
                    {
                        for (int d = 1; d <= 15; d++)
                        {
                            //Calculated CP = INT((BaseAtk+ IVAtkGuess)*((BaseDef+IVDefGuess)^0.5*(BaseStamina+IVHPCppos)^0.5*CPMultiCppos^2)/10)
                             int cpGuess = (int)Math.Floor((baseAtk + a) * 
                                                  Math.Pow((baseDef+d),0.5)*
                                                  Math.Pow((baseStam+_cppos[i].ivHPPossibility),0.5) * 
                                                  Math.Pow(_cppos[i].cpPossibility,2) / 10);
                            if(cpGuess == targetCP)
                            {
                                PossibleIV pivTemp = new PossibleIV();
                                pivTemp.atk = a;
                                pivTemp.def = d;
                                pivTemp.hp = _cppos[i].ivHPPossibility;
                                pivTemp.cp_multiplier = _cppos[i].cpPossibility;
                                pivTemp.IV = (a + d + _cppos[i].ivHPPossibility) / 45f;
                                pivTemp.Level = _cppos[i].lvlPossibility;
                                piv.Add(pivTemp);
                            }
                        }
                    }
                }

            }
            catch { }

            return piv;
        }

        public void CalculatePokemonLevelRange()
        {
            //StarDustLookup
            PokemonLevelMin = 1;
            PokemonLevelMax = 15;

            try
            {
                int dustInput = Int32.Parse(txtStardust.Text);
                for (int i = 0; i < Pokemon.powerUpData.Count; i++)
                {
                    if (Pokemon.powerUpData[i].Stardust == dustInput)
                    {//First occurance of Star Dust = min
                        PokemonLevelMin = Pokemon.powerUpData[i].Level;
                        for (int y = i; y < Pokemon.powerUpData.Count; y++)
                        {
                            if (Pokemon.powerUpData[y].Stardust != dustInput)
                            {//First mismatch, take the one before
                                PokemonLevelMax = Pokemon.powerUpData[y--].Level;
                                y = 99999;
                                i = 99999;
                                break;

                            }
                        }
                    }

                }

                lblLvlMin.Content = PokemonLevelMin.ToString();
                lblLvlMax.Content = PokemonLevelMax.ToString();

            }
            catch{ }
            
        }

        

        public List<CP_Pos> CalculateCP_Multiplier()
        {
            List<CPMultiplier> cpml = Pokemon.cpData;

            int potentialHP; //Base Stam + IV HP * CP_Multiplier
            int ivGuess = 0;

            if(ckHP.IsChecked == true)
            {
                ivGuess = statMin;
            }
            else
            {
                ivGuess = 0;
            }

            calculatedHPIV = 0;

            List<CP_Pos> cppos = new List<CP_Pos>();

            if (ckPoweredUp.IsChecked == true)


            for (int iv = ivGuess; iv <= 15; iv++)
            {
                for (int i = 0; i < cpml.Count;)
                {
                        //Legal CP_Multiplier?
                    if (cpml[i].Level >= PokemonLevelMin && cpml[i].Level <= PokemonLevelMax)
                    {

                        potentialHP = (int)Math.Floor(cpml[i].CP_Multiplier * (baseStam + iv));

                        if (potentialHP == actualHP)
                        {
                            CP_Pos cp = new CP_Pos();
                            cp.cpPossibility = cpml[i].CP_Multiplier;
                            cp.ivHPPossibility = iv;
                            cp.lvlPossibility = cpml[i].Level;
                            cppos.Add(cp);

                        }
                    }

                    if (ckPoweredUp.IsChecked == true)
                    {
                        i += 1;
                    }
                    else
                    {
                        i += 2;
                    }

                }
            }

            return cppos;

        }

        public void PopulateComboWithPokemon()
        {
            foreach(PokeData pd in Pokemon.pokemonData)
            {
                cbPokemon.Items.Add(pd.PokemonName);
            }
        }

        public void PopulateTeamChoices()
        {
            cbTeam.Items.Clear();
            cbTeam.Items.Add("Instinct");
            cbTeam.Items.Add("Valor");
            cbTeam.Items.Add("Mystic");
        }

        public void PopulateAp1()
        {
            cbA1.Items.Clear();
            if(cbTeam.SelectedIndex != -1)
            {
                if(cbTeam.SelectedIndex == 0)
                {//Instinct
                    cbA1.Items.Add("...battle with the best of them!");
                    cbA1.Items.Add("...is really strong!");
                    cbA1.Items.Add("...is pretty decent!");
                    cbA1.Items.Add("...room for improvement as far as battling goes!");
                }
                else if(cbTeam.SelectedIndex == 1)
                {//Valor
                    cbA1.Items.Add("...simply amazes me. It can accomplish anything!");
                    cbA1.Items.Add("...is a strong Pokemon. You should be proud!");
                    cbA1.Items.Add("...is a decent Pokemon");
                    cbA1.Items.Add("...may not be great in battle, but I still like it!");
                }
                else if (cbTeam.SelectedIndex == 2)
                {//Mystic
                    cbA1.Items.Add("...is a wonder! What a breathtaking Pokemon!");
                    cbA1.Items.Add("...has certainly caught my attention.");
                    cbA1.Items.Add("...is above average.");
                    cbA1.Items.Add("...is not likely to make much headway in battle.");
                }
            }
        }

        public void PopulateAp2()
        {
            cbA2.Items.Clear();
            if (cbTeam.SelectedIndex != -1)
            {
                if (cbTeam.SelectedIndex == 0)
                {//Instinct
                    cbA2.Items.Add("Its stats are the best I’ve ever seen! No doubt about it!");
                    cbA2.Items.Add("Its stats are really strong! Impressive.");
                    cbA2.Items.Add("It’s definitely got some good stats. Definitely!");
                    cbA2.Items.Add("Its stats are all right, but kinda basic, as far as I can see.");
                }
                else if (cbTeam.SelectedIndex == 1)
                {//Valor
                    cbA2.Items.Add("I’m blown away by its stats. WOW!");
                    cbA2.Items.Add("It’s got excellent stats! How exciting!");
                    cbA2.Items.Add("Its stats indicate that in battle, it’ll get the job done.");
                    cbA2.Items.Add("Its stats don’t point to greatness in battle.");
                }
                else if (cbTeam.SelectedIndex == 2)
                {//Mystic
                    cbA2.Items.Add("Its stats exceed my calculations. It’s incredible!");
                    cbA2.Items.Add("I am certainly impressed by its stats, I must say.");
                    cbA2.Items.Add("Its stats are noticeably trending to the positive.");
                    cbA2.Items.Add("Its stats are not out of the norm, in my opinion.");
                }
            }
        }

        public void PopulateIVRange()
        {
            if (cbA1.SelectedIndex != -1)
            {
                if (cbA1.SelectedIndex == 0)
                {//82% - 100%
                    IVMin = 82;
                    IVMax = 100;
                }
                else if (cbA1.SelectedIndex == 1)
                {//67% - 80%
                    IVMin = 67;
                    IVMax = 80;
                }
                else if (cbA1.SelectedIndex == 2)
                {//51.1% - 64.4%
                    IVMin = 51.1;
                    IVMax = 64.4;
                }
                else if (cbA1.SelectedIndex == 3)
                {//0% - 48.9%
                    IVMin = 0;
                    IVMax = 48.9;
                }

                lblIVRangeMin.Content = IVMin.ToString();
                lblIVRangeMax.Content = IVMax.ToString();
            }
        }

        public void PopulateStatRange()
        {
            if(cbA2.SelectedIndex != -1)
            {
                if(cbA2.SelectedIndex == 0)
                {//15
                    statMin = 15;
                    statMax = 15;
                }
                else if(cbA2.SelectedIndex == 1)
                {//13 - 14
                    statMin = 13;
                    statMax = 14;
                }
                else if (cbA2.SelectedIndex == 2)
                {//8 - 12
                    statMin = 8;
                    statMax = 12;
                }
                else if(cbA2.SelectedIndex == 3)
                {//0 - 7
                    statMin = 0;
                    statMax = 7;
                }

                lblStatMin.Content = statMin.ToString();
                lblStatMax.Content = statMax.ToString();
            }
        }

        

        private void cbTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PopulateAp1();
                PopulateAp2();
                CalculateIV();
            }
            catch{}
        }

        private void cbPokemon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulatePokemonStats();
            CalculateIV();
        }

        public void PopulatePokemonStats()
        {
            if (cbPokemon.SelectedIndex != -1)
            {
                PokeData pd = Pokemon.pokemonData[cbPokemon.SelectedIndex];
                lblPokeGen.Content = pd.Gen;
                lblPokeNum.Content = pd.PokemonNumber;
                lblBaseStam.Content = pd.HP;
                baseStam = pd.HP;
                lblBaseAtk.Content = pd.Atk;
                baseAtk = pd.Atk;
                lblBaseDef.Content = pd.Def;
                baseDef = pd.Def;
                
            }
        }

        private void cbA1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PopulateIVRange();
                CalculateIV();
            }
            catch { }
        }

        private void cbA2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PopulateStatRange();
                CalculateIV();
            }
            catch { }
        }

        private void CheckStatBools()
        {
            lblIVDef.Content = (ckDef.IsChecked == true) ? lblStatMax.Content : "...";
            lblIVAtk.Content = (ckAtk.IsChecked == true) ? lblStatMax.Content : "...";
            lblIVHP.Content = (ckHP.IsChecked == true) ? lblStatMax.Content : "...";
            
        }

        private void ckDef_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckStatBools();
                CalculateIV();
            }
            catch { }
        }

        private void ckHP_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckStatBools();
                CalculateIV();
            }
            catch { }
        }

        private void ckAtk_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckStatBools();
                CalculateIV();
            }
            catch { }
        }

        private void txtHP_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                actualHP = Int32.Parse(txtHP.Text);
                CalculateCP_Multiplier();
                CalculateIV();
            }
            catch { }
        }

        private void txtStardust_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CalculatePokemonLevelRange();
                CalculateIV();
            }
            catch { }
        }

        private void txtCP_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CalculateIV();
            }
            catch { }
        }

        private void ckPoweredUp_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CalculateIV();
            }
            catch { }
        }
    }
}
