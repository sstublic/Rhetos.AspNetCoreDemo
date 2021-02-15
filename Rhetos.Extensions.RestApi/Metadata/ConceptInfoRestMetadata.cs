using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rhetos.Extensions.RestApi.Metadata
{
    public class ConceptInfoRestMetadata
    {
        public Type ControllerType { get; set; }
        public string RelativeRoute { get; set; }
        public string ControllerName { get; set; }
        public string ApiExplorerGroupName { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
