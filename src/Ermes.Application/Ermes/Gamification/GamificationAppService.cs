﻿using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Answers;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Dto.Datatable;
using Ermes.Ermes.Gamification.Dto;
using Ermes.Gamification.Dto;
using Ermes.Linq.Extensions;
using Ermes.Persons;
using Ermes.Quizzes;
using Ermes.Tips;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Gamification
{
    [ErmesAuthorize]
    [ErmesGamification]
    public class GamificationAppService : ErmesAppServiceBase, IGamificationAppService
    {
        private readonly TipManager _tipManager;
        private readonly QuizManager _quizManager;
        private readonly AnswerManager _answerManager;
        private readonly PersonManager _personManager;
        private readonly GamificationManager _gamificationManager;
        private readonly ErmesAppSession _session;
        public GamificationAppService(
                TipManager tipManager,
                QuizManager quizManager,
                AnswerManager answerManager,
                ErmesAppSession session,
                PersonManager personManager,
                GamificationManager gamificationManager
            )
        {
            _tipManager = tipManager;
            _quizManager = quizManager;
            _answerManager = answerManager;
            _session = session;
            _personManager = personManager;
            _gamificationManager = gamificationManager;
        }

        #region Private
        private async Task<PagedResultDto<TipDto>> InternalGetTips(GetTipsInput input)
        {
            PagedResultDto<TipDto> result = new PagedResultDto<TipDto>();
            IQueryable<Tip> query = _tipManager.Tips;

            if (input.Hazards != null && input.Hazards.Count > 0)
            {
                var hazardList = input.Hazards.Select(a => a.ToString()).ToList();
                query = query.Where(a => hazardList.Contains(a.HazardString));
            }

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderByDescending(a => a.Code);
                query = query.PageBy(input);
            }
            else
            {
                query = query.DTOrderedBy(input)
                    .PageBy(input);
            }

            var items = await query.ToListAsync();
            result.Items = ObjectMapper.Map<List<TipDto>>(items);
            //Check for tips the current logged user has already read
            //Cannot be done with Join statement due to a limitation in EF Core
            var tipCodesLit = await _personManager.GetTipsReadByPersonIdAsync(_session.LoggedUserPerson.Id);
            result.Items = result.Items.Select(t => { t.ReadByUser = tipCodesLit.Contains(t.Code); return t; }).ToList();
            return result;
        }

        private async Task<PagedResultDto<QuizDto>> InternalGetQuizzes(GetQuizzesInput input)
        {
            PagedResultDto<QuizDto> result = new PagedResultDto<QuizDto>();
            IQueryable<Quiz> query = _quizManager.Quizzes;

            if (input.Hazards != null && input.Hazards.Count > 0)
            {
                var hazardList = input.Hazards.Select(a => a.ToString()).ToList();
                query = query.Where(a => hazardList.Contains(a.HazardString));
            }

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderByDescending(a => a.Code);
                query = query.PageBy(input);
            }
            else
            {
                query = query.DTOrderedBy(input)
                    .PageBy(input);
            }

            var items = await query.ToListAsync();
            result.Items = ObjectMapper.Map<List<QuizDto>>(items);
            //Check for quizzes the current logged user has already solved
            //Cannot be done with Join statement due to a limitation in EF Core
            var quizCodesLit = await _personManager.GetQuizzesReadByPersonIdAsync(_session.LoggedUserPerson.Id);
            result.Items = result.Items.Select(t => { t.SolvedByUser = quizCodesLit.Contains(t.Code); return t; }).ToList();
            return result;
        }

        private async Task<PagedResultDto<AnswerDto>> InternalGetAnswers(GetAnswersInput input)
        {
            PagedResultDto<AnswerDto> result = new PagedResultDto<AnswerDto>();
            IQueryable<Answer> query = _answerManager.Answers;

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderByDescending(a => a.Code);
                query = query.PageBy(input);
            }
            else
            {
                query = query.DTOrderedBy(input)
                    .PageBy(input);
            }

            var items = await query.ToListAsync();
            result.Items = ObjectMapper.Map<List<AnswerDto>>(items);
            return result;
        }
        #endregion



        public virtual async Task<DTResult<TipDto>> GetTips(GetTipsInput input)
        {
            PagedResultDto<TipDto> result = await InternalGetTips(input);
            return new DTResult<TipDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        public virtual async Task<DTResult<QuizDto>> GetQuizzes(GetQuizzesInput input)
        {
            PagedResultDto<QuizDto> result = await InternalGetQuizzes(input);
            return new DTResult<QuizDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        public virtual async Task<DTResult<AnswerDto>> GetAnswers(GetAnswersInput input)
        {
            PagedResultDto<AnswerDto> result = await InternalGetAnswers(input);
            return new DTResult<AnswerDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        public virtual async Task<bool> SetTipAsRead(SetTipAsReadInput input)
        {
            try
            {
                await _personManager.CreatePersonTipAsync(_session.LoggedUserPerson.Id, input.TipCode);
            }
            catch(Exception e)
            {
                Logger.ErrorFormat("Errro while inserting PersonTip: {0}", e.Message);
                return false;
            }

            return true;
        }

        public virtual async Task<bool> CheckPersonAnswer(CheckPersonAnswerInput input)
        {
            try
            {
                var ans = await _answerManager.GetAnswerByCodeAsync(input.AnswerCode);
                if (ans.IsTheRightAnswer)
                    await _personManager.CreatePersonQuizAsync(_session.LoggedUserPerson.Id, ans.QuizCode);
                else
                    return false;
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Errro while inserting PersonQuiz: {0}", e.Message);
                return false;
            }

            return true;
        }

        public virtual async Task<GetLevelsOutput> GetLevels()
        {
            var levels = await _gamificationManager.GetLevelsAsync();
            return new GetLevelsOutput() { Levels = ObjectMapper.Map<List<LevelDto>>(levels) };
        }

        public async Task<List<GamificationActionDto>> GetActions()
        {
            var items = await _gamificationManager.Actions.ToListAsync();
            var result = ObjectMapper.Map<List<GamificationActionDto>>(items);
            return result;
        }
    }
}
