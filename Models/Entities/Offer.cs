namespace StudyHubAPI.Models.Entities
{
    public class Offers
    {
        public int OfferID { get; set; }
        public string OfferName { get; set; } = null!;
        public decimal OfferPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public short? MaximumDiscountAmount { get; set; }
        public short? MinimumDiscountAmount { get; set; }
        public ICollection<Invoices> Invoices { get; set; } = new List<Invoices>();
    }
}
