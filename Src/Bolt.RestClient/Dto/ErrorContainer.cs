using System.Collections.Generic;

namespace Bolt.RestClient.Dto
{
    internal class ErrorContainer
    {
        public IEnumerable<Error> Errors { get; set; }
    }
}