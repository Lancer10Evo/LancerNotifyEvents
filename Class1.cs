using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace LancerNotifyEvent
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "LancerNotifyEvent";
        public override string Author => "Lancer Team";
        public override Version Version => new Version(1, 3, 0);

        private static readonly HttpClient Http = new HttpClient();
        private bool _hudLoopRunning = false;

        public override void OnEnabled()
        {
            if (!_hudLoopRunning)
            {
                _hudLoopRunning = true;
                _ = UpdateHUDLoop();
            }
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _hudLoopRunning = false;
            base.OnDisabled();
        }

        private async Task UpdateHUDLoop()
        {
            while (_hudLoopRunning)
            {
                string hudMessage = $"<color=#00FFFF>📸 Проводящий:</color> {Config.EventHost}\n" +
                                    $"<color=#00FF00>💪 Уровень РП:</color> {Config.EventRp}\n" +
                                    $"<color=#FF4500>♦️ Название:</color> {Config.EventName}\n\n" +
                                    $"<size=12><color=#AAAAAA>Сделано By Lancer Team</color></size>";

                foreach (var player in Player.List)
                {
                    player.ShowHint(hudMessage, 5f); // HUD 5 секунд
                }

                if (!string.IsNullOrEmpty(Config.DiscordWebhook))
                {
                    _ = SendToDiscord(hudMessage);
                }

                await Task.Delay(3000); // Обновляем каждые 3 секунды
            }
        }

        private async Task SendToDiscord(string message)
        {
            try
            {
                var json = $"{{\"content\":\"{message}\"}}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await Http.PostAsync(Config.DiscordWebhook, content);
            }
            catch (Exception e)
            {
                Log.Error($"[LancerNotifyEvent] Ошибка отправки в Discord: {e.Message}");
            }
        }
    }

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public string EventHost { get; set; } = "Admin";
        public string EventRp { get; set; } = "Medium RP";
        public string EventName { get; set; } = "Lancer Classic Event";

        // 🔗 Discord Webhook (оставь пустым, если не нужен)
        public string DiscordWebhook { get; set; } = "";
    }
}