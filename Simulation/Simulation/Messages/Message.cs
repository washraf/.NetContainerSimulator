﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.DataCenter;

namespace Simulation.Messages
{
    /// <summary>
    /// Respresents all message Base Class
    /// All Message Must be emuatable
    /// </summary>
    public abstract class Message
    {
        public Message(int target,int sender,MessageTypes messageType,int messageSize = 0)
        {
            TargetId = target;
            SenderId = sender;
            MessageType = messageType;
            MessageSize = messageSize;
        }
        public int TargetId { get; private set; }

        public int SenderId { get; private set; }
        public MessageTypes MessageType { get; private set; }

        /// <summary>
        /// By MByte
        /// </summary>
        public int MessageSize { get; private set; }

    }
}
