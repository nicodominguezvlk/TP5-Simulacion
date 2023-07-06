using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combinado
{
    public class RungeKutta
    {
        public RungeKutta() { }

        public decimal calcularRKDetencion(string tipoDetencion, decimal A, decimal? B, decimal h, decimal t, out DataTable dt)
        {
            //tipoDetencion: "Llegadas", "Servidor".
            //A: 111.9339 (es lo que corrimos en el tp4 para la llegada del auto 150)
            //B: Número aleatorio U(0,1)
            //h: 0.01
            //t: 0
            //
            //Retornos:
            //valor: Valor que requiere la celda de la simulación
            //out dt: Tabla RungeKutta completa

            dt = new DataTable();
            dt.Columns.Add("t_detencion");
            dt.Columns.Add("A_detencion");
            dt.Columns.Add("K1_detencion");
            dt.Columns.Add("K2_detencion");
            dt.Columns.Add("K3_detencion");
            dt.Columns.Add("K4_detencion");
            dt.Columns.Add("tdeimas1_detencion");
            dt.Columns.Add("Adeimas1_detencion");

            // Primera fila
            int i = 0;
            dt.Rows.Add();
            DataRow filaAnterior;
            DataRow filaActual = dt.Rows[i];
            filaActual["t_detencion"] = t;
            filaActual["A_detencion"] = A;
            filaActual["K1_detencion"] = B * A;
            filaActual["K2_detencion"] = B * (A + (h / 2) * Convert.ToDecimal(filaActual["K1_detencion"]));
            filaActual["K3_detencion"] = B * (A + (h / 2) * Convert.ToDecimal(filaActual["K2_detencion"]));
            filaActual["K4_detencion"] = B * (A + h * Convert.ToDecimal(filaActual["K3_detencion"]));
            filaActual["tdeimas1_detencion"] = t + h;
            filaActual["Adeimas1_detencion"] = A + ((h / 6)*(Convert.ToDecimal(filaActual["K1_detencion"]) + 2 * Convert.ToDecimal(filaActual["K2_detencion"])+ 2* Convert.ToDecimal(filaActual["K3_detencion"]) + Convert.ToDecimal(filaActual["K4_detencion"])));

            while (1 == 1)
            {
                i++;
                dt.Rows.Add();
                filaAnterior = filaActual;
                filaActual = dt.Rows[i];
                filaActual["t_detencion"] = filaAnterior["tdeimas1_detencion"];
                filaActual["A_detencion"] = filaAnterior["Adeimas1_detencion"];
                filaActual["K1_detencion"] = B * Convert.ToDecimal(filaActual["A_detencion"]);
                filaActual["K2_detencion"] = B * (Convert.ToDecimal(filaActual["A_detencion"]) + (h / 2) * Convert.ToDecimal(filaActual["K1_detencion"]));
                filaActual["K3_detencion"] = B * (Convert.ToDecimal(filaActual["A_detencion"]) + (h / 2) * Convert.ToDecimal(filaActual["K2_detencion"]));
                filaActual["K4_detencion"] = B * (Convert.ToDecimal(filaActual["A_detencion"]) + h * Convert.ToDecimal(filaActual["K3_detencion"]));
                filaActual["tdeimas1_detencion"] = Convert.ToDecimal(filaActual["t_detencion"]) + h;
                filaActual["Adeimas1_detencion"] = Convert.ToDecimal(filaActual["A_detencion"]) + ((h / 6) * (Convert.ToDecimal(filaActual["K1_detencion"]) + 2 * Convert.ToDecimal(filaActual["K2_detencion"]) + 2 * Convert.ToDecimal(filaActual["K3_detencion"]) + Convert.ToDecimal(filaActual["K4_detencion"])));

                if (Convert.ToDecimal(filaActual["A_detencion"]) >= 3 * A)
                {
                    break;
                }

                if (i == 999999)
                {
                    throw new Exception("Este ciclo nunca corta");
                }
            }

            // Retornos
            decimal valor = Convert.ToDecimal(filaActual["t_detencion"]) * 30;
            return valor;
        }

        public decimal calcularRKReanudacionLlegadas(decimal? L, decimal t, decimal h, out DataTable dt)
        {
            //L: Reloj de la simulación para cuando ocurre evento detención
            //t: 0
            //h: 0.01
            //
            //Retornos:
            //valor: Valor que requiere la celda de la simulación
            //out dt: Tabla RungeKutta completa

            dt = new DataTable();
            dt.Columns.Add("t_reanudLleg");
            dt.Columns.Add("L_reanudLleg");
            dt.Columns.Add("K1_reanudLleg");
            dt.Columns.Add("K2_reanudLleg");
            dt.Columns.Add("K3_reanudLleg");
            dt.Columns.Add("K4_reanudLleg");
            dt.Columns.Add("tdeimas1_reanudLleg");
            dt.Columns.Add("Ldeimas1_reanudLleg");
            dt.Columns.Add("LmenosLmenos1_reanudLleg");

            // Primera fila
            int i = 0;
            dt.Rows.Add();
            DataRow filaAnterior;
            DataRow filaActual = dt.Rows[i];
            filaActual["t_reanudLleg"] = t;
            filaActual["L_reanudLleg"] = L;
            //-(L/(0.1+0,8*t^2))-L
            filaActual["K1_reanudLleg"] = -(L / ((decimal)0.1 +(decimal)0.8 *((decimal)Math.Pow((double)t,(double)2))))- L;
            filaActual["K2_reanudLleg"] = -((L + (h/2)* Convert.ToDecimal(filaActual["K1_reanudLleg"])) / ((decimal)0.1 +(decimal)0.8 * ((decimal)Math.Pow((double)(t+(h/2)), (double)2)))) - (L + (h / 2) * Convert.ToDecimal(filaActual["K1_reanudLleg"]));
            filaActual["K3_reanudLleg"] = -((L + (h / 2) * Convert.ToDecimal(filaActual["K2_reanudLleg"])) / ((decimal)0.1 + (decimal)0.8 * ((decimal)Math.Pow((double)(t + (h / 2)), (double)2)))) - (L + (h / 2) * Convert.ToDecimal(filaActual["K2_reanudLleg"]));
            filaActual["K4_reanudLleg"] = -((L + h * Convert.ToDecimal(filaActual["K3_reanudLleg"])) / ((decimal)0.1 + (decimal)0.8 * ((decimal)Math.Pow((double)(t + h), (double)2)))) - (L + h * Convert.ToDecimal(filaActual["K3_reanudLleg"]));
            filaActual["tdeimas1_reanudLleg"] = t + h;
            filaActual["Ldeimas1_reanudLleg"] = L + (h / 6) * (Convert.ToDecimal(filaActual["K1_reanudLleg"]) + 2 * Convert.ToDecimal(filaActual["K2_reanudLleg"]) + 2 * Convert.ToDecimal(filaActual["K3_reanudLleg"]) + Convert.ToDecimal(filaActual["K4_reanudLleg"]));
            filaActual["LmenosLmenos1_reanudLleg"] = (decimal)0;

            while (1 == 1)
            {
                i++;
                dt.Rows.Add();
                filaAnterior = filaActual;
                filaActual = dt.Rows[i];
                filaActual["t_reanudLleg"] = filaAnterior["tdeimas1_reanudLleg"];
                filaActual["L_reanudLleg"] = filaAnterior["Ldeimas1_reanudLleg"];
                //-(L/(0,8*t^2))-L
                filaActual["K1_reanudLleg"] = -((Convert.ToDecimal(filaActual["L_reanudLleg"])) / ((decimal)0.1 + (decimal)0.8 * ((decimal)Math.Pow(Convert.ToDouble(filaActual["t_reanudLleg"]), (double)2 )))) - (Convert.ToDecimal(filaActual["L_reanudLleg"]));
                filaActual["K2_reanudLleg"] = -(((Convert.ToDecimal(filaActual["L_reanudLleg"])) + (h / 2) * Convert.ToDecimal(filaActual["K1_reanudLleg"])) / ((decimal)0.1 + (decimal)0.8 * ((decimal)Math.Pow((double)(Convert.ToDecimal(filaActual["t_reanudLleg"]) + (h / 2)), (double)2)))) - ((Convert.ToDecimal(filaActual["L_reanudLleg"])) + (h / 2) * Convert.ToDecimal(filaActual["K1_reanudLleg"]));
                filaActual["K3_reanudLleg"] = -(((Convert.ToDecimal(filaActual["L_reanudLleg"])) + (h / 2) * Convert.ToDecimal(filaActual["K2_reanudLleg"])) / ((decimal)0.1 + (decimal)0.8 * ((decimal)Math.Pow((double)(Convert.ToDecimal(filaActual["t_reanudLleg"]) + (h / 2)), (double)2)))) - ((Convert.ToDecimal(filaActual["L_reanudLleg"])) + (h / 2) * Convert.ToDecimal(filaActual["K2_reanudLleg"]));
                filaActual["K4_reanudLleg"] = -(((Convert.ToDecimal(filaActual["L_reanudLleg"])) + h * Convert.ToDecimal(filaActual["K3_reanudLleg"])) / ((decimal)0.1 + (decimal)0.8 * ((decimal)Math.Pow((double)(Convert.ToDecimal(filaActual["t_reanudLleg"]) + h), (double)2)))) - ((Convert.ToDecimal(filaActual["L_reanudLleg"])) + h * Convert.ToDecimal(filaActual["K3_reanudLleg"]));
                filaActual["tdeimas1_reanudLleg"] = (Convert.ToDecimal(filaAnterior["t_reanudLleg"])) + h;
                filaActual["Ldeimas1_reanudLleg"] = Convert.ToDecimal(filaActual["L_reanudLleg"]) + (h / 6) * (Convert.ToDecimal(filaActual["K1_reanudLleg"]) + 2 * Convert.ToDecimal(filaActual["K2_reanudLleg"]) + 2 * Convert.ToDecimal(filaActual["K3_reanudLleg"]) + Convert.ToDecimal(filaActual["K4_reanudLleg"]));
                filaActual["LmenosLmenos1_reanudLleg"] = Math.Abs(Convert.ToDecimal(filaActual["L_reanudLleg"]) - Convert.ToDecimal(filaAnterior["L_reanudLleg"]));

                if (Convert.ToDecimal(filaActual["LmenosLmenos1_reanudLleg"]) < 1)
                {
                    break;
                }

                if (i == 999999)
                {
                    throw new Exception("Este ciclo nunca corta");
                }
            }

            decimal valor = Convert.ToDecimal(filaActual["t_reanudLleg"]) * 27;
            return valor;
        }

        public decimal calcularRKReanudacionServidor(decimal? S, decimal t, decimal h, out DataTable dt)
        {
            //S: Reloj de la simulación cuando se da el evento detención
            //t: 0
            //h: 0.1
            //
            //Retornos:
            //valor: Valor que requiere la celda de la simulación
            //out dt: Tabla RungeKutta completa

            dt = new DataTable();
            dt.Columns.Add("t_reanudServ");
            dt.Columns.Add("S_reanudServ");
            dt.Columns.Add("K1_reanudServ");
            dt.Columns.Add("K2_reanudServ");
            dt.Columns.Add("K3_reanudServ");
            dt.Columns.Add("K4_reanudServ");
            dt.Columns.Add("tdeimas1_reanudServ");
            dt.Columns.Add("Sdeimas1_reanudServ");

            // Primera fila
            int i = 0;
            decimal? freno = S *(decimal) 1.5;
            dt.Rows.Add();
            DataRow filaAnterior;
            DataRow filaActual = dt.Rows[i];
            filaActual["t_reanudServ"] = t;
            filaActual["S_reanudServ"] = S;
            //(0.2 * S) + 3 - t
            filaActual["K1_reanudServ"] = ((decimal)0.2 * S) + 3 - t;
            filaActual["K2_reanudServ"] = ((decimal)0.2 * (S + (h / 2) * Convert.ToDecimal(filaActual["K1_reanudServ"])) + 3 - (t + (h / 2)));
            filaActual["K3_reanudServ"] = ((decimal)0.2 * (S + (h / 2) * Convert.ToDecimal(filaActual["K2_reanudServ"])) + 3 - (t + (h / 2)));
            filaActual["K4_reanudServ"] = ((decimal)0.2 * (S + h * Convert.ToDecimal(filaActual["K3_reanudServ"])) + 3 - (t +  h));
            filaActual["tdeimas1_reanudServ"] = t + h;
            filaActual["Sdeimas1_reanudServ"] = S + ((h / 6)*(Convert.ToDecimal(filaActual["K1_reanudServ"]) + 2 * Convert.ToDecimal(filaActual["K2_reanudServ"]) + 2 * Convert.ToDecimal(filaActual["K3_reanudServ"]) + Convert.ToDecimal(filaActual["K4_reanudServ"])));

            while (1 == 1)
            {
                i++;
                dt.Rows.Add();
                filaAnterior = filaActual;
                filaActual = dt.Rows[i];
                filaActual["t_reanudServ"] = filaAnterior["tdeimas1_reanudServ"];
                filaActual["S_reanudServ"] = filaAnterior["Sdeimas1_reanudServ"];
                //(0.2 * S) + 3 - t
                filaActual["K1_reanudServ"] = ((decimal)0.2 * (Convert.ToDecimal(filaActual["S_reanudServ"]))) + 3 - Convert.ToDecimal(filaActual["t_reanudServ"]);
                filaActual["K2_reanudServ"] = ((decimal)0.2 * ((Convert.ToDecimal(filaActual["S_reanudServ"])) + (h / 2) * Convert.ToDecimal(filaActual["K1_reanudServ"]))) + 3 - (Convert.ToDecimal(filaActual["t_reanudServ"]) + (h / 2));
                filaActual["K3_reanudServ"] = ((decimal)0.2 * ((Convert.ToDecimal(filaActual["S_reanudServ"])) + (h / 2) * Convert.ToDecimal(filaActual["K2_reanudServ"]))) + 3 - (Convert.ToDecimal(filaActual["t_reanudServ"]) + (h / 2));
                filaActual["K4_reanudServ"] = ((decimal)0.2 * ((Convert.ToDecimal(filaActual["S_reanudServ"]) + h * Convert.ToDecimal(filaActual["K3_reanudServ"]))) + 3 - (Convert.ToDecimal(filaActual["t_reanudServ"]) + h));
                filaActual["tdeimas1_reanudServ"] = Convert.ToDecimal(filaActual["t_reanudServ"]) + h;
                filaActual["Sdeimas1_reanudServ"] = Convert.ToDecimal(filaActual["S_reanudServ"]) + ((h / 6) * (Convert.ToDecimal(filaActual["K1_reanudServ"]) + 2 * Convert.ToDecimal(filaActual["K2_reanudServ"]) + 2 * Convert.ToDecimal(filaActual["K3_reanudServ"]) + Convert.ToDecimal(filaActual["K4_reanudServ"])));

                if(Convert.ToDecimal(filaActual["S_reanudServ"]) > freno)
                {
                    break;
                }

                if (i == 999999)
                {
                    throw new Exception("Este ciclo nunca corta");
                }
            }

            decimal valor = Convert.ToDecimal(filaActual["t_reanudServ"]) * 8;
            return valor;

        }
    }
}
