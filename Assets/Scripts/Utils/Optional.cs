
using System;

public class Optional<T>
{
    private readonly T value;

    private Optional(T value)
    {
        this.value = value;
    }

    public static Optional<T> Of(T value) => new Optional<T>(value);
    public static Optional<T> Empty() => new Optional<T>(default);

    public bool IfPresent(Action<T> action)
    {
        if (this.value == null) return false;
        action.Invoke(this.value);
        return true;
    }

    public void IfPresentOrElse(Action<T> action, Action onElse)
    {
        if (this.value == null)
        {
            onElse.Invoke();
            return;
        }

        action(this.value);
    }

    public T GetValueOrDefault() => value;
}