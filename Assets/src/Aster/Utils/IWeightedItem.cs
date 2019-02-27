namespace Aster.Utils {

public interface IWeightedItem<T>
{
    float Weight { get; }
    T Item { get; }
}

}
