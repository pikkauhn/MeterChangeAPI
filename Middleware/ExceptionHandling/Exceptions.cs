namespace MeterChangeApi.Middleware.ExceptionHandling
{
    public class RepositoryException : Exception
    {
        public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ServiceException : Exception
    {
        public ServiceException(string message, Exception innerException) : base(message, innerException) { }
        public ServiceException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class DatabaseConflictException : Exception
    {
        public DatabaseConflictException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidInputException : Exception
    {
        public InvalidInputException(string message) : base(message) { }
    }


}