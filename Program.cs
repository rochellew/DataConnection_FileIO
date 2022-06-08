using Microsoft.Data.Sqlite;

namespace DataConnection
{
    class Program
    {
        // If you want to use this program on your machine, you'll need to change
        // this connection string to be the location of your SQLite db file :)
        const string CONNECTION_STRING = @"Data Source=C:\Users\whroc\Documents\SQLite\test.db";
        static void Main(string[] args)
        {
            //ReadUsers();

            var pokedex = ReadPokemonFromFile();
            Console.WriteLine("Pokemon read from file...now sorting by dex number.");

            pokedex.Sort();
            Console.WriteLine("\n=====================================================");
            Console.WriteLine("Here's a list of all your Pokemon (read from file)!");
            Console.WriteLine("=====================================================");
            foreach (Pokemon p in pokedex)
            {
                Console.WriteLine(p.ToString());
                Console.WriteLine("-----------------------------");
            }

            // clear the db of all pokemon to prevent SQLite errors for duplicate pokemon
            ClearPokemonFromDB();

            // create a pokemon object to add to the database
            Pokemon charizard = new Pokemon(6, "Charizard", 99, "Fire", "Flying");
            AddPokemonToDB(charizard);

            // OR using anonymous variables like below
            // AddPokemonToDB(new Pokemon(6, "Charizard", 99, "Fire", "Flying"));

            // add all pokemon from the list to the db using a loop
            Console.WriteLine("\n\nAdding pokemon to the database....hang tight!");
            foreach(Pokemon p in pokedex)
            {
                AddPokemonToDB(p);
            }
            Console.WriteLine("Pokemon successfully added. Now reading from DB...\n");

            Console.WriteLine("\n======================================================================");
            Console.WriteLine("Here's a list of all your Pokemon (read from database (name desc))!");
            Console.WriteLine("======================================================================");
            ReadAllPokemonFromDB();

            Console.Write("\nEnter the number of a Pokemon you'd like to see:");
            var dexNumber = Convert.ToInt32(Console.ReadLine());
            ReadPokemonFromDBByNumber(dexNumber);
        }

        static void ReadUsers()
        {
            Console.Write("Enter the ID of the user you would like to see: ");
            var id = Convert.ToInt32(Console.ReadLine());

            // set up the SQLite connection to test.db (your connection string will be different)
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                // open the connection
                connection.Open();

                // create the command
                var command = connection.CreateCommand();

                // add text to command
                command.CommandText =
                @"
                    SELECT First_Name, Last_Name
                    FROM users
                    WHERE id = $id;
                ";

                command.Parameters.AddWithValue("$id", id);

                // we now want to use the command to read data FROM the database

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var firstName = reader.GetString(0);
                            var lastName = reader.GetString(1);
                            Console.WriteLine($"The user's name that you selected is {firstName} {lastName}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }

        static List<Pokemon> ReadPokemonFromFile()
        {
            var pokedex = new List<Pokemon>();
            using (StreamReader reader = new StreamReader(ProjectRoot.Root + @"\Pokemon.csv"))
            {
                try
                {
                    while(!reader.EndOfStream)
                    {
                        // read a line from the file
                        var line = reader.ReadLine();

                        // split that string on a delimiter (csv's have comma delimiters)
                        string[] elements = line.Split(",");

                        pokedex.Add(new Pokemon(
                            Convert.ToInt32(elements[0]),
                            elements[1],
                            Convert.ToInt32(elements[2]),
                            elements[3],
                            elements[4]
                        ));
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            } //end using

            return pokedex;
        }

        static void AddPokemonToDB(Pokemon p)
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                // open connection
                connection.Open();

                // command text in variable  to keep code ~pretty~
                var cmdText = @"INSERT INTO pokemon 
                                VALUES($num, $name, $lvl, $pt, $st)";

                // create the command
                using (var command = new SqliteCommand(cmdText, connection))
                {
                    // optional -- may help with faster transaction
                    command.CommandType = System.Data.CommandType.Text;

                    // we now need to add in the values to the parameters in our cmdText
                    command.Parameters.AddWithValue("$num", p.DexNumber).SqliteType = SqliteType.Integer;
                    command.Parameters.AddWithValue("$name", p.Name).SqliteType = SqliteType.Text;
                    command.Parameters.AddWithValue("$lvl", p.Level).SqliteType = SqliteType.Integer;
                    command.Parameters.AddWithValue("$pt", p.PrimaryType).SqliteType = SqliteType.Text;
                    command.Parameters.AddWithValue("$st", p.SecondaryType).SqliteType = SqliteType.Text;

                    // so far, we've created and fleshed out our command,
                    // but we still haven't run it. Let's do that.

                    // "nonquery" refers to anything besides a read operation
                    // think of queries like asking the database for data
                    command.ExecuteNonQuery();
                }
            }
        }

        static void ClearPokemonFromDB()
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();

                using(var command = new SqliteCommand(@"DELETE FROM pokemon;", connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        static void ReadAllPokemonFromDB()
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT name FROM pokemon ORDER BY name DESC;";
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }
                }
            }
        }

        static void ReadPokemonFromDBByNumber(int dexNumber)
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT name, level, primary_type, secondary_type
                                        FROM pokemon
                                        WHERE dex_number = $dexNumber";

                command.Parameters.AddWithValue("$dexNumber", dexNumber);

                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        Console.WriteLine("We found that Pokemon! Here is its information:\n");
                        Console.WriteLine($"Name:\t{reader.GetString(0)}");
                        Console.WriteLine($"Level:\t{reader.GetString(1)}");
                        Console.WriteLine($"Primary Type:\t{reader.GetString(2)}");
                        Console.WriteLine($"Secondary Type:\t{reader.GetString(3)}");
                    }

                    if(reader.Read() == false)
                    {
                        Console.WriteLine("\nSorry...we couldn't find that Pokemon. Try again!");
                    }
                }    
            }
        }
    }
}