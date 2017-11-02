using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation.Accounting;
using Simulation.Configuration;
using Simulation.Loads;
using Simulation.Messages;

namespace Simulation.Modules.LoadManagement
{
    public class CommonLoadManager
    {
        private readonly IAccountingModule _accountingModule;
        private object _lock = new object();
        private Dictionary<int,HostLoadInfo> _currentHostLoadInfos = new Dictionary<int, HostLoadInfo>();

        public CommonLoadManager(IAccountingModule accountingModule)
        {
            _accountingModule = accountingModule;
        }

        public HostLoadInfo GetHostLoadInfoByHostId(int hostId)
        {
            lock (_lock)
            {
                _accountingModule.RequestCreated(MessageTypes.CommonLoadManager);
                return _currentHostLoadInfos[hostId];
            }
        }

        public HostLoadInfo GetHostLoadInfoByHostIdAfterContainer(int hostId,ContainerLoadInfo info)
        {
            lock (_lock)
            {
                _accountingModule.RequestCreated(MessageTypes.CommonLoadManager);
                var item = _currentHostLoadInfos[hostId];
                var nload =  item.CurrentLoad+info.CurrentLoad;
                var machinePower = Global.DataCenterHostConfiguration;
                var cpu = nload.CpuLoad/machinePower.CpuLoad;
                var mem = nload.MemorySize / machinePower.MemorySize;
                var io = nload.IoSecond / machinePower.IoSecond;

                return new HostLoadInfo(hostId,nload,item.ContainersCount+1,cpu,mem,io);
            }
        }

        public void UpdateHostLoadInfo(HostLoadInfo info)
        {
            lock (_lock)
            {
                _accountingModule.RequestCreated(MessageTypes.CommonLoadManager);

                if (_currentHostLoadInfos.ContainsKey(info.HostId))
                    _currentHostLoadInfos[info.HostId] = info;
                else
                {
                    _currentHostLoadInfos.Add(info.HostId,info);
                }
            }
        }

        public List<HostLoadInfo> GetAllHostLoadInfos()
        {
            lock (_lock)
            {
                _accountingModule.RequestCreated(MessageTypes.CommonLoadManager);

                return _currentHostLoadInfos.Values.ToList();
            }
        } 

        public void Clear()
        {
            lock (_lock)
            {
                _accountingModule.RequestCreated(MessageTypes.CommonLoadManager);
                _currentHostLoadInfos.Clear();
            }
        }
    }
}
