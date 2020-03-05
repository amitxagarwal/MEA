using System;

namespace Kmd.Momentum.Mea.Api
{
	public interface ICaseworker
	{
		Task<ResultOrHttpError<CaseworkerResponse, string>> CaseworkerAsync( CaseworkerRequest request);
	}
}
