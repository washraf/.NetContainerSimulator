using Simulation.DataCenter.Images;

namespace Simulation.Messages
{
    public class ImagePullResponce : Message
    {
        public ImagePullResponce(int target, int sender, int imageId, Image image) : base(target, sender, MessageTypes.ImagePullResponce)
        {
            ImageId = imageId;
            Image = image;
        }

        public int ImageId { get; }
        public Image Image { get; }
    }
}
