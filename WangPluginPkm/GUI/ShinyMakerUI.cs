using PKHeX.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using Overworld8RNG = WangPluginPkm.RNG.Methods.Overworld8RNG;

namespace WangPluginPkm.GUI
{
    partial class ShinyMakerUI : Form
    {
        public static uint XorNumber;
        public static Stopwatch sw = new();
        private ShinyRange T = ShinyRange.BOX;
        private SoundPlayer Player = new SoundPlayer();
        public enum ShinyRange
        {
            [Description("闪一箱")]
            BOX,
            [Description("闪全部")]
            All,
        }
        public enum Shinytype
        {
            SID16,
            Star,
            Square,
            RandomStar,
            Xor,
        }
        public static Shinytype shinyflag = Shinytype.Star;

        private ISaveFileProvider SAV { get; }
        private IPKMView Editor { get; }

        public ShinyMakerUI(ISaveFileProvider sav, IPKMView editor)
        {
            SAV = sav;
            Editor = editor;
            InitializeComponent();
            BindingData();
        }
        private void BindingData()
        {
            int[] shiny8 = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            int[] shiny = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
            if (Editor.Data.Format < 8)
                this.XorBox.DataSource = shiny;
            else
                this.XorBox.DataSource = shiny8;
            this.XorBox.SelectedIndexChanged += (_, __) =>
            {
                XorNumber = Convert.ToUInt16(this.XorBox.SelectedItem.ToString());
            };
            this.XorBox.SelectedIndex = 0;
            this.RangeBox.DisplayMember = "Description";
            this.RangeBox.ValueMember = "Value";
            this.RangeBox.DataSource = Enum.GetValues(typeof(ShinyRange))
                .Cast<Enum>()
                .Select(value => new
                {
                    (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description,
                    value
                })
                .OrderBy(item => item.value)
                .ToList();

            this.RangeBox.SelectedIndexChanged += (_, __) =>
            {
                T = (ShinyRange)Enum.Parse(typeof(ShinyRange), this.RangeBox.SelectedValue.ToString(), false);
            };
            this.RangeBox.SelectedIndex = 0;
        }
        public static void SetAllShiny(ISaveFileProvider SaveFileEditor)
        {
            var sav = SaveFileEditor.SAV;
            sav.ModifyBoxes(ShinyFunction);
            SaveFileEditor.ReloadSlots();
        }
        public static void SetBoxShiny(ISaveFileProvider SaveFileEditor, int i)
        {
            var sav = SaveFileEditor.SAV;
            sav.ModifyBoxes(ShinyFunction, i, i);
            SaveFileEditor.ReloadSlots();
        }
        public static void ShinySID16(PKM pkm)
        {

            pkm.SetShinySID();

        }
        private static uint RandomStar(PKM pk)
        {
            Random myObject = new Random();
            var r = (uint)myObject.Next(1, 8);
            if (pk.Gen8 == true)
                r = (uint)myObject.Next(1, 16);
            return (((uint)(pk.TID16 ^ pk.SID16) ^ (pk.PID & 0xFFFF) ^ r) << 16) | (pk.PID & 0xFFFF);
        }
        private static uint ShinyPID(PKM val, int f = 0)
        {
            if (shinyflag == Shinytype.RandomStar && f == 0)
                val.PID = RandomStar(val);
            else if (shinyflag == Shinytype.Star && f == 0)
                val.PID = (((uint)(val.TID16 ^ val.SID16) ^ (val.PID & 0xFFFF) ^ 1u) << 16) | (val.PID & 0xFFFF);
            else if (shinyflag == Shinytype.Square && f == 0)
                val.PID = (((uint)(val.TID16 ^ val.SID16) ^ (val.PID & 0xFFFF) ^ 0u) << 16) | (val.PID & 0xFFFF);
            else if (shinyflag == Shinytype.Xor && f == 0)
                val.PID = (((uint)(val.TID16 ^ val.SID16) ^ (val.PID & 0xFFFF) ^ XorNumber) << 16) | (val.PID & 0xFFFF);
            else if (f == 1)
                val.PID = (((uint)(val.TID16 ^ val.SID16) ^ (val.PID & 0xFFFF) ^ 1u) << 16) | (val.PID & 0xFFFF);
            return val.PID;
        }
        private static int ShinySID16Lite(PKM val, int f = 0)
        {

            if (shinyflag == Shinytype.Star && f == 0)
                val.SID16 = SetShinySID16(val.TID16, val.PID, Shinytype.Star);
            else if (shinyflag == Shinytype.Square && f == 0)
                val.SID16 = SetShinySID16(val.TID16, val.PID, Shinytype.Square);
            else if (shinyflag == Shinytype.Xor && f == 0)
                val.SID16 = SetShinySID16(val.TID16, val.PID, Shinytype.Xor); ;
            return val.SID16;
        }
        private static uint ShinyPIDLite(PKM val, int f = 0)
        {
            if (shinyflag == Shinytype.RandomStar && f == 0)
                val.PID = RandomStar(val);
            return val.PID;
        }
        public static void ShinyFunction(PKM pkm)
        {
            PKM val = pkm.Clone();
            PKM va = pkm.Clone();
          
            if (!MythicalFlag.MFlag(pkm.Species))
            {
                bool EggFlag = val.IsEgg || val.WasEgg || val.IsTradedEgg || val.WasTradedEgg || val.MetLevel <= 1;
                
                if(VersionFlag.Gen1VCFlag(val.Version)||
                    VersionFlag.Gen2VCFlag(val.Version))
                {
                    pkm.IV_ATK = 15;
                    pkm.IV_DEF = 10;
                    pkm.IV_SPA = 10;
                    pkm.IV_SPD = 10;
                    pkm.IV_SPE = 10;
                
                }
                if (VersionFlag.Gen3Flag(val.Version) ||
                    VersionFlag.Gen4Flag(val.Version) ||
                    VersionFlag.Gen5Flag(val.Version) ||
                    VersionFlag.CXDFlag(val.Version))
                {
                    if (EggFlag)
                    {

                        pkm.PID = ShinyPID(val);
                        if (pkm.Format != 3)
                        {
                            pkm.Ball = 4;
                        }
                        pkm.RefreshAbility((int)(pkm.PID & 1));
                        Span<int> abilityarray = stackalloc int[10]; ;
                        pkm.PersonalInfo.GetAbilities(abilityarray);
                        if (abilityarray[0] == abilityarray[1])
                        {
                            pkm.AbilityNumber = 1;
                        }
                        pkm.Nature = (Nature)(int)(pkm.PID % 25);
                        pkm.Gender = EntityGender.GetFromPID(pkm.Species, pkm.PID);
                        pkm.StatNature = pkm.Nature;
                        pkm.EncryptionConstant = pkm.PID;
                        pkm.RefreshChecksum();

                    }
                    if ((!EggFlag) && (!VersionFlag.CXDFlag(val.Version)))
                    {
                        const int maxResults = LCRNG.MaxCountSeedsIV;
                        Span<uint> seeds = stackalloc uint[maxResults];
                        int count = 0;
                        if (shinyflag == Shinytype.RandomStar)
                        {
                            for (int i = 0; ; i++)
                            {
                                val.PID = ShinyPIDLite(val);
                                count = LCRNGReversal.GetSeeds(seeds, val.PID);
                                if (count == 0)
                                    val.PID = Util.Rand32();
                                else
                                    break;
                            }
                            var reg = seeds[..count];
                            int[] iv;
                            foreach (var seed in reg)
                            {
                                iv = LCRNGReversal.SetValuesFromSeedLCRNG(val, seed);
                                if (val.IsShiny)
                                    val.IVs = iv;
                            }
                        }
                        else
                        {
                            pkm.SID16 = (ushort)ShinySID16Lite(val);
                        }
                        pkm.PID = val.PID;
                        pkm.IVs = val.IVs;
                        pkm.Gender = EntityGender.GetFromPID(pkm.Species, pkm.PID);
                        pkm.Nature = (Nature)(pkm.PID % 25);
                        pkm.StatNature = pkm.Nature;
                        pkm.EncryptionConstant = pkm.PID;
                        pkm.RefreshAbility((int)(pkm.PID & 1));
                        Span<int> abilityarray = stackalloc int[10]; ;
                        pkm.PersonalInfo.GetAbilities(abilityarray);
                        if (abilityarray[0] == abilityarray[1])
                        {
                            pkm.AbilityNumber = 1;
                        }
                        pkm.RefreshChecksum();
                    }
                    if (VersionFlag.CXDFlag(val.Version) && !GiftAndStarter.XDCGFFlag(val.Species))
                    {

                        const int maxResults = XDRNG.MaxCountSeedsIV;
                        Span<uint> seeds = stackalloc uint[maxResults];
                        int count = 0;
                        if (shinyflag == Shinytype.RandomStar)
                        {
                            for (int i = 0; ; i++)
                            {
                                val.PID = ShinyPIDLite(val);
                                count = XDRNGReversal.GetSeeds(seeds, val.PID);
                                if (count == 0)
                                    val.PID = Util.Rand32();
                                var reg = seeds[..count];
                                int[] iv;
                                foreach (var seed in reg)
                                {
                                    iv = XDRNGReversal.SetValuesFromSeedXDRNG(val, seed);
                                    if (val.IsShiny)
                                        val.IVs = iv;
                                }
                                if (!val.IsShiny)
                                {
                                    continue;
                                }
                                else
                                    break;
                            }
                        }
                        else
                        {
                            pkm.SID16 = (ushort)ShinySID16Lite(val);
                        }
                        pkm.PID = val.PID;
                        pkm.IVs = val.IVs;
                        pkm.Gender = EntityGender.GetFromPID(pkm.Species, pkm.PID);
                        pkm.Nature = (Nature)(pkm.PID % 25);
                        pkm.EncryptionConstant = pkm.PID;
                        pkm.RefreshChecksum();
                    }

                }
                if (VersionFlag.Gen6Flag(val.Version) ||
                    VersionFlag.Gen7Flag(val.Version))
                {
                    pkm.PID = ShinyPID(val);
                    CommonEdits.SetRandomEC(pkm);
                }
                if (VersionFlag.Gen8SWSHFlag(val.Version))
                {
                    pkm.PID = Util.Rand32();
                    if (EggFlag || pkm.MetLocation == 162)
                    {
                        pkm.PID = ShinyPID(val);
                        CommonEdits.SetRandomEC(pkm);
                    }
                    else if (Gen8MaxLairGodPool.MaxLairGodFlag(pkm.Species))
                    {
                        pkm.PID = ShinyPID(val, 1);
                        CommonEdits.SetRandomEC(pkm);
                    }
                    else
                    {
                        bool[] iv = new bool[] { false, false, false };
                        for (; ; )
                        {
                            uint seed = Util.Rand32();
                            if (shinyflag != Shinytype.Xor && Overworld8RNG.GenPkmQ(ref pkm, seed, ShinyArray(), iv))
                            {
                                pkm.RefreshChecksum();
                                break;
                            }
                            else if (shinyflag == Shinytype.Xor && Overworld8RNG.GenPkmQ(ref pkm, seed, ShinyArray(), iv, XorNumber))
                            {

                                pkm.RefreshChecksum();
                                break;
                            }
                            else
                                Overworld8RNG.Next(seed);
                        }
                    }
                }
                if (VersionFlag.Gen8PLAFlag(val.Version))
                {
                    pkm.PID = ShinyPID(val);
                    CommonEdits.SetRandomEC(pkm);
                }
                if (VersionFlag.Gen8BDSPFlag(val.Version))
                {
                    pkm.PID = ShinyPID(val);
                    CommonEdits.SetRandomEC(pkm);
                }
                if (VersionFlag.Gen9Flag(val.Version))
                {
                    pkm.PID = ShinyPID(val);
                    CommonEdits.SetRandomEC(pkm);
                }
            }
            var la = new LegalityAnalysis(pkm);
            if (!la.Valid)
            {
                pkm.TID16 = va.TID16;
                pkm.SID16 = va.SID16;
                pkm.PID = va.PID;
                pkm.IVs = va.IVs;
                pkm.Ability = va.Ability;
                pkm.AbilityNumber = va.AbilityNumber;
                pkm.Nature = va.Nature;
                pkm.StatNature = va.StatNature;
                pkm.Gender = va.Gender;
                pkm.EncryptionConstant = va.EncryptionConstant;
                if (pkm is IScaledSize s && va is IScaledSize p)
                {
                    s.HeightScalar = p.HeightScalar;
                    s.WeightScalar = p.WeightScalar;
                }
            }
        }
        public static ushort SetShinySID16(ushort TID16, uint PID, Shinytype shiny = Shinytype.Square)
        {

            var xor = TID16 ^ (PID >> 16) ^ (PID & 0xFFFF);
            uint bits = shiny switch
            {
                Shinytype.Square => 0,
                Shinytype.Star => 1,
                Shinytype.Xor => XorNumber,
                _ => 0,
            };
            ushort SID16 = (ushort)(xor ^ bits);
            return SID16;
        }
        private void ShinySID16_BTN_Click(object sender, EventArgs e)
        {
            sw.Start();
            switch (T)
            {
                case ShinyRange.BOX:
                    int i = SAV.CurrentBox;
                    SAV.SAV.ModifyBoxes(ShinySID16, i, i);
                    break;
                case ShinyRange.All:
                    SAV.SAV.ModifyBoxes(ShinySID16);
                    break;
            }
            SAV.ReloadSlots();
            sw.Stop();
            MessageBox.Show($"搞定啦！用时：{sw.ElapsedMilliseconds}毫秒", "SuperWang");
        }
        private void Shiny_BTN_Click(object sender, EventArgs e)
        {
            sw.Start();
            switch (T)
            {
                case ShinyRange.BOX:

                    int i = SAV.CurrentBox;
                    shinyflag = Shinytype.RandomStar;
                    SetBoxShiny(SAV, i);
                    break;
                case ShinyRange.All:
                    shinyflag = Shinytype.RandomStar;
                    SetAllShiny(SAV);
                    break;
            }
            sw.Stop();
            MessageBox.Show($"搞定啦！用时：{sw.ElapsedMilliseconds}毫秒", "SuperWang");
            sw.Reset();
            Player.Stream = Properties.Resources.shinys;
            Player.Play();

        }
        private static bool[] ShinyArray()
        {
            bool[] shiny = new bool[6] { false, false, false, false, false, false };

            if (shinyflag == Shinytype.RandomStar)
            {
                shiny = new bool[6] { false, false, true, false, false, false };
            }
            else if (shinyflag == Shinytype.Star)
            {
                shiny = new bool[6] { false, false, false, false, true, false };
            }
            else if (shinyflag == Shinytype.Square)
            {
                shiny = new bool[6] { false, false, false, true, false, false };
            }
            else if (shinyflag == Shinytype.Xor)
            {
                shiny = new bool[6] { false, false, false, false, false, true };
            }
            return shiny;
        }
        private void ForceSquare_BTN_Click(object sender, EventArgs e)
        {
            sw.Start();
            switch (T)
            {
                case ShinyRange.BOX:

                    int i = SAV.CurrentBox;
                    shinyflag = Shinytype.Square; ;
                    SetBoxShiny(SAV, i);
                    break;
                case ShinyRange.All:
                    shinyflag = Shinytype.Square; ;
                    SetAllShiny(SAV);
                    break;
            }
            sw.Stop();
            MessageBox.Show($"搞定啦！用时：{sw.ElapsedMilliseconds}毫秒", "SuperWang");
            sw.Reset();
        }
        private void Xor_BTN_Click(object sender, EventArgs e)
        {
            sw.Start();
            switch (T)
            {
                case ShinyRange.BOX:

                    int i = SAV.CurrentBox;
                    shinyflag = Shinytype.Xor;
                    SetBoxShiny(SAV, i);
                    break;
                case ShinyRange.All:
                    shinyflag = Shinytype.Xor;
                    SetAllShiny(SAV);
                    break;
            }
            sw.Stop();
            MessageBox.Show($"搞定啦！用时：{sw.ElapsedMilliseconds}毫秒", "SuperWang");
            sw.Reset();
        }
        private void ForceStar_Click(object sender, EventArgs e)
        {
            sw.Start();
            switch (T)
            {
                case ShinyRange.BOX:

                    int i = SAV.CurrentBox;
                    shinyflag = Shinytype.Star;
                    SetBoxShiny(SAV, i);
                    break;
                case ShinyRange.All:
                    shinyflag = Shinytype.Star;
                    SetAllShiny(SAV);
                    break;
            }
            sw.Stop();
            MessageBox.Show($"搞定啦！用时：{sw.ElapsedMilliseconds}毫秒", "SuperWang");
            sw.Reset();
        }
        public static PKM ShinyFunctionPlus(PKM pkm)
        {
            PKM val = pkm.Clone();
            PKM va = pkm.Clone();
            bool EggFlag = val.IsEgg || val.WasEgg || val.IsTradedEgg || val.WasTradedEgg || val.MetLevel <= 1;
            if (VersionFlag.Gen1VCFlag(val.Version) || VersionFlag.Gen2VCFlag(val.Version))
            {
                pkm.PID = ShinyPID(val);
            }
            if (VersionFlag.Gen3Flag(val.Version) ||
                VersionFlag.Gen4Flag(val.Version) ||
                VersionFlag.Gen5Flag(val.Version) ||
                VersionFlag.CXDFlag(val.Version))
            {
                if (EggFlag)
                {

                    pkm.PID = ShinyPID(val);
                    if (pkm.Format != 3)
                    {
                        pkm.Ball = 4;
                    }
                    pkm.RefreshAbility((int)(pkm.PID & 1));
                    Span<int> abilityarray = stackalloc int[10]; ;
                    pkm.PersonalInfo.GetAbilities(abilityarray);
                    if (abilityarray[0] == abilityarray[1])
                    {
                        pkm.AbilityNumber = 1;
                    }
                    pkm.Nature = (Nature)(int)(pkm.PID % 25);
                    pkm.Gender = EntityGender.GetFromPID(pkm.Species, pkm.PID);
                    pkm.StatNature = pkm.Nature;
                    pkm.EncryptionConstant = pkm.PID;
                    pkm.RefreshChecksum();

                }
                if ((!EggFlag) && (!VersionFlag.CXDFlag(val.Version)))
                {
                    const int maxResults = LCRNG.MaxCountSeedsIV;
                    Span<uint> seeds = stackalloc uint[maxResults];
                    int count = 0;
                    if (shinyflag == Shinytype.RandomStar)
                    {
                        for (int i = 0; ; i++)
                        {
                            val.PID = ShinyPIDLite(val);
                            count = LCRNGReversal.GetSeeds(seeds, val.PID);
                            if (count == 0)
                                val.PID = Util.Rand32();
                            else
                                break;
                        }
                        var reg = seeds[..count];
                        int[] iv;
                        foreach (var seed in reg)
                        {
                            iv = LCRNGReversal.SetValuesFromSeedLCRNG(val, seed);
                            if (val.IsShiny)
                                val.IVs = iv;
                        }
                    }
                    else
                    {
                        pkm.SID16 = (ushort)ShinySID16Lite(val);
                    }
                    pkm.PID = val.PID;
                    pkm.IVs = val.IVs;
                    pkm.Gender = EntityGender.GetFromPID(pkm.Species, pkm.PID);
                    pkm.Nature = (Nature)(int)(pkm.PID % 25);
                    pkm.StatNature = pkm.Nature;
                    pkm.EncryptionConstant = pkm.PID;
                    pkm.RefreshAbility((int)(pkm.PID & 1));
                    Span<int> abilityarray = stackalloc int[10]; ;
                    pkm.PersonalInfo.GetAbilities(abilityarray);
                    if (abilityarray[0] == abilityarray[1])
                    {
                        pkm.AbilityNumber = 1;
                    }
                    pkm.RefreshChecksum();
                }
                if (VersionFlag.CXDFlag(val.Version) && !GiftAndStarter.XDCGFFlag(val.Species))
                {

                    const int maxResults = XDRNG.MaxCountSeedsIV;
                    Span<uint> seeds = stackalloc uint[maxResults];
                    int count = 0;
                    if (shinyflag == Shinytype.RandomStar)
                    {
                        for (int i = 0; ; i++)
                        {
                            val.PID = ShinyPIDLite(val);
                            count = XDRNGReversal.GetSeeds(seeds, val.PID);
                            if (count == 0)
                                val.PID = Util.Rand32();
                            var reg = seeds[..count];
                            int[] iv;
                            foreach (var seed in reg)
                            {
                                iv = XDRNGReversal.SetValuesFromSeedXDRNG(val, seed);
                                if (val.IsShiny)
                                    val.IVs = iv;
                            }
                            if (!val.IsShiny)
                            {
                                continue;
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        pkm.SID16 = (ushort)ShinySID16Lite(val);
                    }
                    pkm.PID = val.PID;
                    pkm.IVs = val.IVs;
                    pkm.Gender = EntityGender.GetFromPID(pkm.Species, pkm.PID);
                    pkm.Nature = (Nature)(int)(pkm.PID % 25);
                    pkm.EncryptionConstant = pkm.PID;
                    pkm.RefreshChecksum();
                }

            }
            if (VersionFlag.Gen6Flag(val.Version) ||
                VersionFlag.Gen7Flag(val.Version))
            {
                pkm.PID = ShinyPID(val);
                CommonEdits.SetRandomEC(pkm);
            }
            if (VersionFlag.Gen8SWSHFlag(val.Version))
            {
                pkm.PID = Util.Rand32();
                if (EggFlag || pkm.MetLocation == 162)
                {
                    pkm.PID = ShinyPID(val);
                    CommonEdits.SetRandomEC(pkm);
                }
                else if (Gen8MaxLairGodPool.MaxLairGodFlag(pkm.Species))
                {
                    pkm.PID = ShinyPID(val, 1);
                    CommonEdits.SetRandomEC(pkm);
                }
                else
                {
                    bool[] iv = new bool[] { false, false, false };
                    for (; ; )
                    {
                        uint seed = Util.Rand32();
                        if (shinyflag != Shinytype.Xor && Overworld8RNG.GenPkmQ(ref pkm, seed, ShinyArray(), iv))
                        {
                            pkm.RefreshChecksum();
                            break;
                        }
                        else if (shinyflag == Shinytype.Xor && Overworld8RNG.GenPkmQ(ref pkm, seed, ShinyArray(), iv, XorNumber))
                        {

                            pkm.RefreshChecksum();
                            break;
                        }
                        else
                            Overworld8RNG.Next(seed);
                    }
                }
            }
            if (VersionFlag.Gen8PLAFlag(val.Version))
            {
                pkm.PID = ShinyPID(val);
                CommonEdits.SetRandomEC(pkm);
            }
            if (VersionFlag.Gen8BDSPFlag(val.Version))
            {
                pkm.PID = ShinyPID(val);
                CommonEdits.SetRandomEC(pkm);
            }
            if (VersionFlag.Gen9Flag(val.Version))
            {
                pkm.PID = ShinyPID(val);
                CommonEdits.SetRandomEC(pkm);
            }

            var la = new LegalityAnalysis(pkm);
            if (!la.Valid)
            {
                pkm.TID16 = va.TID16;
                pkm.SID16 = va.SID16;
                pkm.PID = va.PID;
                pkm.IVs = va.IVs;
                pkm.Ability = va.Ability;
                pkm.AbilityNumber = va.AbilityNumber;
                pkm.Nature = va.Nature;
                pkm.StatNature = va.StatNature;
                pkm.Gender = va.Gender;
                pkm.EncryptionConstant = va.EncryptionConstant;
                if (pkm is IScaledSize s && va is IScaledSize p)
                {
                    s.HeightScalar = p.HeightScalar;
                    s.WeightScalar = p.WeightScalar;
                }
            }
            return pkm;
        }
        private void GEN1GEN2_SHING_Click(object sender, EventArgs e)
        {
            sw.Start();
            switch (T)
            {
                case ShinyRange.BOX:
                    int i = SAV.CurrentBox;
                    SAV.SAV.ModifyBoxes(GEN1GEN2SHING, i, i);
                    break;
                case ShinyRange.All:
                    SAV.SAV.ModifyBoxes(GEN1GEN2SHING);
                    break;
            }
            sw.Stop();
            MessageBox.Show($"搞定啦！用时：{sw.ElapsedMilliseconds}毫秒", "SuperWang");
            sw.Reset();
        }
        private static void GEN1GEN2SHING(PKM pkm)
        {
            pkm.IV_ATK = 15;
            pkm.IV_DEF = 10;
            pkm.IV_SPA = 10;
            pkm.IV_SPD = 10;
            pkm.IV_SPE = 10;
        }
    }
}