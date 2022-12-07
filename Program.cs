namespace SemestralnaPracaTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();
            Data data = new Data();

            //test.LoginLogoutWithCorrectCredentialsUser();
            //test.ChangeCredentialsUser(null);
            //test.ChangeCredentialsUser(false);
            test.SendRequestUser(data.GetRandomNumber(5), data.GetRandomText(data.GetRandomNumber(100)), data.GetRandomDate("REQUESTS"));

            test.SaveReport();
            data.CloseConnection();
        }
    }
}