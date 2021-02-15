using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Rhetos.Utilities;

namespace Rhetos.Extensions.RestApi.Utilities
{
    public class JsonErrorHandler
    {
        private readonly ILocalizer localizer;

        public class ResponseMessage
        {
            public string UserMessage;
            public string SystemMessage;
            public override string ToString()
            {
                return "SystemMessage: " + (SystemMessage ?? "<null>") + ", UserMessage: " + (UserMessage ?? "<null>");
            }
        }

        public JsonErrorHandler(ILocalizer localizer)
        {
            this.localizer = localizer;
        }

        public (object response, int statusCode) CreateResponseFromException(Exception error)
        {
            object responseMessage;
            int responseStatusCode;

            if (error is UserException userException)
            {
                responseStatusCode = StatusCodes.Status400BadRequest;
                responseMessage = new ResponseMessage
                {
                    UserMessage = localizer[userException.Message, userException.MessageParameters],
                    SystemMessage = userException.SystemMessage
                };
            }
            else if (error is LegacyClientException legacyClientException)
            {
                responseStatusCode = (int)legacyClientException.HttpStatusCode;
                responseMessage = legacyClientException.Message;
            }
            else if (error is ClientException clientException)
            {
                responseStatusCode = (int)clientException.HttpStatusCode;
                responseMessage = new ResponseMessage { SystemMessage = clientException.Message };

                // HACK: Old Rhetos plugins could not specify the status code. Here we match by message convention.
                if (clientException.Message == "User is not authenticated." && responseStatusCode == StatusCodes.Status400BadRequest)
                    responseStatusCode = StatusCodes.Status401Unauthorized;
            }
            /*
            else if (error is InvalidOperationException && error.Message.StartsWith("The incoming message has an unexpected message format 'Raw'"))
            {
                responseStatusCode = StatusCodes.Status400BadRequest;
                responseMessage = new ResponseMessage
                {
                    SystemMessage = "The incoming message has an unexpected message format 'Raw'. Set the Content-Type to 'application/json'." +
                        " " + FrameworkException.SeeLogMessage(error)
                };
            }
            */
            /*
            else if (error is SerializationException && !error.StackTrace.ToString().Contains("Rhetos"))
            {
                responseStatusCode = StatusCodes.Status400BadRequest;
                responseMessage = new ResponseMessage
                {
                    SystemMessage = "Serialization error: Please check if the request body has a valid JSON format."
                        + " " + FrameworkException.SeeLogMessage(error)
                };
            }*/
            else
            {
                responseStatusCode = StatusCodes.Status500InternalServerError;
                responseMessage = new ResponseMessage { SystemMessage = FrameworkException.GetInternalServerErrorMessage(localizer, error) };
            }

            return (responseMessage, responseStatusCode);
        }
    }
}
