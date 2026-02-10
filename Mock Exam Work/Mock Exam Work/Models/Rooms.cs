 namespace Mock_Exam_Work.Models
{
    public class Rooms
    {
        public int RoomsId { get; set; }
        public required string RoomsName { get; set; }
        public string? RoomsDescription { get; set; }
        public required int Capacity { get; set; }
        public required float HourlyRate { get; set; }
        public required string City { get; set; }
        public required bool IsAvailable { get; set; }
        public ICollection<Bookings>? Bookings { get; set; }
     
    }
}
