using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public class CitizenHttpClientHelper : ICitizenHttpClientHelper
    {
        private readonly IMeaClient _meaClient;
        private readonly string _correlationId;
        private readonly string _clientId;

        public CitizenHttpClientHelper(IMeaClient meaClient, IHttpContextAccessor httpContextAccessor)
        {
            _meaClient = meaClient;
            _correlationId = httpContextAccessor.HttpContext.TraceIdentifier;
        }

        public async Task<ResultOrHttpError<CitizenList, Error>> GetAllActiveCitizenDataFromMomentumCoreAsync(string path, int pageNumber)
        {
            List<JToken> totalRecords = new List<JToken>();
            List<string> JsonStringList = new List<string>();

            int pageNo = pageNumber;
            var size = 100;
            var skip = (pageNo - 1) * size;

            var queryStringParams = $"term=Citizen&size={size}&skip={skip}&isActive=true";
            var response = await _meaClient.GetAsync(path + "?" + queryStringParams).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<CitizenList, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            int.TryParse(JObject.Parse(content)["totalCount"].ToString(), out int totalCount);

            if (pageNumber > (totalCount / size) + 1)
            {
                var error = new Error(_correlationId, new[] { "No Records are available for entered page number" }, "MEA");
                Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                .Error("No Records are available for entered page number");
                return new ResultOrHttpError<CitizenList, Error>(error, HttpStatusCode.BadRequest);
            }

            var jsonArray = JArray.Parse(JObject.Parse(content)["results"].ToString());            

            totalRecords.AddRange(jsonArray.Children());

            foreach (var item in totalRecords)
            {
                var jsonToReturn = JsonConvert.SerializeObject(new
                {
                    citizenId = item["id"],
                    displayName = item["name"],
                    givenName = (string)null,
                    middleName = (string)null,
                    initials = (string)null,
                    address = (string)null,
                    number = (string)null,
                    caseworkerIdentifier = (string)null,
                    description = item["description"],
                    isBookable = true,
                    isActive = true
                });
                JsonStringList.Add(jsonToReturn);
            }

            var totalPages = (totalCount / size) + 1;

            var data = JsonStringList.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x));

            var citizenList = new CitizenList(totalPages, totalCount, pageNo, data.ToList());

            return new ResultOrHttpError<CitizenList, Error>(citizenList);
        }

        public async Task<ResultOrHttpError<string, Error>> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(string path)
        {
            var response = await _meaClient.GetAsync(path).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            return new ResultOrHttpError<string, Error>(content);
        }

        public async Task<ResultOrHttpError<string, Error>> CreateJournalNoteInMomentumCoreAsync(string path, string momentumCitizenId, JournalNoteResponseModel requestModel)
        {
            List<JournalNoteAttachmentModel> attachmentList = new List<JournalNoteAttachmentModel>();

            foreach (var doc in requestModel.Documents)
            {
                var attachemnt = new JournalNoteAttachmentModel()
                {
                    ContentType = doc.ContentType,
                    Document = doc.Content,
                    Title = doc.Name
                };
                attachmentList.Add(attachemnt);
            }

            JournalNoteModel mcaRequestModel = new JournalNoteModel()
            {
                Id = requestModel.Cpr,
                OccurredAt = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.ff'Z'"),
                Title = requestModel.Title,
                Body = requestModel.Body,
                Source = "Mea",
                ReferenceId = momentumCitizenId,
                JournalTypeId = requestModel.Type.ToLower() == "sms" ? "022.247.000" : "022.420.000",
                Attachments = attachmentList
            };

            string serializedRequest = JsonConvert.SerializeObject(mcaRequestModel);
            StringContent stringContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

            var response = await _meaClient.PostAsync(path, stringContent).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;

            return new ResultOrHttpError<string, Error>(content);
        }
    }
}
