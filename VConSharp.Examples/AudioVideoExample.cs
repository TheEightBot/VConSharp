using VConSharp;

namespace VConSharp.Examples
{
    public class AudioVideoExample
    {
        public static void Run()
        {
            Console.WriteLine("Running Audio Video Example");
            Console.WriteLine("==========================");

            // Create a new vCon
            var vcon = VCon.BuildNew();

            // Add parties
            var caller = new Party(new Dictionary<string, object>
            {
                ["tel"] = "+1234567890",
                ["name"] = "John Doe",
                ["role"] = "customer",
            });

            var agent = new Party(new Dictionary<string, object>
            {
                ["tel"] = "+0987654321",
                ["name"] = "Jane Smith",
                ["role"] = "agent",
            });

            vcon.AddParty(caller);
            vcon.AddParty(agent);

            // Add a text dialog (initial request)
            var textDialog = new Dialog(
                "text/plain",
                DateTime.UtcNow,
                new[] { 0, 1 });

            textDialog.Originator = 0;
            textDialog.Body = "I need to schedule a video call for technical support.";
            textDialog.Mimetype = "text/plain";

            vcon.AddDialog(textDialog);

            // Add an audio dialog (phone call)
            var audioDialog = new Dialog(
                "audio/wav",
                DateTime.UtcNow.AddSeconds(1),
                new[] { 0, 1 });

            audioDialog.Originator = 0;
            audioDialog.Mimetype = "audio/wav";
            audioDialog.Filename = "call-recording.wav";
            audioDialog.Duration = 300; // 5 minutes

            // Add external audio data
            audioDialog.AddExternalData(
                "https://example.com/recordings/call-recording.wav",
                "call-recording.wav",
                "audio/wav");

            vcon.AddDialog(audioDialog);

            // Add a video dialog (screen sharing)
            var videoDialog = new Dialog(
                "video/mp4",
                DateTime.UtcNow.AddSeconds(310),
                new[] { 0, 1 });

            videoDialog.Originator = 1;
            videoDialog.Mimetype = "video/mp4";
            videoDialog.Filename = "screen-sharing.mp4";
            videoDialog.Duration = 600; // 10 minutes

            // Add inline video data (base64 encoded)
            videoDialog.AddInlineData(
                "base64EncodedVideoContent",
                "screen-sharing.mp4",
                "video/mp4");

            vcon.AddDialog(videoDialog);

            // Add analysis for audio sentiment
            vcon.AddAnalysis(new Dictionary<string, object>
            {
                ["type"] = "sentiment",
                ["dialog"] = 1, // Analyze the audio dialog
                ["vendor"] = "sentiment-analyzer",
                ["body"] = new Dictionary<string, object>
                {
                    ["score"] = 0.7,
                    ["label"] = "positive",
                    ["segments"] = new[]
                    {
                        new Dictionary<string, object> { ["start"] = 0, ["end"] = 60, ["score"] = 0.6, ["label"] = "neutral" },
                        new Dictionary<string, object> { ["start"] = 60, ["end"] = 180, ["score"] = 0.8, ["label"] = "positive" },
                        new Dictionary<string, object> { ["start"] = 180, ["end"] = 300, ["score"] = 0.7, ["label"] = "positive" },
                    },
                },
            });

            // Add analysis for video content
            vcon.AddAnalysis(new Dictionary<string, object>
            {
                ["type"] = "content-classification",
                ["dialog"] = 2, // Analyze the video dialog
                ["vendor"] = "content-analyzer",
                ["body"] = new Dictionary<string, object>
                {
                    ["labels"] = new[] { "screen-sharing", "technical-support", "software-demo" },
                    ["confidence"] = 0.92,
                },
            });

            // Add tags
            vcon.AddTag("category", "technical-support");
            vcon.AddTag("priority", "high");
            vcon.AddTag("media-types", "audio,video");

            // Check dialog types
            Console.WriteLine($"Text dialog is text: {textDialog.IsText()}"); // true
            Console.WriteLine($"Audio dialog is audio: {audioDialog.IsAudio()}"); // true
            Console.WriteLine($"Video dialog is video: {videoDialog.IsVideo()}"); // true

            // Check data storage
            Console.WriteLine($"Audio is external: {audioDialog.IsExternalData()}"); // true
            Console.WriteLine($"Video is inline: {videoDialog.IsInlineData()}"); // true

            // Convert to JSON
            var json = vcon.ToJson();
            Console.WriteLine($"vCon JSON length: {json.Length}");

            // Load from JSON
            var loadedVcon = VCon.BuildFromJson(json);
            Console.WriteLine($"Loaded vCon UUID: {loadedVcon.Uuid}");
            Console.WriteLine($"Loaded vCon parties: {loadedVcon.Parties.Count}");
            Console.WriteLine($"Loaded vCon dialogs: {loadedVcon.Dialogs.Count}");
            Console.WriteLine($"Loaded vCon analysis: {loadedVcon.Analysis.Count}");
            Console.WriteLine($"Loaded vCon tags: {string.Join(", ", loadedVcon.Tags!.Select(t => $"{t.Key}={t.Value}"))}");
        }
    }
}
