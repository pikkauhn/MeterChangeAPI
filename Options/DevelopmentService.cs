using MeterChangeApi.Options;
public class DevelopmentService
{
    public DevelopmentOptions Options {get; set;}
    public DevelopmentService(DevelopmentOptions options)
    {
        Options = options;
    }
}