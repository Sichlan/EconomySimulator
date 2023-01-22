using System;
using System.Windows;
using System.Windows.Media;

namespace EconomySimulator.CustomControls.Abstract;

public abstract class AbstractVisualCollectionRenderer : FrameworkElement
{
    protected VisualCollection _visualCollection;

    protected override Visual GetVisualChild(int index)
    {
        if (index < 0 || index >= _visualCollection.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        
        return _visualCollection[index];
    }

    protected override int VisualChildrenCount
    {
        get
        {
            return _visualCollection.Count;
        }
    }
}