using Xunit;

// Integration tests share one database, so they must not run at the same time.
[assembly: CollectionBehavior(DisableTestParallelization = true)] 