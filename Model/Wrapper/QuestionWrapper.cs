

using System.Collections.ObjectModel;

namespace QuizMe.Model.Wrapper;

public class QuestionWrapper
{
    public double Score { get; set; } = 0;
    public string Title { get; set; }
    public string Type { get; set; }

    public string Punctation { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsWrong { get; set; }
    public bool IsPartial { get; set; }

    public bool ToVerify { get; set; }
    public Question ReferencedObject { get; set; }

    public ObservableCollection<TestAnswerWrapper> Answers { get; set; }

    public QuestionWrapper(Question question)
    {
        Title = question.Content;
        Type = question.Type;
        ReferencedObject = question;
        Punctation = question.Punctation;
    }
}
