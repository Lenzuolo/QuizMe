using QuizMe.Model.Wrapper;
using QuizMe.Services;
using System.Collections.ObjectModel;

namespace QuizMe.ViewModel;

[QueryProperty("Test","Test")]
[QueryProperty("IsLearning","IsLearning")]
public partial class TestParticipationVM : ObservableObject
{
    private readonly DatabaseService databaseService;

    [ObservableProperty]
    public string title;

    [ObservableProperty]
    public string nextButtonText = "Next question";

    [ObservableProperty]
    public Test test;

    [ObservableProperty]
    public bool isLearning;

    [ObservableProperty]
    public bool isOpen;
    [ObservableProperty]
    public bool isMulti;
    [ObservableProperty]
    public bool isSingle;
    [ObservableProperty]
    public bool isTrueFalse;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotFinished))]
    public bool isFinished;

    public bool IsNotFinished => !IsFinished;

    [ObservableProperty]
    public bool isFinalQuestion;

    [ObservableProperty]
    public bool showExplanation;

    [ObservableProperty]
    public QuestionWrapper currentQuestion;
    public ObservableCollection<TestAnswerWrapper> Answers { get; set; } = new ObservableCollection<TestAnswerWrapper>();
    public ObservableCollection<QuestionWrapper> Questions { get; set; } = new ObservableCollection<QuestionWrapper>();

    [ObservableProperty]
    public string selectedSingleAnswerIndex; //For RB group

    [ObservableProperty]
    public double score;

    [ObservableProperty]
    public string scoreLabel;

    [ObservableProperty]
    public string maxScore;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowOpenAnswer))]
    public bool showAnswers;

    public bool ShowOpenAnswer => !ShowAnswers;

    [ObservableProperty]
    public string openAnswerContent;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisableFinishButton))]
    public bool enableFinishButton;
 
    public bool DisableFinishButton => !EnableFinishButton;
    

    public TestParticipationVM(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
        IsFinished = false;
        Title = "Test";
    }

    partial void OnIsLearningChanged(bool value)
    {
        if (value)
        {
            ShowAnswers = true;
            NextButtonText = "Check answer";
        }
    }

    partial void OnTestChanged(Test value)
    {
        if (value != null)
        {
            Questions.Clear();
            Test.Questions.ToList().ForEach(q =>
            {
                Questions.Add(new QuestionWrapper(q));
            });

            if (Questions.Count > 0)
            {
                CurrentQuestion = Questions.First();
                SetFlagsByType(CurrentQuestion.Type);
                Answers.Clear();
                if (CurrentQuestion.ReferencedObject.Answers != null)
                {
                    var count = 1;
                    CurrentQuestion.ReferencedObject.Answers.ToList().ForEach((answer) => Answers.Add(new TestAnswerWrapper(count.ToString(), answer, CurrentQuestion.Type == "O")));
                }
            }
        }
    }

    [RelayCommand]
    async Task Next()
    {
        CurrentQuestion.Answers = new ObservableCollection<TestAnswerWrapper>(Answers);
        var index = Questions.IndexOf(CurrentQuestion);

        if (!ShowExplanation || !IsLearning)
        {
            NextButtonText = index == Questions.Count - 1 ? "Finish learning" : IsLearning ? "Continue" : "Next";
            ShowExplanation = true;
            CheckAnswers();
            if (IsLearning) return;
        } else if (ShowExplanation)
        {
            ShowExplanation = false;
            NextButtonText = "Check answer";
        }

        if (index + 1 == Questions.Count - 1)
        {
            CurrentQuestion = Questions.Last();
            if (!IsLearning)
            {
                NextButtonText = "Submit Test";
            }
        } else if (index == Questions.Count - 1)
        {
            IsFinished = true;
            ScoreLabel = $"Score:  {Score}";
            MaxScore = $"Max:  {Questions.Count}";
            EnableFinishButton = !Questions.Any(q => q.ToVerify);
            return;
        } 
        else
        {
            CurrentQuestion = Questions.ElementAt(index+1);
        }

        SetFlagsByType(CurrentQuestion.Type);

        Answers.Clear();
        if (CurrentQuestion.ReferencedObject.Answers != null)
        {
            var count = 1;
            CurrentQuestion.ReferencedObject.Answers.ToList().ForEach((answer) => Answers.Add(new TestAnswerWrapper(count.ToString(), answer, CurrentQuestion.Type == "O")));
        }
    }

    [RelayCommand]
    async Task SaveButton()
    {
        if (IsFinished && !IsLearning)
        {
            Test.BestScore = Score > Test.BestScore ? Score : Test.BestScore;
            await databaseService.SaveTestAsync(Test);
        }
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task ShowOpenQuestionToVerify(QuestionWrapper question)
    {
        var correctAnswer = question.Answers.First().ReferencedObject.Content;
        var givenAnswer = question.Answers.First().Content;

        var verified = await Shell.Current.DisplayAlert(question.Title, $"Correct answer: {correctAnswer}\n Your answer: {givenAnswer}\n", "Mark as correct", "Mark as wrong");
        if (verified)
        {
            question.Score = 1;
            Score += 1;
            question.IsCorrect = true;
        } else
        {
            question.Score = Test.Penalize ? -1 : 0;
            Score -= Test.Penalize ? 1 : 0;
            question.IsWrong = true;
        }
        ScoreLabel = $"Score:  {Score}";
        question.ToVerify = false;
        Refresh();
    }

    private void SetFlagsByType(string type)
    {
        ClearFlags();
        switch (type)
        {
            case "S":
                IsSingle = true;
                break;
            case "TF":
                IsTrueFalse = true;
                break;
            case "M":
                IsMulti= true;
                break;
            case "O":
                IsOpen= true;
                break;
        }
    }

    private void ClearFlags()
    {
        IsMulti = false;
        IsSingle = false;
        IsOpen = false;
        IsTrueFalse = false;
    }

    private void CheckAnswers()
    {
        if (CurrentQuestion.Type != "O")
        {
            ShowAnswers = true;
            var correctAnswers = 0;
            var allAnswers = Answers.Count;
            Answers.ToList().ForEach(a =>
            {
                var originalAnswer = CurrentQuestion.ReferencedObject.Answers.ToList().Find(ans => ans.Id == a.ReferencedObject.Id);
                if (CurrentQuestion.Type == "S" || CurrentQuestion.Type == "M")
                {

                    if ((a.IsTrue && originalAnswer.IsCorrect) ||
                        (!a.IsTrue && !originalAnswer.IsCorrect))
                    {
                        correctAnswers++;
                        a.AnsweredCorrectly = true;
                    }
                    else
                    {
                        a.AnsweredIncorrectly = true;
                    }
                }
                else
                {
                    if ((originalAnswer.IsCorrect && a.IsTrue) || (!originalAnswer.IsCorrect && a.IsFalse))
                    {
                        correctAnswers++;
                        a.AnsweredCorrectly = true;
                    }
                    else
                    {
                        a.AnsweredIncorrectly = true;
                    }
                }

            });

            if (CurrentQuestion.Punctation == "B")
            {
                if (correctAnswers == allAnswers)
                {
                    CurrentQuestion.Score = 1;
                    CurrentQuestion.IsCorrect = true;
                    Score += 1;
                }
                else
                {
                    if (Test.Penalize)
                    {
                        CurrentQuestion.Score = -1;
                        Score -= 1;
                    }
                    else
                    {
                        CurrentQuestion.Score = 0;
                    }
                    CurrentQuestion.IsWrong = true;
                }
            }
            else if (CurrentQuestion.Punctation == "C")
            {
                if (correctAnswers == allAnswers)
                {
                    CurrentQuestion.Score = 1;
                    CurrentQuestion.IsCorrect = true;
                    Score += 1;
                }
                else
                {
                    var percentage = (double)correctAnswers / allAnswers;
                    if (percentage > 0.66)
                    {
                        CurrentQuestion.Score = 0.5;
                        CurrentQuestion.IsPartial = true;
                        Score += 0.5;
                    }
                    else
                    {
                        if (Test.Penalize)
                        {
                            CurrentQuestion.Score = -1;
                            Score -= 1;
                        }
                        else
                        {
                            CurrentQuestion.Score = 0;
                        }
                        CurrentQuestion.IsWrong = true;
                    }
                }
            }
        }
        else
        {
            if (IsLearning)
            {
                ShowAnswers = false;
                OpenAnswerContent = Answers.ToList()[0].ReferencedObject.Content;
            }
            CurrentQuestion.ToVerify = true;
        }
    }

    [RelayCommand]
    void Refresh()
    {
        var refresh = new ObservableCollection<QuestionWrapper>(Questions);
        Questions.Clear();
        refresh.ToList().ForEach(q => Questions.Add(q));
        EnableFinishButton = !Questions.Any(q => q.ToVerify);
    }
}
