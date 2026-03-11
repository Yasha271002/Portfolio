using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolgaTermalBathsEnter.Models
{
    public class UpdateTicketsTrigger
    {
        public event Action UpdateTicketsEvent;

        public void OnUpdateTickets()
        {
            UpdateTicketsEvent?.Invoke();
        }
    }
}
