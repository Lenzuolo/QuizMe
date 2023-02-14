namespace QuizMe.View;

public partial class MainPanel : ContentPage
{
	public MainPanel(MainPanelVM mainPanelVM)
	{
		InitializeComponent();

		BindingContext = mainPanelVM;
	}
    void ResetButtonVisualState(object sender, EventArgs e)
    {
        (sender as Button).Unfocus();
    }

}