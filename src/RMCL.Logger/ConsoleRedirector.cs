using System.Collections.Concurrent;
using System.Text;

namespace RMCL.Logger;

public class ConsoleRedirector : IDisposable
{
    public static string FileName { get; private set; }
    
    private StreamWriter _writer;
    private TextWriter _originalOutput;
    private readonly string _timestampFormat;
    private static readonly ConcurrentDictionary<int, string> _threadNames = new ConcurrentDictionary<int, string>();

    /// <summary>
    /// 初始化控制台重定向器
    /// </summary>
    /// <param name="filePath">日志文件路径</param>
    /// <param name="timestampFormat">时间戳格式(默认: HH:mm:ss.fff)</param>
    public ConsoleRedirector(string filePath, string timestampFormat = "HH:mm:ss.fff")
    {
        FileName = filePath;
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        
        _originalOutput = Console.Out;
        _timestampFormat = timestampFormat;
        
        // 注册主线程名称
        RegisterThread(Thread.CurrentThread, "Main");
        
        _writer = new StreamWriter(filePath)
        {
            AutoFlush = true
        };
        
        Console.SetOut(new ThreadAwareTextWriter(_writer,_originalOutput, _timestampFormat));
    }

    /// <summary>
    /// 注册线程名称
    /// </summary>
    public static void RegisterThread(Thread thread, string name)
    {
        _threadNames.AddOrUpdate(thread.ManagedThreadId, name, (id, oldName) => name);
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _writer?.Dispose();
    }

    /// <summary>
    /// 自定义TextWriter，为每行添加线程名和时间戳
    /// </summary>
    private class ThreadAwareTextWriter : TextWriter
    {
        private readonly TextWriter _innerWriter;
        private readonly TextWriter _orgwriter;
        private readonly string _timestampFormat;
        private readonly StringBuilder _lineBuffer = new StringBuilder();

        public ThreadAwareTextWriter(TextWriter innerWriter,TextWriter orgwriter, string timestampFormat)
        {
            _innerWriter = innerWriter;
            _orgwriter = orgwriter;
            _timestampFormat = timestampFormat;
        }

        public override Encoding Encoding => _innerWriter.Encoding;

        public override void Write(char value)
        {
            if (value == '\n')
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                string threadName = GetThreadName(threadId);
                var timestamp = DateTime.Now.ToString(_timestampFormat);
                
                _innerWriter.Write($"[{timestamp}][TID {threadId}][{threadName}] {_lineBuffer}{value}");
                _lineBuffer.Clear();
            }
            else if (value != '\r')
            {
                _lineBuffer.Append(value);
            }
        }

        private string GetThreadName(int threadId)
        {
            if (_threadNames.TryGetValue(threadId, out var name))
            {
                return name;
            }
            
            // 自动为未命名的线程生成名称
            string newName = $"Undefinded Thread-{threadId}";
            _threadNames.TryAdd(threadId, newName);
            return newName;
        }

        public override void Write(string value)
        {
            if (value == null) return;
            
            foreach (char c in value)
            {
                Write(c);
            }
            Console.SetOut(_orgwriter);
            int threadId = Thread.CurrentThread.ManagedThreadId;
            string threadName = GetThreadName(threadId);
            var timestamp = DateTime.Now.ToString(_timestampFormat);
            Console.WriteLine($"[{timestamp}][TID {threadId}][{threadName}] {_lineBuffer}");
            Console.SetOut(this);
        }

        public override void WriteLine(string value)
        {
            Write(value);
            Write('\n');
        }

        public override void Flush()
        {
            if (_lineBuffer.Length > 0)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                string threadName = GetThreadName(threadId);
                var timestamp = DateTime.Now.ToString(_timestampFormat);
                _innerWriter.Write($"[{timestamp}][{threadName}] {_lineBuffer}");
                _lineBuffer.Clear();
            }
            _innerWriter.Flush();
        }
    }
}