namespace Domain
{
    public static class LoginExtensions
    {
        extension(Login login)
        {
            public void ResetCounter()
            {
                login.WrongLoginAttemptsCounter = 0;
                login.LastLoginDate = DateTime.UtcNow;
            }
        }
    }
}