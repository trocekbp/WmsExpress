namespace WmsCore.Definitions
{
    public static class Units
    {
        //Ilość
        public const string Sztuka = "szt.";

        //Masa
        public const string Tona = "t";
        public const string Kilogram = "kg";
        public const string Gram = "g";

        //Objętość
        public const string Litr = "l";
        public const string Mililitr = "ml";
        public const string M2 = "m2";

        //Długość
        public const string Metr = "m";
        public const string Centymetr = "cm";
        public const string Milimetr = "mm";

        public static readonly List<string> unit_list = new List<string>() { 
            Sztuka, Tona, Kilogram, Gram, Litr, Milimetr
        };

        public static bool IsDecimal(string unit) {
            var parsed_unit = unit.ToLower();

            if (parsed_unit.Equals(Sztuka))
            {
                return false;
            }
            else { 
                return true;
            }
        }
        public static List<string> GetAllUnits() {
            return unit_list;
        }
    }


}
