using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using YTStdLogger.Core;

namespace YTStdTenantPlatform.Infrastructure.Scheduling
{
    /// <summary>后台任务调度器，管理所有后台定时任务的生命周期</summary>
    public sealed class BackgroundTaskScheduler : BackgroundService
    {
        /// <summary>任务列表</summary>
        private readonly IReadOnlyList<IScheduledTask> _tasks;

        /// <summary>构造后台任务调度器</summary>
        public BackgroundTaskScheduler()
        {
            _tasks = BuildTasks();
        }

        /// <summary>启动所有后台任务</summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.Info(0, 0, "[BackgroundTaskScheduler] 后台任务调度器已启动, 任务数=" + _tasks.Count);

            var taskRunners = new List<Task>(_tasks.Count);
            for (int i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                taskRunners.Add(RunTaskLoop(task, stoppingToken));
            }

            await Task.WhenAll(taskRunners);

            Logger.Info(0, 0, "[BackgroundTaskScheduler] 后台任务调度器已停止");
        }

        /// <summary>运行单个任务的循环</summary>
        private static async Task RunTaskLoop(IScheduledTask task, CancellationToken ct)
        {
            Logger.Info(0, 0, "[BackgroundTaskScheduler] 任务已注册: " + task.Name +
                " 间隔=" + task.IntervalSeconds + "秒");

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(task.InitialDelaySeconds), ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                return;
            }

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    Logger.Debug(0, 0, () => BuildTaskExecuteDebugMessage(task.Name));
                    await task.ExecuteAsync(ct);
                    await Task.Delay(TimeSpan.FromSeconds(task.IntervalSeconds), ct);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Error(0, 0, "[BackgroundTaskScheduler] 任务执行异常: " +
                        task.Name + " - " + ex.Message);
                }
            }
        }

        /// <summary>构建任务列表</summary>
        private static IReadOnlyList<IScheduledTask> BuildTasks()
        {
            return new IScheduledTask[]
            {
                new TenantInitializationTask(),
                new ExpiryReminderTask(),
                new BillingGenerationTask(),
                new WebhookRetryTask()
            };
        }

        private static string BuildTaskExecuteDebugMessage(string taskName)
        {
            const string prefix = "[BackgroundTaskScheduler] 执行任务: ";
            return string.Create(
                prefix.Length + taskName.Length,
                taskName,
                static (span, value) =>
                {
                    prefix.AsSpan().CopyTo(span);
                    value.AsSpan().CopyTo(span[prefix.Length..]);
                });
        }
    }

    /// <summary>定时任务接口</summary>
    public interface IScheduledTask
    {
        /// <summary>任务名称</summary>
        string Name { get; }

        /// <summary>执行间隔（秒）</summary>
        int IntervalSeconds { get; }

        /// <summary>首次延迟（秒）</summary>
        int InitialDelaySeconds { get; }

        /// <summary>执行任务逻辑</summary>
        ValueTask ExecuteAsync(CancellationToken cancellationToken);
    }
}
