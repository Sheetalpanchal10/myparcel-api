using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class SapService
{
    private readonly IHttpClientFactory _factory;
    private readonly string _baseUrl = "https://YOUR_CLOUD_URL/b1s/v1/";

    private readonly string _companyDb = "YOUR_COMPANY_DB";
    private readonly string _username = "YOUR_USERNAME";
    private readonly string _password = "YOUR_PASSWORD";

    public SapService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<string> LoginAsync()
    {
        var client = _factory.CreateClient();

        var loginPayload = new
        {
            CompanyDB = _companyDb,
            UserName = _username,
            Password = _password
        };

        var content = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");

        var res = await client.PostAsync(_baseUrl + "Login", content);
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception("SAP Login Failed: " + body);

        var cookieHeader = res.Headers.GetValues("Set-Cookie")
            .FirstOrDefault(x => x.Contains("B1SESSION"));

        if (string.IsNullOrEmpty(cookieHeader))
            throw new Exception("Could not retrieve B1SESSION cookie from SAP login.");

        return cookieHeader.Split(';')[0];
    }

    public async Task<string> GetDeliveryAsync(string docNum, string sessionCookie)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var res = await client.GetAsync(_baseUrl + $"DeliveryNotes?$filter=DocNum eq {docNum}");
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception("SAP Delivery Fetch Error: " + body);

        return body;
    }

    public async Task<string> UpdateDeliveryUdfAsync(string docNum, string shipmentNumber, string sessionCookie)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);

        var payload = new
        {
            U_MyParcelNo = shipmentNumber
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var res = await client.PatchAsync(_baseUrl + $"DeliveryNotes({docNum})", content);
        return await res.Content.ReadAsStringAsync();
    }
}
