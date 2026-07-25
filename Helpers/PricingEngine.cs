using System;

public static class PricingEngine
{
    public static decimal GetPrice(string category, string productName)
    {
        category = category.ToLower();
        productName = productName.ToLower();

        decimal basePrice = category switch
        {
            "shoes" => 450,
            "women" => 300,
            "men" => 320,
            "bags & luggages" => 500,
            "toys" => 150,
            "appliances" => 600,
            "jewellery & accessories" => 200,
            _ => 250
        };

        // premium boost keywords
        if (productName.Contains("leather") ||
            productName.Contains("premium") ||
            productName.Contains("designer"))
        {
            basePrice *= 1.5m;
        }

        // cheap items
        if (productName.Contains("mini") ||
            productName.Contains("basic"))
        {
            basePrice *= 0.7m;
        }

        return Math.Round(basePrice + RandomVariance(), 2);
    }

    private static decimal RandomVariance()
    {
        var rand = new Random();
        return rand.Next(-30, 80);
    }
}