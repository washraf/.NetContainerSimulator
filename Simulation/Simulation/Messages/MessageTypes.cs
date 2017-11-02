﻿namespace Simulation.Messages
{
    public enum MessageTypes
    {
        //GetHostLoadInfoRequest,
        //GetHostLoadInfoResponce,
        //PushContainerRequest,
        //PushContainerResponse,
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
        CancelEvacuation
    }
}