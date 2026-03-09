using System.IO;
using System.Windows.Input;
using Core;
using SberUniversity.Helpers;
using SberUniversity.Model;

namespace SberUniversity.ViewModel.Views;

public class QuizPageViewModel : ObservableObject
{
    public QuizModel Quizzes
    {
        get => GetOrCreate<QuizModel>();
        set => SetAndNotify(value);
    }

    private List<ResultModel> ResultsModel
    {
        get => GetOrCreate<List<ResultModel>>();
        set => SetAndNotify(value);
    }

    private ResultModel ResultModel
    {
        get => GetOrCreate<ResultModel>();
        set => SetAndNotify(value);
    }

    private int QuizResult
    {
        get => GetOrCreate<int>();
        set => SetAndNotify(value);
    }
    public bool ThumbDragging
    {
        get => GetOrCreate<bool>();
        set => SetAndNotify(value);
    }

    private readonly JsonHelper _jsonHelper;

    public QuizPageViewModel(QuizModel quizzes)
    {
        _jsonHelper = new JsonHelper();
        Quizzes = quizzes;
        ResultsModel = new List<ResultModel>
        {
            new()
            {
                ResultTitle = "Ваш результат 0-2 баллов",
                Description = "Похоже, вы\u00a0упускаете карьерные возможности, которые вас окружают, а\u00a0вместе с\u00a0ними и\u00a0интересные перспективы. Пройдя этот тест, вы\u00a0уже сделали первый шаг, поздравляем!\r\nДля дальнейшего развития предлагаем ознакомиться с возможностями диагностики и карьерного консультирования SberQ. Вот программы, на которые вы можете обратить внимание прямо сейчас:",
                ResultUpgrade = new ResultUpgradeModel
                {
                    FirstUpgradeTitle = "Upgrade: личностный интенсив",
                    FirstUpgradeDescription = "позволит увидеть перспективы и найти свои точка роста",

                    SecondUpgradeTitle = "Управление изменениями ",
                    SecondUpgradeDescription = "научит навигировать компанию в постоянно меняющемся окружении",

                    LastUpgradeTitle = "Бизнес с AI: от теории к практике",
                    LastUpgradeDescription = "программа направлена на изучение процессов разработки и внедрения AI-технологий в бизнесе с целью оптимизации процессов и повышение экономической эффективности"
                }
            }
        };

        if (!Directory.Exists("Result"))
            Directory.CreateDirectory("Result");

        GetResultsDataPage();
    }

    public ICommand NextPage => GetOrCreate(new RelayCommand(f =>
    {
        SelectResultModel();

        CommonCommands.NavigateCommand.Execute(ResultModel);
    }));
    public ICommand DragStartedCommand => GetOrCreate(new RelayCommand(f =>
    {
        ThumbDragging = true;
    }));

    public ICommand DragCompletedCommand => GetOrCreate(new RelayCommand(async f =>
    {
        ThumbDragging = false;
        await Task.Delay(300);
        if (f is not QuizQuestionModel model) return;
        model.QuestionCount = model.QuestionCount switch
        {
            < 225 => 0,
            >= 675 => 900,
            >= 225 and < 675 => 450
        };
    }));
    private void SelectResultModel()
    {
        QuizResult = 0;
        foreach (var quiz in Quizzes.Question)
        {
            QuizResult += quiz.QuestionCount;
        }

        QuizResult = QuizResult switch
        {
            0 => 0,
            450 => 1,
            900 => 2,
            1350 => 3,
            1800 => 4,
            2250 => 5,
            2700 => 6,
            3150 => 7,
            3600 => 8,
            4050 => 9,
            4500 => 10,
            _ => QuizResult
        };

        foreach (var resultModel in ResultsModel)
        {
            resultModel.PointsCount = QuizResult;
        }

        ResultModel = QuizResult switch
        {
            < 3 => ResultsModel[0],
            <= 7 and >= 3 => ResultsModel[1],
            > 7 => ResultsModel[2]
        };
    }

    private void GetResultsDataPage()
    {

        var path = "Result/ResultData.Json";
        ResultsModel = _jsonHelper.ReadJsonFromFile(path, ResultsModel);
    }
}