namespace P01_StudentSystem.P01_StudentSystem.Data.Models;

public class Student
{
    public int StudentId { get; set; }
    public string Name { get; set; }
    public int PhoneNumber { get; set; }
    public string RegisteredOn { get; set; }
    public DateTime Birthday { get; set; }
}