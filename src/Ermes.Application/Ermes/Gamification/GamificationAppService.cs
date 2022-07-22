using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Answers;
using Ermes.Attributes;
using Ermes.Authorization;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Enums;
using Ermes.Ermes.Gamification.Dto;
using Ermes.EventHandlers;
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
        private readonly IBackgroundJobManager _backgroundJobManager;
        public GamificationAppService(
                TipManager tipManager,
                QuizManager quizManager,
                AnswerManager answerManager,
                ErmesAppSession session,
                PersonManager personManager,
                GamificationManager gamificationManager,
                IBackgroundJobManager backgroundJobManager
            )
        {
            _tipManager = tipManager;
            _quizManager = quizManager;
            _answerManager = answerManager;
            _session = session;
            _personManager = personManager;
            _gamificationManager = gamificationManager;
            _backgroundJobManager = backgroundJobManager;
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

        private async Task SendNotification(List<(EntityWriteAction Action, string NewValue)> list, string actionName)
        {
            foreach (var item in list)
            {
                NotificationEvent<GamificationNotificationDto> notification = new NotificationEvent<GamificationNotificationDto>(0,
                _session.LoggedUserPerson.Id,
                new GamificationNotificationDto()
                {
                    PersonId = _session.LoggedUserPerson.Id,
                    ActionName = actionName,
                    NewValue = item.NewValue
                },
                item.Action,
                true);
                await _backgroundJobManager.EnqueueEventAsync(notification);
            }
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

        public virtual async Task<GamificationResponse> SetTipAsRead(SetTipAsReadInput input)
        {
            var result = new GamificationResponse()
            {
                Response = new ResponseBaseDto()
                {
                    Success = true
                }
            };
            Person person = await _personManager.GetPersonByIdAsync(_session.LoggedUserPerson.Id);

            try
            {
                Tip tip = await _tipManager.GetTipByCodeAsync(input.TipCode);
                int id = await _personManager.CreatePersonTipAsync(_session.LoggedUserPerson.Id, input.TipCode);

                if (id > 0)
                {
                    
                    async Task<List<(EntityWriteAction, string NewValue)>> AssignRewards(long personId) {
                        List<(EntityWriteAction, string newValue)> result = new List<(EntityWriteAction, string newValue)>();
                        var action = await _gamificationManager.GetActionByNameAsync(ErmesConsts.GamificationActionConsts.READ_TIP);
                        var person = await _personManager.GetPersonByIdAsync(personId);
                        if(action != null && action.Achievements != null && action.Achievements.Count > 0)
                        {
                            var personTips = await _tipManager.GetTipsByPersonAsync(_session.LoggedUserPerson.Id);
                            foreach (var item in action.Achievements)
                            {
                                if(item is Medal)
                                {
                                    if(item.Detail.Threshold == personTips.Count)
                                    {
                                        await _gamificationManager.InsertAudit(_session.LoggedUserPerson.Id, null, item.Id, null);
                                        person.Points += item.Detail.Points;
                                        result.Add((EntityWriteAction.MedalObtained, item.Name));
                                    }

                                }
                                else if(item is Badge)
                                {
                                    Badge badge = (Badge)item;
                                    if (badge.CrisisPhase != tip.CrisisPhaseKey)
                                        continue;

                                    var personTipsByPhase = personTips.Where(a => a.CrisisPhaseKey == tip.CrisisPhaseKey).ToList();
                                    var tipsByPhaseCount = _tipManager.Tips.Where(t => t.CrisisPhaseKeyString == tip.CrisisPhaseKeyString).Count();
                                    if(tipsByPhaseCount == personTipsByPhase.Count)
                                    {
                                        await _gamificationManager.InsertAudit(_session.LoggedUserPerson.Id, null, item.Id, null);
                                        person.Points += badge.Detail.Points;
                                        result.Add((EntityWriteAction.BadgeObtained, badge.Name));
                                    }
                                }

                            }
                        }

                        return result;
                    }


                    //The list contains the information about the notifications to be sent
                    var list = await _gamificationManager.UpdatePersonGamificationProfileAsync(_session.LoggedUserPerson.Id, ErmesConsts.GamificationActionConsts.READ_TIP, AssignRewards);
                    await SendNotification(list, ErmesConsts.GamificationActionConsts.READ_TIP);
                }
                else
                {
                    var message = string.Format("Person {0} has already read tip {1}", _session.LoggedUserPerson.Id, input.TipCode);
                    Logger.Error(message);
                    result.Response.Success = false;
                    result.Response.ErrorMessage = message;
                }
            }
            catch(Exception e)
            {
                var message = string.Format("Error while inserting PersonTip: {0}", e.Message);
                Logger.Error(message);
                result.Response.Success = false;
                result.Response.ErrorMessage = message;
            }

            if (result.Response.Success)
                result.Gamification = new GamificationBaseDto(person.Points, person.LevelId);

            return result;
        }

        public virtual async Task<GamificationResponse> CheckPersonAnswer(CheckPersonAnswerInput input)
        {
            var result = new GamificationResponse()
            {
                Response = new ResponseBaseDto()
                {
                    Success = true
                }
            };
            Person person = await _personManager.GetPersonByIdAsync(_session.LoggedUserPerson.Id);
            try
            {
                var ans = await _answerManager.GetAnswerByCodeAsync(input.AnswerCode);
                Quiz quiz = await  _quizManager.GetQuizByCodeAsync(ans.QuizCode);
                if (ans.IsTheRightAnswer)
                {
                    await _personManager.CreatePersonQuizAsync(_session.LoggedUserPerson.Id, ans.QuizCode);

                    async Task<List<(EntityWriteAction, string NewValue)>> AssignRewards(long personId)
                    {
                        List<(EntityWriteAction, string newValue)> result = new List<(EntityWriteAction, string newValue)>();
                        var action = await _gamificationManager.GetActionByNameAsync(ErmesConsts.GamificationActionConsts.ANSWER_QUIZ);
                        var person = await _personManager.GetPersonByIdAsync(personId);
                        if (action != null && action.Achievements != null && action.Achievements.Count > 0)
                        {
                            var personQuizzes = await _quizManager.GetQuizzesByPersonAsync(_session.LoggedUserPerson.Id);
                            foreach (var item in action.Achievements)
                            {
                                if (item is Medal)
                                {
                                    if (item.Detail.Threshold == personQuizzes.Count)
                                    {
                                        await _gamificationManager.InsertAudit(_session.LoggedUserPerson.Id, null, item.Id, null);
                                        person.Points += item.Detail.Points;
                                        result.Add((EntityWriteAction.MedalObtained, item.Name));
                                    }

                                }
                                else if (item is Badge)
                                {
                                    Badge badge = (Badge)item;
                                    if (badge.CrisisPhase != quiz.CrisisPhaseKey)
                                        continue;

                                    var personQuizsByPhase = personQuizzes.Where(a => a.CrisisPhaseKey == quiz.CrisisPhaseKey).ToList();
                                    var quizzesByPhaseCount = _quizManager.Quizzes.Where(t => t.CrisisPhaseKeyString == quiz.CrisisPhaseKeyString).Count();
                                    if (quizzesByPhaseCount == personQuizsByPhase.Count)
                                    {
                                        await _gamificationManager.InsertAudit(_session.LoggedUserPerson.Id, null, item.Id, null);
                                        person.Points += badge.Detail.Points;
                                        result.Add((EntityWriteAction.BadgeObtained, badge.Name));
                                    }
                                }

                            }
                        }

                        return result;
                    }

                    //The list contains the information about the notification to be sent
                    var list = await _gamificationManager.UpdatePersonGamificationProfileAsync(_session.LoggedUserPerson.Id, ErmesConsts.GamificationActionConsts.ANSWER_QUIZ, AssignRewards);
                    await SendNotification(list, ErmesConsts.GamificationActionConsts.ANSWER_QUIZ);
                }
                else
                {
                    var message = string.Format("This is not the right answer: {0}", input.AnswerCode);
                    Logger.Error(message);
                    result.Response.Success = false;
                    result.Response.ErrorMessage = message;
                }
            }
            catch (Exception e)
            {
                var message = string.Format("Error while inserting PersonQuiz: {0}", e.Message);
                Logger.Error(message);
                result.Response.Success = false;
                result.Response.ErrorMessage = message;
            }

            if (result.Response.Success)
                result.Gamification = new GamificationBaseDto(person.Points, person.LevelId);

            return result;
        }

        public async Task<GetLevelsOutput> GetLevels()
        {
            var levels = await _gamificationManager.GetLevelsAsync();
            return new GetLevelsOutput() { Levels = ObjectMapper.Map<List<LevelDto>>(levels) };
        }

        public virtual async Task<GetLeaderboardOutput> GetLeaderboard()
        {
            var competitors = await _personManager
                                .Persons
                                .Include(p => p.Level)
                                .Join(
                                    _personManager.PersonRoles,
                                    a => a.Id,
                                    b => b.PersonId,
                                    (a, b) => new { Person = a, role = b.Role }
                                )
                                .Where(ab => ab.role.Name == AppRoles.CITIZEN)
                                .Select(ab => ab.Person)
                                .OrderByDescending(p => p.Points)
                                .ThenBy(p => p.Id)
                                .ToListAsync();

            int indexOfPerson = competitors.FindIndex(0, p => p.Id == _session.LoggedUserPerson.Id);
            var subList = competitors
                            .Select((p, index) => new
                            {
                                Person = p,
                                Index = ++index
                            })
                            .Where((p, index) => index >= indexOfPerson - 2 && index <= indexOfPerson + 2)
                            .ToList();

            var result = new GetLeaderboardOutput();
            
            foreach (var item in subList)
            {
                var newCompetitor = ObjectMapper.Map<GamificationBaseDto>(item.Person);
                newCompetitor.Position = item.Index;
                result.Competitors.Add(newCompetitor);
            }

            return result;
        }
    }
}
