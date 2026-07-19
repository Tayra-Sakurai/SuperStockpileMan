using CommunityToolkit.Mvvm.Messaging.Messages;
using SuperStockpileMan.Bus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperStockpileMan.Bus.Messages
{
    public class ItemAddedMessage : ValueChangedMessage<Item>
    {
        public ItemAddedMessage(Item value) : base(value)
        {
        }
    }
}
