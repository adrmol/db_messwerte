using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace db_messwerte
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<List<string>>();

            using (var reader = new StreamReader("2023-02-17.dat"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',').ToList();
                    list.Add(values);
                }
            }


            // Daten für das Einfügen
            foreach (var v in list)
            {
                string t_fuehler = v[0];
                string datum = v[1];
                string temperatur = v[2];

                // Verbindung zur MySQL-Datenbank herstellen
                string connectionString = "Server=localhost;Database=messwerte;Uid=root;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "CREATE DATABASE IF NOT EXISTS messwerte";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE IF NOT EXISTS messungen(
                            id int AUTO_INCREMENT PRIMARY KEY NOT NULL,
                            temperaturfuehler varchar(32) NOT NULL,
                            datum DATETIME NOT NULL,
                            temperatur float)";
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO messungen (temperaturfuehler, datum, temperatur) VALUES (@tf, @d, @t);";
                        command.Parameters.AddWithValue("@tf", t_fuehler);
                        command.Parameters.AddWithValue("@d", datum);
                        command.Parameters.AddWithValue("@t", temperatur);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
