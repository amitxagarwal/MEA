using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Client.Sample
{
    public enum CommandLineAction
    {
        None = 0,
        GetAllCaseworkers,
        GetCaseworkerById,
        GetTasksbyCaseworker,
        GetAllActiveCitizens,
        GetCitizenByCpr,
        GetCitizenById,
        CreateJournalNote,
        UpdateTaskStatus,
    }
}
