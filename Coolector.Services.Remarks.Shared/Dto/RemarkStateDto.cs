using System;

namespace Coolector.Services.Remarks.Shared.Dto
{
    public class RemarkStateDto
    {
        public string State { get; set; }
        public RemarkUserDto User { get; set; }
        public string Description { get; set; }
        public LocationDto Location { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}