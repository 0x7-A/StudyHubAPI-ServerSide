namespace StudyHubAPI.Models.DTOs.Invoice
{
    public class InvoiceDetailsDto
    {
        public int InvoiceID { get; set; }
        public int PaymentID { get; set; }
        public int? GeneralOfferID { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
