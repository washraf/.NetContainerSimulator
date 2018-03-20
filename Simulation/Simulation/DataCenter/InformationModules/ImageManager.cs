using Simulation.DataCenter.Images;
using System.Collections.Generic;
using Simulation.Messages;
using System.Threading.Tasks;
using Simulation.Configuration;

namespace Simulation.DataCenter.InformationModules
{
    public class ImageManager
    {
        //private DockerRegistry Registry { get; }
        public Dictionary<int, Image> dictionary;
        private readonly NetworkInterfaceCard _communicationModule;

        public ImageManager(NetworkInterfaceCard CommunicationModule)
        {

            //Registry = registry;
            dictionary = new Dictionary<int, Image>();
            _communicationModule = CommunicationModule;
        }

        //add puling time to downtime
        public void LoadImage(int imageId)
        {
            if (dictionary.ContainsKey(imageId))
            {
                return;
            }
            //Task t = new Task(() =>
            //{
                var list = GetImageTree(imageId);
                foreach (var item in list)
                {
                    if (!dictionary.ContainsKey(item))
                    {
                        dictionary.Add(item, GetImage(item));
                    }
                }
            //    Task.Delay(Global.Second).Wait();
            //});
            //t.Start();
        }

        private List<int> GetImageTree(int imageId)
        {
            var request = new ImageTreeRequest(int.MaxValue, _communicationModule.MachineId, imageId);
            var r = _communicationModule.RequestData(request);
            var result = r as ImageTreeResponce;
            return result.ImageTree;
        }
        private Image GetImage(int imageId)
        {
            var request = new ImagePullRequest(int.MaxValue, _communicationModule.MachineId, imageId);
            var r = _communicationModule.RequestData(request);
            var result = r as ImagePullResponce;
            return result.Image;
        }
    }
}
