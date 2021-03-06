﻿namespace Simulation.Messages
{
    public enum MessageTypes
    {
        PushRequest,
        PushLoadAvailabilityRequest,
        PullRequest,
        PullLoadAvailabilityRequest,
        InitiateMigration,
        MigrateContainerRequest,
        MigrateContainerResponse,
        RejectRequest,
        BidCancellationRequest,
        LoadAvailabilityResponse,
        UtilizationStateChange,
        EvacuationDone,
        WinnerAnnouncementMessage,
        CommonLoadManager,
        CancelEvacuation,

        //Images
        ImageTreeRequest,
        ImageTreeResponce,
        ImagePullRequest,
        ImagePullResponce,
        ImageLoadRequest,
        ImageLoadResponce,

        //Pulls
        PullsCountRequest,
        PullsCountResponce,

        //Scheduling
        CanHaveContainerRequest,
        CanHaveContainerResponce,
        AddContainerRequest,

    }
}