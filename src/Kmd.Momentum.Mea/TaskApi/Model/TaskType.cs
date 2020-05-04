using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.TaskApi.Model
{
    public enum TaskType
    {
        Simple = 0,
        Interview = 1,
        Appointment = 2,
        BookingDeadline = 3,
        MyPlan = 4,
        Education = 5,
        SicknessBenefitReevaluation = 7,
        SicknessBenefitClose = 8,
        TargetDeadline = 9,
        TargetGroupChanged = 10,
        EnrollmentMissing = 11,
        IntegrationContract = 12,
        Message = 13,
        MedicalData = 14,
        FutureCaseEndDate = 15,
        UnemploymentFundAvailabilityAssessment = 16,
        JobcenterChange = 17,
        ClassificationConflict = 18,
        MissingJobCheckWarning = 19,
        JoblogInactiveFor30DaysWarning = 20,
        NoAvailableBookingSlotsWarning = 21,
        CprIncident = 22,
        YearlyReevaluation = 23,
        AddressChange = 24
    }
}
