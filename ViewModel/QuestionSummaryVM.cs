using QuizMe.Model.Wrapper;
using System.Collections.ObjectModel;

namespace QuizMe.ViewModel;

[QueryProperty("Question","Question")]
public partial class QuestionSummaryVM : ObservableObject
{
    [ObservableProperty]
    public QuestionWrapper question;

    public QuestionSummaryVM(QuestionWrapper question)
    {
        Question = question;
    }


    [RelayCommand]
    async Task Back()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
}
