using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Behaviors
{
  public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse> where TResponse : notnull
  {
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
      logger.LogInformation("[START] Handle request = {request} - Response = {response} - Request data = {requestData}", 
        typeof(TRequest).Name, typeof(TResponse).Name, request);
      var timer = new Stopwatch();
      timer.Start();
 
      var response = await next();
      
      timer.Stop();
      var timeTaken = timer.Elapsed;
      if (timeTaken.Minutes > 3)
      {
        logger.LogWarning("[PERFORMANCE] The request {request} took {timeTaken}", typeof(TRequest).Name, timeTaken.Minutes);
      }
      logger.LogInformation("[END] Handler request {request} with {response}", typeof(TRequest).Name, response);
      return response;
    }
  }
}
