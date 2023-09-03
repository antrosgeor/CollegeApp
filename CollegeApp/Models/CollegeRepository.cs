namespace CollegeApp.Models
{
    public class CollegeRepository
    {
        public static List<Student> Students { get; set; } = new List<Student>(){ 
                new Student 
                { 
                    Id= 1,
                    StudentName="Student 1",
                    Email = "studentemail1@gmail.com",
                    Address = "Hyd, soksoks , soksoks"
                },
                new Student
                {
                    Id = 2,
                    StudentName = "Student 2",
                    Email = "studentemail2@gmail.com",
                    Address = "Banglore, INDIA"
                }
            };
    }
}
