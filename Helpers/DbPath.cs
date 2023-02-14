namespace QuizMe.Helpers;

public class DbPath
{
    public string GetPath()
    {
        return Path.Combine(FileSystem.AppDataDirectory, "QuizMe.db");
    }
}
