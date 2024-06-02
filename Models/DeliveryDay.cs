namespace Ecommerce.Models
{
    public class DeliveryDay
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsSelected { get; set; }
        public List<TimeSlot> TimeSlots { get; set; }
    }
}
