﻿using System.Threading.Tasks;
using Collectively.Messages.Commands;
using Collectively.Common.Services;
using Collectively.Services.Remarks.Services;
using Collectively.Messages.Commands.Remarks;
using Collectively.Messages.Events.Remarks;
using RawRabbit;

namespace Collectively.Services.Remarks.Handlers
{
    public class DeleteRemarkHandler : ICommandHandler<DeleteRemark>
    {
        private readonly IHandler _handler;
        private readonly IBusClient _bus;
        private readonly IRemarkService _remarkService;
        private readonly IResourceFactory _resourceFactory;

        public DeleteRemarkHandler(IHandler handler, IBusClient bus, IRemarkService remarkService, IResourceFactory resourceFactory)
        {
            _handler = handler;
            _bus = bus;
            _remarkService = remarkService;
            _resourceFactory = resourceFactory;
        }

        public async Task HandleAsync(DeleteRemark command)
        {
            await _handler
                .Validate(async () => await _remarkService.ValidateEditorAccessOrFailAsync(command.RemarkId, command.UserId))
                .Run(async () =>
                {
                    await _remarkService.DeleteAsync(command.RemarkId);
                })
                .OnSuccess(async () =>
                {
                    var resource = _resourceFactory.Resolve<RemarkDeleted>(command.RemarkId);
                    await _bus.PublishAsync(new RemarkDeleted(command.Request.Id, resource, 
                        command.UserId, command.RemarkId));
                })
                .OnCustomError(ex => _bus.PublishAsync(new DeleteRemarkRejected(command.Request.Id,
                    command.RemarkId, command.UserId, ex.Code, ex.Message)))
                .OnError(async (ex, logger) =>
                {
                    logger.Error(ex, "Error occured while deleting a remark.");
                    await _bus.PublishAsync(new DeleteRemarkRejected(command.Request.Id,
                        command.RemarkId, command.UserId, OperationCodes.Error, ex.Message));
                })
                .ExecuteAsync();
        }
    }
}