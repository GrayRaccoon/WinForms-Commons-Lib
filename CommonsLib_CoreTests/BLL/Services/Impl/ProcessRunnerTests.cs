using System.Threading.Tasks;
using CommonsLib_BLL.Services;
using CommonsLib_DAL.Config;
using CommonsLib_IOC.Config;
using NUnit.Framework;

namespace CommonsLib_CoreTests.BLL.Services.Impl
{
    [TestFixture]
    public class ProcessRunnerTests: BaseTestClass
    {
        [Test]
        public async Task RunNewProcess_MultiCommand_Success()
        {
            var processRunner = IoCManager.Resolver.ResolveInstance<IProcessRunner>();

            var statusCode = await processRunner.RunNewProcess(new[]
                {
                    "echo STARTING",
                    "touch test_file.txt",
                    "ls -al | grep test_file",
                    "rm test_file.txt",
                    "echo FINISHING"
                },
                workingDirectory: BasePathManager.BasePath, runner: "sh");
            
            Assert.AreEqual(0, statusCode);
        }
    }
}