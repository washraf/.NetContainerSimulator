using System.Collections.Generic;

namespace Simulation.Messages
{
    public class ImageTreeResponce : Message
    {
        public ImageTreeResponce(int target, int sender, int imageId, List<int> imageTree) : base(target, sender, MessageTypes.ImageTreeResponce)
        {
            ImageId = imageId;
            ImageTree = imageTree;
        }

        public int ImageId { get; }
        public List<int> ImageTree { get; }
    }
}
