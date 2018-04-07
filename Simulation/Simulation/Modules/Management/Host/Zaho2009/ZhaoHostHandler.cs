using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulation.Configuration;
using Simulation.DataCenter;
using Simulation.DataCenter.InformationModules;
using Simulation.Helpers;
using Simulation.Loads;
using Simulation.Messages;
using Simulation.Modules.LoadManagement;

namespace Simulation.Modules.Management.Host.Other
{
    /// <summary>
    /// http://ieeexplore.ieee.org/document/5331732/?reload=true
    /// </summary>
    public class ZhaoHostHandler:HostHandlerModule
    {
        private readonly CommonLoadManager _commonLoadManager;
        private object _hostLock = new object();
        //public int BidLock { get; set; } = -1;

        public ZhaoHostHandler(NetworkInterfaceCard communicationModule, ContainerTable containerTable, ILoadManager loadManager,CommonLoadManager commonLoadManager) : base(communicationModule, containerTable, loadManager)
        {
            _commonLoadManager = commonLoadManager;
        }

        
        public override void MachineAction()
        {
            while (Started)
            {
                Thread.Sleep(GetBackOffTime());
                lock (_hostLock)
                {
                    //if (BidLock == -1)
                    {
                    //    BidLock =0;
                        UpdateInformation();
                        CompareAndBalance();
                    }
                    //else
                    //{
                    //    IncreaseBackOffTime();
                    //}
                }
                
            }
        }

        
        private void CompareAndBalance()
        {
            var hosts = _commonLoadManager.GetAllHostLoadInfos();
            Dictionary<int, double> pdis = CalculatePropDistributionForAllHosts(hosts);
            Random r = new Random(Guid.NewGuid().GetHashCode());
            List<ContainerToHost> list = new List<ContainerToHost>();
            foreach (var container in ContainerTable.GetAllContainers())
            {
                var k = r.GetRandomFromDictionary(pdis);
                var cdash = _commonLoadManager.GetHostLoadInfoByHostIdAfterContainer(k,container.GetContainerNeededLoadInfo()).Volume;//Should be after adding the current container
                var c = LoadManager.GetNeededHostLoadInfo().Volume;
                if (cdash < c)
                {
                    list.Add(new ContainerToHost(container.ContainerId,k,c-cdash));
                }
            }
            if(list.Count==0) return;
            var total = list.Select(x => x.Cost).Sum();
            foreach (var item in list)
            {
                item.Probaility = item.Cost/total;
            }
            
            var result = r.GetRandomFromContainerToHost(list);
            MigrationContainer(result);

        }

        private void MigrationContainer(ContainerToHost result)
        {
            var con = ContainerTable.GetContainerById(result.ConId);
            var size = (int)con.GetContainerNeededLoadInfo().CurrentLoad.MemorySize;
            ContainerTable.LockContainer(con.ContainerId);
            con.Checkpoint(this.MachineId);

            MigrateContainerRequest request =
                new MigrateContainerRequest(result.HostId, this.MachineId, con, size);
            CommunicationModule.SendMessage(request);
        }

        private Dictionary<int,double> CalculatePropDistributionForAllHosts(List<HostLoadInfo> hosts)
        {
            var n = hosts.Select(x => x.ContainersCount).Sum();
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (var host in hosts)
            {
                result.Add(host.HostId,host.ContainersCount * 1.0/n);
            }
            return result;
        }

        private void UpdateInformation()
        {
            _commonLoadManager.UpdateHostLoadInfo(LoadManager.GetNeededHostLoadInfo());
        }

        public override void HandleMessage(Message message)
        {
            lock (_hostLock)
            {
                var mt = message.MessageType;
                switch (mt)
                {
                    case MessageTypes.MigrateContainerRequest:
                        HandleMigrateContainerRequest(message as MigrateContainerRequest);
                        break;
                    case MessageTypes.MigrateContainerResponse:
                        HandleMigrateContainerResponse(message as MigrateContainerResponse);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override Message HandleRequestData(Message message)
        {
            throw new NotImplementedException();
        }

        private void HandleMigrateContainerResponse(MigrateContainerResponse message)
        {
            if (message.Done)
            {
                ContainerTable.FreeLockedContainer();
                //ResetBackOff();

                //_containersTable.Remove(sendContainerResponce.ContainerId);
            }
            else
            {
                throw new NotImplementedException("responce");
                //_containerTable.UnLockContainer();
            }
            //BidLock = -1;
        }
        private void HandleMigrateContainerRequest(MigrateContainerRequest message)
        {
            //if (BidLock == -1)
            {
                //BidLock = message.SenderId;
                message.MigratedContainer.Restore(this.MachineId);
                ContainerTable.AddContainer(message.MigratedContainer.ContainerId, message.MigratedContainer);
                var responce =
                    new MigrateContainerResponse(message.SenderId, this.MachineId, message.MigratedContainer.ContainerId,
                        true);
                CommunicationModule.SendMessage(responce);
                //BidLock = -1;
                //IncreaseBackOffTime();
            }
            //else
            //{
            //    var responce =
            //       new MigrateContainerResponse(message.SenderId, this.MachineId, message.MigratedContainer.ContainerId,
            //           false);
            //    CommunicationModule.SendMessage(responce);
            //}
            //_lastDelay = 5;
            //ResetBackOff();
        }

    }

}


public class ContainerToHost
{
    public ContainerToHost(int conId, int hostId, double cost)
    {
        ConId = conId;
        HostId = hostId;
        Cost = cost;
    }
    public int ConId { get; private set; }
    public int HostId { get; private set; }
    public double Cost { get; private set; }
    public double Probaility { get; set; }
}