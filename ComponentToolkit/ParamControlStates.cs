using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentToolkit
{
    public enum String_Control
    {

    }

    public enum Boolean_Control
    {

    }

    public enum Integer_Control
    {

    }

    public enum Number_Control
    {

    }

    public enum Colour_Control
    {

    }

    public enum Material_Control
    {

    }

    public enum Domain_Control
    {
        T0_T1,
        General,
    }

    public enum Point_Control
    {
        XYZ,
        General,
    }

    public enum Vector_Control
    {
        XYZ,
        General,
    }

    public enum Complex_Control
    {
        Real_Imaginary,
        General,
    }

    public enum Domain2D_Control
    {
        U_V,
        U0_U1_V0_V1,
        ReadOnly,
    }

    public enum Line_Control
    {
        From_To,
        Start_Direction,
        SDL,
        ReadOnly,
    }

    public enum Plane_Control
    {
        OZ,
        OXY,
        ReadOnly,
    }

    public enum Circle_Control
    {
        CNR,
        Plane_Radius,
        ReadOnly,
    }

    public enum General_Control
    {
        General,
        ReadOnly,
    }
}
