using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kmd.Momentum.Mea.Citizen.Model;
using System.Net.Http;
using System.Text;
using System.Net;
using Serilog;
using Microsoft.AspNetCore.Http;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public class CitizenHttpClientHelper : ICitizenHttpClientHelper
    {
        private readonly IMeaClient _meaClient;
        private readonly string _correlationId;

        public CitizenHttpClientHelper(IMeaClient meaClient, IHttpContextAccessor httpContextAccessor)
        {
            _meaClient = meaClient;
            _correlationId = httpContextAccessor.HttpContext.TraceIdentifier;
        }

        public async Task<ResultOrHttpError<IReadOnlyList<string>, Error>> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url, int pageNumber)
        {
            List<JToken> totalRecords = new List<JToken>();
            List<string> JsonStringList = new List<string>();

            var pageNo = pageNumber;
            var size = 100;
            var skip = (pageNo - 1) * size;

            var queryStringParams = $"term=Citizen&size={size}&skip={skip}&isActive=true";
            var response = await _meaClient.GetAsync(new Uri(url + "?" + queryStringParams)).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<IReadOnlyList<string>, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            int.TryParse(JObject.Parse(content)["totalCount"].ToString(), out int totalCount);

            if (pageNumber > (totalCount / size) + 1)
            {
                var error = new Error(_correlationId, new[] { "No Records are available for entered page number" }, "MEA");                
                return new ResultOrHttpError<IReadOnlyList<string>, Error>(error, HttpStatusCode.BadRequest);
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

            return new ResultOrHttpError<IReadOnlyList<string>, Error>(JsonStringList);
        }

        public async Task<ResultOrHttpError<string, Error>> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url)
        {
            var response = await _meaClient.GetAsync(url).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            return new ResultOrHttpError<string, Error>(content);
        }

        public async Task<ResultOrHttpError<string, Error>> CreateJournalNoteInMomentumCoreAsync(Uri url, string momentumCitizenId, JournalNoteResponseModel requestModel)
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

            var response = await _meaClient.PostAsync(url, stringContent).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;

            return new ResultOrHttpError<string, Error>(content);
        }
    }
}
