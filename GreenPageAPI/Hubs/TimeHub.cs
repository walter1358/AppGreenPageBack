using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

public class TimeHub : Hub
{
    private static Timer _timer;
    private static IHubContext<TimeHub> _hubContext;

    public TimeHub(IHubContext<TimeHub> hubContext)
    {
        _hubContext = hubContext;
        StartSendingTime();
    }

    private void StartSendingTime()
    {
        // Evitar múltiples timers si la clase se instancia varias veces
        if (_timer == null)
        {
            _timer = new Timer(SendServerTimeToClients, null, TimeSpan.Zero, TimeSpan.FromSeconds(1)); // Enviar cada segundo
        }
    }

    private async void SendServerTimeToClients(object state)
    {
        var currentTime = DateTime.UtcNow.ToString("o");  // Usa el formato que prefieras
        await _hubContext.Clients.All.SendAsync("ReceiveServerTime", currentTime);  // Enviar la hora a todos los clientes conectados
    }
}
