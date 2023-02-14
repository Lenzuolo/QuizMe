namespace QuizMe.Model;

public class Test
{ 

    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public bool Penalize { get; set; }
    public double BestScore { get; set; }
    public ICollection<Question> Questions { get; set; }
}
