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
            test.RegisterCorrectCredentialsUser();
            test.CannotRegisterTwiceUser();
            test.SetAdminRoleAdmin();
            test.CheckStatisticsAdmin();
            test.DeleteUserAdmin();
            test.SendRequestUser();
            test.EditRequestUser();
            test.FinishRequestAdmin();
            test.CheckRequestUser();
            test.DeleteRequestAdmin();

            test.End();
        }
    }
}