public class ShipmentRequest
{
    public string ReferenceIdentifier { get; set; }
    public RecipientInfo Recipient { get; set; }
    public ShipmentOptions Options { get; set; }
    public int Carrier { get; set; }
}

public class RecipientInfo
{
    public string CountryCode { get; set; }  // cc
    public string Region { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string Number { get; set; }
    public string PostalCode { get; set; }
    public string Person { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}

public class ShipmentOptions
{
    public int PackageType { get; set; }
    public int OnlyRecipient { get; set; }
    public int Signature { get; set; }
    public int Return { get; set; }
    public InsuranceInfo Insurance { get; set; }
    public int LargeFormat { get; set; }
    public string LabelDescription { get; set; }
    public int AgeCheck { get; set; }
}

public class InsuranceInfo
{
    public int Amount { get; set; }
    public string Currency { get; set; }
}
