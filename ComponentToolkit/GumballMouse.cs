using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using Rhino.Input.Custom;
using Rhino.UI;
using Rhino.UI.Gumball;

namespace ComponentToolkit
{
    internal class GumballMouse<T> : MouseCallback, IGumball where T : class, IGH_GeometricGoo
    {
		public bool IsMouseUp { get; private set; } = false;

		private GH_PersistentParam<T> _owner;

		private T[] _geometries;
		private GumballDisplayConduit[] _conduits;
		private GumballObject[] _gumballs;

		private int _index;
        public GumballMouse(GH_PersistentParam<T> owner)
        {
			_owner = owner;
		}

        private GumballAppearanceSettings settings
        {
            get
            {
				GumballAppearanceSettings gumballAppearanceSettings = new GumballAppearanceSettings();
				gumballAppearanceSettings.MenuEnabled = false;
				gumballAppearanceSettings.RotateXEnabled = Datas.GeoParamGumballRotate;
				gumballAppearanceSettings.RotateYEnabled = Datas.GeoParamGumballRotate;
				gumballAppearanceSettings.RotateZEnabled = Datas.GeoParamGumballRotate;
				gumballAppearanceSettings.ScaleXEnabled = Datas.GeoParamGumballScale;
				gumballAppearanceSettings.ScaleYEnabled = Datas.GeoParamGumballScale;
				gumballAppearanceSettings.ScaleZEnabled = Datas.GeoParamGumballScale;
				gumballAppearanceSettings.TranslateXYEnabled = true;
				gumballAppearanceSettings.TranslateYZEnabled = true;
				gumballAppearanceSettings.TranslateZXEnabled = true;
				gumballAppearanceSettings.RelocateEnabled = false;
				gumballAppearanceSettings.Radius = Datas.ParamGumballRadius;
				return gumballAppearanceSettings;
			}
        }

		public void CreateGumballs()
        {

        }

		public void Dispose()
		{
			if(_conduits != null)
			{
				for (int i = 0; i < _conduits.Length; i++)
				{
					_conduits[i].Enabled = false;
					_conduits[i].Dispose();
					_gumballs[i].Dispose();
				}
			}
			_conduits = new GumballDisplayConduit[0];
			_gumballs = new GumballObject[0];
			_geometries = new T[0];
			this.Enabled = false;

			Rhino.RhinoDoc.ActiveDoc?.Views?.Redraw();
		}

		public void ShowAllGumballs()
		{
			Dispose();

			if (!Datas.UseGeoParamGumball) return;
			if (_owner == null || _owner.OnPingDocument() == null) return;
			if (_owner.Locked || !_owner.Attributes.Selected) return;
			if (_owner is IGH_PreviewObject && ((IGH_PreviewObject)_owner).Hidden) return;
			if (_owner.Attributes.GetTopLevel.DocObject is IGH_PreviewObject && ((IGH_PreviewObject)_owner.Attributes.GetTopLevel.DocObject).Hidden) return;


			//Get PersistentData.
			_geometries = _owner.PersistentData.NonNulls.Where((goo) => !goo.IsReferencedGeometry).ToArray();

			if(_geometries.Length > Datas.GumballMaxShowCount)
            {
				BoundingBox box = BoundingBox.Empty;
				foreach (var geom in _geometries)
                {
					box.Union(geom.Boundingbox);
                }

				GumballObject gumballObject = new GumballObject();
				gumballObject.SetFromBoundingBox(box);

				GumballDisplayConduit gumballDisplayConduit = new GumballDisplayConduit();
				gumballDisplayConduit.SetBaseGumball(gumballObject, settings);
				gumballDisplayConduit.Enabled = true;
				_gumballs = new GumballObject[] { gumballObject };
				_conduits = new GumballDisplayConduit[] { gumballDisplayConduit };
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

					GumballDisplayConduit gumballDisplayConduit = new GumballDisplayConduit();
					gumballDisplayConduit.SetBaseGumball(gumballObject, settings);
					gumballDisplayConduit.Enabled = true;
					_gumballs[i] = gumballObject;
					_conduits[i] = gumballDisplayConduit;
				}
			}


