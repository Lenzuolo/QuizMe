using Microsoft.Extensions.DependencyInjection;
using QuizMe.Helpers;
using QuizMe.Model;
using QuizMe.Services;

namespace QuizMe;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddDbContext<QuizMeContext>();
		builder.Services.AddSingleton<DatabaseService>();

		builder.Services.AddSingleton<DbPath>();

		builder.Services.AddSingleton<MainPanel>();
		builder.Services.AddSingleton<MainPanelVM>();

		builder.Services.AddTransient<CreateTestPanel>();
		builder.Services.AddTransient<CreateTestVM>();

		builder.Services.AddTransient<CreateQuestionPanel>();
		builder.Services.AddTransient<CreateQuestionVM>();

		builder.Services.AddTransient<TestParticipationView>();
		builder.Services.AddTransient<TestParticipationVM>();

		return builder.Build();
	}
}
