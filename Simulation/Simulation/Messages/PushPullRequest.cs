using System.Collections.Generic;
using Simulation.Helpers;
using Simulation.Loads;

namespace Simulation.Messages
{

    public class Request : Message
    {
        public Request(int target, int sender, HostLoadInfo hostLoad,MessageTypes requestType) :
            base(target, sender, requestType)
        {
            CurrentHostLoadInfo = hostLoad;
        }

        public HostLoadInfo CurrentHostLoadInfo { get; private set; }
    }
    public class PushRequest:Request
    {
        public PushRequest(int target, int sender,HostLoadInfo hostLoad,ContainerLoadInfo containerLoad ) :
            base(target,sender,hostLoad, MessageTypes.PushRequest)
        {
            SelectedContainerLoadInfo = containerLoad;
        }

        public ContainerLoadInfo SelectedContainerLoadInfo { get; private set; }
    }


    public class PullRequest : Request
    {
        public PullRequest(int target, int sender,  HostLoadInfo hostLoad) :
            base(target, sender, hostLoad, MessageTypes.PullRequest)
        {
        }

    }
}