using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.TaskApi.Model
{
    public class TaskUpdateModel
    {
        public enum TaskUpdateStatus
        {
            Completed = 0,
            Cancel = 1,
            Delete = 2,
            Start = 3
        }

        public enum ApplicationContext
        {
            Citizens = 0,
            Companies = 1,
            Offers = 2,
            Dashboard = 3,
            Admin = 4,
            Search = 5
        }
    }
}

