using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace WebSocketManager
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, 
                                          WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                throw new Exception("Invalid web socket request.");

            AuthenticateResult authenticateResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                throw new Exception("Authentication failed during web socket request.");

            if (!authenticateResult.Principal.Identity.IsAuthenticated)
                throw new Exception("User is not authenticated.");

            string userId = authenticateResult.Principal.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Jti)
                   .Select(c => c.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(userId))
                throw new Exception("User id is not present in the authentication details.");



            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await _webSocketHandler.OnConnected(socket); //userId);
            
            await Receive(socket, async(result, buffer) =>
            {
                if(result.MessageType == WebSocketMessageType.Text)
                {
                    await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                    return;
                }

                else if(result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocketHandler.OnDisconnected(socket, userId);
                    return;
                }

            });
            
            //TODO - investigate the Kestrel exception thrown when this is the last middleware
            //await _next.Invoke(context);
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while(socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);                
            }
        }
    }
}