namespace QuizMe.Model.Wrapper;

public class TestAnswerWrapper
{
    public string IndexLetter { get; set; }
    public bool IsFalse { get; set; }
    public bool IsTrue { get; set; }
    public string Content { get; set; }
    public bool AnsweredCorrectly { get; set; }
    public bool AnsweredIncorrectly { get; set; }
    public Answer ReferencedObject { get; set; }

    public bool IsCorrect { get; set; }

    public bool IsIncorrect { get; set; }

    public TestAnswerWrapper(string indexLetter, Answer answer, bool isOpen = false)
    {
        ReferencedObject = answer;
        IndexLetter = indexLetter;
        if (!isOpen)
        {
            Content = answer.Content;
        }
        IsCorrect = answer.IsCorrect;
        IsIncorrect = !IsCorrect;
    }
}
