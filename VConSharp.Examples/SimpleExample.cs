namespace VConSharp.Examples
{
    public static class SimpleExample
    {
        public static void Run()
        {
            Console.WriteLine("Running Simple Example");
            Console.WriteLine("======================");

            // Create a new vCon
            var vcon = VCon.BuildNew();

            // Add parties
            var party1 = new Party(new Dictionary<string, object>
            {
                ["tel"] = "+1234567890",
                ["name"] = "John Doe",
                ["role"] = "customer",
            });

            var party2 = new Party(new Dictionary<string, object>
            {
                ["tel"] = "+0987654321",
                ["name"] = "Jane Smith",
                ["role"] = "agent",
            });

            vcon.AddParty(party1);
            vcon.AddParty(party2);

            // Add a text dialog
            var textDialog = new Dialog(
                "text/plain",
                DateTime.UtcNow,
                new[] { 0, 1, });

            textDialog.Originator = 0;
            textDialog.Body = "Hello, I need help with my account.";
            textDialog.Mimetype = "text/plain";

            vcon.AddDialog(textDialog);

            // Add a response dialog
            var responseDialog = new Dialog(
                "text/plain",
                DateTime.UtcNow.AddSeconds(1),
                new[] { 0, 1, });

            responseDialog.Originator = 1;
            responseDialog.Body = "Hello John, how can I assist you today?";
            responseDialog.Mimetype = "text/plain";

            vcon.AddDialog(responseDialog);

            // Add an attachment
            vcon.AddAttachment(
                "application/pdf",
                "base64EncodedContent",
                Encoding.Base64);

            // Add analysis
            vcon.AddAnalysis(new Dictionary<string, object>
            {
                ["type"] = "sentiment",
                ["dialog"] = new[] { 0, 1, }, // Analyze both dialogs
                ["vendor"] = "sentiment-analyzer",
                ["body"] = new Dictionary<string, object>
                {
                    ["score"] = 0.8,
                    ["label"] = "positive",
                },
            });

            // Add tags
            vcon.AddTag("category", "support");
            vcon.AddTag("priority", "medium");

            // Convert to JSON
            var json = vcon.ToJson();
            Console.WriteLine($"vCon JSON length: {json.Length}");

            // Load from JSON
            var loadedVcon = VCon.BuildFromJson(json);
            Console.WriteLine($"Loaded vCon UUID: {loadedVcon.Uuid}");
            Console.WriteLine($"Loaded vCon parties: {loadedVcon.Parties.Count}");
            Console.WriteLine($"Loaded vCon dialogs: {loadedVcon.Dialog.Count}");
            Console.WriteLine($"Loaded vCon attachments: {loadedVcon.Attachments.Count}");
            Console.WriteLine($"Loaded vCon analysis: {loadedVcon.Analysis.Count}");
            Console.WriteLine($"Loaded vCon tags: {loadedVcon.GetTag("category")}, {loadedVcon.GetTag("priority")}");
        }
    }
}
