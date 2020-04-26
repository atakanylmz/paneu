using PanEU.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PanEU.ViewModels
{
    public class MakeAnAppointmentModel
    {
        public int StoreId { get; set; }
        public int UserId { get; set; }
        public int DayInWeek { get; set; }
        public System.TimeSpan AppointmentTime{ get; set; }
        public Store Store { get; set; }

    }
}