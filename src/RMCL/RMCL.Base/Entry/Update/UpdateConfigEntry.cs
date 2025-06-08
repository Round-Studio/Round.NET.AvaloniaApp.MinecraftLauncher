using RMCL.Base.Enum.Update;

namespace RMCL.Base.Entry.Update;

public class UpdateConfigEntry
{
    public UpdateBranch Branch { get; set; } = UpdateBranch.Release;
    public UpdateRoute Route { get; set; } = UpdateRoute.Official;
    public UpdateProxy Proxy { get; set; } = UpdateProxy.Official;
}