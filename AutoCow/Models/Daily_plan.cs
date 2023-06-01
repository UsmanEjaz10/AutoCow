namespace AutoCow.Models
{
    public class Daily_plan
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public int milk { get; set; }  
        public int temperature { get; set; }
        public string insemination { get; set; }
        public string disease { get; set; }
        public string condition { get; set; }
        public string type { get; set; }

        public int total { get; set; }

        public int count_condition { get; set; }
    }
}
