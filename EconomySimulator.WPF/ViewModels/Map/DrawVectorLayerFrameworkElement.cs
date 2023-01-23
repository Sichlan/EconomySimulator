using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using EconomySimulator.BusinessLogic.Models.Simulation.Layers;
using Mars.Components.Layers;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using Point = System.Windows.Point;

namespace EconomySimulator.WPF.ViewModels.Map;

public class DrawVectorLayerFrameworkElement<T> : DrawLayerFrameworkElement where T : VectorLayer
{
    private readonly Func<T, Brush> _colourVectorPredicate;
    private T? _gisLayer;

    public DrawVectorLayerFrameworkElement(T gisVectorLayer, double outerXMin, double outerXMax, double outerYMin, double outerYMax, Func<T, Brush> colourVectorPredicate, Func<IVectorFeature, bool> skipVectorPredicate) 
        : base(outerXMin, outerXMax, outerYMin, outerYMax, skipVectorPredicate)
    {
        _colourVectorPredicate = colourVectorPredicate;
        Loaded += async (_, _) =>
        {
            await SetGisVectorLayerAsync(gisVectorLayer);
        };
    }

    private async Task SetGisVectorLayerAsync(T gisVectorLayer)
    {
        _gisLayer = gisVectorLayer;
        await RenderGisLayerAsync();
    }

    protected override async Task RenderGisLayerAsync()
    {
        if (_gisLayer == null)
            return;

        await Task.Yield();
        
        // the outer min / max fields are already set as base corner points for the canvas
        // calculate inner min / max:

        double innerXMin = _gisLayer.Features.Min(x => x.VectorStructured.Geometry.Coordinates.Min(y => y.X)),
            innerXMax = _gisLayer.Features.Max(x => x.VectorStructured.Geometry.Coordinates.Max(y => y.X)),
            innerYMin = _gisLayer.Features.Min(x => x.VectorStructured.Geometry.Coordinates.Min(y => y.Y)),
            innerYMax = _gisLayer.Features.Max(x => x.VectorStructured.Geometry.Coordinates.Max(y => y.Y));
        
        // calculate scale factor to properly align the canvas
        double scaleFactorX = ActualWidth / (OuterXMax - OuterXMin),
            scaleFactorY = ActualHeight / (OuterYMax - OuterYMin),
            scaleFactor = Math.Min(scaleFactorX, scaleFactorY);
        
        // calculate new origin point because gis files reach into negative values, while canvas only allows positives
        var xDifference = 0 - OuterXMin;
        var yDifference = 0 - OuterYMin;
        
        // iterate through all vector features in the current layer
        foreach (var vectorFeature in _gisLayer.Features)
        {
            // Invoke the skip predicate to see if the vector should be drawn or not.
            // A return value of true means the vector should be skipped
            if(SkipVectorPredicate.Invoke(vectorFeature))
                continue;
                
            // Instantiate a new VectorFeatureDrawingVisual that can provide a drawing context
            var vectorFeatureDrawingVisual = new VectorFeatureDrawingVisual(vectorFeature);
            var streamGeometry = new StreamGeometry();
            
            // Activate the drawing and geometry contexts to draw the vector
            using (var drawingContext = vectorFeatureDrawingVisual.RenderOpen())
            using (var geometryContext = streamGeometry.Open())
            {
                // Find the first coordinate to start the figure at:
                var firstCoordinate = vectorFeature.VectorStructured.Geometry.Coordinates.First();
                
                geometryContext.BeginFigure(CalculatePoint(firstCoordinate, xDifference, yDifference, scaleFactor), true, true);

                var points = new PointCollection();
                foreach (var geometryCoordinate in vectorFeature.VectorStructured.Geometry.Coordinates.Skip(1))
                {
                    points.Add(CalculatePoint(geometryCoordinate, xDifference, yDifference, scaleFactor));
                }
                geometryContext.PolyLineTo(points, true, true);

                drawingContext.DrawGeometry(_colourVectorPredicate.Invoke(_gisLayer), null, streamGeometry);
            }

            VisualCollection.Add(vectorFeatureDrawingVisual);
        }
        
        
                
        var vectorFeatureDrawingVisual1 = new VectorFeatureDrawingVisual(null);

        using (var drawingContext = vectorFeatureDrawingVisual1.RenderOpen())
        {
            var streamGeometry = new StreamGeometry();

            using (var geometryContext = streamGeometry.Open())
            {
                // Top Left
                geometryContext.BeginFigure(CalculatePoint(new Coordinate(innerXMin, innerYMin), xDifference, yDifference, scaleFactor),
                    true,
                    true);

                var points = new PointCollection
                {
                    // Bottom Left
                    CalculatePoint(new Coordinate(innerXMin, innerYMax), xDifference, yDifference, scaleFactor),
                    
                    // Bottom Right
                    CalculatePoint(new Coordinate(innerXMax, innerYMax), xDifference, yDifference, scaleFactor),
                    
                    // Top Right
                    CalculatePoint(new Coordinate(innerXMax, innerYMin), xDifference, yDifference, scaleFactor),
                };

                geometryContext.PolyLineTo(points, true, true);
            }

            drawingContext.DrawGeometry(Brushes.Transparent, new Pen(_gisLayer is GisCellsLayer ? Brushes.Red : Brushes.DeepPink, 5), streamGeometry);
        }

        VisualCollection.Add(vectorFeatureDrawingVisual1);
        
        UpdateLayout();
    }

    private Point CalculatePoint(Coordinate coordinate, double xDifference, double yDifference, double scaleFactor)
    {
        double
            //X: 
            x = (coordinate.X + xDifference) * scaleFactor,
            y = ActualHeight - (coordinate.Y + yDifference) * scaleFactor;
        
        return new Point(x, y);
    }
}