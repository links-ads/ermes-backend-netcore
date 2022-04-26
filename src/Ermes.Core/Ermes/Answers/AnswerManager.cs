using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Answers
{
    public class AnswerManager : DomainService
    {
        public IQueryable<Answer> Answers { get { return AnswerRepository.GetAll().Include(c => c.Translations).Include(a => a.Quiz).ThenInclude(t => t.Translations); } }
        protected IRepository<Answer> AnswerRepository { get; set; }
        public IQueryable<AnswerTranslation> AnswerTranslations { get { return AnswerTranslationRepository.GetAllIncluding(ct => ct.Core); } }
        protected IRepository<AnswerTranslation> AnswerTranslationRepository { get; set; }

        public AnswerManager(IRepository<Answer> answerRepository, IRepository<AnswerTranslation> answerTranslationRepository)
        {
            AnswerRepository = answerRepository;
            AnswerTranslationRepository = answerTranslationRepository;
        }

        public async Task<List<Answer>> GetAnswersAsync()
        {
            return await Answers.ToListAsync();
        }

        public async Task<Answer> GetAnswerByCodeAsync(string code)
        {
            return await Answers.SingleOrDefaultAsync(a => a.Code == code);
        }

        public async Task InsertOrUpdateAnswerAsync(Answer answer)
        {
            await AnswerRepository.InsertOrUpdateAsync(answer);
        }
        public async Task InsertOrUpdateAnswerTranslationAsync(AnswerTranslation translation)
        {
            await AnswerTranslationRepository.InsertOrUpdateAsync(translation);
        }

        public async Task<AnswerTranslation> GetAnswerTranslationByCoreIdLanguageAsync(int coreId, string language)
        {
            return await AnswerTranslations.SingleOrDefaultAsync(a => a.CoreId == coreId && a.Language == language);
        }

        public async Task<bool> IsTheRightAnswerAsync(string answerCode)
        {
            var ans = await Answers.FirstAsync(a => a.Code == answerCode);
            return ans.IsTheRightAnswer;
        }
    }
}
