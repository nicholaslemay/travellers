namespace Travellers.Support.Db;

public class DatabaseTimeoutException(Exception innerException)
    : Exception("The database operation exceeded its configured timeout.", innerException);
