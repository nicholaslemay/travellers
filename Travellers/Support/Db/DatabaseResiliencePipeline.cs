using Polly;

namespace Travellers.Support.Db;

public static class DatabaseResiliencePipeline
{
    public static ResiliencePipeline Build(DatabaseResilienceOptions options, TimeProvider timeProvider) =>
        new ResiliencePipelineBuilder { TimeProvider = timeProvider }
            .AddTimeout(options.Timeout)
            .Build();
}
