using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Bolt.Logger;
using Bolt.RestClient.Dto;
using Bolt.RestClient.Extensions;
using Bolt.Serializer;

namespace Bolt.RestClient.Impl
{
    public class ErrorLoader : IErrorLoader
    {
        protected readonly ILogger Logger;

        public ErrorLoader(ILogger logger)
        {
            Logger = logger;

            logger.Trace("ErrorLoader initialized.");
        }

        public void LoadError<TRestResponse>(TRestResponse restResponse, ISerializer serializer) where TRestResponse : RestResponse
        {
            Logger.Trace("Executing loaderror method of errorloader...");

            if (!CanHaveError(restResponse) || restResponse.RawBody.IsNullOrWhiteSpace()) return;
            
            Logger.Trace("Deserializing errors from body...");

            restResponse.Errors = DeserializeError(restResponse, serializer)
                   ?? Enumerable.Empty<Error>();
        }

        public virtual bool CanHaveError<TRestResponse>(TRestResponse restResponse) where TRestResponse : RestResponse
        {
            return restResponse.StatusCode == HttpStatusCode.BadRequest;
        }

        protected virtual IEnumerable<Error> DeserializeError<TRestResponse>(TRestResponse restResponse, ISerializer serializer)
            where TRestResponse : RestResponse
        {
            try
            {
                Logger.Trace("Trying to deserialize errors from raw body..");

                var errorContainer = serializer.Deserialize<ErrorContainer>(restResponse.RawBody);

                Logger.Trace("Deseialization of errors from raw body completed successfully");

                return errorContainer == null ? Enumerable.Empty<Error>() : errorContainer.Errors;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to deserialize errors from rawbody");
            }

            return Enumerable.Empty<Error>();
        }
    }
}