using System;
using System.CodeDom;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensions.RestApi.Metadata;
using Rhetos.Extensions.RestApi.Utilities;
using Rhetos.Processing;

namespace Rhetos.Extensions.RestApi.Controllers
{
    // We are using ActionResult<TResult> in each action and return JsonResult to circumvent JsonOutputFormatter bug
    // bug causes Actions which return TResult directly to ignore some serializer settings (e.g. MicrosoftDateTime)
    public class DataApiController<T> : RhetosApiControllerBase<T>
    {
        private readonly ServiceUtility serviceUtility;
        private readonly Lazy<Tuple<string, Type>[]> dataStructureParameters;

        public DataApiController(ServiceUtility serviceUtility, ControllerRestInfoRepository controllerRestInfoRepository)
        {
            this.serviceUtility = serviceUtility;
            dataStructureParameters = new Lazy<Tuple<string, Type>[]>(() =>
            {
                var dataStructureInfoMetadata = controllerRestInfoRepository.ControllerConceptInfo[this.GetType()] as DataStructureInfoMetadata;
                if (dataStructureInfoMetadata == null)
                    throw new InvalidOperationException(
                        $"Registered {nameof(ConceptInfoRestMetadata)} for {GetType().Name} should be an instance of {nameof(DataStructureInfoMetadata)}.");
                return dataStructureInfoMetadata.GetParameters();
            });

            // TODO: remove debug code
            /*
            var type = typeof(T);
            IDslModel dslModel = null;

            // brzi nacin - dovoljno brzo za svaki request
            var conceptInfo = dslModel.FindByKey($"DataStructureInfo {type.FullName}");

            // drugi nacin
            dslModel.FindByType<DataStructureInfo>().Single(a => a.FullName == type.FullName);
            */
        }
        
        [HttpGet]
        public ActionResult<RecordsResult<T>> Get(string filter = null, string fparam = null, string genericfilter = null, string filters = null,
            int top = 0, int skip = 0, int page = 0, int psize = 0, string sort = null)
        {
            var data = serviceUtility.GetData<T>(filter, fparam, genericfilter, filters, dataStructureParameters.Value, top, skip, page, psize, sort, true, false);
            return new JsonResult(new RecordsResult<T>() { Records = data.Records });
        }

        [Obsolete("Use GetTotalCount instead.")]
        [HttpGet]
        [Route("Count")]
        public ActionResult<CountResult> GetCount(string filter, string fparam, string genericfilter, string filters, string sort)
        {
            var data = serviceUtility.GetData<T>(filter, fparam, genericfilter, filters, dataStructureParameters.Value, 0, 0, 0, 0, sort,
                readRecords: false, readTotalCount: true);
            return new JsonResult(new CountResult {TotalRecords = data.TotalCount });
        }

        [HttpGet]
        [Route("TotalCount")]
        public ActionResult<TotalCountResult> GetTotalCount(string filter, string fparam, string genericfilter, string filters, string sort)
        {
            var data = serviceUtility.GetData<T>(filter, fparam, genericfilter, filters, dataStructureParameters.Value, 0, 0, 0, 0, sort,
                readRecords: false, readTotalCount: true);
            return new JsonResult(new TotalCountResult { TotalCount = data.TotalCount });
        }

        [HttpGet]
        [Route("RecordsAndTotalCount")]
        public ActionResult<RecordsAndTotalCountResult<T>> GetRecordsAndTotalCount(string filter, string fparam, string genericfilter, string filters, int top, int skip, int page, int psize,
            string sort)
        {
            var result = serviceUtility.GetData<T>(filter, fparam, genericfilter, filters, dataStructureParameters.Value, top, skip, page, psize, sort,
                readRecords: true, readTotalCount: true);

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<T> GetById(string id)
        {
            var result = serviceUtility.GetDataById<T>(id);
            if (result == null)
                throw new LegacyClientException("There is no resource of this type with a given ID.") {HttpStatusCode = HttpStatusCode.NotFound, Severe = false};
            return new JsonResult(result);
        }


        [HttpPost]
        public ActionResult<ProcessingResult> Insert([FromBody]T item)
        {
            /*
            // TODO: How to check this??
            if (!RestServiceMetadata.WritableDataStructures.Contains(typeof(TDataStructure).FullName))
                throw new ClientException($"Invalid request: '{typeof(TDataStructure).FullName}' is not writable.");
            */

            if (item == null)
                throw new ClientException("Invalid request: Missing the record data.The data should be provided in the request message body.");

            var entity = item as IEntity;

            if (Guid.Empty == entity.ID)
                entity.ID = Guid.NewGuid();
            
            serviceUtility.InsertData(item);
            return new JsonResult(new InsertDataResult() { ID = entity.ID});
        }

        [HttpPut]
        [Route("{id}")]
        public void Update(string id, [FromBody] T item)
        {
            /*
            if (!RestServiceMetadata.WritableDataStructures.Contains(typeof(TDataStructure).FullName))
                throw new Rhetos.ClientException($"Invalid request: '{{typeof(TDataStructure).FullName}}' is not writable.");
            */

            if (item == null)
                throw new ClientException("Invalid request: Missing the record data.The data should be provided in the request message body.");

            if (!Guid.TryParse(id, out Guid guid))
                throw new LegacyClientException("Invalid format of GUID parameter 'ID'.");

            var entity = item as IEntity;

            if (Guid.Empty == entity.ID)
                entity.ID = guid;
            if (guid != entity.ID)
                throw new LegacyClientException("Given entity ID is not equal to resource ID from URI.");

            serviceUtility.UpdateData(item);
        }

        [HttpDelete]
        [Route("{id}")]
        public void Delete(string id)
        {
            /*
            if (!RestServiceMetadata.WritableDataStructures.Contains(typeof(TDataStructure).FullName))
                throw new Rhetos.ClientException($""Invalid request: '{{typeof(TDataStructure).FullName}}' is not writable."");
            */
            if (!Guid.TryParse(id, out Guid guid))
                throw new LegacyClientException("Invalid format of GUID parameter 'ID'.");
            
            var item = Activator.CreateInstance<T>();
            (item as IEntity).ID = guid;

            serviceUtility.DeleteData(item);
        }
    }
}