			Rhino.RhinoDoc.ActiveDoc?.Views?.Redraw();
			this.Enabled = true;
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
			_conduits[index].SetBaseGumball(_gumballs[index], settings);
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
			_conduits[index].SetBaseGumball(_gumballs[index], settings);
			_conduits[index].Enabled = true;

		}

		protected override void OnMouseDown(MouseCallbackEventArgs e)
		{
			_index = -1;
			if (_conduits.Length == 0 || e.MouseButton != MouseButton.Left)
			{
				return;
			}

			PickContext pickContext = new PickContext();
			pickContext.View = e.View;
			pickContext.PickStyle = PickStyle.PointPick;
			pickContext.SetPickTransform(e.View.ActiveViewport.GetPickTransform(e.ViewportPoint));
			e.View.ActiveViewport.GetFrustumLine(e.ViewportPoint.X, e.ViewportPoint.Y, out Line line);
			pickContext.PickLine = line;
			pickContext.UpdateClippingPlanes();

			for (int i = 0; i < _conduits.Length; i++)
            {
				GumballDisplayConduit conduit = _conduits[i];
				if (conduit.PickGumball(pickContext, null))
				{
					_index = i;
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

            //Grid Snap

            //if (Rhino.ApplicationSettings.ModelAidSettings.GridSnap)
            //{
            //    if (_conduits[_index].PickResult.Mode == GumballMode.TranslateFree || _conduits[_index].PickResult.Mode == GumballMode.TranslateX ||
            //        _conduits[_index].PickResult.Mode == GumballMode.TranslateY || _conduits[_index].PickResult.Mode == GumballMode.TranslateZ ||
            //        _conduits[_index].PickResult.Mode == GumballMode.TranslateXY || _conduits[_index].PickResult.Mode == GumballMode.TranslateZX ||
            //        _conduits[_index].PickResult.Mode == GumballMode.TranslateYZ)
            //    {
            //        Point3d snap = new Point3d((int)dragPoint.X, (int)dragPoint.Y, (int)dragPoint.Z);
            //        worldLine.Transform(Transform.Translation(snap - dragPoint));
            //        dragPoint = snap;
            //    }
            //}

            if (gumballDisplayConduit.UpdateGumball(dragPoint, worldLine))
			{
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
				StartPosition = FormStartPosition.CenterScreen
			};
			TextBox textBox = new TextBox() { Width = 200, Top = 5, Left = 5 };
			Button confirmation = new Button() { Text = "Ok", Left = 5, Top = 30, Width = 100, DialogResult = DialogResult.OK };
			Button cancel = new Button() { Text = "cancel", Left = 105, Top = 30, Width = 100, DialogResult = DialogResult.Cancel };
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

			Transform trans = _conduits[_index].GumballTransform;
			if (trans == null) return;
            if (trans.IsIdentity)
            {
                if (_conduits[_index].PickResult.Mode == GumballMode.TranslateX || _conduits[_index].PickResult.Mode == GumballMode.TranslateY ||
					_conduits[_index].PickResult.Mode == GumballMode.TranslateZ || _conduits[_index].PickResult.Mode == GumballMode.RotateX ||
					_conduits[_index].PickResult.Mode == GumballMode.RotateY || _conduits[_index].PickResult.Mode == GumballMode.RotateZ ||
					 _conduits[_index].PickResult.Mode == GumballMode.ScaleX || _conduits[_index].PickResult.Mode == GumballMode.ScaleY ||
					  _conduits[_index].PickResult.Mode == GumballMode.ScaleZ)
					if(GH_Convert.ToDouble(ShowDialog(_conduits[_index].PickResult.Mode.ToString()), out double number, GH_Conversion.Both))
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
			if (trans.IsIdentity) return;

			_owner.RecordUndoEvent("Gumball drag");

			if(_geometries.Length == _conduits.Length)
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
            _owner.ExpireSolution(true);
            IsMouseUp = false;
            UpdateGumball(_index);
			RhinoDoc.ActiveDoc.Views.Redraw();
			_index = -1;
			e.Cancel = true;
		}
	}

	public interface IGumball : IDisposable
    {
		void ShowAllGumballs();
		bool IsMouseUp { get; }
	}
}
