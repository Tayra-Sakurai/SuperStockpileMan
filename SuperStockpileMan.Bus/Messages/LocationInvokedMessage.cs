using CommunityToolkit.Mvvm.Messaging.Messages;
using SuperStockpileMan.Bus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Messages
{
    public class LocationInvokedMessage : ValueChangedMessage<Location>
    {
        public LocationInvokedMessage(Location value) : base(value) { }
    }
}
