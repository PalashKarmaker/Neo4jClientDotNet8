using Neo4j.Driver;
using System;
using System.Threading.Tasks;

namespace Neo4jClient;

internal class DriverWrapper : IDriver
{
    private readonly IDriver driver;
    public string Username { get; }
    public string Password { get; }
    public string Realm { get; }
    public EncryptionLevel? EncryptionLevel { get; }

    public DriverWrapper(IDriver driver) => this.driver = driver;

    public DriverWrapper(string uri, IServerAddressResolver addressResolver, string username, string pass, string realm, EncryptionLevel? encryptionLevel)
        : this(new Uri(uri), addressResolver, username, pass, realm, encryptionLevel)
    {
    }

    public DriverWrapper(Uri uri, IServerAddressResolver addressResolver, string username, string pass, string realm, EncryptionLevel? encryptionLevel)
    {
        Uri = uri;
        Username = username;
        Password = pass;
        Realm = realm;
        EncryptionLevel = encryptionLevel;

        var authToken = GetAuthToken(username, pass, realm);
        if (addressResolver != null)
        {
            driver = encryptionLevel == null
                ? GraphDatabase.Driver(uri, authToken, builder => builder.WithResolver(addressResolver))
                : GraphDatabase.Driver(uri, authToken, builder => builder.WithResolver(addressResolver).WithEncryptionLevel(encryptionLevel.Value));
        }
        else
        {
            driver = GraphDatabase.Driver(uri, authToken);
        }
    }

    public IAsyncSession AsyncSession() => driver.AsyncSession();

    public IAsyncSession AsyncSession(Action<SessionConfigBuilder> action) => driver.AsyncSession(action);

    public Task CloseAsync() => driver.DisposeAsync().AsTask();

    /// <inheritdoc />
    public Task<IServerInfo> GetServerInfoAsync() => driver.GetServerInfoAsync();

    public Task VerifyConnectivityAsync() => driver.VerifyConnectivityAsync();

    public Task<bool> SupportsMultiDbAsync() => driver.SupportsMultiDbAsync();

    public Config Config => driver.Config;

    /// <inheritdoc />
    public bool Encrypted => driver.Encrypted;

    public Uri Uri { get; private set; }

    public IServerAddressResolver AddressResolver => Config.Resolver;

    private static IAuthToken GetAuthToken(string username, string password, string realm) => 
        string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
            ? AuthTokens.None
            : AuthTokens.Basic(username, password, realm);

    public void Dispose() => driver?.Dispose();

    public ValueTask DisposeAsync() => driver.DisposeAsync();

    public Task<bool> TryVerifyConnectivityAsync() => driver.TryVerifyConnectivityAsync();
}