using QuizMe.Model;
using QuizMe.Model.Wrapper;
using QuizMe.Services;
using System.Collections.ObjectModel;

namespace QuizMe.ViewModel;

public partial class MainPanelVM : ObservableObject
{
    private readonly DatabaseService databaseService;

    [ObservableProperty]
    public string title;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelected))]
    [NotifyPropertyChangedFor(nameof(IsNotSelected))]
    public TestWrapper selectedItem = null;

    public bool IsSelected => SelectedItem != null;

    public bool IsNotSelected => SelectedItem == null;
    public ObservableCollection<TestWrapper> Tests { get; } = new ObservableCollection<TestWrapper>();
    public MainPanelVM(DatabaseService databaseService)
    {
        Title = "Panel";
        this.databaseService = databaseService;
        GetTestsCommand.Execute(null);
    }

    [RelayCommand]
    async Task AddCourse()
    {
        
        await Shell.Current.GoToAsync(nameof(CreateTestPanel), true, new Dictionary<string,object> { { "Test", new Test() } });
    }

    [RelayCommand]
    async Task EditCourse()
    {
        if (SelectedItem != null)
        {
            await Shell.Current.GoToAsync(nameof(CreateTestPanel), true, new Dictionary<string, object> { { "Test", SelectedItem.ReferencedObject } });
        }
    }

    [RelayCommand]
    async Task GetTests()
    {
        var tests = await databaseService.GetTests();
        if (tests != null)
        {
            Tests.Clear();
            var count = 1;
            tests.ToList().ForEach(test => { Tests.Add(new TestWrapper(count, test)); count++; });
        }
    }

    [RelayCommand]
    async Task RemoveTest()
    {
        if (SelectedItem != null)
        {
            var result = await databaseService.RemoveTest(SelectedItem.ReferencedObject);
            if (result)
            {
                await GetTests();
            } else
            {
                await Shell.Current.DisplayAlert("Error!", "Could not delete selected item, try again", "OK :(");
            }
        }
    }

    [RelayCommand]
    async Task TakeTest()
    {
        if (SelectedItem.ReferencedObject.Questions != null && SelectedItem.ReferencedObject.Questions.Count > 0) 
        {
            await Shell.Current.GoToAsync(nameof(TestParticipationView), true, new Dictionary<string, object> { { "Test", SelectedItem.ReferencedObject }, { "IsLearning", false } });
        } else
        {
            await Shell.Current.DisplayAlert("Validation Error!", "Selected test has no questions, add them to continue","OK");
        }
    }

    [RelayCommand]
    async Task LearnTest()
    {
        if (SelectedItem.ReferencedObject.Questions != null && SelectedItem.ReferencedObject.Questions.Count > 0)
        {
            await Shell.Current.GoToAsync(nameof(TestParticipationView), true, new Dictionary<string, object> { { "Test", SelectedItem.ReferencedObject }, { "IsLearning", true } });
        }
        else
        {
            await Shell.Current.DisplayAlert("Validation Error!", "Selected test has no questions, add them to continue", "OK");
        }
    }
}
