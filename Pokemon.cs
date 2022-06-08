namespace DataConnection
{
    internal class Pokemon : IComparable
    {
        // properties of a Pokemon object
        public int DexNumber { get; init; }
        public string Name { get; init; } = String.Empty;
        public int Level { get; init; }
        public string PrimaryType { get; init; } = String.Empty;
        public string SecondaryType { get; init; } = String.Empty;

        // constructor
        public Pokemon(int dexNumber, string name, int level, string primaryType, string secondaryType)
        {
            DexNumber = dexNumber;
            Name = name;
            Level = level;
            PrimaryType = primaryType;
            SecondaryType = secondaryType;
        }

        // string representation of a Pokemon object
        public override string ToString()
        {
            return
                $"Number:\t\t{DexNumber}\n" +
                $"Name:\t\t{Name}\n" +
                $"Level:\t\t{Level}\n" +
                $"Primary Type:\t{PrimaryType}\n" +
                $"Secondary Type:\t{SecondaryType}";
        }

        public int CompareTo(object? obj)
        {
            Pokemon comparedPokemon = (Pokemon)obj;
            if (comparedPokemon.DexNumber > this.DexNumber)
                return -1;
            else if (comparedPokemon.DexNumber == this.DexNumber)
                return 0;
            else
                return 1;
        }
    }
}
