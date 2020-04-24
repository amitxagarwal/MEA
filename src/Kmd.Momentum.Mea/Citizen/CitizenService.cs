using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Authorization;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
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

        public async Task<ResultOrHttpError<IReadOnlyList<CitizenDataResponseModel>, Error>> GetAllActiveCitizensAsync()
        {
            var response = await _citizenHttpClient.GetAllActiveCitizenDataFromMomentumCoreAsync
                ("/search").ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                .Error("An error occurred while retrieving data of all active citizens" + error);
                return new ResultOrHttpError<IReadOnlyList<CitizenDataResponseModel>, Error>(response.Error, response.StatusCode.Value);
            }

            var result = response.Result;
            var content = result.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x));

            Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                .Information("All the active citizens data retrieved successfully");
            return new ResultOrHttpError<IReadOnlyList<CitizenDataResponseModel>, Error>(content.ToList());
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

        public async Task<ResultOrHttpError<string, Error>> CreateJournalNoteAsync(string momentumCitizenId, JournalNoteResponseModel requestModel)
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