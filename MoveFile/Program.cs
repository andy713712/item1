using Microsoft.Extensions.Configuration;
using Serilog;

try
{
    var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

    string src = config["FileTransferSettings:src"] ?? "C:\\Tmp\\MoveFile\\src";
    string dest = config["FileTransferSettings:dest"] ?? "C:\\Tmp\\MoveFile\\dest";
    string logPath = config["FileTransferSettings:LogPath"] ?? "C:\\Tmp\\MoveFile\\log";

    Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()  
                .WriteTo.Console()     
                .WriteTo.File($"{logPath}/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();


    // 檢查來源目錄是否存在
    if (!Directory.Exists(src))
    {
        Log.Error($"來源目錄不存在: {src}");
        return;
    }

    // 檢查或創建目標目錄
    if (!Directory.Exists(dest))
    {
        Directory.CreateDirectory(dest);
    }

    string[] files = Directory.GetFiles(src);
    foreach (string file in files)
    {
        string fileName = Path.GetFileName(file);
        string destFile = Path.Combine(dest, fileName);

        // 複製檔案
        File.Move(file, destFile);
        Log.Information($"{fileName} 檔案已複製");

        // 刪除檔案
        File.Delete(file);
        Log.Information($"{fileName} 檔案已刪除");
    }
}
catch (Exception ex)
{
    Log.Error(ex, "發生例外錯誤");
}
finally
{
    Log.CloseAndFlush();
}
