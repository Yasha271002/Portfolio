using Core;
using Newtonsoft.Json;

namespace SberUniversity.Model;

public class QuizQuestionModel:ObservableObject
{
    [JsonProperty("Question")]
    public string? Question
    {
        get => GetOrCreate("1. На какой срок у вас есть карьерная цель:");
        set => SetAndNotify(value);
    }

    [JsonProperty("Answers")]
    public AnswersModel Answers
    {
        get => GetOrCreate(new AnswersModel());
        set => SetAndNotify(value);
    }

    [JsonIgnore]
    public int Answer
    {
        get => GetOrCreate<int>();
        set => SetAndNotify(value);
    }

    [JsonIgnore]
    public int QuestionCount
    {
        get => GetOrCreate(0);
        set => SetAndNotify(value);
    }
}

public class AnswersModel:ObservableObject
{
    [JsonProperty("First")]
    public string? FirstQuestion
    {
        get => GetOrCreate("Нет цели");
        set => SetAndNotify(value);
    }

    [JsonProperty("Second")]
    public string? SecondQuestion   
    {
        get => GetOrCreate("1-3 года");
        set => SetAndNotify(value);
    }

    [JsonProperty("Three")]
    public string? ThreeQuestion
    {
        get => GetOrCreate("3+ лет");
        set => SetAndNotify(value);
    }
}