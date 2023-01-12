using Abp.UI;
using Ermes.Attributes;
using Ermes.EntityHistory;
using Ermes.Helpers;
using Ermes.Interfaces;
using Ermes.Logging.Dto;
using Ermes.EntityFrameworkCore;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ermes.Logging
{
    //[ErmesAuthorize]
    public class WebLogAppService : ErmesAppServiceBase, IWebLogAppService
    {
        private readonly IAppFolders _appFolders;
        private readonly EntityHistoryManager _entityHistoryManager;

        public WebLogAppService(IAppFolders appFolders, EntityHistoryManager entityHistoryManager)
        {
            _appFolders = appFolders;
            _entityHistoryManager = entityHistoryManager;
        }

        public virtual GetLatestWebLogsOutput GetLatestWebLogs(GetLatestWebLogsInput input)
        {
            var directory = new DirectoryInfo(_appFolders.WebLogsFolder);
            var lastLogFile = directory.GetFiles("*.txt", SearchOption.AllDirectories)
                                        .OrderByDescending(f => f.LastWriteTime)
                                        .FirstOrDefault();

            if (lastLogFile == null)
            {
                return new GetLatestWebLogsOutput();
            }

            var lines = FileHelper.ReadLines(lastLogFile.FullName).Reverse().Take(input.NumberOfRows).Reverse().ToList();
            var logLineCount = 0;
            var lineCount = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("DEBUG") ||
                    line.StartsWith("INFO") ||
                    line.StartsWith("WARN") ||
                    line.StartsWith("ERROR") ||
                    line.StartsWith("FATAL"))
                {
                    logLineCount++;
                }

                lineCount++;

                if (logLineCount == input.NumberOfRows)
                {
                    break;
                }
            }

            return new GetLatestWebLogsOutput
            {
                LatesWebLogLines = lines.Take(lineCount).ToList()
            };
        }

        private List<ChangeDto> MergeTruncated(List<ChangeDto> raws) 
        {
            if (raws.Any(cd => cd.SplitIndex != null))
            {
                IEnumerable<IGrouping<String, ChangeDto>> grouped = raws.GroupBy(cd => cd.PropertyName);

                List<ChangeDto> notTruncated = grouped.Where(g => g.Count() == 1)
                    .Select(g => g.FirstOrDefault())
                    .ToList();

                foreach (IGrouping<string, ChangeDto> g in grouped.Where(g => g.Count() > 1))
                {
                    IOrderedEnumerable<ChangeDto> sorted = g.OrderBy(cd => cd.PropertyName).ThenBy(cd => cd.SplitIndex);

                    ChangeDto cdmaster = sorted.FirstOrDefault();

                    foreach (ChangeDto cd in sorted.Skip(1))
                    {
                        cdmaster.NewValue += (cd.NewValue ?? "");
                        cdmaster.OriginalValue += (cd.OriginalValue ?? "");
                    }

                    notTruncated.Add(cdmaster);
                }
                return notTruncated;
            }
            else
                return raws;
        }

        [OpenApiOperation("Get Entity History",
            @"
                Allowed values for EntityTypeName input field:
                    Communication,
                    Mission,
                    Notification,
                    Organization,
                    Person,
                    Preference,
                    ReportRequest,
                    Report,
                    PersonAction
            "
        )]
        public virtual EntityHistoryOutputDto GetEntityHistory(EntitySelectDto input)
        {
            List<string> EntitiesNames = typeof(ErmesDbContext).GetProperties()
                .Select(p => p.PropertyType.GenericTypeArguments)
                .Where(ts => ts != null && ts.Length > 0)
                .Select(ts => ts[0].Name)
                .ToList();

            if (!EntitiesNames.Contains(input.EntityTypeName))
                throw new UserFriendlyException(L("UnexistentEntityTypeName", input.EntityTypeName));

            List<SplitEntityChange> changes = _entityHistoryManager.EntityChanges
                .Where(ec => ec.EntityTypeName == input.EntityTypeName && ec.EntityId == input.Id)
                .ToList();

            List<EntityHistoryOutputItemDto> historyItems = changes
                .Join(_entityHistoryManager.EntityChangeSets, ec => ec.EntityChangeSetId, ecs => ecs.Id, (ec, ecs) => new Tuple<SplitEntityChange, SplitEntityChangeSet>(ec, ecs))
                .OrderBy(t => t.Item1.ChangeTime)
                .ToList()
                .Select(t => new EntityHistoryOutputItemDto()
                {
                    ChangeAuthor = ObjectMapper.Map<ChangeAuthorDto>(t.Item2),
                    ChangeInfo = ObjectMapper.Map<ChangeInfoDto>(t.Item1),
                    Changes = MergeTruncated(ObjectMapper.Map<List<ChangeDto>>(t.Item1.PropertyChanges))
                })
                .ToList();

            return new EntityHistoryOutputDto()
            {
                HistoryItems = historyItems
            };
        }
        [OpenApiOperation("Get Entity Histories",
            @"
                Allowed values for EntityTypeName input field:
                    Communication,
                    Mission,
                    Notification,
                    Organization,
                    Person,
                    Preference,
                    ReportRequest,
                    Report,
                    PersonAction
            "
        )]
        public virtual EntityHistoryOutputDto GetEntityHistories(EntityHistoriesInputDto input)
        {
            List<string> EntitiesNames = typeof(ErmesDbContext).GetProperties()
                .Select(p => p.PropertyType.GenericTypeArguments)
                .Where(ts => ts != null && ts.Length > 0)
                .Select(ts => ts[0].Name)
                .ToList();

            IQueryable<SplitEntityChange> changes = _entityHistoryManager.EntityChanges;

            if (input.EntityTypeNames != null && input.EntityTypeNames.Count > 0)
            {
                foreach (String EntityTypeName in input.EntityTypeNames)
                    if (!EntitiesNames.Contains(EntityTypeName))
                        throw new UserFriendlyException(L("UnexistentEntityTypeName", EntityTypeName));
                changes = changes.Where(ec => input.EntityTypeNames.Contains(ec.EntityTypeName));
            }
            if (input.StartDate != null && input.StartDate != DateTime.MinValue)
                changes = changes.Where(ec => ec.ChangeTime >= input.StartDate);
            if (input.EndDate != null && input.EndDate != DateTime.MinValue)
                changes = changes.Where(ec => ec.ChangeTime <= input.EndDate);

            List<SplitEntityChange> change_list = changes.ToList();

            List<EntityHistoryOutputItemDto> historyItems = change_list
                .Join(_entityHistoryManager.EntityChangeSets, ec => ec.EntityChangeSetId, ecs => ecs.Id, (ec, ecs) => new Tuple<SplitEntityChange, SplitEntityChangeSet>(ec, ecs))
                .OrderBy(t => t.Item1.ChangeTime)
                .ToList()
                .Select(t => new EntityHistoryOutputItemDto()
                {
                    ChangeAuthor = ObjectMapper.Map<ChangeAuthorDto>(t.Item2),
                    ChangeInfo = ObjectMapper.Map<ChangeInfoDto>(t.Item1),
                    Changes = MergeTruncated(ObjectMapper.Map<List<ChangeDto>>(t.Item1.PropertyChanges))
                })
                .ToList();

            return new EntityHistoryOutputDto()
            {
                HistoryItems = historyItems
            };
        }
    }
}
