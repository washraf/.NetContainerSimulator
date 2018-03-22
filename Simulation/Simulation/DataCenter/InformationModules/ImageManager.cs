using Simulation.DataCenter.Images;
using System.Collections.Generic;
using Simulation.Messages;
using System.Threading.Tasks;
using Simulation.Configuration;
using System;

namespace Simulation.DataCenter.InformationModules
{
    public class ImageManager
    {
        //private DockerRegistry Registry { get; }
        public Dictionary<int, Image> dictionary;
        private readonly NetworkInterfaceCard _communicationModule;
        private object _lock = new object();
        public ImageManager(NetworkInterfaceCard CommunicationModule)
        {

            //Registry = registry;
            dictionary = new Dictionary<int, Image>();
            _communicationModule = CommunicationModule;
        }

        public bool ContainsImage(int imageId)
        {
            return dictionary.ContainsKey(imageId);
        }

        //add puling time to downtime
        public async Task LoadImage(int imageId)
        {
            if (ContainsImage(imageId))
            {
                return;
            }
            var list = await GetImageTree(imageId);
            foreach (var item in list)
            {
                if (!ContainsImage(item))
                {
                    var image = await GetImage(item);
                    lock (_lock)
                    {
                        if (!ContainsImage(image.Id))
                        {

                            dictionary.Add(item, image);
                        }
                    }
                }
            }
        }

        private async Task<List<int>> GetImageTree(int imageId)
        {
            var request = new ImageTreeRequest(int.MaxValue, _communicationModule.MachineId, imageId);
            var r = await _communicationModule.RequestData(request);
            var result = r as ImageTreeResponce;
            return result.ImageTree;
        }
        private async Task<Image> GetImage(int imageId)
        {
            var request = new ImagePullRequest(int.MaxValue, _communicationModule.MachineId, imageId);
            var r = await _communicationModule.RequestData(request);
            var result = r as ImagePullResponce;
            return result.Image;
        }
    }
}
