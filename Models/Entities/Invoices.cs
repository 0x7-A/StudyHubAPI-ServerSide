namespace StudyHubAPI.Models.Entities
{
    public class Invoices
    {
        public int InvoiceID { get; set; }
        public int PaymentID { get; set; }
        public int? GeneralOfferID { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation Properties
        public Payments Payment { get; set; } = null!;
        public Offers? Offer { get; set; }
    }
}
