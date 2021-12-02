using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    public class CreateObjectItem
    {
        public static MethodInfo functions = typeof(GH_Canvas).GetRuntimeMethods().Where(m => m.Name.Contains("InstantiateNewObject") && !m.IsPublic).First();

        public ushort Index { get; }
        public Guid ObjectGuid { get; }
        public string InitString { get; set; }
        public Bitmap Icon { get; } = null;
        public string ShowName { get; } = "";
        public string Name { get; } = "";
        public bool IsInput { get; }
        public CreateObjectItem(Guid guid, ushort index, string init, bool isInput)
        {
            ObjectGuid = guid;
            InitString = init;
            Index = index;
            IsInput = isInput;

            IGH_ObjectProxy proxy = Grasshopper.Instances.ComponentServer.EmitObjectProxy(guid);
            if(proxy == null) return;

            Icon = proxy.Icon;
            Name = proxy.Desc.Name;
            ShowName = $"{proxy.Desc.Name}[{index}]";
        }

        public CreateObjectItem(CreateObjectItemSave save, bool isInput):this(save.ObjectGuid, save.Index, save.initString, isInput)
        {

        }

        public IGH_DocumentObject CreateObject(IGH_Param param, Action<IGH_DocumentObject> action = null)
        {
            float move = 100;

            IGH_DocumentObject obj = Grasshopper.Instances.ComponentServer.EmitObject(ObjectGuid);
            if (obj == null) return null;

            if(action != null) action(obj);

            RectangleF outBound = param.Attributes.GetTopLevel.Bounds;
            RectangleF thisBound = param.Attributes.Bounds;

            PointF objCenter = new PointF(outBound.Left + (IsInput ? - move : (move + outBound.Width)),
                thisBound.Top + thisBound.Height / 2);

            if (obj is IGH_Component)
            {
                IGH_Component com = obj as IGH_Component;
                AddAObjectToCanvas(obj, objCenter, InitString);

                if (IsInput)
                {
                    param.AddSource(com.Params.Output[Index]);
                }
                else
                {
                    com.Params.Input[Index].AddSource(param);
                }

                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false);
            }
            else if(obj is IGH_Param)
            {
                IGH_Param par = obj as IGH_Param;
                AddAObjectToCanvas(obj, objCenter, InitString);

                if (IsInput)
                {
                    param.AddSource(par);
                }
                else
                {
                    par.AddSource(param);
                }

                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false);
            }

            return obj;
        }

        public static void AddAObjectToCanvas(IGH_DocumentObject obj, PointF pivot, string init, bool update = false)
        {
            functions.Invoke(Grasshopper.Instances.ActiveCanvas, new object[] { obj, init, pivot, update });
        }
    }

    public struct CreateObjectItemSave
    {
        public Guid ObjectGuid { get; set; }
        public string initString { get; set; }
        public ushort Index { get; set; }

        public CreateObjectItemSave(Guid guid, string init, ushort index)
        {
            ObjectGuid = guid;
            initString = init;
            Index = index;
        }

        public CreateObjectItemSave(CreateObjectItem item)
        {
            ObjectGuid = item.ObjectGuid;
            initString = item.InitString;
            Index = item.Index;
        }
    }
}
