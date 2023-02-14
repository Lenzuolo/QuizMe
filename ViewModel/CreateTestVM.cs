using QuizMe.Services;
using System.Collections.ObjectModel;

namespace QuizMe.ViewModel;



[QueryProperty("Test","Test")]
public partial class CreateTestVM : ObservableObject
{
    public ObservableCollection<Question> Questions { get; } = new ObservableCollection<Question>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelected))]
    [NotifyPropertyChangedFor(nameof(IsNotSelected))]
    public Question selectedItem;

    [ObservableProperty]
    public string title;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaved))]
    [NotifyPropertyChangedFor(nameof(IsNotSaved))]
    [NotifyCanExecuteChangedFor(nameof(RemoveQuestionCommand))]
    public Test test;

    [ObservableProperty]
    public string type;

    [ObservableProperty]
    public string name;

    [ObservableProperty]
    public bool shouldPenalize;

    public bool IsSelected => SelectedItem != null;
    public bool IsNotSelected => SelectedItem == null;
    public bool IsSaved => Test != null && Test.Id != 0;

    public bool IsNotSaved => Test == null || Test.Id == 0;

    private readonly DatabaseService databaseService;

    public CreateTestVM(DatabaseService databaseService)
    {
        Type = "Q";
        this.databaseService = databaseService;
    }

    partial void OnTestChanged(Test value)
    {
        if (value.Id != 0)
        {
            Name = value.Name;
            Type = value.Type;
            Title = value.Name;
            ShouldPenalize = value.Penalize;
            Test.Questions = value.Questions;
            Refresh();
        }
        else
        {
            Title = "New test";
        }
    }

    [RelayCommand]
    async Task SaveTest()
    {
        if (Name == null) { await Shell.Current.DisplayAlert("Validation error", "Name is required","OK"); return; }
        Test.Name = Name;
        Test.Penalize = ShouldPenalize;
        Test.Type = Type;
        Test.Questions = Questions.ToList();
        try
        {
            var result = await databaseService.SaveTestAsync(Test);
            Test = result;
            await Shell.Current.DisplayAlert("Success", "Test saved!","Great!");
        } catch(Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK :(");
        }

    }

    [RelayCommand]
    async Task AddQuestion()
    {
        await Shell.Current.GoToAsync(nameof(CreateQuestionPanel), true, new Dictionary<string, object> { { "Question", new Question { Type = "S", Punctation = "B", Test = Test } } });
    }

    [RelayCommand]
    async Task EditQuestion()
    {
        await Shell.Current.GoToAsync(nameof(CreateQuestionPanel), true, new Dictionary<string, object> { { "Question", SelectedItem } });
    }

    [RelayCommand]
    void Refresh()
    {
        if (Test.Questions != null)
        {
            Questions.Clear();
            Test.Questions.ToList().ForEach(x => Questions.Add(x));
        }
    }

    [RelayCommand]
    async Task BackButton()
    {
        try
        {
            await Shell.Current.GoToAsync("..", true, new Dictionary<string, object> { { "Refresh", true } });
        } catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", ex.Message, "Ok :(");
        }
    }

    [RelayCommand]
    async Task RemoveQuestion()
    {
        var result = await databaseService.RemoveQuestion(SelectedItem);
        if (result != null)
        {
            Test = result;
            Refresh();
        } else
        {
            await Shell.Current.DisplayAlert("Error!", "Could not remove selected question", "OK");
        }
    }
}
