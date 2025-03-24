using MassTransit.SagaStateMachine;
using MassTransit.Visualizer;

namespace Blog.SagaVisualizer;

public static class SagaVisualizer
{
    public static string Visualize()
    {
        var machine = new OrderStateMachine();
        var graph = machine.GetGraph();
        var generator = new StateMachineMermaidGenerator(graph);
        string mermaid = generator.CreateMermaidFile();
        return mermaid;
    }
}