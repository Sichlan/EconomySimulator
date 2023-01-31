using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Mars.Interfaces.Layers;
using Microsoft.Extensions.Logging;

namespace EconomySimulator.WPF.ViewModels.Map;

public abstract class DrawLayerFrameworkElement : FrameworkElement
{
    internal readonly double OuterXMin;
    internal readonly double OuterXMax;
    internal readonly double OuterYMin;
    internal readonly double OuterYMax;
    internal readonly Func<IVectorFeature, bool> VectorSkipPredicate;
    internal readonly Func<IVectorFeature, Brush> VectorFillColourPredicate;
    internal readonly Func<IVectorFeature, Pen> VectorPenPredicate;
    internal readonly VisualCollection VisualCollection;
    private readonly ILogger<DrawLayerFrameworkElement> _logger;

    public DrawLayerFrameworkElement(double outerXMin, double outerXMax, double outerYMin, double outerYMax, Func<IVectorFeature,bool> vectorSkipPredicate, Func<IVectorFeature, Brush> vectorFillColourPredicate, Func<IVectorFeature, Pen> vectorPenPredicate, ILogger<DrawLayerFrameworkElement> logger) 
        : this(vectorSkipPredicate, vectorFillColourPredicate, vectorPenPredicate, logger)
    {
        OuterXMin = outerXMin;
        OuterXMax = outerXMax;
        OuterYMin = outerYMin;
        OuterYMax = outerYMax;
    }
    
    public DrawLayerFrameworkElement(Func<IVectorFeature,bool> vectorSkipPredicate, Func<IVectorFeature, Brush> vectorFillColourPredicate, Func<IVectorFeature, Pen> vectorPenPredicate, ILogger<DrawLayerFrameworkElement> logger)
    {
        VectorSkipPredicate = vectorSkipPredicate;
        VectorFillColourPredicate = vectorFillColourPredicate;
        VectorPenPredicate = vectorPenPredicate;
        _logger = logger;
        VisualCollection = new VisualCollection(this);
        
        MouseUp += OnMouseUp;
        MouseWheel += OnMouseWheel;
        Loaded += OnLoaded;
    }

    protected async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await RenderGisLayerAsync();
    }

    protected void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        
    }

    protected void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is UIElement element)
        {
            var point = e.GetPosition(element);
            VisualTreeHelper.HitTest(this, null, HitTestResultCallback, new PointHitTestParameters(point));
        }
    }

    protected HitTestResultBehavior HitTestResultCallback(HitTestResult result)
    {
        // TODO: Add useful content in here
        if (result is PointHitTestResult {VisualHit: VectorFeatureDrawingVisual vectorFeatureDrawingVisual} pointHit)
        {
            _logger.LogDebug("vectorFeatureDrawingVisual: {@VectorFeatureDrawingVisual}", vectorFeatureDrawingVisual);
            
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