using System.Diagnostics;
using TestSystem.Models;

public class JudgeService
{
    private readonly IWebHostEnvironment _env;

    public JudgeService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<JudgeResult> RunSubmissionAsync(Submission submission)
    {
        var result = new JudgeResult();
        string workDir = Path.Combine(_env.ContentRootPath, "JudgeTemp", submission.Id.ToString());
        Directory.CreateDirectory(workDir);

        string codePath = Path.Combine(workDir, "solution.cpp");

        // сохраняем код
        await File.WriteAllTextAsync(codePath, submission.Code);

        // Компиляция
        var compile = RunProcess("g++", $"-O2 -std=c++17 solution.cpp -o solution", workDir, 5000);

        if (compile.ExitCode != 0)
        {
            result.Status = SubmissionStatus.CompilationError;
            result.ErrorMessage = compile.Error;
            return result;
        }

        // прогоняем по тестам
        var testsDir = Path.Combine(_env.ContentRootPath, "Tests", "Task1");
        var inFiles = Directory.GetFiles(testsDir, "*.in");

        result.TotalTests = inFiles.Length;

        foreach (var inputFile in inFiles)
        {
            string expected = File.ReadAllText(inputFile.Replace(".in", ".out")).Trim();

            var exec = RunProcess(
                "./solution",
                "",
                workDir,
                2000,                     
                File.ReadAllText(inputFile)
            );

            if (exec.ExitCode != 0)
            {
                result.Status = SubmissionStatus.RuntimeError;
                result.ErrorMessage = exec.Error;
                return result;
            }

            if (exec.Output.Trim() == expected)
                result.Passed++;
        }

        if (result.Passed == result.TotalTests)
            result.Status = SubmissionStatus.Accepted;
        else
            result.Status = SubmissionStatus.WrongAnswer;

        return result;
    }

    private (int ExitCode, string Output, string Error) RunProcess(
        string exe, string args, string workDir, int timeoutMs, string stdin = null)
    {
        var p = new Process();
        p.StartInfo.FileName = exe;
        p.StartInfo.Arguments = args;
        p.StartInfo.WorkingDirectory = workDir;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = stdin != null;
        p.Start();

        if (stdin != null)
            p.StandardInput.Write(stdin);

        p.WaitForExit(timeoutMs);

        if (!p.HasExited)
        {
            p.Kill();
            return (-1, "", "Time Limit Exceeded");
        }

        return (p.ExitCode, p.StandardOutput.ReadToEnd(), p.StandardError.ReadToEnd());
    }
}
