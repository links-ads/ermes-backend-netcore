using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Quizzes
{
    public class QuizManager: DomainService
    {
        public IQueryable<Quiz> Quizzes { get { return QuizRepository
                                                        .GetAll()
                                                        .Include(c => c.Translations)
                                                        .Include(c => c.Answers)
                                                        .ThenInclude(a => a.Translations)
                                                        .Include(q => q.Tip).ThenInclude(t => t.Translations); } }
        protected IRepository<Quiz> QuizRepository { get; set; }
        public IQueryable<QuizTranslation> QuizTranslations { get { return QuizTranslationRepository.GetAllIncluding(ct => ct.Core); } }
        protected IRepository<QuizTranslation> QuizTranslationRepository { get; set; }
        protected IRepository<PersonQuiz> PersonQuizRepository { get; set; }
        public IQueryable<PersonQuiz> PersonQuizzes { get { return PersonQuizRepository.GetAllIncluding(a => a.Quiz); } }

        public QuizManager(IRepository<Quiz> quizRepository, IRepository<QuizTranslation> quizTranslationRepository, IRepository<PersonQuiz> personQuizRepository)
        {
            QuizRepository = quizRepository;
            QuizTranslationRepository = quizTranslationRepository;
            PersonQuizRepository = personQuizRepository;
        }

        public async Task<List<Quiz>> GetQuizzesAsync()
        {
            return await Quizzes.ToListAsync();
        }

        public async Task<Quiz> GetQuizByCodeAsync(string code)
        {
            return await Quizzes.SingleOrDefaultAsync(a => a.Code == code);
        }

        public async Task InsertOrUpdateQuizAsync(Quiz quiz)
        {
            await QuizRepository.InsertOrUpdateAsync(quiz);
        }
        public async Task InsertOrUpdateQuizTranslationAsync(QuizTranslation translation)
        {
            await QuizTranslationRepository.InsertOrUpdateAsync(translation);
        }

        public async Task<QuizTranslation> GetQuizTranslationByCoreIdLanguageAsync(int coreId, string language)
        {
            return await QuizTranslations.SingleOrDefaultAsync(a => a.CoreId == coreId && a.Language == language);
        }

        public async Task<List<Quiz>> GetQuizzesByPersonAsync(long personId)
        {
            return await PersonQuizzes.Where(a => a.PersonId == personId).Select(a => a.Quiz).ToListAsync();
        }
    }
}
