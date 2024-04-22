namespace API_GesSIgn.Models
{
    public class Class
    {
        public int Class_Id { get; set; }

        public required string Class_Name { get; set; }

        public List<Student>? Class_Students { get; set; }
    }
}
