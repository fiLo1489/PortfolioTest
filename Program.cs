namespace SemestralnaPracaTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();

            test.LoginLogoutWithCorrectCredentialsUser();
            test.ChangeCredentialsUser(null);
            test.ChangeCredentialsUser(false);
            test.RegisterCorrectCredentialstUser();
            // TODO admin nastavenie admin prav
            // TODO user kontrola ci je admin
            test.DeleteUserAdmin();

            test.SendRequestUser(false);
            test.EditRequestUser();
            test.DeleteRequestUser();
            test.SendRequestUser(true);
            // TODO admin uprava a schvalenie requestu
            // TODO user nemoze upravovat ani mazat schvaleny request
            // TODO admin vymazanie requestu

            // TODO pridanie nevalidnej fotografie
            // TODO pridanie validnej fotografie
            // TODO vymazanie fotografie

            test.End();
        }
    }
}