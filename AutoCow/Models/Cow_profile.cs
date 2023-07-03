namespace AutoCow.Models
{
    public class Cow_profile
    {
        public int id {  get; set; }
        public String color { get; set; }
        public String type { get; set; }
        public int price { get; set; }
        public int avg_milk { get; set; }
        public int avg_temperature { get; set; }
        public String category { get; set; }

        public DateTime dob { get; set; }

        public int age { get; set; }
    }
}
