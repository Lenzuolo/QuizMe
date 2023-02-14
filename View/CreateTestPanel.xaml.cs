namespace QuizMe.View;

public partial class CreateTestPanel : ContentPage
{
	public CreateTestPanel(CreateTestVM createTestVM)
	{
		InitializeComponent();

		BindingContext = createTestVM;
	}
}