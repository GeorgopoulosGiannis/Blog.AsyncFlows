using MassTransit;

namespace Blog.SagaVisualizer;

public record OrderSubmitted(Guid OrderId, DateTime Timestamp);

public record PaymentCompleted(Guid OrderId, DateTime Timestamp);

public record OrderShipped(Guid OrderId, DateTime Timestamp);

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public DateTime OrderSubmittedAt { get; set; }
    public DateTime? OrderPayedAt { get; set; }
    public DateTime? OrderShippedAt { get; set; }
}

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State Submitted { get; private set; }
    public State Paid { get; private set; }
    public State Completed { get; private set; }

    public Event<OrderSubmitted> OrderSubmittedEvent { get; private set; }
    public Event<PaymentCompleted> PaymentCompletedEvent { get; private set; }
    public Event<OrderShipped> OrderShippedEvent { get; private set; }

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmittedEvent, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentCompletedEvent, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => OrderShippedEvent, x => x.CorrelateById(ctx => ctx.Message.OrderId));

        Initially(
            When(OrderSubmittedEvent)
                .Then(context => { context.Saga.OrderSubmittedAt = context.Message.Timestamp; })
                .TransitionTo(Submitted));

        During(Submitted,
            When(PaymentCompletedEvent)
                .Then(context => { context.Saga.OrderPayedAt = context.Message.Timestamp; })
                .TransitionTo(Paid));

        During(Paid,
            When(OrderShippedEvent)
                .Then(context => { context.Saga.OrderShippedAt = context.Message.Timestamp; })
                .TransitionTo(Completed));
    }
}