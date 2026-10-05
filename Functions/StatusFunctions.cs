using System.ComponentModel;

namespace GamebookAgents.Functions;

public static class StatusFunctions
{
    [Description("Report the agent's current or ongoing status to the user through the console.")]
    public static string ReportStatus(
        [Description("A concise message describing the agent's current status or progress.")] string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "Error: A status message is required.";
        }

        Console.WriteLine($"Agent status: {message}");
        return "Status reported successfully.";
    }
}
