namespace Simulation.Messages
{
    public class ImagePullRequest : Message
    {
        public ImagePullRequest(int target, int sender, int imageId) : base(target, sender, MessageTypes.ImagePullRequest)
        {
            ImageId = imageId;
        }

        public int ImageId { get; }
    }
}
