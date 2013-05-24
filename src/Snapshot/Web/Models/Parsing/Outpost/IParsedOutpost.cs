using System;
namespace Web.Models.Parsing.Outpost
{
    public interface IParsedOutpost
    {
        string Country { get; set; }
        string ContactDetail { get; set; }
        string District { get; set; }
        string Latitude { get; set; }
        string Longitude { get; set; }
        string Region { get; set; }
        string Name { get; set; }
    }
}
