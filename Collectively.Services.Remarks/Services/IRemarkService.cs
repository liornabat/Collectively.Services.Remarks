﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using  Collectively.Common.Types;
using Collectively.Services.Remarks.Domain;
using Collectively.Services.Remarks.Queries;
using File = Collectively.Services.Remarks.Domain.File;

namespace Collectively.Services.Remarks.Services
{
    public interface IRemarkService
    {
        Task<Maybe<Remark>> GetAsync(Guid id);
        Task<Maybe<PagedResult<Remark>>> BrowseAsync(BrowseRemarks query);
        Task<Maybe<PagedResult<Category>>> BrowseCategoriesAsync(BrowseCategories query);
        Task<Maybe<PagedResult<Tag>>> BrowseTagsAsync(BrowseTags query);
        Task<Maybe<FileStreamInfo>> GetPhotoAsync(Guid id, string size);
        Task ValidateEditorAccessOrFailAsync(Guid remarkId, string userId);

        Task CreateAsync(Guid id, string userId, string category,
            Location location, string description = null, IEnumerable<string> tags = null);

        Task ResolveAsync(Guid id, string userId, File photo = null, Location location = null, bool validateLocation = false);
        Task UpdateUserNamesAsync(string userId, string name);
        Task AddPhotosAsync(Guid id, params File[] photos);
        Task<Maybe<IEnumerable<string>>> GetPhotosForGroupsAsync(Guid id, params Guid[] groupIds);
        Task RemovePhotosAsync(Guid id, params string[] names);
        Task DeleteAsync(Guid id);

        Task SubmitVoteAsync(Guid remarkId, string userId, bool positive, DateTime createdAt);
        Task DeleteVoteAsync(Guid remarkId, string userId);
    }
}