namespace DataSyncClient
{
    using System;
    using Dotmim.Sync;

    internal class ConsoleLogProgress : IProgress<ProgressArgs>
    {
        public void Report(ProgressArgs args)
        {
            Console.WriteLine($"{args.ProgressPercentage:p}:  \t[{args.Source[..Math.Min(4, args.Source.Length)]}] {args.TypeName}: {args.Message}");
        }
    }
}
