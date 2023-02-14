namespace QuizMe.View;

public partial class CreateQuestionPanel : ContentPage
{
	public CreateQuestionPanel(CreateQuestionVM createQuestionVM)
	{
		InitializeComponent();

		BindingContext = createQuestionVM;
	}
}