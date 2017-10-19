using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLiteTest
{
    class Program
    {
        private const string ConnectionString = @"Data Source=c:\Transfer\DocumentMgt.db;Version=3;";


        static void Main(string[] args)
        {
            //SelectDocuments();
            //InsertDocument(@"C:\TEMP\tumblr_lv4nzjCm8D1r2vji3o1_1280.png");
            GetDocumentPicture(@"C:\TEMP\output");
        }

        private static void SelectDocuments()
        {
            SQLiteConnection connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM Document";

            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"Entry No: {reader[0]}");
            }
            reader.Close();
            connection.Close();
            Console.ReadLine();
        }

        private static void InsertDocument(string fileName)
        {
            byte[] docData = GetDocumentDataFromFile(fileName);

            SQLiteConnection connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = connection;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add("@DocumentEntryNo", System.Data.DbType.Int32);
            cmd.Parameters.Add("@EntryNo", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Data", System.Data.DbType.Binary, docData.Length);

            cmd.CommandText = "INSERT INTO Document_Files(DocumentEntryNo,EntryNo,Data) VALUES(@DocumentEntryNo, @EntryNo, @Data);";
            cmd.Parameters["@DocumentEntryNo"].Value = 1;
            cmd.Parameters["@EntryNo"].Value = 1;
            cmd.Parameters["@Data"].Value = docData;

            cmd.ExecuteNonQuery();
        }

        private static byte[] GetDocumentDataFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException($"{nameof(fileName)} must be set");
            if (!File.Exists(fileName))
                throw new IOException($"File {fileName} does not exist");

            return File.ReadAllBytes(fileName);
        }

        public static void GetDocumentPicture(string directory)
        {
            SQLiteConnection connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = connection;
            cmd.CommandType = System.Data.CommandType.Text;

            cmd.CommandText = "SELECT DocumentEntryNo,EntryNo,Data FROM Document_Files";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                string targetFileName = Path.Combine(directory, Guid.NewGuid().ToString() + ".png");
                File.WriteAllBytes(targetFileName, (byte[])reader[2]);
            }
            reader.Close();
            connection.Close();       

        }
    }
}
