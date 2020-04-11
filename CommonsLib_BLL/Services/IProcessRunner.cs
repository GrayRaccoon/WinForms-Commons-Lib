using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonsLib_BLL.Services
{

    /// <summary>
    /// Simple Process runner
    /// </summary>
    public interface IProcessRunner
    {
        /// <summary>
        /// Starts a new process and waits until it completes.
        /// </summary>
        /// <param name="commands">Set of commands to be run in process session.</param>
        /// <param name="workingDirectory">Initial working directory for the process.</param>
        /// <param name="runner">Process shell to use, it must be available in PATH (bash/CMD/sh/zsh)</param>
        /// <param name="onDataLine">Callback to receive new data lines.</param>
        /// <param name="onErrorLine">Callback to receive new error lines.</param>
        /// <returns>Process status code.</returns>
        Task<int> RunNewProcess(
            IEnumerable<string> commands, 
            string workingDirectory = ".",
            string runner = "bash",
            Action<string>? onDataLine = null,
            Action<string>? onErrorLine = null);
    }

}