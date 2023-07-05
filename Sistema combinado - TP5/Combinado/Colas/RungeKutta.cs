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

        public DataTable calcularRKDetencion(string tipoDetencion, decimal A, decimal B, decimal h, decimal t0, out decimal valor)
        {
            //tipoDetencion: "Llegadas", "Servidor".
            //A: DEFINIR
            //B: DEFINIR
            //h: Precisión
            //t0: DEFINIR
            //
            //Retornos:
            //Tabla RungeKutta completa
            //out valor: Valor que requiere la celda de la simulación


            throw new NotImplementedException();
        }

        public DataTable calcularRKReanudacionLlegadas(decimal L, decimal t, decimal h, out decimal valor)
        {
            //L: DEFINIR
            //t: DEFINIR
            //h: Precisión
            //
            //Retornos:
            //Tabla RungeKutta completa
            //out valor: Valor que requiere la celda de la simulación


            throw new NotImplementedException();
        }

        public DataTable calcularRKReanudacionServidor(decimal S, decimal t, decimal h, decimal freno, out decimal valor)
        {
            //S: DEFINIR
            //t: DEFINIR
            //h: Precisión
            //freno: Valor de S para frenar el cálculo
            //
            //Retornos:
            //Tabla RungeKutta completa
            //out valor: Valor que requiere la celda de la simulación


            throw new NotImplementedException();
        }
    }
}
