using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using Rhino.Input.Custom;
using Rhino.UI;
using Rhino.UI.Gumball;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PersistentDataEditor;

internal class GumballMouse<T>(GH_PersistentParam<T> owner) : MouseCallback, IGumball where T : class, IGH_GeometricGoo
{
    public bool IsMouseUp { get; private set; }

    private T[] _geometries;
    private GumballDisplayConduit[] _conduits;
    private GumballObject[] _gumballs;

    private bool IsOnlyOneGumball => _conduits?.Length != _geometries?.Length;

    private int _index;

    private static GumballAppearanceSettings Settings => new()
    {
        MenuEnabled = false,
        RotateXEnabled = GumballData.GeoParamGumballRotate,
        RotateYEnabled = GumballData.GeoParamGumballRotate,
        RotateZEnabled = GumballData.GeoParamGumballRotate,
        ScaleXEnabled = GumballData.GeoParamGumballScale,
        ScaleYEnabled = GumballData.GeoParamGumballScale,
        ScaleZEnabled = GumballData.GeoParamGumballScale,
        TranslateXYEnabled = true,
        TranslateYZEnabled = true,
        TranslateZXEnabled = true,
        RelocateEnabled = false,
        Radius = GumballData.ParamGumballRadius
    };

    #region Show and Off
    public void Dispose()
    {
        if (_conduits != null)
        {
            for (int i = 0; i < _conduits.Length; i++)
            {
                _conduits[i].Enabled = false;
                _conduits[i].Dispose();
                _gumballs[i].Dispose();
            }
        }
        _conduits = [];
        _gumballs = [];
        _geometries = [];
        Enabled = false;

        RhinoDoc.ActiveDoc?.Views?.Redraw();
    }

    public void ShowAllGumballs()
    {
        Dispose();

        if (!GumballData.UseGeoParamGumball) return;
        if (owner == null || owner.OnPingDocument() == null) return;
        if (owner.Locked || !owner.Attributes.Selected) return;
        if (owner is IGH_PreviewObject previewObject && previewObject.Hidden) return;
        if (owner.Attributes.GetTopLevel.DocObject is IGH_PreviewObject ghPreviewObject && ghPreviewObject.Hidden) return;

        //Get PersistentData.
        _geometries = owner.PersistentData.NonNulls.Where((goo) => !goo.IsReferencedGeometry).ToArray();

        if (_geometries.Length > GumballData.GumballMaxShowCount)
        {
            BoundingBox box = BoundingBox.Empty;
            foreach (var geom in _geometries)
            {
                box.Union(geom.Boundingbox);
            }

            GumballObject gumballObject = new GumballObject();
            gumballObject.SetFromBoundingBox(box);

            GumballDisplayConduit gumballDisplayConduit = new (Rhino.DocObjects.ActiveSpace.None);
            gumballDisplayConduit.SetBaseGumball(gumballObject, Settings);
            gumballDisplayConduit.Enabled = true;
            _gumballs = [gumballObject];
            _conduits = [gumballDisplayConduit];
        }
        else
        {
            _gumballs = new GumballObject[_geometries.Length];
            _conduits = new GumballDisplayConduit[_geometries.Length];

            for (int i = 0; i < _geometries.Length; i++)
            {
                IGH_GeometricGoo geo = _geometries[i];
                GumballObject gumballObject = new GumballObject();
                gumballObject.SetFromBoundingBox(geo.Boundingbox);

                GumballDisplayConduit gumballDisplayConduit = new(Rhino.DocObjects.ActiveSpace.None);
                gumballDisplayConduit.SetBaseGumball(gumballObject, Settings);
                gumballDisplayConduit.Enabled = true;
                _gumballs[i] = gumballObject;
                _conduits[i] = gumballDisplayConduit;
            }
        }

        RhinoDoc.ActiveDoc?.Views?.Redraw();
        Enabled = true;
    }

    private void UpdateGumball(int index)
    {
        if (!_conduits[index].InRelocate)
        {
            Transform trans = _conduits[index].TotalTransform;
            _conduits[index].PreTransform = trans;
        }

        GumballFrame gbFrame = _conduits[index].Gumball.Frame;
        GumballFrame baseFrame = _gumballs[index].Frame;

        baseFrame.Plane = gbFrame.Plane;
        baseFrame.ScaleGripDistance = gbFrame.ScaleGripDistance;
        _gumballs[index].Frame = baseFrame;
        _conduits[index].SetBaseGumball(_gumballs[index], Settings);
        _conduits[index].Enabled = true;

    }

