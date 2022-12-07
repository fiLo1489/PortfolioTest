using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace SemestralnaPracaTest
{
    public class Data
    {
        private Random random;
        private SqlConnection connection;

        public Data()
        {
            random = new Random();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Database"].ToString());
            connection.Open();
        }

        public int GetRandomNumber(int range)
        { 
            return (random.Next() % range);
        }

        public string GetRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string GetRandomDate(string table)
        {
            string date;

            do
            {
                date = DateTime.Now.AddDays(GetRandomNumber(90)).ToString("MM-dd-yyyy");
            }
            while (!CheckDate(table, date));

            return date;
        }

        public void CloseConnection()
        { 
            connection.Close();
        }

        private bool CheckDate(string table, string date)
        {
            bool valid = false;
            string query = "select count(*) from " + table + " where SCHEDULED = CONVERT(VARCHAR(10), '" + date + "', 110);";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                valid = (Convert.ToInt32(cmd.ExecuteScalar()) != 0 ? false : true);
            }

            return valid;
        }
    }
}