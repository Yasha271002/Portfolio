using Core;
using SberUniversity.Model;
using System.IO;
using SberUniversity.Helpers;

namespace SberUniversity.ViewModel.Views;

public class StartPageViewModel : ObservableObject
{
    private readonly JsonHelper _jsonHelper;

    public QuizModel Quizzes
    {
        get => GetOrCreate<QuizModel>();
        set => SetAndNotify(value);
    }

    public StartPageViewModel()
    {
        _jsonHelper = new JsonHelper();
        Quizzes = new QuizModel
        {
            Question =
            [
                new QuizQuestionModel
                {
                    Answers = new AnswersModel
                    {
                        FirstQuestion = "Нет цели",
                        SecondQuestion = "1-3 года",
                        ThreeQuestion = "3+ лет"
                    },
                    Question = "1. На какой срок у вас есть карьерная цель:"
                }
            ],
        };

        GetQuizzes();
    }

    private void GetQuizzes()
    {
        var path = "Quizzes";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        Quizzes = _jsonHelper.ReadJsonFromFile(SingletonSettings.Instance.Settings.QuizzesSettingsPaths!, Quizzes);
    }
}