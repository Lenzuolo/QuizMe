using QuizMe.Services;
using System.Collections.ObjectModel;

namespace QuizMe.ViewModel;

public struct States
{
    public bool IsSelection { get; set; }
    public bool IsOpen { get; set; }
}


[QueryProperty("Question","Question")]
public partial class CreateQuestionVM : ObservableObject
{

    private readonly DatabaseService _databaseService;

    public ObservableCollection<Answer> Answers { get; } = new ObservableCollection<Answer>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Type))]
    [NotifyPropertyChangedFor(nameof(Punctation))]
    public Question question;

    [ObservableProperty]
    public string title;

    [ObservableProperty]
    public string description;

    [ObservableProperty]
    public string type;

    [ObservableProperty]
    public string punctation;

    [ObservableProperty]
    public States statesDef;

    partial void OnTypeChanged(string value)
    {
        States states;
        Answers.Clear();
        if (value == "O")
        {
            if (Answers.Count == 0)
            {
                Answers.Add(new Answer { Question = Question, Content = "" });
            }
        }

        switch(value)
        {
            case "O":
                states = new States
                {
                    IsOpen = true, IsSelection = false
                };
                StatesDef = states;
                break;
            case "M":
            case "S":
            case "TF":
            default:
                states = new States
                {
                    IsSelection = true,
                    IsOpen = false,
                };
                StatesDef = states;
                break;
        }
    }

    partial void OnQuestionChanged(Question value)
    {
        Type = value.Type;
        Punctation = value.Punctation;
        if (value.Id != 0)
        {
            Title = "Modify question";
            Description = value.Content;
            if (value.Answers != null && value.Answers.Count() > 1)
            {
                Answers.Clear();
                value.Answers.ToList().ForEach(a => Answers.Add(a));
            }
        }
        else Title = "New question";
    }

    public CreateQuestionVM(DatabaseService databaseService) 
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    async Task SaveQuestion()
    {
        Question.Punctation = Punctation;
        Question.Type = Type;
        Question.Content = Description;

        if (Question.Content == null)
        {
            await Shell.Current.DisplayAlert("Validation Error", "Question content was not specified", "OK");
            return;
        }

        if (Answers.Count > 0)
        {
            if (Answers.Any(a => a.Content.Length == 0))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Some answers have no content", "OK");
                return;
            }
        }


        if (Answers.Count > 0 && Question.Type == "S")
        {
            var countCorrect = Answers.Count(a => a.IsCorrect);
            if (countCorrect > 1)
            {
                await Shell.Current.DisplayAlert("Validation Error", "Question of type \"Single\" cannot have more than 2 correct answers","OK");
                return;
            } else if (countCorrect == 0)
            {
                await Shell.Current.DisplayAlert("Validation Error", "Question of type \"Single\" cannot have no correct answers - for this specific case use types \"Multiple\" or \"True/False\"", "OK");
                return;
            }
        }
        Question.Answers = Answers.ToList();
        if (((Question.Type == "O" && Question.Answers.Count == 1) || (Question.Type != "O" && Question.Answers.Count > 1)))
        {
            try
            {
                var result = await _databaseService.SaveQuestionAsync(Question);
                if (result != null)
                {
                    Question = result;
                    await Shell.Current.DisplayAlert("Success", "Question saved!", "Great!");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK :(");
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Validation error!", "Not enough answers were given", "OK :(");
        }
    }

    [RelayCommand]
    public void AddAnswer()
    {
        Answers.Add(new Answer { Question = Question, IsCorrect = false, Content = "" });
    }

    [RelayCommand]
    public async Task RemoveAnswer(Answer answer)
    {
        if (answer.Id == 0)
        {
            Answers.Remove(answer);
        } else
        {
            var result = await _databaseService.RemoveAnswer(answer);
            if (result != null)
            {
                Question = result;
                if (Question.Answers != null)
                {
                    Answers.Clear();
                    Question.Answers.ToList().ForEach(a => Answers.Add(a));
                }
            } else
            {
                await Shell.Current.DisplayAlert("Error!", "Could not delete answer", "OK :(");
            }
        }
    }
}
