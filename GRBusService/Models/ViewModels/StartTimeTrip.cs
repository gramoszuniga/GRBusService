/* File name: StartTimeTrip.cs
 * Description: ViewModel class for trip table.
 *              Helps create custom drop down list for route schedule id in Create action of GRTripController.
 * Name: Gonzalo Ramos Zúñiga
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRBusService.Models.ViewModels
{
    public class StartTimeTrip
    {
        public string routeScheduleId { get; set; }
        public string startTime { get; set; }
    }
}