using Microsoft.EntityFrameworkCore.ChangeTracking;
using QuizMe.Model;

namespace QuizMe.Services;

public class DatabaseService
{
    private readonly QuizMeContext context;

    public DatabaseService (QuizMeContext context)
    {
        this.context = context;
#if DEBUG
       // context.Database.EnsureDeleted();
#endif
        context.Database.EnsureCreated();
    }

    public async Task<Test> SaveTestAsync(Test test)
    {
        EntityEntry<Test> result;
        if (test.Id == 0)
        {
            result = await context.Tests.AddAsync(test);
        }
        else
        {
            result = context.Tests.Update(test);
        }
        await context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<ICollection<Test>> GetTests()
    {
        var result = await context.Tests.Include(x => x.Questions).ThenInclude(q => q.Answers).ToListAsync();
        return result;
    }

    public async Task<bool> RemoveTest(Test test)
    {
        if (test.Id == 0) return false;
        context.Tests.Remove(test);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Question> SaveQuestionAsync(Question question)
    {
        if (question.Id == 0)
        {
            if (question.Test != null)
            {
                var result = await context.Questions.AddAsync(question);
                await context.SaveChangesAsync();
                return result.Entity;
            }
        } 
        else
        {
            if (question.Test != null)
            {
                var result = context.Questions.Update(question);
                await context.SaveChangesAsync();
                return result.Entity;
            }
        }
        return null;
    }

    public async Task<Test> RemoveQuestion(Question question)
    {
        if (question.Id == 0 || question.Test == null) return null;
        var testId = question.Test.Id;
        if (testId != 0)
        {
            context.Questions.Remove(question);
            await context.SaveChangesAsync();
            return await context.Tests.Include(x => x.Questions).FirstOrDefaultAsync(x => x.Id == testId);
        }
        return null;
    }

    public async Task<ICollection<Question>> GetQuestionsAsync(int testId)
    {
        return await context.Questions.Include(q => q.Test).Where(q => q.Test.Id == testId).ToListAsync();
    }

    public async Task<Question> RemoveAnswer(Answer answer)
    {
        if (answer.Id == 0 || answer.Question == null) return null;
        var questionId = answer.Question.Id;
        if (questionId != 0)
        {
            context.Answers.Remove(answer);
            await context.SaveChangesAsync();
            return await context.Questions.Include(x => x.Answers).FirstOrDefaultAsync(q => q.Id == questionId);
        }
        return null;
    }
}
