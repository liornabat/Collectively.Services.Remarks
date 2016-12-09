﻿using System;
using Coolector.Common.Events;

namespace Coolector.Services.Remarks.Shared.Events
{
    public class ResolveRemarkRejected : IRejectedEvent
    {
        public Guid RequestId { get; }
        public string UserId { get; }
        public Guid RemarkId { get; }
        public string Code { get; }
        public string Reason { get; }

        protected ResolveRemarkRejected()
        {
        }

        public ResolveRemarkRejected(Guid requestId, 
            string userId, Guid remarkId,
            string code, string reason)
        {
            RequestId = requestId;
            UserId = userId;
            RemarkId = remarkId;
            Code = code;
            Reason = reason;
        }
    }
}