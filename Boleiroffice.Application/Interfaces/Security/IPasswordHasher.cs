namespace Boleiroffice.Application.Interfaces.Security;

public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verify(string senha, string senhaHash);
}
