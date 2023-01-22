using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Mars.Components.Layers;
using Mars.Interfaces.Layers;

namespace EconomySimulator.WPF.ViewModels.Map;

public abstract class DrawVectorLayerFrameworkElement : FrameworkElement { }

public class DrawVectorLayerFrameworkElement<T> : DrawVectorLayerFrameworkElement where T : VectorLayer
{
    private readonly Func<T, Brush> _colourVectorPredicate;
    private readonly Func<IVectorFeature, bool> _skipVectorPredicate;
    private readonly VisualCollection _visualCollection;
    private T? _gisVectorLayer;

    public DrawVectorLayerFrameworkElement(T gisVectorLayer, Func<T, Brush> colourVectorPredicate, Func<IVectorFeature, bool> skipVectorPredicate) : this()
    {
        _colourVectorPredicate = colourVectorPredicate;
        _skipVectorPredicate = skipVectorPredicate;
        Loaded += async (_, _) =>
        {
            await SetGisVectorLayerAsync(gisVectorLayer);
        };
    }
    
    public DrawVectorLayerFrameworkElement()
    {
        _visualCollection = new VisualCollection(this);

        _colourVectorPredicate ??= _ => Brushes.White;
        _skipVectorPredicate ??= _ => false;
        
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

    private async Task SetGisVectorLayerAsync(T gisVectorLayer)
    {
        _gisVectorLayer = gisVectorLayer;
        await RenderGisVectorLayerAsync();
    }

    private async Task RenderGisVectorLayerAsync()
    {
        if (_gisVectorLayer == null)
            return;
        
        await Task.Yield();

        double xMin = _gisVectorLayer.Features.Min(x => x.VectorStructured.Geometry.Coordinates.Min(y => y.X)),
            xMax = _gisVectorLayer.Features.Max(x => x.VectorStructured.Geometry.Coordinates.Max(y => y.X)),
            yMin = _gisVectorLayer.Features.Min(x => x.VectorStructured.Geometry.Coordinates.Min(y => y.Y)),
            yMax = _gisVectorLayer.Features.Max(x => x.VectorStructured.Geometry.Coordinates.Max(y => y.Y));

        double scaleFactorX = ActualWidth / (xMax - xMin),
            scaleFactorY = ActualHeight / (yMax - yMin),
            scaleFactor = Math.Min(scaleFactorX, scaleFactorY);
        
        var xDifference = 0 - xMin;
        var yDifference = 0 - yMin;
        
        foreach (var vectorFeature in _gisVectorLayer.Features)
        {
            if(_skipVectorPredicate.Invoke(vectorFeature))
                continue;
                
            var vectorFeatureDrawingVisual = new VectorFeatureDrawingVisual(vectorFeature);

            using (var drawingContext = vectorFeatureDrawingVisual.RenderOpen())
            {
                var streamGeometry = new StreamGeometry();

                using (var geometryContext = streamGeometry.Open())
                {
                    var firstCoordinate = vectorFeature.VectorStructured.Geometry.Coordinates.First(); 
                    geometryContext.BeginFigure(new Point((firstCoordinate.X + xDifference) * scaleFactor, ActualHeight-((firstCoordinate.Y + yDifference) * scaleFactor)), true, true);

                    var points = new PointCollection();
                    foreach (var geometryCoordinate in vectorFeature.VectorStructured.Geometry.Coordinates.Skip(1))
                    {
                        points.Add(new Point((geometryCoordinate.X + xDifference) * scaleFactor, ActualHeight-((geometryCoordinate.Y + yDifference) * scaleFactor)));
                    }
                    geometryContext.PolyLineTo(points, true, true);
                }

                drawingContext.DrawGeometry(_colourVectorPredicate.Invoke(_gisVectorLayer), null, streamGeometry);
            }

            _visualCollection.Add(vectorFeatureDrawingVisual);
        }
        
        UpdateLayout();
    }

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