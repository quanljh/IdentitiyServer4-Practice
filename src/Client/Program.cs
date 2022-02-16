using IdentityModel.Client;

var client = new HttpClient();
var discoveryDoc = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
if (discoveryDoc.IsError)
{
    Console.WriteLine(discoveryDoc.Error);
    return;
}

var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = discoveryDoc.TokenEndpoint,
    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api2"
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}

Console.WriteLine(tokenResponse.Json);

var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);

var response = await apiClient.GetAsync("https://localhost:6001/identity");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(content);
}