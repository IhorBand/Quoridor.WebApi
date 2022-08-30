using System;
using Microsoft.AspNetCore.SignalR;
using Quoridor.DataAccess.Repositories;
using Quoridor.Service.Services;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.Abstractions.Services;
using Quoridor.Shared.DTO.Enums;
using Quoridor.Shared.DTO.InputModels.Game;
using Quoridor.Shared.Exceptions;
using SignalRSwaggerGen.Attributes;

namespace Quoridor.WebApi.Hubs
{
    [SignalRHub]
    public class GameHub : UserHubBase
    {
        private readonly ILogger<GameHub> logger;
        private readonly IGameBoardService gameBoardService;
        private readonly IGameUserService gameUserService;
        public GameHub(
            ILogger<GameHub> logger,
            IGameBoardService gameBoardService,
            IGameUserService gameUserService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gameBoardService = gameBoardService ?? throw new ArgumentNullException(nameof(gameBoardService));
            this.gameUserService = gameUserService ?? throw new ArgumentNullException(nameof(gameUserService));
        }

        /// <summary>
        ///   OnConnectedAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [SignalRHidden]
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        /// <summary>
        ///   OnDisconnectedAsync.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [SignalRHidden]
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGameAsync(Guid gameId)
        {
            try
            {
                var output = await this.gameUserService.AddPlayerToGameAsync(this.UserId, gameId).ConfigureAwait(false);
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, gameId.ToString()).ConfigureAwait(false);
                await this.Clients.GroupExcept(gameId.ToString(), this.Context.ConnectionId).SendAsync("OnUserJoinedGame", this.DisplayName, this.UserId, output.PlayerPosition, output.DirectionToWin).ConfigureAwait(false);
                await this.Clients.Caller.SendAsync("OnJoinGameSuccess", output.PlayerPosition, output.EnemyPosition, output.IsFull, output.IsAbleToMove, output.DirectionToWin).ConfigureAwait(false);
            }
            catch (RoomFullException exception)
            {
                this.logger.LogError(exception, "Room is full.");
                await this.Clients.Caller.SendAsync("OnJoinGameError", "Room is full.").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "[JoinGameAsync] Error.");
                await this.Clients.Caller.SendAsync("OnJoinGameError", "Server-side error.").ConfigureAwait(false);
            }
        }

        public async Task LeaveGameAsync(Guid gameId)
        {
            try
            {
                await this.gameUserService.RemoveUserFromGameAsync(gameId, this.UserId).ConfigureAwait(false);
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, gameId.ToString()).ConfigureAwait(false);
                await this.Clients.GroupExcept(gameId.ToString(), this.Context.ConnectionId).SendAsync("OnUserLeftGame", this.DisplayName, this.UserId).ConfigureAwait(false);
                await this.Clients.Caller.SendAsync("OnLeaveGameSuccess", "Success").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "[LeaveGameAsync] Error.");
                await this.Clients.Caller.SendAsync("OnLeaveGameError", "Server-side error.").ConfigureAwait(false);
            }
        }

        public async Task MakeMoveAsync(Guid gameId, Direction direction)
        {
            try
            {
                var isPlayerWon = await this.gameBoardService.MakeMoveAsync(gameId, this.UserId, direction).ConfigureAwait(false);
                if (isPlayerWon)
                {
                    await this.Clients.GroupExcept(gameId.ToString(), this.Context.ConnectionId).SendAsync("OnUserMadeWinningMove", this.UserId, direction).ConfigureAwait(false);
                    await this.Clients.Caller.SendAsync("OnPlayerWonAfterMove", "Success").ConfigureAwait(false);
                }
                else
                {
                    await this.Clients.GroupExcept(gameId.ToString(), this.Context.ConnectionId).SendAsync("OnUserMadeMove", this.UserId, direction).ConfigureAwait(false);
                    await this.Clients.Caller.SendAsync("OnMakeMoveSuccess", "Success").ConfigureAwait(false);
                }
            }
            catch (InvalidPositionException exception)
            {
                this.logger.LogError(exception, "[MakeMoveAsync] InvalidPositionException Error.");
                await this.Clients.Caller.SendAsync("OnMakeMoveError", exception.Message).ConfigureAwait(false);
            }
            catch (EmptyBoardException exception)
            {
                this.logger.LogError(exception, "[MakeMoveAsync] EmptyBoardException Error.");
                await this.Clients.Caller.SendAsync("OnMakeMoveError", exception.Message).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "[MakeMoveAsync] Error.");
                await this.Clients.Caller.SendAsync("OnMakeMoveError", "Server-side Error.").ConfigureAwait(false);
            }
        }

        public async Task BuildWallAsync(Guid gameId, Position positionStart, Position positionEnd)
        {
            try
            {
                await this.gameBoardService.BuildWallAsync(gameId, this.UserId, positionStart, positionEnd).ConfigureAwait(false);
                await this.Clients.GroupExcept(gameId.ToString(), this.Context.ConnectionId).SendAsync("OnUserBuiltWall", this.UserId, positionStart, positionEnd).ConfigureAwait(false);
                await this.Clients.Caller.SendAsync("OnBuildWallSuccess", "Success").ConfigureAwait(false);
            }
            catch (InvalidPositionException exception)
            {
                this.logger.LogError(exception, "[BuildWallAsync] InvalidPositionException Error.");
                await this.Clients.Caller.SendAsync("OnBuildWallError", exception.Message).ConfigureAwait(false);
            }
            catch (EmptyBoardException exception)
            {
                this.logger.LogError(exception, "[BuildWallAsync] EmptyBoardException Error.");
                await this.Clients.Caller.SendAsync("OnBuildWallError", exception.Message).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "[BuildWallAsync] Error.");
                await this.Clients.Caller.SendAsync("OnBuildWallError", "Server-side Error.").ConfigureAwait(false);
            }
        }

        /// <summary>
        ///   Use this method to send message to all clients.
        /// </summary>
        /// <param name="message">Message retrived from client.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SendMessageAsync(string message)
        {
            Console.WriteLine("Retrived Message");
            var newMessage = message + " UserId: " + this.UserId;

            await this.Clients.All.SendAsync("ReceiveMessage", newMessage).ConfigureAwait(false);
        }
    }
}
