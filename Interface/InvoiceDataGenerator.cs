using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revmax_Interface_Promun
{
    
        // List of products offered by town councils
        public class InvoiceDataGenerator
        {
        // List of products offered by town councils
        private readonly List<Product> Products = new List<Product>
        {
            // Fruits & Vegetables
            new Product { Code = "PRD001", Name = "Apple (1 kg)", Price = 3.50m },
            new Product { Code = "PRD002", Name = "Banana (1 kg)", Price = 2.00m },
            new Product { Code = "PRD003", Name = "Carrots (1 kg)", Price = 1.80m },
            new Product { Code = "PRD004", Name = "Tomatoes (1 kg)", Price = 2.50m },

            // Dairy
            new Product { Code = "PRD005", Name = "Milk (1 L)", Price = 1.50m },
            new Product { Code = "PRD006", Name = "Cheddar Cheese (500 g)", Price = 4.50m },
            new Product { Code = "PRD007", Name = "Yogurt (500 g)", Price = 3.00m },

            // Meat & Poultry
            new Product { Code = "PRD008", Name = "Chicken Breast (1 kg)", Price = 8.00m },
            new Product { Code = "PRD009", Name = "Beef Mince (1 kg)", Price = 9.50m },
            new Product { Code = "PRD010", Name = "Bacon (200 g)", Price = 5.00m },

            // Beverages
            new Product { Code = "PRD011", Name = "Orange Juice (1 L)", Price = 4.20m },
            new Product { Code = "PRD012", Name = "Coca-Cola (2 L)", Price = 2.50m },
            new Product { Code = "PRD013", Name = "Bottled Water (500 ml)", Price = 1.00m },
            new Product { Code = "PRD014", Name = "Green Tea (box of 20)", Price = 3.20m },
            new Product { Code = "PRD015", Name = "Coffee (200 g)", Price = 5.80m },

            // Bakery
            new Product { Code = "PRD016", Name = "Bread (1 loaf)", Price = 2.20m },
            new Product { Code = "PRD017", Name = "Croissant (pack of 6)", Price = 4.50m },
            new Product { Code = "PRD018", Name = "Muffins (pack of 4)", Price = 3.80m },

            // Snacks
            new Product { Code = "PRD019", Name = "Potato Chips (150 g)", Price = 2.00m },
            new Product { Code = "PRD020", Name = "Chocolate Bar (100 g)", Price = 1.50m },
            new Product { Code = "PRD021", Name = "Cookies (200 g)", Price = 2.80m },

            // Household & Cleaning Supplies
            new Product { Code = "PRD022", Name = "Laundry Detergent (1.5 L)", Price = 6.50m },
            new Product { Code = "PRD023", Name = "Dishwashing Liquid (500 ml)", Price = 2.00m },
            new Product { Code = "PRD024", Name = "Paper Towels (pack of 2)", Price = 3.00m },

            // Toiletries & Personal Care
            new Product { Code = "PRD025", Name = "Toothpaste (100 ml)", Price = 2.50m },
            new Product { Code = "PRD026", Name = "Shampoo (500 ml)", Price = 4.00m },
            new Product { Code = "PRD027", Name = "Body Wash (500 ml)", Price = 3.50m },

            // Frozen Foods
            new Product { Code = "PRD028", Name = "Frozen Pizza (500 g)", Price = 6.00m },
            new Product { Code = "PRD029", Name = "Frozen Peas (1 kg)", Price = 2.80m },
            new Product { Code = "PRD030", Name = "Ice Cream (1 L)", Price = 5.50m },

            // Dry Goods & Staples
            new Product { Code = "PRD031", Name = "Rice (1 kg)", Price = 2.80m },
            new Product { Code = "PRD032", Name = "Pasta (500 g)", Price = 1.50m },
            new Product { Code = "PRD033", Name = "Cereal (500 g)", Price = 3.80m },
            new Product { Code = "PRD034", Name = "Sugar (1 kg)", Price = 2.20m },
            new Product { Code = "PRD035", Name = "Flour (1 kg)", Price = 2.00m },
            new Product { Code = "PRD036", Name = "Cooking Oil (1 L)", Price = 5.00m },

            // Condiments & Sauces
            new Product { Code = "PRD037", Name = "Tomato Ketchup (500 ml)", Price = 2.50m },
            new Product { Code = "PRD038", Name = "Mayonnaise (400 ml)", Price = 3.00m },
            new Product { Code = "PRD039", Name = "Soy Sauce (250 ml)", Price = 1.80m }
        };


        // List of reasons for credit notes
        private readonly List<string> creditNoteReasons = new List<string>
        {
            "Incorrect billing amount",
            "Product returned",
            "Service not provided",
            "Customer dissatisfaction",
            "Promotional discount adjustment",
            "Duplicate transaction",
            "Invoice correction",
            "Overcharged customer",
            "Product defect or damage",
            "Order cancellation",
            "Refund due to late delivery",
            "Incorrect product delivered",
            "Product exchange",
            "Tax adjustment",
            "Quantity discrepancy",
            "Pricing error",
            "Unfulfilled service commitment",
            "Customer loyalty adjustment",
            "Billing system error",
            "Special customer request"
        };

        private string GetRandomCreditNoteReason(Random random)
        {
            int index = random.Next(creditNoteReasons.Count);
            return creditNoteReasons[index];
        }

        public Invoice GenerateInvoiceData()
            {
                var random = new Random();
            var invoice = new Invoice
                {
                    
                    Currency = "USD",
                    BranchName = "Main Branch",
                    InvoiceNumber = Guid.NewGuid().ToString().Substring(0, 8), // Generate a unique invoice number
                    CustomerName = "Axis Solutions",
                    CustomerVATNumber = "222222222",
                    CustomerAddress = "123 Main St, Harare, Zimbabwe",
                    CustomerTelephoneNumber = "+263712345678",
                    CustomerTIN = "1111111111",
                    CustomerEmail = "axissol@axissol.com",
                    Cashier = "Jane Smith",
                    InvoiceComment = "Thank you for your business!",
                    InvoiceFlag = "01",
                    CustomerNum = "CUST001",
                    Discount = "0",
                    DeviceId = "DEVICE123",
                    OriginalInvoiceNumber = null,
                    ReceiptGlobalNumber = Guid.NewGuid().ToString(),
                    ReasonForCreditNote = GetRandomCreditNoteReason(random),
                    
                };

                // Creating XML for items
                StringBuilder itemsXMLBuilder = new StringBuilder();
                itemsXMLBuilder.Append("<ITEMS>");
                decimal totalAmount = 0;
                decimal totalTax = 0;
                int numberOfItems = random.Next(3, 6); // Number of items (between 2 and 5)

                // Select random products
                var selectedProducts = Products.OrderBy(x => random.Next()).Take(numberOfItems).ToList();
                int hh = 0;
                foreach (var product in selectedProducts)
                {
                    hh++;
                    decimal quantity = random.Next(1, 5);
                    decimal price = product.Price;
                    decimal txRate = (random.Next(0, 2) == 0) ? 0.15m : 0.0m; // Randomly select tax rate (0% or 15%)
                    decimal amount = quantity * price;
                    decimal taxAmount = amount -amount/(1 +txRate); // Calculate tax amount based on txRate

                    totalAmount += amount;
                    totalTax += taxAmount;

                    itemsXMLBuilder.Append("<ITEM>");
                    itemsXMLBuilder.Append($"<HH>{hh}</HH>"); // Incremental item count
                    itemsXMLBuilder.Append($"<ITEMCODE>{product.Code}</ITEMCODE>");
                    itemsXMLBuilder.Append($"<ITEMNAME1>{product.Name}</ITEMNAME1>");
                    itemsXMLBuilder.Append($"<ITEMNAME2>{product.Name}</ITEMNAME2>");
                    itemsXMLBuilder.Append($"<QTY>{quantity:F0}</QTY>"); // Quantity as whole number
                    itemsXMLBuilder.Append($"<PRICE>{price:F2}</PRICE>"); // Format price
                    itemsXMLBuilder.Append($"<AMT>{amount:F2}</AMT>"); // Format amount
                    itemsXMLBuilder.Append($"<TAX>{taxAmount:F2}</TAX>"); // Format tax amount
                    itemsXMLBuilder.Append($"<TAXR>{txRate:F2}</TAXR>"); // Format tax rate
                    itemsXMLBuilder.Append("</ITEM>");
                }
                itemsXMLBuilder.Append("</ITEMS>");

            // Setting ItemsXML
                invoice.ItemsXML = itemsXMLBuilder.ToString();
                invoice.InvoiceAmount = totalAmount.ToString("F2"); // Format total amount
                invoice.InvoiceTaxAmount = totalTax.ToString("F2"); // Format total tax amount

                // Creating XML for currencies
                StringBuilder currenciesXMLBuilder = new StringBuilder();
                currenciesXMLBuilder.Append("<CurrenciesReceived>");
                currenciesXMLBuilder.Append("<Currency>");
                currenciesXMLBuilder.Append("<Name>USD</Name>");
                currenciesXMLBuilder.Append($"<Amount>{invoice.InvoiceAmount}</Amount>");
                currenciesXMLBuilder.Append("<Rate>1</Rate>");
                currenciesXMLBuilder.Append("</Currency>");
                currenciesXMLBuilder.Append("</CurrenciesReceived>");

                // Setting CurrenciesXML
                invoice.CurrenciesXML = currenciesXMLBuilder.ToString();

                return invoice;
            }
        }

        public class Product
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
        }
    
}
