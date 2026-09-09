namespace CatoriShared.Hotspots;

public enum HotspotActionType
{
    None,
    OpenImageWindow,
    ShowMessage,
    InvokeButton,
    OpenWindow,
    StartPath
}

public sealed class HotspotActionDefinition
{
    public HotspotActionType ActionType { get; set; }
    public string? Target { get; set; }
    public string? ImagePath { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? ToolTip { get; set; }
    public bool HighlightOnHover { get; set; } = true;
}
