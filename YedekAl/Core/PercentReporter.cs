using System;

namespace YedekAl
{
    // Forwards progress only when the whole percentage changes, so the UI thread is not flooded.
    internal sealed class PercentReporter
    {
        private readonly IProgress<int> progress;
        private readonly long total;
        private int last = -1;

        public PercentReporter(IProgress<int> progress, long total)
        {
            this.progress = progress;
            this.total = Math.Max(1, total);
        }

        public void report(long done)
        {
            reportPercent((int)Math.Min(100, done * 100 / total));
        }

        public void reportPercent(int percent)
        {
            if (progress == null || percent == last) return;
            last = percent;
            progress.Report(percent);
        }
    }
}
