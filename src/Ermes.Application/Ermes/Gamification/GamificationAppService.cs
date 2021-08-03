using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;
using Ermes.Answers;
using Ermes.Attributes;
using Ermes.Dto.Datatable;
using Ermes.Gamification.Dto;
using Ermes.Linq.Extensions;
using Ermes.Quizzes;
using Ermes.Tips;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Gamification
{
    [ErmesAuthorize]
    public class GamificationAppService : ErmesAppServiceBase, IGamificationAppService
    {
        private readonly TipManager _tipManager;
        private readonly QuizManager _quizManager;
        private readonly AnswerManager _answerManager;
        public GamificationAppService(
                TipManager tipManager,
                QuizManager quizManager,
                AnswerManager answerManager
            )
        {
            _tipManager = tipManager;
            _quizManager = quizManager;
            _answerManager = answerManager;
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
    }
}
