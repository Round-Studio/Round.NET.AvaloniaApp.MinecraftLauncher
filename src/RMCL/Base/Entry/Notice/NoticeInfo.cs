using RMCL.Base.Enum.Notice;

namespace RMCL.Base.Entry.Notice;

public class NoticeInfo
{
    public string Title { get; set; } = "Info";
    public string Message { get; set; } = "Message";
    public NoticeType NoticeType { get; set; } = NoticeType.Info;
}