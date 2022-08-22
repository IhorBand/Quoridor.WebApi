namespace Quoridor.WebApi.Hubs
{
    using System;
    using Microsoft.AspNetCore.SignalR;

    public class TestHub : Hub
    {
        /// <summary>
        ///   Equals.
        /// </summary>
        /// <param name="obj">object to check.</param>
        /// <returns>A <see cref="bool"/> isEqual.</returns>
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        ///   Returns HashCode.
        /// </summary>
        /// <returns>A <see cref="int"/> hashCode.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///   OnConnectedAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        /// <summary>
        ///   OnDisconnectedAsync.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        ///   Use this method to send message to all clients.
        /// </summary>
        /// <param name="message">Message retrived from client.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SendMessageAsync(string message)
        {
            Console.WriteLine("Retrived Message");
            await this.Clients.All.SendAsync("ReceiveMessage", message).ConfigureAwait(false);
        }

        /// <summary>
        ///   ToString.
        /// </summary>
        /// <returns>A <see cref="string"/> returns string.</returns>
        public override string? ToString()
        {
            return base.ToString();
        }

        /// <summary>
        ///   Disposing.
        /// </summary>
        /// <param name="disposing"> isDisposing.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
