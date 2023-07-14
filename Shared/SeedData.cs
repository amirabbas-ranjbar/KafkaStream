namespace Shared;

public  class CountryGenerator
{
    private static readonly Random random = new Random();

    private static readonly List<string> countryList = new List<string>
    {
        "Iran",  "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Antigua and Barbuda",
    };

    public static string GenerateRandomCountry()
    {
        int randomIndex = random.Next(countryList.Count);
        return countryList[randomIndex];
    }
}
public  class EmailGenerator
{
    private static readonly Random random = new Random();

    public static string GenerateRandomEmail()
    {
        string[] domains = { "gmail.com", "yahoo.com", "hotmail.com", "example.com" }; // Add more domains as needed

        string randomString = Path.GetRandomFileName().Replace(".", "");
        string domain = domains[random.Next(domains.Length)];

        return $"{randomString}@{domain}";
    }
}
public  class FirstNameGenerator
{
    private static readonly Random random = new Random();

    private static readonly string[] firstNames = { "John", "Emma", "Michael", "Sophia", "Robert", "Olivia", "William", "Ava" }; // Add more first names as needed

    public static string GenerateRandomFirstName()
    {
        int randomIndex = random.Next(firstNames.Length);
        return firstNames[randomIndex];
    }
}
public class LastNameGenerator
{
    private static readonly Random random = new Random();

    private static readonly string[] lastNames = { "Smith", "Johnson", "Brown", "Taylor", "Miller", "Wilson", "Clark", "Jones" }; // Add more last names as needed

    public static string GenerateRandomLastName()
    {
        int randomIndex = random.Next(lastNames.Length);
        return lastNames[randomIndex];
    }
}

public class PhoneNumberGenerator
{
    private static readonly Random random = new Random();

    public static string GenerateRandomPhoneNumber()
    {
        string phoneNumber = "0";

        for (int i = 0; i < 10; i++)
        {
            phoneNumber += random.Next(0, 10);
        }

        return phoneNumber;
    }
}
public class ProductNameGenerator
{
    private static readonly Random random = new Random();

    private static readonly string[] adjectives = { "Cool", "Amazing", "Fantastic", "Incredible", "Awesome", "Wonderful", "Unique", "Elegant" };
    private static readonly string[] nouns = { "Product", "Item", "Gadget", "Device", "Tool", "Widget", "Appliance", "Accessory" };

    public static string GenerateRandomProductName()
    {
        string adjective = adjectives[random.Next(adjectives.Length)];
        string noun = nouns[random.Next(nouns.Length)];

        return $"{adjective} {noun}";
    }
}public class ProductPriceGenerator
{
    private static readonly Random random = new Random();

    public static decimal GenerateRandomProductPrice(decimal minPrice, decimal maxPrice)
    {
        if (minPrice >= maxPrice)
            throw new ArgumentException("Minimum price must be less than maximum price.");

        decimal randomPrice = (decimal)(random.NextDouble() * (double)(maxPrice - minPrice)) + minPrice;
        return Math.Round(randomPrice, 2);
    }
}