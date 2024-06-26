﻿using System.Net.Sockets;
using System.Text;
using System.Text.Json;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace MerrMail.Infrastructure.Services;

public partial class TensorFlowEmailAnalyzerService
{
    /// <summary>
    /// Gets the similarity score between two strings provided by TensorFlow.
    /// The snake casing indicates that it's using Python to get similarity score.
    /// </summary>
    /// <param name="first">The first text to compare.</param>
    /// <param name="second">The second text to compare.</param>
    /// <returns>The similarity score between the two texts.</returns>
#pragma warning disable IDE1006 // Naming Styles
    private static float get_similarity_score(string first, string second)
#pragma warning restore IDE1006 // Naming Styles
    {
        using var tcp = new TcpClient("localhost", 63778);
        using var network = tcp.GetStream();
        try
        {
            var data = new { first, second };
            var json = JsonSerializer.Serialize(data);
            var serialized_data = Encoding.UTF8.GetBytes(json);

            network.Write(serialized_data, 0, serialized_data.Length);

            var buffer = new byte[8192];
            var bytes_read = network.Read(buffer, 0, buffer.Length);
            var response_json = Encoding.UTF8.GetString(buffer, 0, bytes_read);

            var response = JsonSerializer.Deserialize<float>(response_json);

            // The cosine similarity score provided by TensorFlow is in negative value, so we reverse the sign
            var fixedScore = Math.Abs(response);
            return fixedScore;
        }
        catch
        {
            return 0.0f;
        }
    }
}