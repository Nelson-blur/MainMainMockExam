namespace Mock_Exam_Work.Models
{
    public class Bookings
    {
        public int BookingsId { get; set; }
        public required string UserId { get; set; }
        public required int RoomsId { get; set; }
        public required DateTime CheckInDate { get; set; }
        public required DateTime CheckOutDate { get; set; }
        public required string Status { get; set; }
        public required DateTime BookingCreatedAt { get; set; }
        public required string SpecialRequest { get; set; }
        public required bool IsPayed { get; set; }
        public required DateTime PayedAt { get; set; }

        public Rooms Room {  get; set; }


    }
}
