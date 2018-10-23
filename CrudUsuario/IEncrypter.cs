namespace CrudUsuario
{
    public interface IEncrypter
    {
        string GetHash(string value, string salt);
        string GetSalt();
    }
}