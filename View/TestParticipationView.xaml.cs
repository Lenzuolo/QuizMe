namespace QuizMe.View;

public partial class TestParticipationView : ContentPage
{
	public TestParticipationView(TestParticipationVM testParticipationVM)
	{
		InitializeComponent();

		BindingContext = testParticipationVM;
	}
}