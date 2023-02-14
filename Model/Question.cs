namespace QuizMe.Model;

public class Question
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Type { get; set; }
    public string Punctation { get; set; }
    public ICollection<Answer> Answers { get; set; }

    public int TestId { get; set; }
    public Test Test { get; set; }
}
