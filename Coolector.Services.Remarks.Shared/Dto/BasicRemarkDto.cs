﻿using System;
using Coolector.Common.Dto;

namespace Coolector.Services.Remarks.Shared.Dto
{
    public class BasicRemarkDto
    {
        public Guid Id { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public LocationDto Location { get; set; }
        public string SmallPhotoUrl { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Resolved { get; set; }
    }
}