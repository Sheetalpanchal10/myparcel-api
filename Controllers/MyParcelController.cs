
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class MyParcelController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MyParcelController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("MyParcel API is running ✅");
    }
    [HttpPost]
    public async Task<IActionResult> CreateShipment([FromBody] ShipmentRequest input)
    {
        try
        {
            var shipment = new
            {
                data = new[]
                {
                new
                {
                    recipient = new
                    {
                        name = input.RecipientName,
                        street = input.Street,
                        postal_code = input.PostalCode,
                        city = input.City,
                        country_code = input.CountryCode,
                        email = input.Email
                    }
                }
            }
            };

            var client = _httpClientFactory.CreateClient();
            //client.DefaultRequestHeaders.Add("Authorization", "your-api-key-here");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer MGUxYzE0ODFlOTIzZDExZWRhNGQzZGI5ZmVkNGMwNGEyMWNhZDVjNg");
           // client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            var response = await client.PostAsJsonAsync("https://api.myparcel.nl/shipments", shipment);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    Status = "Error",
                    MyParcelResponse = content
                });
            }

            return Ok(new
            {
                Status = "Success",
                Message = "Shipment received successfully!",
                MyParcelResponse = content
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "Failure",
                Error = ex.Message
            });
        }
    }
}