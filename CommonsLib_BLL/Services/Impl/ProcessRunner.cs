using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommonsLib_DAL.Attributes;
using Serilog;

namespace CommonsLib_BLL.Services.Impl
{
    /// <inheritdoc/>
    [Component]
    public class ProcessRunner: IProcessRunner
    {
        private ILogger _logger;

        public ILogger Logger
        {
            get => _logger;
            set => _logger = value.ForContext(GetType());
        }


        /// <inheritdoc/>
        public Task<int> RunNewProcess(IEnumerable<string> commands, 
            string workingDirectory = ".", 
            string runner = "bash", 
            Action<string>? onDataLine = null,
            Action<string>? onErrorLine = null)
        {
            return Task.Run(() =>
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = runner,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                };
                var process = Process.Start(processInfo);

                process.OutputDataReceived += (sender, args) =>
                {
                    var data = args.Data;
                    _logger.Information(data);
                    onDataLine?.Invoke(data);
                };
                
                process.ErrorDataReceived += (sender, args) =>
                {
                    var data = args.Data;
                    _logger.Error(data);
                    onErrorLine?.Invoke(data);
                };
                
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                
                using (var sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        foreach (var cmd in commands)
                            sw.WriteLine (cmd);
                    }
                }
                
                process.WaitForExit();
                return process.ExitCode;
            });
        }
    }

}