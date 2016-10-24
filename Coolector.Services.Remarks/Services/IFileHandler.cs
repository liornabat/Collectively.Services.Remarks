﻿using System;
using System.Threading.Tasks;
using Coolector.Common.Types;
using Coolector.Services.Remarks.Domain;

namespace Coolector.Services.Remarks.Services
{
    public interface IFileHandler
    {
        Task UploadAsync(File file, string newName, Action<string> onUploaded = null);
        Task<Maybe<FileStreamInfo>> GetFileStreamInfoAsync(Guid remarkId, string size);
        Task<Maybe<FileStreamInfo>> GetFileStreamInfoAsync(string fileId);
        Task DeleteAsync(string fileId);
    }
}