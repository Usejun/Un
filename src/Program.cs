using Un;
using Un.Object.Collections;
using Un.Object.Primitive;

Global.Init();
Runner runner = Runner.Load("main", Global.GetScope(), $"main.un");
runner.Run();