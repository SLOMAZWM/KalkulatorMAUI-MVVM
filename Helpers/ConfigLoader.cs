using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KalkulatorMAUI_MVVM.Helpers
{
    public static class ConfigLoader
    {
        public static Dictionary<string, string> LoadConfig()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
            var json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
    }
}
