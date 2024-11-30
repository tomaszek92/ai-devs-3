namespace S04E05;

public sealed class PageNumberComparer : IComparer<int>
{
    public int Compare(int x, int y) => x - y;
}