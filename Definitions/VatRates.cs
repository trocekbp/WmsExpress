namespace WmsCore.Definitions
{
        public static class VatRates
        {
            public const string Stawka23 = "23%";
            public const string Stawka8 = "8%";
            public const string Stawka5 = "5%";
            public const string Stawka0 = "0%";
            public const string Zwolniony = "ZW";
            public const string NiePodlega = "NP";

            public static readonly Dictionary<string, decimal> Map = new()
        {
            { Stawka23, 0.23m },
            { Stawka8,  0.08m },
            { Stawka5,  0.05m },
            { Stawka0,  0.00m },
            { Zwolniony, 0.00m },
            { NiePodlega, 0.00m }
        };


        public static decimal GetMultiplier(string symbol)
            {
                //Domyślna stawka 23% VAT
                if (string.IsNullOrEmpty(symbol)) return 0.23m; 
                return Map.TryGetValue(symbol, out var val) ? val : 0.23m;
            }
            public static List<string> GetSymbols()
            {
                return Map.Keys.ToList();
            }
        }
    }
