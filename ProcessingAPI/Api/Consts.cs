namespace ProcessingAPI;

public static class Consts
{
    public const string FoodsRoute = "api/foods";
    public const int RequestFailedOnSmallCount = 5;
    public const int RequestFailedOnBigCount = 10;
    public const int TimeOfCalculation = 60;
    public const int CacheExpirationTime = 300;
    
//TDB put those in configuration and pass as an interface -
// this will allow to speed up tests(they will not be dependent onthose values)

}

public static class Flags
{
    //TDB move request count to injectable service
    public static int _requestCount = 0;
    public static bool _failOnNextRequest = false;
}