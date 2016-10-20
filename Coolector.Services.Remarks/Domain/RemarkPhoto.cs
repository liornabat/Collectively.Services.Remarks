﻿using System;
using Coolector.Common.Extensions;
using Coolector.Common.Domain;

namespace Coolector.Services.Remarks.Domain
{
    public class RemarkPhoto : ValueObject<RemarkPhoto>
    {
        public string FileId { get; protected set; }
        public string Name { get; protected set; }
        public string OriginalName { get; protected set; }
        public string ContentType { get; protected set; }

        protected RemarkPhoto()
        {
        }

        protected RemarkPhoto(string fileId, string name, string originalName, string contentType)
        {
            if (fileId.Empty())
                throw new ArgumentException("Photo file id can not be empty.", nameof(fileId));
            if (name.Empty())
                throw new ArgumentException("Photo name id can not be empty.", nameof(name));
            if (originalName.Empty())
                throw new ArgumentException("Photo original name id can not be empty.", nameof(originalName));
            if (contentType.Empty())
                throw new ArgumentException("Photo content type id can not be empty.", nameof(contentType));

            FileId = fileId;
            Name = name;
            OriginalName = originalName;
            ContentType = contentType;
        }

        public static RemarkPhoto Empty => new RemarkPhoto();

        public static RemarkPhoto Create(string fileId, string name, string originalName, string contentType)
            => new RemarkPhoto(fileId, name, originalName, contentType);

        protected override bool EqualsCore(RemarkPhoto other) => FileId.Equals(other.FileId);

        protected override int GetHashCodeCore() => FileId.GetHashCode();
    }
}