namespace Simulation.Messages
{
    public class ImageTreeRequest : Message
    {
        public ImageTreeRequest(int target, int sender, int imageId) : base(target, sender, MessageTypes.ImageTreeRequest)
        {
            ImageId = imageId;
        }

        public int ImageId { get; }
    }
}
