
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
        //return ContentResult;
    }
    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> CreateShipment([FromBody] ShipmentRequest input)
    {
        try
        {
            var shipment = new
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

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_MY_PARCEL_API_KEY");

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
                Message = "Shipment created successfully!",
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