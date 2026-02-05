using MepoExpedicaoRfid;

Console.WriteLine("[SMOKE TEST] Starting...");

try
{
    var exitCode = await SmokeTestRunner.RunAsync(CancellationToken.None);
    Console.WriteLine($"[SMOKE TEST] Completed with exit code: {exitCode}");
    return exitCode;
}
catch (Exception ex)
{
    Console.WriteLine($"[SMOKE TEST] FATAL ERROR:\n{ex}");
    return 1;
}
