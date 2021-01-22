namespace UspsOpenApi.Models.RateAPI.Content
{
    public static class ContentType
    {
        public const string Hazmat = "HAZMAT",
            CrematedRemains = "CREMATEDREMAINS",
            Fragile = "FRAGILE",
            Perishable = "PERISHABLE",
            Pharmaceuticals = "PHARMACEUTICALS",
            MedicalSupplies = "MEDICAL SUPPLIES",
            Lives = "LIVES";
    }

    public static class ContentDescription
    {
        public const string Bees = "BEES",
            DayOldPoultry = "DAYOLDPOULTRY",
            AdultBirds = "ADULTBIRDS",
            Other = "OTHER";
    }
}
