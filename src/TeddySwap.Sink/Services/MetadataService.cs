using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Services;

public class MetadataService
{
    public List<AssetClass> FindAssets(OuraTransaction transaction, List<string> policyIds)
    {

        List<AssetClass> assets = new();
        if (transaction.Metadata is not null)
        {
            assets.AddRange(ExtractAssets(transaction.Metadata.ToList(), policyIds));
        }

        return assets;
    }

    public List<AssetClass> FindAssets(Transaction transaction, List<string> policyIds)
    {

        List<AssetClass> assets = new();
        if (transaction.Metadata is not null)
        {
            byte[] metadataBytes = Convert.FromBase64String(transaction.Metadata);
            string stringMetadata = Encoding.UTF8.GetString(metadataBytes);
            List<Metadatum>? metadata = JsonSerializer.Deserialize<IEnumerable<Metadatum>>(stringMetadata)?.ToList();

            if (metadata is not null)
            {
                assets.AddRange(ExtractAssets(metadata, policyIds));
            }
        }

        return assets;
    }

    public List<AssetClass> ExtractAssets(List<Metadatum> metadata, List<string> policyIds)
    {
        List<AssetClass> assets = new();
        foreach (Metadatum metadatum in metadata)
        {
            if (metadatum.Content is null) continue;
            foreach (string policyId in policyIds)
            {
                // Deserialize Content to a JsonElement
                JsonElement? jsonElement = JsonSerializer.Deserialize<JsonElement>(metadatum.Content.ToString()!);

                if (jsonElement.Value.TryGetProperty(policyId, out JsonElement policyMetadata))
                {
                    foreach (var asset in policyMetadata.EnumerateObject())
                    {
                        assets.Add(new()
                        {
                            PolicyId = policyId.ToLower(),
                            Name = Convert.ToHexString(Encoding.ASCII.GetBytes(asset.Name)).ToLower(),
                            AsciiName = asset.Name,
                            Metadata = JsonSerializer.Serialize(asset)
                        });
                    }
                }
            }
        }
        return assets;
    }
}