using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.OrderAggregate;

[Owned]
public class ShippingAddress
{
    public required string BuildingNumber { get; set; }
    public required string Street { get; set; }
    public required string District { get; set; }
    public required string Government { get; set; }

    [JsonPropertyName("postal_code")]
    public  required string PostalCode { get; set; }

    public required string Country { get; set; }
    public string? LandMark { get; set; }

}
