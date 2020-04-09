using Kmd.Momentum.Mea.Caseworker1.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Caseworker1
{
    public class CaseworkerService : ICaseworkerService
    {
        private readonly ICaseworkerHttpClientHelper _caseworkerHttpClient;
        private readonly IConfiguration _config;

        public CaseworkerService(ICaseworkerHttpClientHelper caseworkerHttpClient, IConfiguration config)
        {
            _caseworkerHttpClient = caseworkerHttpClient;
            _config = config;
        }

        public async Task<ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>> GetAllCaseworkersAsync()
        {
            var response = await _caseworkerHttpClient.GetAllCaseworkerDataFromMomentumCoreAsync
                (new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}/search")).ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("GetAllCaseworkersAsync", "All Caseworkers")
                .Error("An Error Occured while retriving data of all caseworkers" + error);
                return new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>(response.Error, response.StatusCode.Value);
            }

            var result = response.Result;
            var content = result.Select(x => JsonConvert.DeserializeObject<CaseworkerDataResponseModel>(x));

            Log.ForContext("GetAllCaseworkersAsync", "All Caseworkers")
                .Information("All the caseworkers data retrived successfully");
            return new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>(content.ToList());
        }

        //public async Task<ResultOrHttpError<CaseworkerDataResponseModel, Error>> GetCaseworkerByIdAsync(string caseworkerId)
        //{

        //    var caseworkerData = new CaseworkerDataResponseModel("testId1", "TestDisplay1",
        //       "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true);

        //    var response = JsonConvert.SerializeObject(caseworkerData);

          

        //    return new ResultOrHttpError<CaseworkerDataResponseModel, Error>(response);
            //var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")).ConfigureAwait(false);

            //if (response.IsError)
            //{
            //    var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
            //    Log.ForContext("GetCitizenByIdAsync", "citizenId")
            //    .ForContext("CitizenId", citizenId)
            //    .Error("An Error Occured while retriving citizen data by citizenID" + error);
            //    return new ResultOrHttpError<CitizenDataResponseModel, Error>(response.Error, response.StatusCode.Value);
            //}

            //var json = JObject.Parse(response.Result);
            //var citizenData = JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());

            //Log.ForContext("GetCitizenByIdAsync", "citizenId")
            //    .ForContext("CitizenId", citizenData.CitizenId)
            //    .Information("The citizen details by CitizenId has been returned successfully");

            //return new ResultOrHttpError<CitizenDataResponseModel, Error>(citizenData);
        }
    }

