using System;
using Rhetos.Dsl;

namespace Rhetos.Extensions.RestApi.Metadata
{
    public interface IConceptInfoRestMetadataProvider
    {
        ConceptInfoRestMetadata GetConceptInfoRestMetadata(IConceptInfo conceptInfo);
    }
}
