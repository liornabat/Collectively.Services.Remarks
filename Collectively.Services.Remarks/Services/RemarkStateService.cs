using System;
using System.Threading.Tasks;
using Collectively.Common.Domain;
using Collectively.Common.Files;
using Collectively.Services.Remarks.Domain;
using Collectively.Services.Remarks.Extensions;
using Collectively.Services.Remarks.Repositories;
using Collectively.Services.Remarks.Settings;
using NLog;

namespace Collectively.Services.Remarks.Services
{
    public class RemarkStateService : IRemarkStateService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRemarkRepository _remarkRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRemarkPhotoService _remarkPhotoService;
        private readonly GeneralSettings _settings;

        public RemarkStateService(IRemarkRepository remarkRepository, 
            IUserRepository userRepository, 
            IRemarkPhotoService remarkPhotoService,
            GeneralSettings settings)
        {
            _remarkRepository = remarkRepository;
            _userRepository = userRepository;
            _remarkPhotoService = remarkPhotoService;
            _settings = settings;
        }

        public async Task ResolveAsync(Guid id, string userId, string description = null, Location location = null, 
            File photo = null, bool validateLocation = false)
         => await UpdateRemarkStateAsync(RemarkState.Names.Resolved, 
            (r,u,d) => r.SetResolvedState(u,d, location), id, userId, description, location, photo, validateLocation);

        public async Task ProcessAsync(Guid id, string userId, string description = null, Location location = null)
         => await UpdateRemarkStateAsync(RemarkState.Names.Processing, 
            (r,u,d) => r.SetProcessingState(u,d), id, userId, description, location);

        public async Task RenewAsync(Guid id, string userId, string description = null, Location location = null)
         => await UpdateRemarkStateAsync(RemarkState.Names.Renewed, 
            (r,u,d) => r.SetRenewedState(u,d), id, userId, description, location);

        public async Task CancelAsync(Guid id, string userId, string description = null, Location location = null)
         => await UpdateRemarkStateAsync(RemarkState.Names.Canceled, 
            (r,u,d) => r.SetCanceledState(u,d), id, userId, description, location);

        private async Task UpdateRemarkStateAsync(string state, Action<Remark,User,string> updateStateAction,
            Guid id, string userId, string description = null, Location location = null, 
            File photo = null, bool validateLocation = false)
        {
            var user = await _userRepository.GetByUserIdAsync(userId);
            if (user.HasNoValue)
            {
                throw new ServiceException(OperationCodes.UserNotFound,
                    $"User with id: '{userId}' does not exist!");
            }

            var remark = await _remarkRepository.GetByIdAsync(id);
            if (remark.HasNoValue)
            {
                throw new ServiceException(OperationCodes.RemarkNotFound,
                    $"Remark with id: {id} does not exist!");
            }
            if (location != null && validateLocation && remark.Value.Location.IsInRange(location, _settings.AllowedDistance) == false)
            {
                throw new ServiceException(OperationCodes.DistanceBetweenUserAndRemarkIsTooBig,
                    $"The distance between user and remark: {id} is too big! " +
                    $"lat:{location.Latitude}, long:{location.Longitude}");
            }
            if (photo != null)
            {
                await _remarkPhotoService.UploadImagesWithDifferentSizesAsync(remark.Value, photo, state);
            }
            updateStateAction(remark.Value, user.Value, description);
            await _remarkRepository.UpdateAsync(remark.Value);              
        }        
    }
}