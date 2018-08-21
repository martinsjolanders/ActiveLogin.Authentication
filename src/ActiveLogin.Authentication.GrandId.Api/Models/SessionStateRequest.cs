﻿namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class SessionStateRequest
    {
        public SessionStateRequest(DeviceOption deviceOption, string sessionId)
        {
            DeviceOption = deviceOption;
            SessionId = sessionId;
        }
        public DeviceOption DeviceOption { get; set; }
        
        public string SessionId { get; set; }
    }
}