    private void UpdateGumball(int index, Transform xform)
    {
        GumballFrame gbFrame = _conduits[index].Gumball.Frame;
        GumballFrame baseFrame = _gumballs[index].Frame;
        Plane pl = gbFrame.Plane;
        pl.Transform(xform);

        baseFrame.Plane = pl;
        baseFrame.ScaleGripDistance = gbFrame.ScaleGripDistance;
        _gumballs[index].Frame = baseFrame;
        _conduits[index].SetBaseGumball(_gumballs[index], Settings);
        _conduits[index].Enabled = true;

    }

    #endregion

    #region Respond
    protected override void OnMouseDown(MouseCallbackEventArgs e)
    {
        _index = -1;
        if (_conduits.Length == 0 || e.MouseButton != MouseButton.Left)
        {
            return;
        }
        e.View.ActiveViewport.GetFrustumLine(e.ViewportPoint.X, e.ViewportPoint.Y, out Line line);

        PickContext pickContext = new()
        {
            View = e.View,
            PickStyle = PickStyle.PointPick,
            PickLine = line,
        };
        pickContext.SetPickTransform(e.View.ActiveViewport.GetPickTransform(e.ViewportPoint));
        pickContext.UpdateClippingPlanes();

        for (int i = 0; i < _conduits.Length; i++)
        {
            GumballDisplayConduit conduit = _conduits[i];
            if (conduit.PickGumball(pickContext, null))
            {
                _index = i;

                if (IsOnlyOneGumball)
                {
                    List<IGH_GeometricGoo> goosRelay = [];
                    foreach (var item in _geometries)
                    {
                        if (item is not IGH_PreviewData) continue;

                        IGH_GeometricGoo addObj = item.DuplicateGeometry();
                        if (addObj != null) goosRelay.Add(addObj);
                    }
                    MoveShowConduit.MoveObjects = [.. goosRelay];
                }
                else
                {
                    if (_geometries[i] is IGH_PreviewData)
                    {
                        IGH_GeometricGoo addObj = _geometries[i].DuplicateGeometry();
                        MoveShowConduit.MoveObjects = addObj != null ? [addObj] : [];
                    }
                }

                e.Cancel = true;
                return;
            }
        }
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseCallbackEventArgs e)
    {
        if (_index < 0 || _conduits.Length == 0 || Control.MouseButtons != MouseButtons.Left || _index >= _conduits.Length)
        {
            return;
        }

        GumballDisplayConduit gumballDisplayConduit = _conduits[_index];
        if (gumballDisplayConduit.PickResult.Mode == GumballMode.None)
        {
            return;
        }
        gumballDisplayConduit.CheckShiftAndControlKeys();
        if (!e.View.MainViewport.GetFrustumLine(e.ViewportPoint.X, e.ViewportPoint.Y, out Line worldLine))
        {
            worldLine = Line.Unset;
        }
        Plane plane = e.View.MainViewport.GetConstructionPlane().Plane;
        Intersection.LinePlane(worldLine, plane, out var lineParameter);
        Point3d dragPoint = worldLine.PointAt(lineParameter);

        if (gumballDisplayConduit.UpdateGumball(dragPoint, worldLine))
        {
            MoveShowConduit.Trans = gumballDisplayConduit.GumballTransform;
            RhinoDoc.ActiveDoc.Views.Redraw();
            e.Cancel = true;
        }
    }

    public static string ShowDialog(string caption)
    {
        Form prompt = new Form()
        {
            Width = 225,
            Height = 100,
            FormBorderStyle = FormBorderStyle.FixedToolWindow,
            Text = caption,
            StartPosition = FormStartPosition.CenterScreen,
            TopMost = true,
        };
        TextBox textBox = new TextBox() { Width = 200, Top = 5, Left = 5 };
        Button confirmation = new Button() { Text = "Ok", Left = 5, Top = 30, Width = 100, DialogResult = DialogResult.OK };
        Button cancel = new Button() { Text = "Cancel", Left = 105, Top = 30, Width = 100, DialogResult = DialogResult.Cancel };
        confirmation.Click += (sender, e) => { prompt.Close(); };
        cancel.Click += (sender, e) => { prompt.Close(); };
        prompt.Controls.Add(textBox);
        prompt.Controls.Add(confirmation);
        prompt.Controls.Add(cancel);
        prompt.AcceptButton = confirmation;
        prompt.CancelButton = cancel;

        return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
    }

