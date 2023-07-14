using Streamiz.Kafka.Net.Crosscutting;

namespace WebApplication1.Controllers
{
    static class EnrichedOrderBuilder
    {
        public static EnrichedOrder Build(CustomerOrder customerOrder, Product product)
        {
            EnrichedOrder enrichedOrder = new EnrichedOrder();
            enrichedOrder.OrderId = customerOrder.Order.order_id;
            enrichedOrder.OrderTime = customerOrder.Order.ts.FromMilliseconds();
            enrichedOrder.CustomerId = customerOrder.Customer.id;
            enrichedOrder.CustomerEmail = customerOrder.Customer.email;
            enrichedOrder.CustomerFirstName = customerOrder.Customer.first_name;
            enrichedOrder.CustomerLastName = customerOrder.Customer.last_name;
            enrichedOrder.CustomerPhone = customerOrder.Customer.phone;
            enrichedOrder.ProductId = product.id;
            enrichedOrder.ProductPrice = product.sale_price;
            enrichedOrder.ProductName = product.name;
            return enrichedOrder;
        }
    }

    public class CustomerOrder
    {
        private readonly Customer customer;
        private readonly Order order;

        public CustomerOrder(Customer customer, Order order)
        {
            this.customer = customer;
            this.order = order;
        }

        public Customer Customer => customer;

        public Order Order => order;
    }

    public class EnrichedOrder
    {
        public int OrderId { get; set; }
        public String ProductId { get; set; }
        public String ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public String CustomerId { get; set; }
        public DateTime OrderTime { get; set; }
        public String CustomerFirstName { get; set; }
        public String CustomerLastName { get; set; }
        public String CustomerEmail { get; set; }
        public String CustomerPhone { get; set; }
    }

    public class Order
    {
        public Order(int orderId, string productId, string customerId)
        {
            order_id = orderId;
            product_id = productId;
            customer_id = customerId;
            ts = DateTime.UtcNow.GetMilliseconds();
        }
        public int order_id { get; set; }
        public string product_id { get; set; }
        public string customer_id { get; set; }
        public long ts { get; set; }
    }

    public class Product
    {
        public static Product Create()
        {
            return new Product()
            {
                id = Guid.NewGuid().ToString(),
                name = ProductNameGenerator.GenerateRandomProductName(),
                sale_price = ProductPriceGenerator.GenerateRandomProductPrice(1000, 100000)
            };
        }
        public string id { get; set; }
        public string name { get; set; }
        public decimal sale_price { get; set; }
    }

    public class Customer
    {
        public static Customer Create()
        {
            var countryData = CountryGenerator.GenerateRandomCountry();
            return new Customer()
            {
                id = Guid.NewGuid().ToString(),
                country = countryData,
                country_code = countryData.Substring(0, 2).ToUpper(),
                email = EmailGenerator.GenerateRandomEmail(),
                first_name = FirstNameGenerator.GenerateRandomFirstName(),
                last_name = LastNameGenerator.GenerateRandomLastName(),
                phone = PhoneNumberGenerator.GenerateRandomPhoneNumber(),
            };
        }
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
    }
}