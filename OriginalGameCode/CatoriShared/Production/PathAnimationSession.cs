using CatoriApp.Core.Objects.Production;
using CommunityToolkit.Mvvm.Messaging;

namespace CatoriApp.Game.Objects.AnimationOnPath;

public sealed class PathAnimationSession:IDisposable
{
    private readonly List<PathAnimationHandle> _handles=[];
    // Robot names are identities. Case-insensitive matching makes Robot4 and
    // robot4 consume the same pickup and overwrite each other's drop target.
    private readonly Dictionary<string,string> _dropPathsByRobot=new(StringComparer.Ordinal);
    private readonly HashSet<string> _dropTargetPaths=new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _deferredPaths=new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string,(string Target,TimeSpan Delay)> _pathHandoffs=new(StringComparer.OrdinalIgnoreCase);
    private bool _disposed;

    public PathAnimationSession()
    {
        WeakReferenceMessenger.Default
            .Register<RobotPartTransferMessage>(this,
                static (recipient,message)=>((PathAnimationSession)recipient).HandlePartTransfer(message));
        WeakReferenceMessenger.Default.Register<CatoriApp.Core.Objects.Arguments.AnimationCompleteMessage>(this,
            static (recipient,message)=>((PathAnimationSession)recipient).HandleAnimationComplete(message));
    }

    public IReadOnlyList<PathAnimationHandle> Handles=>_handles;

    public void Add(PathAnimationHandle handle)
    {
        ObjectDisposedException.ThrowIf(_disposed,this);
        ArgumentNullException.ThrowIfNull(handle);
        _handles.Add(handle);
    }

    public void StartAll()
    {
        ObjectDisposedException.ThrowIf(_disposed,this);
        foreach(var handle in _handles)handle.Start();
    }

    public bool StartPath(string animationName)
    {
        ObjectDisposedException.ThrowIf(_disposed,this);
        if(string.IsNullOrWhiteSpace(animationName))return false;
        var handle=_handles.FirstOrDefault(candidate=>
            string.Equals(candidate.AnimationName,animationName,StringComparison.OrdinalIgnoreCase));
        if(handle==null)return false;
        handle.Start();
        return true;
    }

    public void AddDropHandoff(string robotName,string destinationPathName)
    {
        ObjectDisposedException.ThrowIf(_disposed,this);
        ArgumentException.ThrowIfNullOrWhiteSpace(robotName);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPathName);
        _dropPathsByRobot[robotName]=destinationPathName;
        _dropTargetPaths.Add(destinationPathName);
    }

    public void AddDeferredPath(string pathName)
    {
        ObjectDisposedException.ThrowIf(_disposed,this);
        ArgumentException.ThrowIfNullOrWhiteSpace(pathName);
        _deferredPaths.Add(pathName);
    }

    public void AddPathHandoff(string sourcePathName,string targetPathName,TimeSpan delay)
    {
        ObjectDisposedException.ThrowIf(_disposed,this);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePathName);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetPathName);
        if(delay<TimeSpan.Zero)throw new ArgumentOutOfRangeException(nameof(delay));
        _pathHandoffs[sourcePathName]=(targetPathName,delay);
        _deferredPaths.Add(targetPathName);
    }

    public void StartRoots()
    {
        ObjectDisposedException.ThrowIf(_disposed,this);
        foreach(var handle in _handles)
        {
            if(_dropTargetPaths.Contains(handle.AnimationName)
                ||_deferredPaths.Contains(handle.AnimationName))
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Production path '{handle.AnimationName}' is a drop target; hiding it until handoff.");
                handle.Host.Visibility=System.Windows.Visibility.Collapsed;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Production path '{handle.AnimationName}' is a root; starting with host " +
                    $"visibility {handle.Host.Visibility} and content '{handle.Host.Content?.GetType().FullName}'.");
                handle.Start();
            }
        }
    }

    private void HandlePartTransfer(RobotPartTransferMessage message)
    {
        if(message.Stage!=RobotPartTransferStage.Dropped
            ||message.Part==null
            ||!_dropPathsByRobot.TryGetValue(message.RobotName,out string? pathName))return;
        var handle=_handles.FirstOrDefault(candidate=>
            string.Equals(candidate.AnimationName,pathName,StringComparison.OrdinalIgnoreCase));
        if(handle==null)return;

        async void ContinueProduction()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(650));
            if(_disposed||!_handles.Contains(handle))return;
            handle.StartWithPart(message.Part,message.PartName);
        }
        if(handle.Host.Dispatcher.CheckAccess())ContinueProduction();
        else handle.Host.Dispatcher.BeginInvoke((Action)ContinueProduction);
    }

    private void HandleAnimationComplete(CatoriApp.Core.Objects.Arguments.AnimationCompleteMessage message)
    {
        if(message.Action!=ProductionAction.StartPath
            ||!_pathHandoffs.TryGetValue(message.AnimationName,out var handoff))return;
        var handle=_handles.FirstOrDefault(candidate=>
            string.Equals(candidate.AnimationName,handoff.Target,StringComparison.OrdinalIgnoreCase));
        if(handle==null)return;
        var sourceHandle=_handles.FirstOrDefault(candidate=>
            string.Equals(candidate.AnimationName,message.AnimationName,StringComparison.OrdinalIgnoreCase));
        async void ContinuePath()
        {
            if(handoff.Delay>TimeSpan.Zero)await Task.Delay(handoff.Delay);
            if(_disposed||!_handles.Contains(handle))return;
            // The source stays parked at HoldEnd during the delay. Transfer the
            // same visual only when the receiving path is ready to start.
            FrameworkElement? part=sourceHandle?.ReleasePartForPathHandoff();
            if(part!=null)handle.StartWithPart(part,message.PartName);
            else handle.Start();
        }
        if(handle.Host.Dispatcher.CheckAccess())ContinuePath();
        else handle.Host.Dispatcher.BeginInvoke((Action)ContinuePath);
    }

    public void StopAll()
    {
        if(_disposed)return;
        foreach(var handle in _handles)handle.Stop();
    }

    public void Clear()
    {
        if(_disposed)return;
        foreach(var handle in _handles)handle.Dispose();
        _handles.Clear();
        _dropPathsByRobot.Clear();
        _dropTargetPaths.Clear();
        _deferredPaths.Clear();
        _pathHandoffs.Clear();
    }

    public void Dispose()
    {
        if(_disposed)return;
        Clear();
        WeakReferenceMessenger.Default.UnregisterAll(this);
        _disposed=true;
    }
}
