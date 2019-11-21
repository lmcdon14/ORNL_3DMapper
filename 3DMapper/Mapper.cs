﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _3DMapper
{
    public class Node
    {
        public double X_pos { get; set; }
        public double Y_pos { get; set; }
        public double Z_pos { get; set; }
        public double B_x { get; set; }
        public double B_y { get; set; }
        public double B_z { get; set; }
        public double B_v { get; set; }
    }

    public class Plane
    {
        public double Z_pos { get; set; }
        public double B_x_avg { get; set; }
        public double B_y_avg { get; set; }
        public double B_z_avg { get; set; }
        public double G_x_avg { get; set; }
        public double G_y_avg { get; set; }
        public double G_z_avg { get; set; }
        public Node[,] Nodes { get; set; }
}

    public class Field
    {
        public Plane[] Planes { get; set; }
        public double[,,] B_v { get; set; }
        public double[,,] B_x { get; set; }
        public double[,,] B_y { get; set; }
        public double[,,] B_z { get; set; }
        public double[,,] Gradient { get; set; }
        public double[,,] Gradient_x { get; set; }
        public double[,,] Gradient_y { get; set; }
        public double[,,] Gradient_z { get; set; }
        public double B_x_avg { get; set; }
        public double B_y_avg { get; set; }
        public double B_z_avg { get; set; }
        public double G_x_avg { get; set; }
        public double G_y_avg { get; set; }
        public double G_z_avg { get; set; }
        public int X_len { get; set; }
        public int Y_len { get; set; }
        public int Z_len { get; set; }
        public double X_step { get; set; }
        public double Y_step { get; set; }
        public double Z_step { get; set; }
        public double X_start { get; set; }
        public double Y_start { get; set; }
        public double Z_start { get; set; }
        public Boolean Metric { get; set; }
        public Field(string filename)
        {
            double x, y, z;

            if (filename.Substring(5+filename.IndexOf("scan_"),2) == "in")
            {
                this.Metric = false;
            }
            else
            {
                this.Metric = true;
            }

            //Parse data file and initialize member variables
            using (StreamReader sr = new StreamReader(filename))
            {
                string data = sr.ReadLine();
                var line = sr.ReadLine().Split('\t');
                x = Double.Parse(line[0]);
                this.X_step = Double.Parse(line[1]);
                if (this.X_step != 0)
                {
                    this.X_len = Convert.ToInt32(x / this.X_step + 1);
                }
                else
                {
                    this.X_len = 1;
                }
                y = Double.Parse(line[2]);
                this.Y_step = Double.Parse(line[3]);
                if (this.Y_step != 0)
                {
                    this.Y_len = Convert.ToInt32(y / this.Y_step + 1);
                }
                else
                {
                    this.Y_len = 1;
                }
                z = Double.Parse(line[4]);
                this.Z_step = Double.Parse(line[5]);
                if (this.Z_step != 0)
                {
                    this.Z_len = Convert.ToInt32(z / this.Z_step + 1);
                }
                else
                {
                    this.Z_len = 1;
                }

                this.Planes = new Plane[this.Z_len];
                this.Gradient = new double[this.Z_len, this.Y_len, this.X_len];
                this.Gradient_x = new double[this.Z_len, this.Y_len, this.X_len];
                this.Gradient_y = new double[this.Z_len, this.Y_len, this.X_len];
                this.Gradient_z = new double[this.Z_len, this.Y_len, this.X_len];

                int x_ind, y_ind, z_ind;
                data = sr.ReadLine();
                data = sr.ReadLine();
                int check = 0;
                while ((data = sr.ReadLine()) != null)
                {
                    Node n = new Node();
                    line = data.Split();
                    n.X_pos = Double.Parse(line[0]);
                    n.Y_pos = Double.Parse(line[1]);
                    n.Z_pos = Double.Parse(line[2]);
                    n.B_x = Double.Parse(line[3]);
                    n.B_y = Double.Parse(line[4]);
                    n.B_z = Double.Parse(line[5]);
                    n.B_v = Double.Parse(line[6]);
                    if (check == 0)
                    {
                        this.X_start = n.X_pos;
                        this.Y_start = n.Y_pos;
                        this.Z_start = n.Z_pos;
                        for (int i = 0; i < this.Z_len; i++)
                        {
                            Plane p = new Plane();
                            p.Nodes = new Node[this.Y_len, this.X_len];
                            if (this.Z_len != 1)
                            {
                                p.Z_pos = this.Z_start + i * this.Z_step;
                            }
                            else
                            {
                                p.Z_pos = this.Z_start;
                            }
                            this.Planes[i] = p;
                        }
                        this.B_v = new double[this.Z_len, this.Y_len, this.X_len];
                        this.B_x = new double[this.Z_len, this.Y_len, this.X_len];
                        this.B_y = new double[this.Z_len, this.Y_len, this.X_len];
                        this.B_z = new double[this.Z_len, this.Y_len, this.X_len];
                    }
                    if (this.X_step != 0)
                    {
                        x_ind = Convert.ToInt32((n.X_pos-this.X_start) / this.X_step);
                    }
                    else
                    {
                        x_ind = 0;
                    }
                    if (this.Y_step != 0)
                    {
                        y_ind = Convert.ToInt32((n.Y_pos - this.Y_start) / this.Y_step);
                    }
                    else
                    {
                        y_ind = 0;
                    }
                    if (this.Z_step != 0)
                    {
                        z_ind = Convert.ToInt32((n.Z_pos - this.Z_start) / this.Z_step);
                    }
                    else
                    {
                        z_ind = 0;
                    }
                    this.Planes[z_ind].Nodes[y_ind, x_ind] = n;
                    this.B_x[z_ind, y_ind, x_ind] = n.B_x;
                    this.B_y[z_ind, y_ind, x_ind] = n.B_y;
                    this.B_z[z_ind, y_ind, x_ind] = n.B_z;
                    this.B_v[z_ind, y_ind, x_ind] = n.B_v;
                    check++;
                }
            }

            //Calculate gradient components and average values
            Node no;
            double uni_Bz = new double();
            List<double> Bzs = new List<double>();
            double[] xs = new double[2];
            double[] ys = new double[2];
            double[] zs = new double[2];
            int[] dirs = new int[3];
            double Avg_x = new double();
            double stepx = new double();
            double stepy = new double();
            double stepz = new double();
            Avg_x = (2 * this.X_start + this.X_step * (this.X_len - 1)) / 2;
            double Avg_y = new double();
            Avg_y = (2 * this.Y_start + this.Y_step * (this.Y_len - 1)) / 2;
            double Avg_z = new double();
            Avg_z = (2 * this.Z_start + this.Z_step * (this.Z_len - 1)) / 2;
            xs[0] = Avg_x;
            xs[1] = Avg_x;
            ys[0] = Avg_y;
            ys[1] = Avg_y;
            zs[0] = Avg_z;
            zs[1] = Avg_z;

            if (this.X_step == 0)
            {
                stepx = 1;
            }
            else if ((Avg_x-this.X_start) % this.X_step != 0)
            {
                stepx = this.X_step;
                dirs[0] = 1;
                xs[0] = Avg_x - this.X_step / 2;
                xs[1] = Avg_x + this.X_step / 2;
            }
            else
            {
                stepx = this.X_step;
            }

            if (this.Y_step == 0)
            {
                stepy = 1;
            }
            else if ((Avg_y - this.Y_start) % this.Y_step != 0)
            {
                stepy = this.Y_step;
                dirs[1] = 1;
                ys[0] = Avg_y - this.Y_step / 2;
                ys[1] = Avg_y + this.Y_step / 2;
            }
            else
            {
                stepy = this.Y_step;
            }

            if (this.Z_step == 0)
            {
                stepz = 1;
            }
            else if ((Avg_z - this.Z_start) % this.Z_step != 0)
            {
                stepz = this.Z_step;
                dirs[2] = 1;
                zs[0] = Avg_z - this.Z_step / 2;
                zs[1] = Avg_z + this.Z_step / 2;
            }
            else
            {
                stepz = this.Z_step;
            }

            for (int i=0; i<2; i++)
            {
                for (int j=0; j<2; j++)
                {
                    for (int k=0; k<2; k++)
                    {
                        Bzs.Add(this.B_z[Convert.ToInt32((zs[i] - this.Z_start) / stepz), Convert.ToInt32((ys[j] - this.Y_start) / stepy), Convert.ToInt32((xs[k] - this.X_start) / stepx)]);
                    }
                }
            }
            uni_Bz = Bzs.Average();

            double x_comp_x, x_comp_y, x_comp_z, y_comp_x, y_comp_y, y_comp_z, z_comp_x, z_comp_y, z_comp_z;
            double x_sum, y_sum, z_sum, gx_sum, gy_sum, gz_sum;
            double x_tot = 0;
            double y_tot = 0;
            double z_tot = 0;
            double gx_tot = 0;
            double gy_tot = 0;
            double gz_tot = 0;
            for (int k=0; k<this.Z_len; k++)
            {
                x_sum = 0;
                y_sum = 0;
                z_sum = 0;
                gx_sum = 0;
                gy_sum = 0;
                gz_sum = 0;
                for (int j=0; j<this.Y_len; j++)
                {
                    for (int i=0; i<this.X_len; i++)
                    {
                        no = this.Planes[k].Nodes[j, i];
                        x_sum += no.B_x;
                        y_sum += no.B_y;
                        z_sum += no.B_z;
                        if (i != (X_len-1))
                        {
                            x_comp_x = (this.Planes[k].Nodes[j, i + 1].B_x - no.B_x) / (this.X_step);
                            y_comp_x = (this.Planes[k].Nodes[j, i + 1].B_y - no.B_y) / (this.X_step);
                            z_comp_x = (this.Planes[k].Nodes[j, i + 1].B_z - no.B_z) / (this.X_step);
                        }
                        else
                        {
                            x_comp_x = 0;
                            y_comp_x = 0;
                            z_comp_x = 0;
                        }
                        if (j != (Y_len - 1))
                        {
                            x_comp_y = (this.Planes[k].Nodes[j+1, i].B_x - no.B_x) / (this.Y_step);
                            y_comp_y = (this.Planes[k].Nodes[j+1, i].B_y - no.B_y) / (this.Y_step);
                            z_comp_y = (this.Planes[k].Nodes[j+1, i].B_z - no.B_z) / (this.Y_step);
                        }
                        else
                        {
                            x_comp_y = 0;
                            y_comp_y = 0;
                            z_comp_y = 0;
                        }
                        if (k != (Z_len - 1))
                        {
                            x_comp_z = (this.Planes[k+1].Nodes[j, i].B_x - no.B_x) / (this.Z_step);
                            y_comp_z = (this.Planes[k+1].Nodes[j, i].B_y - no.B_y) / (this.Z_step);
                            z_comp_z = (this.Planes[k+1].Nodes[j, i].B_z - no.B_z) / (this.Z_step);
                        }
                        else
                        {
                            x_comp_z = 0;
                            y_comp_z = 0;
                            z_comp_z = 0;
                        }
                        this.Gradient_x[k, j, i] = Math.Sqrt(Math.Pow(x_comp_x, 2) + Math.Pow(x_comp_y, 2) + Math.Pow(x_comp_z, 2))/uni_Bz;
                        this.Gradient_y[k, j, i] = Math.Sqrt(Math.Pow(y_comp_x, 2) + Math.Pow(y_comp_y, 2) + Math.Pow(y_comp_z, 2))/uni_Bz;
                        this.Gradient_z[k, j, i] = Math.Sqrt(Math.Pow(z_comp_x, 2) + Math.Pow(z_comp_y, 2) + Math.Pow(z_comp_z, 2))/uni_Bz;
                        this.Gradient[k, j, i] = Math.Sqrt(Math.Pow(this.Gradient_x[k,j,i], 2) + Math.Pow(this.Gradient_y[k, j, i], 2) + Math.Pow(this.Gradient_z[k, j, i], 2));
                        if ((i != (X_len - 1)) && (j != (Y_len - 1)) && (k != (Z_len - 1)))
                        {
                            gx_sum += this.Gradient_x[k, j, i];
                            gy_sum += this.Gradient_y[k, j, i];
                            gz_sum += this.Gradient_z[k, j, i];
                        }
                    }
                }
                x_tot += x_sum;
                y_tot += y_sum;
                z_tot += z_sum;
                gx_tot += gx_sum;
                gy_tot += gy_sum;
                gz_tot += gz_sum;
                this.Planes[k].B_x_avg = x_sum / (this.X_len * this.Y_len);
                this.Planes[k].B_y_avg = y_sum / (this.X_len * this.Y_len);
                this.Planes[k].B_z_avg = z_sum / (this.X_len * this.Y_len);
                this.Planes[k].G_x_avg = gx_sum / ((this.X_len-1) * (this.Y_len-1));
                this.Planes[k].G_y_avg = gy_sum / ((this.X_len-1) * (this.Y_len-1));
                this.Planes[k].G_z_avg = gz_sum / ((this.X_len-1) * (this.Y_len-1));
            }
            this.B_x_avg = x_tot / (this.X_len * this.Y_len * this.Z_len);
            this.B_y_avg = y_tot / (this.X_len * this.Y_len * this.Z_len);
            this.B_z_avg = z_tot / (this.X_len * this.Y_len * this.Z_len);
            this.G_x_avg = gx_tot / ((this.X_len - 1) * (this.Y_len - 1) * (this.Z_len - 1));
            this.G_y_avg = gy_tot / ((this.X_len - 1) * (this.Y_len - 1) * (this.Z_len - 1));
            this.G_z_avg = gz_tot / ((this.X_len - 1) * (this.Y_len - 1) * (this.Z_len - 1));
        }
        public double[] Rtn_params()
        {
            double[] rv = new double[9];
            rv[0] = this.X_len;
            rv[1] = this.Y_len;
            rv[2] = this.Z_len;
            rv[3] = this.X_start;
            rv[4] = this.Y_start;
            rv[5] = this.Z_start;
            rv[6] = this.X_step;
            rv[7] = this.Y_step;
            rv[8] = this.Z_step;
            return rv;
        }
        public double[,] Rtn_B(string comp, double Z)
        {
            int k = Convert.ToInt32((Z - this.Z_start) / this.Z_step);
            double[,] rv = new double[this.Y_len, this.X_len];
            for (int j = 0; j<this.Y_len; j++)
            {
                for (int i = 0; i < this.X_len; i++)
                {
                    if (comp.Equals("x"))
                    {
                        rv[j, i] = this.Planes[k].Nodes[j, i].B_x;
                    }
                    else if (comp.Equals("y"))
                    {
                        rv[j, i] = this.Planes[k].Nodes[j, i].B_y;
                    }
                    else if (comp.Equals("z"))
                    {
                        rv[j, i] = this.Planes[k].Nodes[j, i].B_z;
                    }
                    else
                    {
                        rv[j, i] = this.Planes[k].Nodes[j, i].B_v;
                    }
                }
            }
            return rv;
        }
        public double[,] Rtn_grad(string comp, double Z)
        {
            int i = Convert.ToInt32((Z - this.Z_start) / this.Z_step);
            double[,] rv = new double[this.Y_len, this.X_len];
            for (int j=0; j<this.Y_len; j++)
            {
                for (int k=0; k<this.X_len; k++)
                {
                    if (comp.Equals("x"))
                    {
                        rv[j, k] = this.Gradient_x[i, j, k];
                    }
                    else if (comp.Equals("y"))
                    {
                        rv[j, k] = this.Gradient_y[i, j, k];
                    }
                    else if (comp.Equals("z"))
                    {
                        rv[j, k] = this.Gradient_z[i, j, k];
                    }
                    else
                    {
                        rv[j, k] = this.Gradient[i, j, k];
                    }
                }
            }
            return rv;
        }
        public double[,] Rtn_avg(string comp, double Z)
        {
            double[,] rv = new double[2,2];
            if (comp.Equals("x"))
            {
                rv[0,0] = this.B_x_avg;
                rv[0,1] = this.G_x_avg;
            }
            else if (comp.Equals("y"))
            {
                rv[0,0] = this.B_y_avg;
                rv[0,1] = this.G_y_avg;
            }
            else if (comp.Equals("z"))
            {
                rv[0,0] = this.B_z_avg;
                rv[0,1] = this.G_z_avg;
            }
            else
            {
                rv[0,0] = Math.Sqrt(Math.Pow(this.B_x_avg,2) + Math.Pow(this.B_y_avg, 2) + Math.Pow(this.B_z_avg, 2));
                rv[0,1] = Math.Sqrt(Math.Pow(this.G_x_avg, 2) + Math.Pow(this.G_y_avg, 2) + Math.Pow(this.G_z_avg, 2));
            }
            int i = Convert.ToInt32((Z - this.Z_start) / this.Z_step);
            if (comp.Equals("x"))
            {
                rv[1, 0] = this.Planes[i].B_x_avg;
                rv[1, 1] = this.Planes[i].G_x_avg;
            }
            else if (comp.Equals("y"))
            {
                rv[1, 0] = this.Planes[i].B_y_avg;
                rv[1, 1] = this.Planes[i].G_y_avg;
            }
            else if (comp.Equals("z"))
            {
                rv[1, 0] = this.Planes[i].B_z_avg;
                rv[1, 1] = this.Planes[i].G_z_avg;
            }
            else
            {
                rv[1, 0] = Math.Sqrt(Math.Pow(this.Planes[i].B_x_avg, 2) + Math.Pow(this.Planes[i].B_y_avg, 2) + Math.Pow(this.Planes[i].B_z_avg, 2));
                rv[1, 1] = Math.Sqrt(Math.Pow(this.Planes[i].G_x_avg, 2) + Math.Pow(this.Planes[i].G_y_avg, 2) + Math.Pow(this.Planes[i].G_z_avg, 2));
            }
            return rv;
        }
        public int Rtn_ind(string comp, double val)
        {
            int rv;
            if (comp.Equals("x"))
            {
                if (this.X_step != 0)
                {
                    rv = Convert.ToInt32((val - this.X_start) / this.X_step);
                }
                else
                {
                    rv = 0;
                }
            }
            else if (comp.Equals("y"))
            {
                if (this.Y_step != 0)
                {
                    rv = Convert.ToInt32((val - this.Y_start) / this.Y_step);
                }
                else
                {
                    rv = 0;
                }
            }
            else
            {
                if (this.Z_step != 0)
                {
                    rv = Convert.ToInt32((val - this.Z_start) / this.Z_step);
                }
                else
                {
                    rv = 0;
                }
            }
            return rv;
        }
        public int Rtn_units()
        {
            if (this.Metric == true)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
