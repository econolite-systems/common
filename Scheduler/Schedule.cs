// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;

namespace Econolite.Ode.Common.Scheduler.Base;

public record Schedule();

public record ScheduleEntry(
    string Name,
    bool IsEnabled,
    IEnumerable<CalendarDates> ExceptionDates,
    IScheduleAction Action,
    DateTime LastModified,
    string TimeZoneId
    );

public record Recurrence(
    ScheduleDate StartDate,
    ScheduleDate? EndDate,
    ScheduleTime StartTime,
    ScheduleTime? EndTime,
    bool HasNoEnd,
    bool IsRepeatedEveryYear,
    int OccurrenceDurationInMinutes,
    string CronExpression
    );

public record CalendarDates(
    Guid Id,
    string Name,
    string Description,
    bool Include,
    IEnumerable<string> DaysCronExpression,
    DateTime LastModified
    );

public record ScheduleDate(int Year, int Month, int Day);
public record ScheduleTime(int Hour, int Minute);

public interface IScheduleAction
{
    string Topic { get; }
    JsonDocument Parameters { get; }
}


