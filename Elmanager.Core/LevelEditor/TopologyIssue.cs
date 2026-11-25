namespace Elmanager.LevelEditor;

public union TopologyIssue(TopologyIssue.Error, TopologyIssue.Warning)
{
    public record Error(string Message);
    public record Warning(string Message);

    public string Message => this switch
    {
        Error error => error.Message,
        Warning warning => warning.Message
    };
}
