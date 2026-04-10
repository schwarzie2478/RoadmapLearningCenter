namespace Learning.Shared.Enums;

/// <summary>
/// Tracks represent curriculum domains within the roadmap.
/// </summary>
public enum RoadmapTrack
{
    Foundation,
    Backend,
    Blazor,
    FullStack,
    Senior
}

/// <summary>
/// Perceived difficulty level of a topic block.
/// </summary>
public enum Difficulty
{
    Beginner,
    Intermediate,
    Advanced
}

/// <summary>
/// Types of topic blocks — intro, explanation, example, exercise, quiz.
/// </summary>
public enum BlockType
{
    Intro,
    Explanation,
    Example,
    Exercise,
    Quiz
}

/// <summary>
/// Learner's current status for a specific topic.
/// </summary>
public enum TopicStatus
{
    NotStarted,
    Exploring,
    NeedReview,
    Done
}

/// <summary>
/// Who (or what) authored a paragraph in the thread.
/// </summary>
public enum ContentAuthor
{
    System,
    User,
    AI
}

/// <summary>
/// Context depth for AI explanations.
/// </summary>
public enum ExplanationContext
{
    Short,
    Deep,
    Analogy
}
