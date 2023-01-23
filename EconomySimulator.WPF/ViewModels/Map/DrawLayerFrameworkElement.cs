using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Mars.Interfaces.Layers;

namespace EconomySimulator.WPF.ViewModels.Map;

public abstract class DrawLayerFrameworkElement : FrameworkElement
{
    internal readonly double OuterXMin;
    internal readonly double OuterXMax;
    internal readonly double OuterYMin;
    internal readonly double OuterYMax;
    internal readonly Func<IVectorFeature, bool> SkipVectorPredicate;
    internal readonly VisualCollection VisualCollection;
    
    public DrawLayerFrameworkElement(double outerXMin, double outerXMax, double outerYMin, double outerYMax, Func<IVectorFeature, bool> skipVectorPredicate) : this()
    {
        OuterXMin = outerXMin;
        OuterXMax = outerXMax;
        OuterYMin = outerYMin;
        OuterYMax = outerYMax;
        SkipVectorPredicate = skipVectorPredicate;
    }
    
    public DrawLayerFrameworkElement()
    {
        VisualCollection = new VisualCollection(this);

        SkipVectorPredicate ??= _ => false;
        
        MouseUp += OnMouseUp;
        MouseWheel += OnMouseWheel;
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is UIElement element)
        {
            var point = e.GetPosition(element);
            VisualTreeHelper.HitTest(this, null, HitTestResultCallback, new PointHitTestParameters(point));
        }
    }

    [Localizable(false)]
    private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
    {
        if (result is PointHitTestResult {VisualHit: VectorFeatureDrawingVisual vectorFeatureDrawingVisual} pointHit)
        {
            using (var visualContext = vectorFeatureDrawingVisual.RenderOpen())
            {
                var text = "lol";
                var formattedText = new FormattedText(text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("ComicSans"),
                    20,
                    Brushes.Black); 
                visualContext.DrawText(formattedText, pointHit.PointHit);
            }
        }

        return HitTestResultBehavior.Stop;
    }

    protected double GetBorderDistance(double outerMin, double outerMax, double innerMin, double innerMax)
    {
        var outerDistance = outerMax - outerMin;
        var innerDistance = innerMax - innerMin;

        return (outerDistance - innerDistance) / 2;
    }
    
    protected abstract Task RenderGisLayerAsync();

    protected override Visual GetVisualChild(int index)
    {
        if (index < 0 || index >= VisualCollection.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        
        return VisualCollection[index];
    }
    
    protected override int VisualChildrenCount
    {
        get
        {
            return VisualCollection.Count;
        }
    }
}