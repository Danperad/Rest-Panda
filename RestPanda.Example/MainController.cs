using RestPanda.Core.Attributes;

namespace RestPanda.Example;

[Controller]
public class MainController
{
    [GetHandler]
    public AnswerModel GetAnswer([Headers] Dictionary<string, string> Headers, [Param("age")] int age,
        [Body] object body)
    {
        return new AnswerModel();
    }
}