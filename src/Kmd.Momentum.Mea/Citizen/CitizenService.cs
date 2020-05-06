using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Citizen
{
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenHttpClientHelper _citizenHttpClient;
        private readonly string _correlationId;
        private readonly string _clientId;

        public CitizenService(ICitizenHttpClientHelper citizenHttpClient, IConfiguration config,
            IHttpContextAccessor httpContextAccessor)
        {
            _citizenHttpClient = citizenHttpClient;
            _correlationId = httpContextAccessor.HttpContext.TraceIdentifier;
            _clientId = httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == "azp").Value;
        }

        public async Task<ResultOrHttpError<CitizenList, Error>> GetAllActiveCitizensAsync(int pageNumber)
        {
            if (pageNumber <= 0)
            {
                var error = new Error(_correlationId, new[] { "PageNumber cannot be less than or equal to zero" }, "Mea");
                Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                .Error("PageNumber is less than or equal to zero");
                return new ResultOrHttpError<CitizenList, Error>(error, System.Net.HttpStatusCode.BadRequest);
            }

            var response = await _citizenHttpClient.GetAllActiveCitizenDataFromMomentumCoreAsync
                ("/search", pageNumber).ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                .Error("An error occurred while retrieving data of all active citizens" + error);
                return new ResultOrHttpError<CitizenList, Error>(response.Error, response.StatusCode.Value);
            }

            var result = response.Result;

            Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                .Information("All the active citizens data retrieved successfully");

            return new ResultOrHttpError<CitizenList, Error>(result);
        }

        public async Task<ResultOrHttpError<CitizenDataResponseModel, Error>> GetCitizenByCprAsync(string cpr)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync
                ($"citizens/{cpr}").ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                .Error("An error occured while retrieving citizen data by cpr" + error);
                return new ResultOrHttpError<CitizenDataResponseModel, Error>(response.Error, response.StatusCode.Value);
            }

            var json = JObject.Parse(response.Result);
            var citizenData = JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());

            Log.ForContext("CorrelationId", _correlationId)
                .ForContext("CitizenId", citizenData.CitizenId)
                .Information("The citizen details by CPR number is returned successfully");

            return new ResultOrHttpError<CitizenDataResponseModel, Error>(citizenData);
        }

        public async Task<ResultOrHttpError<CitizenDataResponseModel, Error>> GetCitizenByIdAsync(string citizenId)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync($"citizens/{citizenId}").ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                    .ForContext("CitizenId", citizenId)
                    .Error("An error occured while retrieving citizen data by citizenID" + error);

                return new ResultOrHttpError<CitizenDataResponseModel, Error>(response.Error, response.StatusCode.Value);
            }

            var json = JObject.Parse(response.Result);
            var citizenData = JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());

            Log.ForContext("CorrelationId", _correlationId)
                .ForContext("Client", _clientId)
                .ForContext("CitizenId", citizenData.CitizenId)
                .Information("The citizen details by CitizenId has been returned successfully");

            return new ResultOrHttpError<CitizenDataResponseModel, Error>(citizenData);
        }

        public async Task<ResultOrHttpError<string, Error>> CreateJournalNoteAsync(string momentumCitizenId, JournalNoteRequestModel requestModel)
        {
            var response = await _citizenHttpClient.CreateJournalNoteInMomentumCoreAsync("journals/note", momentumCitizenId, requestModel).ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);

                Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("ClientId", _clientId)
                    .ForContext("CitizenId", momentumCitizenId)
                    .Error("An Error Occured while creating Journal Note" + error);

                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            Log.ForContext("CorrelationId", _correlationId)
                .ForContext("ClientId", _clientId)
                .ForContext("CitizenId", momentumCitizenId)
                .Information("Journal Note is created successfully");

            return new ResultOrHttpError<string, Error>(response.Result);
        }
    }
}