    protected override void OnMouseUp(MouseCallbackEventArgs e)
    {
        if (_index < 0 || e.MouseButton != MouseButton.Left)
        {
            return;
        }

        if (_conduits.Length <= _index) return;
        Transform trans = _conduits[_index].GumballTransform;
        if (trans.IsIdentity)
        {
            if (_conduits[_index].PickResult.Mode == GumballMode.TranslateX || _conduits[_index].PickResult.Mode == GumballMode.TranslateY ||
            _conduits[_index].PickResult.Mode == GumballMode.TranslateZ || _conduits[_index].PickResult.Mode == GumballMode.RotateX ||
            _conduits[_index].PickResult.Mode == GumballMode.RotateY || _conduits[_index].PickResult.Mode == GumballMode.RotateZ ||
             _conduits[_index].PickResult.Mode == GumballMode.ScaleX || _conduits[_index].PickResult.Mode == GumballMode.ScaleY ||
              _conduits[_index].PickResult.Mode == GumballMode.ScaleZ)
            {
                if (GH_Convert.ToDouble(ShowDialog(_conduits[_index].PickResult.Mode.ToString()), out double number, GH_Conversion.Both))
                {
                    Plane plane = _conduits[_index].Gumball.Frame.Plane;

                    switch (_conduits[_index].PickResult.Mode)
                    {
                        case GumballMode.TranslateX:
                            trans = Transform.Translation(plane.XAxis * number);
                            break;
                        case GumballMode.TranslateY:
                            trans = Transform.Translation(plane.YAxis * number);
                            break;
                        case GumballMode.TranslateZ:
                            trans = Transform.Translation(plane.ZAxis * number);
                            break;
                        case GumballMode.RotateX:
                            number = RhinoMath.ToRadians(number);
                            trans = Transform.Rotation(number, plane.XAxis, plane.Origin);
                            break;
                        case GumballMode.RotateY:
                            number = RhinoMath.ToRadians(number);
                            trans = Transform.Rotation(number, plane.YAxis, plane.Origin);
                            break;
                        case GumballMode.RotateZ:
                            number = RhinoMath.ToRadians(number);
                            trans = Transform.Rotation(number, plane.ZAxis, plane.Origin);
                            break;
                        case GumballMode.ScaleX:
                            trans = Transform.Scale(plane, number, 1, 1);
                            break;
                        case GumballMode.ScaleY:
                            trans = Transform.Scale(plane, 1, number, 1);
                            break;
                        case GumballMode.ScaleZ:
                            trans = Transform.Scale(plane, 1, 1, number);
                            break;
                    }
                    UpdateGumball(_index, trans);
                }
            }

        }
        if (trans.IsIdentity) return;

        owner.RecordUndoEvent("Gumball drag");

        if (_geometries.Length == _conduits.Length)
        {
            _geometries[_index].Transform(trans);
        }
        else
        {
            foreach (var geom in _geometries)
            {
                geom.Transform(trans);
            }
        }

        IsMouseUp = true;
        owner.ExpireSolution(true);
        IsMouseUp = false;
        UpdateGumball(_index);
        RhinoDoc.ActiveDoc.Views.Redraw();
        _index = -1;
        e.Cancel = true;

        //Empty Preview.
        MoveShowConduit.MoveObjects = [];
        RhinoDoc.ActiveDoc.Views.Redraw();
    }

    #endregion
}

public class MoveShowConduit : DisplayConduit
{
    public static IGH_GeometricGoo[] MoveObjects { get; set; }
    public static Transform Trans { get; set; }
    protected override void DrawOverlay(DrawEventArgs e)
    {
        if (MoveObjects != null && MoveObjects.Length != 0)
        {
            int thickness = GumballData.ParamGumballWirePreviewThickness;
            Color wireColor = GumballData.ParamGumballPreviewWire;
            Color metarColor = GumballData.ParamGumballPreviewMesh;

            foreach (var item in MoveObjects)
            {
                IGH_GeometricGoo copy = item.DuplicateGeometry();
                copy.Transform(Trans);

                ((IGH_PreviewData)copy).DrawViewportWires(new GH_PreviewWireArgs(e.Viewport, e.Display, wireColor, thickness));
                ((IGH_PreviewData)copy).DrawViewportMeshes(new GH_PreviewMeshArgs(e.Viewport, e.Display,
                    new DisplayMaterial(metarColor), MeshingParameters.Default));
            }
        }
        base.DrawOverlay(e);
    }
}

public interface IGumball : IDisposable
{
    void ShowAllGumballs();
    bool IsMouseUp { get; }
}
