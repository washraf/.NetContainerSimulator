namespace Simulation.Messages
{
    public class ImageLoadRequest : Message
    {
        public ImageLoadRequest(int target, int sender, int imageId, int containerId) : base(target, sender, MessageTypes.ImageLoadRequest)
        {
            ImageId = imageId;
            ContainerId = containerId;
        }

        public int ImageId { get; }
        public int ContainerId { get; }
    }
    public class ImageLoadResponce : Message
    {
        public ImageLoadResponce(int target, int sender,int containerId, bool state) : base(target, sender, MessageTypes.ImageLoadResponce)
        {
            ContainerId = containerId;
            State = state;
        }

        public int ContainerId { get; }
        public bool State { get; }
    }
}
