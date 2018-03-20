using Simulation.DataCenter.Images;

namespace Simulation.Messages
{
    public class ImagePullResponce : Message
    {
        public ImagePullResponce(int target, int sender, int imageId, Image image, int imageSize) : base(target, sender, MessageTypes.ImagePullResponce, imageSize)
        {
            ImageId = imageId;
            Image = image;
        }

        public int ImageId { get; }
        public Image Image { get; }
    }
}
