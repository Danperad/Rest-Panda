namespace RestPanda.Sample;

public class User
{
    public User()
    {
        Login = Password = string.Empty;
    }
    public User(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public string Login { get; set; }
    public string Password { get; set; }
}