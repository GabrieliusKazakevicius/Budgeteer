using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Budgeteer.Application.Services;


public class ServiceResult<TResult, TError> 
    where TResult : class, new()
    where TError : struct, Enum
{
    public TResult? Result { get; }
    public TError? Error { get; }
    
    public ServiceResult(TResult result, TError error)
    {
        Result = result;
        Error = error;
    }
    
    public ServiceResult(TResult result)
    {
        Result = result;
    }
    
    public ServiceResult(TError error)
    {
        Error = error;
    }
    
    public static implicit operator bool(ServiceResult<TResult, TError> serviceResult)
        => !serviceResult.Error.HasValue && serviceResult.Result != null;

    public static implicit operator ServiceResult<TResult, TError>(TResult result)
    {
        return new ServiceResult<TResult, TError>(result);
    }
    
    public static implicit operator ServiceResult<TResult, TError>(TError error)
    {
        return new ServiceResult<TResult, TError>(error);
    }
}


public class ServiceResult<TError> where TError : Enum 
{
    public TError Error { get; protected set; }
    
    private ServiceResult(TError error)
    {
        Error = error;
    }

    public static implicit operator ServiceResult<TError>(TError error)
    {
        return new ServiceResult<TError>(error);
    }
}