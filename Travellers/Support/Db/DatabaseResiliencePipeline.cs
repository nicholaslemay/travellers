using Polly;

namespace Travellers.Support.Db;

public static class DatabaseResiliencePipeline
{
    public static void Configure(ResiliencePipelineBuilder builder, DatabaseResilienceOptions options) =>
        builder.AddTimeout(options.Timeout);
}
