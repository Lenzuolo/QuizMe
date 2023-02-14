namespace QuizMe;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(CreateTestPanel), typeof(CreateTestPanel));
		Routing.RegisterRoute(nameof(CreateQuestionPanel), typeof(CreateQuestionPanel));
		Routing.RegisterRoute(nameof(TestParticipationView), typeof(TestParticipationView));
		Routing.RegisterRoute(nameof(QuestionSummaryModal), typeof(QuestionSummaryModal));
	}
}
