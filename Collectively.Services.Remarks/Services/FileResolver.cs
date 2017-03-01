﻿using System;
using  Collectively.Common.Extensions;
using  Collectively.Common.Types;
using File = Collectively.Services.Remarks.Domain.File;

namespace Collectively.Services.Remarks.Services
{
    public class FileResolver : IFileResolver
    {
        public Maybe<File> FromBase64(string base64, string name, string contentType)
        {
            if (base64.Empty())
                return new Maybe<File>();
            if (name.Empty())
                return new Maybe<File>();
            if (contentType.Empty())
                return new Maybe<File>();

            var startIndex = 0;
            if (base64.Contains(","))
                startIndex = base64.IndexOf(",", StringComparison.CurrentCultureIgnoreCase) + 1;

            var base64String = base64.Substring(startIndex);
            var bytes = Convert.FromBase64String(base64String);
            
            return File.Create(name, contentType, bytes);
        }
    }
}