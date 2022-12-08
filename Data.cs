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

        public DateTime GetRandomNextDate()
        { 
            return DateTime.Now.AddDays(GetRandomNumber(90));
        }

        public DateTime GetRandomPastDate()
        { 
            return DateTime.Now.AddDays(-GetRandomNumber(90));
        }

        public DateTime GetRandomRequestDate()
        {
            DateTime date;

            do
            {
                date = GetRandomNextDate();
            }
            while (!CheckRequestDate(date));

            return date;
        }

        public void CloseConnection()
        { 
            connection.Close();
        }

        public bool CheckRequestDate(DateTime date)
        {
            bool valid = false;
            string query = "select count(*) from REQUESTS where SCHEDULED = '" + date.ToString("yyyy-MM-dd") + "'";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                valid = (Convert.ToInt32(cmd.ExecuteScalar()) != 0 ? false : true);
            }

            return valid;
        }
    }
}