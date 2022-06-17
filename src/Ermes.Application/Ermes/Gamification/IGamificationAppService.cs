using Abp.Application.Services;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Ermes.Gamification.Dto;
using Ermes.Gamification.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Gamification
{
    public interface IGamificationAppService: IApplicationService
    {
        Task<DTResult<TipDto>> GetTips(GetTipsInput input);
        Task<DTResult<QuizDto>> GetQuizzes(GetQuizzesInput input);
        Task<DTResult<AnswerDto>> GetAnswers(GetAnswersInput input);
        Task<bool> SetTipAsRead(SetTipAsReadInput input);
        Task<bool> CheckPersonAnswer(CheckPersonAnswerInput input);
        Task<GetLevelsOutput> GetLevels();
    }
}
