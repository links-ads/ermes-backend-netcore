﻿using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Tips
{
    public class TipManager : DomainService
    {
        public IQueryable<Tip> Tips { get { return TipRepository.GetAll().Include(c => c.Translations).Include(t => t.Quizzes).ThenInclude(q => q.Translations); } }
        //public IQueryable<Tip> Tips { get { return TipRepository.GetAllIncluding(c => c.Translations); } }
        protected IRepository<Tip> TipRepository { get; set; }
        public IQueryable<TipTranslation> TipsTranslation { get { return TipTranslationRepository.GetAllIncluding(ct => ct.Core); } }
        protected IRepository<TipTranslation> TipTranslationRepository { get; set; }

        protected IRepository<PersonTip> PersonTipRepository { get; set; }
        public IQueryable<PersonTip> PersonTips { get { return PersonTipRepository.GetAllIncluding(a => a.Tip); } }

        public TipManager(IRepository<Tip> tipRepository, IRepository<TipTranslation> tipTranslationRepository, IRepository<PersonTip> personTipRepository)
        {
            TipRepository = tipRepository;
            TipTranslationRepository = tipTranslationRepository;
            PersonTipRepository = personTipRepository;
        }

        public async Task<List<Tip>> GetTipsAsync()
        {
            return await Tips.ToListAsync();
        }

        public async Task<Tip> GetTipByCodeAsync(string code)
        {
            return await Tips.SingleOrDefaultAsync(a => a.Code == code);
        }

        public async Task InsertOrUpdateTipAsync(Tip tip)
        {
            await TipRepository.InsertOrUpdateAsync(tip);
        }
        public async Task InsertOrUpdateTipTranslationAsync(TipTranslation translation)
        {
            await TipTranslationRepository.InsertOrUpdateAsync(translation);
        }

        public async Task<TipTranslation> GetTipTranslationByCoreIdLanguageAsync(int coreId, string language)
        {
            return await TipsTranslation.SingleOrDefaultAsync(a => a.CoreId == coreId && a.Language == language);
        }

        public async Task<List<Tip>> GetTipsByPersonAsync(long personId)
        {
            return await PersonTips.Where(pt => pt.PersonId == personId).Select(a => a.Tip).ToListAsync();
        }
    }
}
