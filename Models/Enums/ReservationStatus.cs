namespace StudyHubAPI.Models.Enums
{
    public enum ReservationStatus : byte
    {
       AwaitingPayment = 1, Confirmed = 2,
       Pending  = 3, Completed = 4, 
       Cancelled = 5, NoShow = 6
    }
}
