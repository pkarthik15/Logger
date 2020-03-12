using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;
using System.IO;

namespace Logger
{
    public static class Logger
    {
        public static ILog GetLoggerInstance(string fileName, string filepath)
        {
            var logFileDirectoryPath = filepath;
            var logFilesDirectory = new DirectoryInfo(logFileDirectoryPath);
            if (!logFilesDirectory.Exists)
            {
                logFilesDirectory.Create();
            }

            var logfilePath = logFileDirectoryPath + "\\" + fileName + ".txt";

            PatternLayout layout = new PatternLayout("%date %level - %message%newline");
            LevelMatchFilter filter = new LevelMatchFilter
            {
                LevelToMatch = Level.All
            };
            filter.ActivateOptions();


            string repositoryName = string.Format("{0}Repository", fileName);
            ILoggerRepository repository;


            var repos = LoggerManager.GetAllRepositories();
            var repoExists = false;

            foreach (var operationRepo in repos)
            {
                if (repositoryName == operationRepo.Name)
                {
                    repoExists = true;
                    break;
                }
            }
            RollingFileAppender appender = new RollingFileAppender(); ;

            if (!repoExists)
            {

                repository = LoggerManager.CreateRepository(repositoryName);

                appender = new RollingFileAppender
                {
                    File = string.Concat(logFileDirectoryPath, fileName, ".txt"),
                    ImmediateFlush = true,
                    AppendToFile = true,
                    RollingStyle = RollingFileAppender.RollingMode.Date,
                    DatePattern = "-yyyy-MM-dd",
                    LockingModel = new FileAppender.MinimalLock(),
                    Name = string.Format("{0}Appender", fileName)
                };
                appender.AddFilter(filter);
                appender.Layout = layout;
                appender.ActivateOptions();
            }
            else
            {
                repository = LoggerManager.GetRepository(repositoryName);
            }


            string loggerName = string.Format("{0}Logger", fileName);
            BasicConfigurator.Configure(repository, appender);
            //BasicConfigurator.Configure()
            ILog logger = LogManager.GetLogger(repositoryName, loggerName);

            return logger;

        }
    }
}
