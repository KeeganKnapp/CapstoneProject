/*
    FakeStorage holds list/dictionaries for different parts of the app
    time entries stores clock in and clock outs, assignmentsbyjobsite is
    hardcoded which employees are at which jobsite, todobyjobsite is a 
    sample todo list for each jobsite, timeoff holds submitted time off
    requests, and payhistory smaples the pay period and amounts
*/

using CapstoneAPI.Dtos;

namespace CapstoneAPI.Services;


public static class FakeStorage
{
    // storing all clock in/out entries during runtime
    public static readonly List<TimeEntryDto> timeEntries = new();

    // hardcoded jobsite list of employees assigned
    public static readonly Dictionary<int, List<AssignmentDto>> assignmentsByJobsite = new()
    {
        [1] = new()
        {
            new AssignmentDto(10, "Nick Neitenbach", "Apprentice"),
            new AssignmentDto(11, "Anthony Chaney", "Laborer")
        }
    };

    // hardcoded jobsite to do tasks
    public static readonly Dictionary<int, List<ToDoDto>> doDosByJobsite = new()
    {
        [1] = new()
        {
            new ToDoDto("Nick Neitenbach", "Drink beer and smoked cigarettes", true),
            new ToDoDto("Anthony Chaney", "Build wall", true)
        }
    };

    // storing submitted time off requests
    public static readonly List<TimeOffItemDto> timeOff = new();

    // sample payroll history
    public static readonly List<PayStubDto> payHistory = new()
    {
        new("2025-09-01 - 2025-09-15", 2200, 1800),
        new("2025-09-16 - 2025-09-30", 2150, 1750)
    };
}

/*
    needs replaced with EF Core services talking to our db
*/