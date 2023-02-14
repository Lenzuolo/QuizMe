using QuizMe.Model.Wrapper;

namespace QuizMe.View;

public partial class QuestionSummaryModal : ContentPage
{
	public QuestionSummaryModal(QuestionWrapper question)
	{
		InitializeComponent();
		BindingContext = new QuestionSummaryVM(question);
	}
}