using System.Diagnostics;
using System.Text.Json;

namespace AliceNeural.Utils
{
    public class StoreManager
    {
		public static BingMapsStore GetBingMapsStore()
		{
			using var stream = FileSystem.Current.OpenAppPackageFileAsync("_secretBingMapsStore.json").Result;
			using var reader = new StreamReader(stream);
			string store = reader.ReadToEnd();
			BingMapsStore? bingMapsStore = JsonSerializer.Deserialize<BingMapsStore>(store);
			return bingMapsStore ?? new BingMapsStore();
		}
		public static AzureSpeechServiceStore GetSpeechDataFromStore()
        {
            //il file _secretMyAzureRoboVoiceStore.json deve essere creato con le coppie chiave-valore contenenti i secrets
            //il .gitignore è stato modificato, aggiungendo una regola che ignora i file del tipo _secret*.json
            using var stream = FileSystem.Current.OpenAppPackageFileAsync("_secretMyAzureRoboVoiceStore.json").Result;
            using var reader = new StreamReader(stream);
            string store = reader.ReadToEnd();
            AzureSpeechServiceStore? azureSpeechServiceStore = JsonSerializer.Deserialize<AzureSpeechServiceStore>(store);
            return azureSpeechServiceStore ?? new AzureSpeechServiceStore();
        }

        public static AzureIntentRecognitionByCLUStore GetCLUDataFromStore()
        {
            //il file _secretMyAzureIntentRecognitionByCLUStore.json deve essere creato con le coppie chiave-valore contenenti i secrets
            using var stream = FileSystem.Current.OpenAppPackageFileAsync("_secretMyAzureIntentRecognitionByCLUStore.json").Result;
            using var reader = new StreamReader(stream);
            string store = reader.ReadToEnd();
            AzureIntentRecognitionByCLUStore? azureSpeechServiceStore = JsonSerializer.Deserialize<AzureIntentRecognitionByCLUStore>(store);
            return azureSpeechServiceStore ?? new AzureIntentRecognitionByCLUStore();
        }
    }
}
