namespace AISEP.Domain.Entities;

/// <summary>
/// AI-generated startup potential score (0-100)
/// </summary>
public class StartupScore : BaseEntity
{
    public int StartupProfileId { get; private set; }
    public decimal OverallScore { get; private set; } // 0-100
    public decimal TeamScore { get; private set; } // Weight: 25%
    public decimal MarketScore { get; private set; } // Weight: 25%
    public decimal ProductScore { get; private set; } // Weight: 20%
    public decimal TractionScore { get; private set; } // Weight: 20%
    public decimal FinancialsScore { get; private set; } // Weight: 10%
    public string? Reasoning { get; private set; } // JSON with detailed reasoning
    public string? Recommendations { get; private set; } // JSON with improvement suggestions
    public DateTime CalculatedAt { get; private set; }

    // Navigation properties
    public StartupProfile StartupProfile { get; private set; } = null!;

    private StartupScore() { }

    public static StartupScore Create(
        int startupProfileId,
        decimal teamScore,
        decimal marketScore,
        decimal productScore,
        decimal tractionScore,
        decimal financialsScore,
        string? reasoning = null,
        string? recommendations = null)
    {
        ValidateScore(teamScore, nameof(teamScore));
        ValidateScore(marketScore, nameof(marketScore));
        ValidateScore(productScore, nameof(productScore));
        ValidateScore(tractionScore, nameof(tractionScore));
        ValidateScore(financialsScore, nameof(financialsScore));

        // Calculate weighted overall score
        var overallScore = 
            (teamScore * 0.25m) +
            (marketScore * 0.25m) +
            (productScore * 0.20m) +
            (tractionScore * 0.20m) +
            (financialsScore * 0.10m);

        var score = new StartupScore
        {
            StartupProfileId = startupProfileId,
            OverallScore = Math.Round(overallScore, 2),
            TeamScore = teamScore,
            MarketScore = marketScore,
            ProductScore = productScore,
            TractionScore = tractionScore,
            FinancialsScore = financialsScore,
            Reasoning = reasoning,
            Recommendations = recommendations,
            CalculatedAt = DateTime.UtcNow
        };

        // TODO: Add domain event StartupScoreCalculatedEvent
        return score;
    }

    private static void ValidateScore(decimal score, string paramName)
    {
        if (score < 0 || score > 100)
            throw new ArgumentOutOfRangeException(paramName, "Score must be between 0 and 100");
    }

    public void UpdateRecommendations(string recommendations)
    {
        Recommendations = recommendations;
        UpdatedAt = DateTime.UtcNow;
    }
}
