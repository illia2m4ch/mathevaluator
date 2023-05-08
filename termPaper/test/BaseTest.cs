namespace termPaper.test; 

public class BaseTest {

    private readonly Dictionary<string, Action> _markedTests = new();
    private readonly Dictionary<string, Action> _tests = new();

    public void Run() {
        _runAll(_markedTests.Any() ? _markedTests : _tests);
    }

    protected void Test(string name, Action action) {
        if (name.StartsWith("!")) {
            _markedTests[name[1..]] = action;
        }
        else {
            _tests[name] = action;
        }
    }
    
    protected static void AssertEquals(double expect, double actual) {
        if (!expect.Equals(actual)) throw new Exception($"actual {actual} not equals expected {expect}");
    }

    private void _runAll(Dictionary<string, Action> tests) {
        foreach (var (name, action) in tests) {
            _runTest(name, action);
        }
    }

    private void _runTest(string name, Action action) {
        try {
            action();
            Console.WriteLine($"[SUCCESS] Test {name}");
        }
        catch (Exception e) {
            Console.WriteLine($"[ERROR] Test {name}");
            Console.WriteLine($"{e.Message}");
        }
    }
    
}