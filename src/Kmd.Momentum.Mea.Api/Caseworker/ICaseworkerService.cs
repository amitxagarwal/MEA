using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
	public interface ICaseworkerService
	{
		 Task <CaseworkerResponse> GetCaseworkerDetailsAsync(Guid Id, CaseworkerRequest request);
	}
}
