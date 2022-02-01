using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.EventApp.Services.ServiceModels;
using Nat.EventApp.Models.EFModel;
using Nat.Core.Exception;
using Nat.EventApp.Models.Repositories;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.KendoX.Extensions;
using Nat.EventApp.Services;
using Nat.Core.ServiceClient;
using Nat.EventApp.Services.ViewModels;
using Nat.Core.Lookup;

namespace Nat.EventApp.Services
{
    public class EventSeatingPlanService : BaseService<EventSeatingPlanModel, NAT_ES_Event_Seating_Plan>
    {
        private static EventSeatingPlanService _service;
        public static EventSeatingPlanService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new EventSeatingPlanService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private EventSeatingPlanService() : base()
        {

        }


     

        /// <summary>
        /// Create: Method for creation of Event SeatingPlan
        /// </summary>
        /// <param name="servicemodel">Service Event Model</param>
        /// <returns>Event ID generated for Event</returns>
        public string CreateEventSeatingPlan(EventSeatingPlanModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {

                    //Event Seat info
                    if (servicemodel.NatEsEventSeat != null)
                    {
                        foreach (EventSeatModel EventSeat in servicemodel.NatEsEventSeat)
                        {
                            EventSeat.ActiveFlag = true;
                            EventSeat.ObjectState = ObjectState.Added;
                        }
                    }

                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;

                    Insert(servicemodel);
                    uow.SaveChanges();
                    return Convert.ToString(Get().SeatingPlanId);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


    }
}

       