using System;

public class RollPitchYaw
{
    public double[] rpy = new double[3];

    public RollPitchYaw()
    {
        rpy[0] = 0;
        rpy[1] = 0;
        rpy[2] = 0;
    }

    public override string ToString()
    {
        string s = "";
        s += String.Format("{0,5:0.00000}", this.rpy[0]) + " ";
        s += String.Format("{0,5:0.00000}", this.rpy[1]) + " ";
        s += String.Format("{0,5:0.00000}", this.rpy[2]) + " ";
        s += "\r\n";
        return s;
    }
}

public class Quaternion
{
    public double w;
    public double x;
    public double y;
    public double z;

    public Quaternion()
    {
        this.w = 1;
        this.x = 0;
        this.y = 0;
        this.z = 0;
    }

    public Quaternion(double _w, double _x, double _y, double _z)
    {
        this.w = _w;
        this.x = _x;
        this.y = _y;
        this.z = _z;
    }

    public Quaternion Round(int dec)
    {
        Quaternion res = new Quaternion();
        res.w = Math.Round(this.w, dec);
        res.x = Math.Round(this.x, dec);
        res.y = Math.Round(this.y, dec);
        res.z = Math.Round(this.z, dec);
        return res;
    }

    public RollPitchYaw toRPY()
    {
        RollPitchYaw rpy = new RollPitchYaw();
        rpy.rpy[0] = Math.Atan2(2 * (this.w*this.x + this.y*this.z), 1 - 2 * (this.x*this.x + this.y*this.y));
        rpy.rpy[1] = Math.Asin(2 * (this.w * this.y - this.z * this.x));
        rpy.rpy[2] = Math.Atan2(2 * (this.w * this.z + this.x * this.y), 1 - 2 * (this.y * this.y + this.z * this.z));
        return rpy;
    }

    public Quaternion Conjugate()
    {
        Quaternion res = new Quaternion();
        res.w = this.w;
        res.x = -this.x;
        res.y = -this.y;
        res.z = -this.z;
        return res;
    }

    public double Norm(Quaternion q)
    {
        return Math.Pow(q.w, 2) + Math.Pow(q.x, 2) + Math.Pow(q.y, 2) + Math.Pow(q.z, 2);
    }

    public Quaternion Normalized()
    {
        return this / Math.Sqrt(Norm(this));
    }

    public Quaternion Inverse()
    {
        return this.Conjugate() / Norm(this);
    }

    public static Quaternion MultiplyScalar(double n, Quaternion q)
    {
        Quaternion res = new Quaternion();
        res.w = q.w * n;
        res.x = q.x * n;
        res.y = q.y * n;
        res.z = q.z * n;
        return res;
    }

    public static Quaternion Divide(double n, Quaternion q)
    {
        if (n == 0)
            return q;
        Quaternion res = new Quaternion();
        res.w = q.w / n;
        res.x = q.x / n;
        res.y = q.y / n;
        res.z = q.z / n;
        return res;
    }

    public static Quaternion Multiply(Quaternion q, Quaternion r)
    {
        Quaternion res = new Quaternion();
        res.w = q.w * r.w - q.x * r.x - q.y * r.y - q.z * r.z;
        res.x = q.x * r.w + q.w * r.x - q.z * r.y + q.y * r.z;
        res.y = q.y * r.w + q.z * r.x + q.w * r.y - q.x * r.z;
        res.z = q.z * r.w - q.y * r.x + q.x * r.y + q.w * r.z;
        return res;
    }

    public static Quaternion Add(Quaternion q, Quaternion r)
    {
        Quaternion res = new Quaternion();
        res.w = q.w + r.w;
        res.x = q.x + r.x;
        res.y = q.y + r.y;
        res.z = q.z + r.z;
        return res;
    }

    public override string ToString()
    {
        string s = "";
        s += String.Format("{0,5:0.00000}", this.w) + " ";
        s += String.Format("{0,5:0.00000}", this.x) + " ";
        s += String.Format("{0,5:0.00000}", this.y) + " ";
        s += String.Format("{0,5:0.00000}", this.z) + " ";
        s += "\r\n";
        return s;
    }

    public static Quaternion operator -(Quaternion q)
    { return Quaternion.MultiplyScalar(-1, q); }

    public static Quaternion operator *(Quaternion q, Quaternion r)
    { return Quaternion.Multiply(q, r); }

    public static Quaternion operator +(Quaternion q, Quaternion r)
    { return Quaternion.Add(q, r); }

    public static Quaternion operator -(Quaternion q, Quaternion r)
    { return Quaternion.Add(q, -r); }

    public static Quaternion operator /(Quaternion q, double n)
    { return Quaternion.Divide(n, q); }
}