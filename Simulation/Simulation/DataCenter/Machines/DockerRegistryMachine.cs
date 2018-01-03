using Simulation.Accounting;
using Simulation.DataCenter.InformationModules;
using Simulation.DataCenter.Machines;
using Simulation.DataCenter.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Messages;
using Simulation.Configuration;

namespace Simulation.DataCenter.Machines
{
    public class DockerRegistryMachine:Machine
    {
        private bool _started;
        private readonly RegistryTable _registryTable;

        public override bool Started
        {
            get { return _started; }
            set
            {
                _started = value;
                CommunicationModule.Started = value;
            }
        }
        public DockerRegistryMachine(int machineId, NetworkSwitch networkSwitch, SimulationSize simulationSize) : base(machineId, networkSwitch)
        {
            _registryTable = new RegistryTable(simulationSize);
            //StartMachine();
        }

        #region --Properties--
        public override sealed void StartMachine()
        {
            Started = true;
        }
        public override void StopMachine()
        {
            Started = false;
        }

        #endregion

        private ImagePullResponce HandleImagePullRequest(ImagePullRequest imagePullRequest)
        {
            var result = new ImagePullResponce(imagePullRequest.SenderId, this.MachineId, imagePullRequest.ImageId, _registryTable.GetImage(imagePullRequest.ImageId));
            return result;
        }

        private ImageTreeResponce HandleImageTreeRequest(ImageTreeRequest imageTreeRequest)
        {
            var result = new ImageTreeResponce(imageTreeRequest.SenderId, this.MachineId, imageTreeRequest.ImageId, _registryTable.GetImageTree(imageTreeRequest.ImageId));
            return result;
        }

        #region --Message Handlers--
        public override void HandleMessage(Message message)
        {
            throw new NotImplementedException("How come");
        }

        public override Message HandleRequestData(Message message)
        {
            switch (message.MessageType)
            {
                case MessageTypes.ImageTreeRequest:
                    return HandleImageTreeRequest(message as ImageTreeRequest);
                case MessageTypes.ImagePullRequest:
                    return HandleImagePullRequest(message as ImagePullRequest);
                default:
                    throw new ArgumentOutOfRangeException("");
                   
            }
        }
        #endregion
    }
}
