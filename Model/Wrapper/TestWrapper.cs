namespace QuizMe.Model.Wrapper;

public class TestWrapper
{
    public TestWrapper(int ind,Test test) 
    {
        ReferencedObject = test;
        Index = $"{ind}.";
        Name = test.Name;
        Points = test.Questions != null ? test.Questions.Count : 0;
        Questions = Points;
        BestScore = test.BestScore;
    }
    public Test ReferencedObject { get; set; }
    public string Index { get; set; }
    public string Name { get; set; }
    public int Points { get; set; }
    public int Questions { get; set; }
    public double BestScore { get; set; }
}
