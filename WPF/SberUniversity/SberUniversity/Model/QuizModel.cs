using Core;
using Newtonsoft.Json;

namespace SberUniversity.Model;

public class QuizModel : ObservableObject
{
    [JsonProperty("Question")]
    public List<QuizQuestionModel> Question
    {
        get => GetOrCreate<List<QuizQuestionModel>>();
        set => SetAndNotify(value);
    }

}