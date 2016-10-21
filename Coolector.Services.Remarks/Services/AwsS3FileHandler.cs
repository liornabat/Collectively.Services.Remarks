﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Coolector.Common.Types;
using Coolector.Services.Remarks.Domain;
using Coolector.Services.Remarks.Settings;
using File = Coolector.Services.Remarks.Domain.File;

namespace Coolector.Services.Remarks.Services
{
    public class AwsS3FileHandler : IFileHandler
    {
        private readonly IAmazonS3 _client;
        private readonly AwsS3Settings _settings;

        public AwsS3FileHandler(IAmazonS3 client, AwsS3Settings settings)
        {
            _client = client;
            _settings = settings;
        }

        public async Task UploadAsync(File file, string newName, Action<string> onUploaded = null)
        {
            using (var stream = new MemoryStream(file.Bytes))
            {

                await _client.UploadObjectFromStreamAsync(_settings.Bucket, newName,
                    stream, new Dictionary<string, object>());
            }
        }

        public async Task<Maybe<FileStreamInfo>> GetFileStreamInfoAsync(Guid remarkId)
        {
            return new Maybe<FileStreamInfo>();
        }

        public async Task<Maybe<FileStreamInfo>> GetFileStreamInfoAsync(string fileId)
        {
            return new Maybe<FileStreamInfo>();
        }

        public async Task DeleteAsync(string fileId)
        {
        }
    }
}