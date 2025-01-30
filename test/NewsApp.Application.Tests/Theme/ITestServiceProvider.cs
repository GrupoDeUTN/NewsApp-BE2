internal interface ITestServiceProvider
{
    void ReplaceService<T>(T mockNewsService);
}