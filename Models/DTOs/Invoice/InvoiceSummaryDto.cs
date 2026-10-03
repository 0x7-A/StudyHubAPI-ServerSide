namespace StudyHubAPI.Models.DTOs.Invoice
{
    public class InvoiceSummaryDto
    {
        public int InvoiceID { get; set; }
        public int PaymentID { get; set; }
        public int? GeneralOfferID { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
