using System.Diagnostics;

namespace TestSystem.Services
{
    public class CodeRunResult
    {
        public string Output { get; set; }
        public string Error { get; set; }
    }

    public class CompilationResult
    {
        public bool Success { get; set; }
        public string ExecutablePath { get; set; }
        public string WorkingDirectory { get; set; }
        public string Error { get; set; }
    }

    public class CodeRunner : IDisposable
    {
        private readonly ILogger<CodeRunner> _logger;

        private const string GppPath = @"E:\dev\w64devkit\bin\g++.exe";
        private const string W64Bin = @"E:\dev\w64devkit\bin";

        private readonly List<string> _tempDirectories = new();

        public CodeRunner(ILogger<CodeRunner> logger)
        {
            _logger = logger;
        }

        public async Task<CompilationResult> CompileAsync(string code, string lang)
        {
            string workDir = Path.Combine(AppContext.BaseDirectory, "Temp", "code_" + Guid.NewGuid());
            Directory.CreateDirectory(workDir);
            _tempDirectories.Add(workDir);

            string srcFile = Path.Combine(workDir, "main.cpp");
            string exeFile = Path.Combine(workDir, "a.exe");

            await File.WriteAllTextAsync(srcFile, code);

            _logger.LogInformation("Compiling in directory: {dir}", workDir);

            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GppPath,
                    Arguments = $"\"{srcFile}\" -O2 -o \"{exeFile}\"",
                    WorkingDirectory = workDir,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            compile.StartInfo.EnvironmentVariables["PATH"] =
                W64Bin + ";" + Environment.GetEnvironmentVariable("PATH");

            _logger.LogInformation("Compile command: {exe} {args}", compile.StartInfo.FileName, compile.StartInfo.Arguments);

            compile.Start();

            string compileOut = await compile.StandardOutput.ReadToEndAsync();
            string compileErr = await compile.StandardError.ReadToEndAsync();

            compile.WaitForExit(5000);

            if (!compile.HasExited)
            {
                compile.Kill(true);
                return new CompilationResult 
                { 
                    Success = false,
                    Error = "Compilation error: compiler hang (timeout)" 
                };
            }

            if (compile.ExitCode != 0)
            {
                _logger.LogError("Compilation failed: {err}", compileErr);
                return new CompilationResult 
                { 
                    Success = false,
                    Error = "Compilation error:\n" + compileErr 
                };
            }

            _logger.LogInformation("Compilation successful, executable: {file}", exeFile);

            return new CompilationResult
            {
                Success = true,
                ExecutablePath = exeFile,
                WorkingDirectory = workDir
            };
        }

        public async Task<CodeRunResult> RunAsync(string executablePath, string workingDirectory, string input)
        {
            _logger.LogInformation("Running compiled file: {file}", executablePath);

            var run = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            run.Start();

            if (!string.IsNullOrEmpty(input))
                await run.StandardInput.WriteAsync(input);

            run.StandardInput.Close();

            string output = await run.StandardOutput.ReadToEndAsync();
            string errorOutput = await run.StandardError.ReadToEndAsync();

            bool exited = run.WaitForExit(2000);
            if (!exited)
            {
                try { run.Kill(true); } catch { }

                return new CodeRunResult
                {
                    Error = "Runtime error: Time limit exceeded (2 seconds)"
                };
            }

            return new CodeRunResult
            {
                Output = output.Trim(),
                Error = errorOutput.Trim()
            };
        }

        public void Dispose()
        {
            foreach (var dir in _tempDirectories)
            {
                try
                {
                    if (Directory.Exists(dir))
                    {
                        Directory.Delete(dir, true);
                        _logger.LogInformation("Cleaned up temporary directory: {dir}", dir);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete temporary directory: {dir}", dir);
                }
            }
        }
    }
}