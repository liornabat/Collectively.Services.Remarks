﻿using System;
using Coolector.Common.Commands;
using Coolector.Services.Remarks.Shared.Commands.Models;

namespace Coolector.Services.Remarks.Shared.Commands
{
    public class CreateRemark : IAuthenticatedCommand
    {
        public Guid RemarkId { get; set; }
        public Request Request { get; set; }
        public string UserId { get; set; }
        public string Category { get; set; }
        public RemarkFile Photo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
    }
}