using System.Diagnostics;

namespace TestSystem.Services
{
    public class CodeRunner
    {
        private readonly ILogger<CodeRunner> _logger;

        public CodeRunner(ILogger<CodeRunner> logger)
        {
            _logger = logger;
        }

        public async Task<RunResult> RunAsync(string code, string language)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            string sourceFile;
            string execFile;

            switch (language.ToLower())
            {
                case "c++":
                    sourceFile = Path.Combine(tempDir, "program.cpp");
                    execFile = Path.Combine(tempDir, "program.exe");
                    await File.WriteAllTextAsync(sourceFile, code);

                    var compileResult = await RunProcessAsync("g++", $"\"{sourceFile}\" -o \"{execFile}\"");
                    if (!string.IsNullOrEmpty(compileResult.ErrorMessage))
                    {
                        return new RunResult
                        {
                            Status = "CompilationError",
                            Output = compileResult.Output,
                            ErrorMessage = compileResult.ErrorMessage
                        };
                    }

                    return await RunProcessAsync(execFile, "");

                case "python":
                    sourceFile = Path.Combine(tempDir, "program.py");
                    await File.WriteAllTextAsync(sourceFile, code);
                    return await RunProcessAsync("python", $"\"{sourceFile}\"");

                default:
                    throw new NotSupportedException($"Язык {language} не поддерживается.");
            }
        }

        private async Task<RunResult> RunProcessAsync(string fileName, string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = psi };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            _logger.LogInformation("RunProcess finished: {File} {Args}, ExitCode={Code}", fileName, args, process.ExitCode);

            return new RunResult
            {
                Output = output,
                ErrorMessage = error,
                ExitCode = process.ExitCode
            };
        }
    }

    public class RunResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string ErrorMessage { get; set; }
        public string Status { get; set; }  // CompilationError, RuntimeError, Accepted, WrongAnswer
    }
}