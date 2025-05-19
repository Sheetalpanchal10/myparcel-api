
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


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
        //return ContentResult;
    }
    [HttpPost]
   public async Task<IActionResult> CreateShipment([FromBody] ShipmentRequest input)
{
    try
    {
        // Build your shipment object
        var shipmentPayload = new
        {
            data = new
            {
                shipments = new[]
                {
                    new
                    {
                        reference_identifier = input.ReferenceIdentifier,
                        recipient = new
                        {
                            cc = input.Recipient.CountryCode,
                            region = input.Recipient.Region,
                            city = input.Recipient.City,
                            street = input.Recipient.Street,
                            number = input.Recipient.Number,
                            postal_code = input.Recipient.PostalCode,
                            person = input.Recipient.Person,
                            phone = input.Recipient.Phone,
                            email = input.Recipient.Email
                        },
                        options = new
                        {
                            package_type = input.Options.PackageType,
                            only_recipient = input.Options.OnlyRecipient,
                            signature = input.Options.Signature,
                            @return = input.Options.Return,
                            insurance = new
                            {
                                amount = input.Options.Insurance.Amount,
                                currency = input.Options.Insurance.Currency
                            },
                            large_format = input.Options.LargeFormat,
                            label_description = input.Options.LabelDescription,
                            age_check = input.Options.AgeCheck
                        },
                        carrier = input.Carrier
                    }
                }
            }
        };

        // Serialize the shipment manually
        var json = JsonSerializer.Serialize(shipmentPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/vnd.shipment+json");

        // Custom content-type header
        content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.shipment+json")
        {
            CharSet = "utf-8",
            Parameters = { new NameValueHeaderValue("version", "1.1") }
        };

        var client = _httpClientFactory.CreateClient();

        // Set custom headers
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "BASE64_ENCODED_API_KEY");
        client.DefaultRequestHeaders.UserAgent.ParseAdd("CustomApiCall/2");

        var response = await client.PostAsync("https://api.myparcel.nl/shipments", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                Status = "Error",
                MyParcelResponse = responseBody
            });
        }

        return Ok(new
        {
            Status = "Success",
            Message = "Shipment created successfully!",
            MyParcelResponse = responseBody
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