﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Combinado
{
    public partial class Visualizador : Form
    {
        Menu menu;
        decimal mediaLlegada;
        decimal mediaAP;
        decimal mediaAE;
        decimal mediaAC;
        decimal mediaACM;
        int cantidadDeSimulaciones;
        int verDesdeSimulacion;
        int verHastaSimulacion;
        int numeroSimulacionActual;
        DataTable dt;
        RungeKutta rk;

        public Visualizador(Menu menu, decimal mediaLlegada, decimal mediaAP, decimal mediaAE, decimal mediaAC, decimal mediaACM, int cant, int desde, int hasta)
        {
            InitializeComponent();
            this.menu = menu;
            this.mediaLlegada = mediaLlegada;
            this.mediaAP = mediaAP;
            this.mediaAE = mediaAE;
            this.mediaAC = mediaAC;
            this.mediaACM = mediaACM;
            cantidadDeSimulaciones = cant;
            verDesdeSimulacion = desde;
            verHastaSimulacion = hasta;
            rk = new RungeKutta();

            eventoInicial();
        }


        // -----INSTRUCCIONES-----
        //
        //
        // Tipos de métodos:
        // - nombreDeEvento(): Métodos que contienen la lógica celda por celda de cada evento.
        // - generarX(): Fórmula (igual al Excel) que genera un valor para una celda relacionada con números aleatorios.
        // - Métodos auxiliares (por ejemplo, para encontrar la cola con menos objetos, etc)
        //
        // EN LOS EVENTOS, UNA VARIABLE POR CELDA, INCLUSO SI NO SE USA EN EL EVENTO EN CUESTIÓN
        //
        // Asegurarse que en los métodos de los eventos esté SÓLO EL ORDEN DE LAS LLAMADAS QUE NECESITA EL EVENTO, y nó la lógica de las fórmulas de las celdas
        // Así también (y más importante), verificar que en los métodos con las fórmulas de las celdas no haya ninguna lógica propia de eventos (para que se pueda usar el mismo método
        // independientemente del evento en cuestión).


        // Generadores
        public decimal? generarRandom()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            decimal? numeroAleatorio = aDecimal(random.NextDouble());
            return numeroAleatorio;
        }

        public decimal? generarTiempoExponencial(decimal? rnd, decimal media)
        {
            decimal? tiempoLlegada = aDecimal(-Convert.ToDouble(media) * Math.Log(Convert.ToDouble(1 - rnd)));
            return tiempoLlegada;
        }

        public decimal? generarProximo(decimal? reloj, decimal? tiempo)
        {
            decimal? proximo = reloj + tiempo;
            return proximo;
        }

        public int? generarCantidadPersonas(decimal? rnd)
        {
            int? cantidadPersonas = Convert.ToInt32(Math.Round(Convert.ToDouble(aDecimal(-Convert.ToDouble(4) * Math.Log(Convert.ToDouble(1 - rnd))))));
            
            return cantidadPersonas;
        }

        public int? generarCantidadPersonasMayores(decimal? rnd, decimal? cantidadTotal)
        {
            int? cantidadPersonasMayores = Convert.ToInt32(Math.Round(Convert.ToDouble(aDecimal(0 + (rnd * cantidadTotal)))));

            return cantidadPersonasMayores;
        }


        // Métodos auxiliares
        public decimal? aDecimal(object valor)
        {
            if (valor == DBNull.Value)
            {
                valor = 0;
            }

            return Convert.ToDecimal(valor);
        }

        public DBNull convertirANull(object valor)
        {
            if (Convert.ToDecimal(valor) == 0)
            {
                valor = DBNull.Value;
            }

            return (DBNull)valor; 
        }

        public int? obtenerColaMenorParking(int? colaPark1, int? colaPark2, int? colaPark3, int? colaPark4, int? colaPark5)
        {
            List<int?> colas = new List<int?> { colaPark1, colaPark2, colaPark3, colaPark4, colaPark5 };
            int? menor = colas.Min();
            return menor;
        }

        public int? obtenerColaMenorEntrada(int? colaEntrada1y2, int? colaEntrada3y4, int? colaEntrada5y6)
        {
            List<int?> colaEntrada = new List<int?> { colaEntrada1y2, colaEntrada3y4, colaEntrada5y6 };
            int? menorColaEntrada = colaEntrada.Min();
            return menorColaEntrada;
        }

        public int? obtenerColaMenorControlComida(int? colaComida1, int? colaComida2, int? colaComida3, int? colaComida4)
        {
            List<int?> colaComida = new List<int?> { colaComida1, colaComida2, colaComida3, colaComida4 };
            int? menorColaComida = colaComida.Min();
            return menorColaComida;
        }

        public string obtenerProximoEvento(decimal? horaDetencion, decimal? proximaLlegada, decimal? horaReanudacionLlegadas, decimal? proximoFinAP1, decimal? proximoFinAP2, decimal? proximoFinAP3, decimal? proximoFinAP4, decimal? proximoFinAP5, decimal? proximoFinAE1, decimal? proximoFinAE2, decimal? proximoFinAE3, decimal? proximoFinAE4, decimal? proximoFinAE5, decimal? proximoFinAE6,
                decimal? proximoFinAC1, decimal? proximoFinAC2, decimal? proximoFinAC3, decimal? proximoFinAC4, decimal? proximoFinACM, decimal? horaReanudacionServidor)
        {
            List<decimal?> ListaFin = new List<decimal?> {horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor };
            List<decimal?> ListaFinNueva = new List<decimal?>();

            // Si los eventos son nulos, 
            for (int i = 0; i < ListaFin.Count; i++)
            {
                if (ListaFin[i] != null && ListaFin[i] != 0)
                {
                    ListaFinNueva.Add(ListaFin[i]);
                }
            }
            decimal? eventoMin = ListaFinNueva.Min();

            string eventoProximo = "";

            // Buscar evento mínimo
            if (eventoMin == horaDetencion)
            { eventoProximo = "Detención"; }

            else if (eventoMin == proximaLlegada)
            { eventoProximo = "LL Auto"; }

            else if (eventoMin == horaReanudacionLlegadas)
            { eventoProximo = "Reanudación Llegadas"; }

            else if (eventoMin == proximoFinAP1 || eventoMin == proximoFinAP2 || eventoMin == proximoFinAP3 || eventoMin == proximoFinAP4 || eventoMin == proximoFinAP5)
            { eventoProximo = "Fin AP"; }

            else if (eventoMin == proximoFinAE1 || eventoMin == proximoFinAE2 || eventoMin == proximoFinAE3 || eventoMin == proximoFinAE4 || eventoMin == proximoFinAE5 || eventoMin == proximoFinAE6)
            { eventoProximo = "Fin AE"; }

            else if (eventoMin == proximoFinAC1 || eventoMin == proximoFinAC2 || eventoMin == proximoFinAC3 || eventoMin == proximoFinAC4)
            { eventoProximo = "Fin AC"; }

            else if (eventoMin == proximoFinACM)
            { eventoProximo = "Fin ACM"; }

            else if (eventoMin == horaReanudacionServidor)
            { eventoProximo = "Reanudación Servidor"; }

            else { throw new Exception("El próximo evento no está incluído entre las condiciones."); }

            return eventoProximo;
        }

        public decimal? obtenerHoraProxEvento(decimal? horaDetencion, decimal? proximaLlegada, decimal? horaReanudacionLlegadas, decimal? proximoFinAP1, decimal? proximoFinAP2, decimal? proximoFinAP3, decimal? proximoFinAP4, decimal? proximoFinAP5, decimal? proximoFinAE1, decimal? proximoFinAE2, decimal? proximoFinAE3, decimal? proximoFinAE4, decimal? proximoFinAE5, decimal? proximoFinAE6,
                decimal? proximoFinAC1, decimal? proximoFinAC2, decimal? proximoFinAC3, decimal? proximoFinAC4, decimal? proximoFinACM, decimal? horaReanudacionServidor)
        {
            List<decimal?> ListaFin = new List<decimal?> { horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor };
            List<decimal?> ListaFinNueva = new List<decimal?>();

            // Si los eventos son nulos, 
            for (int i = 0; i < ListaFin.Count; i++)
            {
                if (ListaFin[i] != 0 && ListaFin[i] != null)
                {
                    ListaFinNueva.Add(ListaFin[i]);
                }
            }
            decimal? horaEventoMin = ListaFinNueva.Min();
            
            return horaEventoMin;
        }


        // Eventos
        public void eventoInicial() // Actualizado
        {
            // Crear el vector de estado
            dt = new DataTable();
            dt.Columns.Add("evento");
            dt.Columns.Add("reloj");

            dt.Columns.Add("rndTipoDetencion");
            dt.Columns.Add("tipoDetencion");
            dt.Columns.Add("beta");
            dt.Columns.Add("tiempoDeDetencion");
            dt.Columns.Add("horaDetencion");

            dt.Columns.Add("rndLlegada");
            dt.Columns.Add("tiempoLlegada");
            dt.Columns.Add("proximaLlegada");

            dt.Columns.Add("l");
            dt.Columns.Add("tiempoDuracionDetencionLlegadas");
            dt.Columns.Add("horaReanudacionLlegadas");

            dt.Columns.Add("rndFinAP");
            dt.Columns.Add("tiempoFinAP");
            dt.Columns.Add("proximoFinAP1");
            dt.Columns.Add("proximoFinAP2");
            dt.Columns.Add("proximoFinAP3");
            dt.Columns.Add("proximoFinAP4");
            dt.Columns.Add("proximoFinAP5");

            dt.Columns.Add("rndFinAE");
            dt.Columns.Add("tiempoFinAE");
            dt.Columns.Add("proximoFinAE1");
            dt.Columns.Add("proximoFinAE2");
            dt.Columns.Add("proximoFinAE3");
            dt.Columns.Add("proximoFinAE4");
            dt.Columns.Add("proximoFinAE5");
            dt.Columns.Add("proximoFinAE6");
            dt.Columns.Add("rndCantidadPersonas");
            dt.Columns.Add("cantidadPersonas");
            dt.Columns.Add("rndCantidadPersonasMayores");
            dt.Columns.Add("cantidadPersonasMayores");
            dt.Columns.Add("cantidadPersonasNoMayores");

            dt.Columns.Add("rndFinAC1");
            dt.Columns.Add("tiempoFinAC1");
            dt.Columns.Add("proximoFinAC1");
            dt.Columns.Add("rndFinAC2");
            dt.Columns.Add("tiempoFinAC2");
            dt.Columns.Add("proximoFinAC2");
            dt.Columns.Add("rndFinAC3");
            dt.Columns.Add("tiempoFinAC3");
            dt.Columns.Add("proximoFinAC3");
            dt.Columns.Add("rndFinAC4");
            dt.Columns.Add("tiempoFinAC4");
            dt.Columns.Add("proximoFinAC4");

            dt.Columns.Add("rndFinACM");
            dt.Columns.Add("tiempoFinACM");
            dt.Columns.Add("proximoFinACM");

            dt.Columns.Add("s");
            dt.Columns.Add("tiempoDuracionDetencionServidor");
            dt.Columns.Add("horaReanudacionServidor");

            dt.Columns.Add("ingresoBloqueado");
            dt.Columns.Add("colaBloqueoLlegadas");

            dt.Columns.Add("colaPark1");
            dt.Columns.Add("estadoCajaPark1");
            dt.Columns.Add("colaPark2");
            dt.Columns.Add("estadoCajaPark2");
            dt.Columns.Add("colaPark3");
            dt.Columns.Add("estadoCajaPark3");
            dt.Columns.Add("colaPark4");
            dt.Columns.Add("estadoCajaPark4");
            dt.Columns.Add("colaPark5");
            dt.Columns.Add("estadoCajaPark5");

            dt.Columns.Add("colaEntrada1y2");
            dt.Columns.Add("estadoCajaEntrada1");
            dt.Columns.Add("estadoCajaEntrada2");
            dt.Columns.Add("colaEntrada3y4");
            dt.Columns.Add("estadoCajaEntrada3");
            dt.Columns.Add("estadoCajaEntrada4");
            dt.Columns.Add("colaEntrada5y6");
            dt.Columns.Add("estadoCajaEntrada5");
            dt.Columns.Add("estadoCajaEntrada6");

            dt.Columns.Add("colaComida1");
            dt.Columns.Add("estadoControlComida1");
            dt.Columns.Add("colaComida2");
            dt.Columns.Add("estadoControlComida2");
            dt.Columns.Add("colaComida3");
            dt.Columns.Add("estadoControlComida3");
            dt.Columns.Add("colaComida4");
            dt.Columns.Add("estadoControlComida4");

            dt.Columns.Add("colaComidaMayores");
            dt.Columns.Add("estadoControlComidaMayores");

            dt.Columns.Add("metrosPromedioNecesariosParaAparcamiento");
            dt.Columns.Add("acumuladorTiempoColaParking");
            dt.Columns.Add("cantidadPromedioAutosEnColaPark");
            dt.Columns.Add("contadorGruposCajaEntrada");
            dt.Columns.Add("acumuladorTiempoColaEntrada");
            dt.Columns.Add("tiempoPromedioEnColaEntrada");
            dt.Columns.Add("contadorPersonasEnControlComida");
            dt.Columns.Add("acumuladorTiempoColaComida");
            dt.Columns.Add("tiempoPromedioEnColaComida");
            dt.Columns.Add("tiempoEnConseguirEntrada");
            dt.Columns.Add("cantidadPromedioGenteEnColaEntrada");
            dt.Columns.Add("tiempoEntradaDespuesDeEstacionar");


            // Marcar número de simulación
            numeroSimulacionActual = 0;


            // Nombres de las variables (una por cada columna)
            string evento;
            decimal? reloj;

            decimal? rndTipoDetencion;
            string tipoDetencion;
            decimal? beta;
            decimal? tiempoDeDetencion;
            decimal? horaDetencion;

            decimal? rndLlegada;
            decimal? tiempoLlegada;
            decimal? proximaLlegada;

            decimal? l;
            decimal? tiempoDuracionDetencionLlegadas;
            decimal? horaReanudacionLlegadas;

            decimal? rndFinAP;
            decimal? tiempoFinAP;
            decimal? proximoFinAP1;
            decimal? proximoFinAP2;
            decimal? proximoFinAP3;
            decimal? proximoFinAP4;
            decimal? proximoFinAP5;

            decimal? rndFinAE;
            decimal? tiempoFinAE;
            decimal? proximoFinAE1;
            decimal? proximoFinAE2;
            decimal? proximoFinAE3;
            decimal? proximoFinAE4;
            decimal? proximoFinAE5;
            decimal? proximoFinAE6;
            decimal? rndCantidadPersonas;
            int? cantidadPersonas;
            decimal? rndCantidadPersonasMayores;
            int? cantidadPersonasMayores;
            int? cantidadPersonasNoMayores;

            decimal? rndFinAC1;
            decimal? tiempoFinAC1;
            decimal? proximoFinAC1;
            decimal? rndFinAC2;
            decimal? tiempoFinAC2;
            decimal? proximoFinAC2;
            decimal? rndFinAC3;
            decimal? tiempoFinAC3;
            decimal? proximoFinAC3;
            decimal? rndFinAC4;
            decimal? tiempoFinAC4;
            decimal? proximoFinAC4;

            decimal? rndFinACM;
            decimal? tiempoFinACM;
            decimal? proximoFinACM;

            decimal? s;
            decimal? tiempoDuracionDetencionServidor;
            decimal? horaReanudacionServidor;

            string ingresoBloqueado;
            int? colaBloqueoLlegadas;

            int? colaPark1;
            string estadoCajaPark1;
            int? colaPark2;
            string estadoCajaPark2;
            int? colaPark3;
            string estadoCajaPark3;
            int? colaPark4;
            string estadoCajaPark4;
            int? colaPark5;
            string estadoCajaPark5;

            int? colaEntrada1y2;
            string estadoCajaEntrada1;
            string estadoCajaEntrada2;
            int? colaEntrada3y4;
            string estadoCajaEntrada3;
            string estadoCajaEntrada4;
            int? colaEntrada5y6;
            string estadoCajaEntrada5;
            string estadoCajaEntrada6;

            int? colaComida1;
            string estadoControlComida1;
            int? colaComida2;
            string estadoControlComida2;
            int? colaComida3;
            string estadoControlComida3;
            int? colaComida4;
            string estadoControlComida4;

            int? colaComidaMayores;
            string estadoControlComidaMayores;

            decimal? metrosPromedioNecesariosParaAparcamiento;
            decimal? acumuladorTiempoColaParking;
            decimal? cantidadPromedioAutosEnColaPark;
            int? contadorGruposCajaEntrada;
            decimal? acumuladorTiempoColaEntrada;
            decimal? tiempoPromedioEnColaEntrada;
            int? contadorPersonasEnControlComida;
            decimal? acumuladorTiempoColaComida;
            decimal? tiempoPromedioEnColaComida;
            decimal? tiempoEnConseguirEntrada;
            decimal? cantidadPromedioGenteEnColaEntrada;
            decimal? tiempoEntradaDespuesDeEstacionar;


            // Evento
            evento = "Evento Inicial";
            

            // Reloj
            reloj = 0;


            // Detención
            rndTipoDetencion = generarRandom();
            
            if (rndTipoDetencion < aDecimal(0.35))
            {
                tipoDetencion = "Llegadas";
            }
            else
            {
                tipoDetencion = "Servidor";
            }

            beta = generarRandom();

            tiempoDeDetencion = rk.calcularRKDetencion(tipoDetencion, (decimal)111.9339, beta, (decimal)0.01, 0, out DataTable dtRKdetencion);
            grdRKDetencion.DataSource = dtRKdetencion;

            horaDetencion = tiempoDeDetencion + reloj;


            // Llegada auto
            rndLlegada = generarRandom();

            tiempoLlegada = generarTiempoExponencial(rndLlegada, mediaLlegada);

            proximaLlegada = generarProximo(reloj, tiempoLlegada);


            // Reanudación llegadas
            l = null;

            tiempoDuracionDetencionLlegadas = null;

            horaReanudacionLlegadas = null;


            // Fin atención parking
            rndFinAP = null;

            tiempoFinAP = null;

            proximoFinAP1 = null;
            proximoFinAP2 = null;
            proximoFinAP3 = null;
            proximoFinAP4 = null;
            proximoFinAP5 = null;


            // Fin atención entrada
            rndFinAE = null;

            tiempoFinAE = null;

            proximoFinAE1 = null;
            proximoFinAE2 = null;
            proximoFinAE3 = null;
            proximoFinAE4 = null;
            proximoFinAE5 = null;
            proximoFinAE6 = null;

            rndCantidadPersonas = null;

            cantidadPersonas = null;

            rndCantidadPersonasMayores = null;

            cantidadPersonasMayores = null;

            cantidadPersonasNoMayores = null;


            // Fin atención control comida
            rndFinAC1 = null;
            tiempoFinAC1 = null;
            proximoFinAC1 = null;

            rndFinAC2 = null;
            tiempoFinAC2 = null;
            proximoFinAC2 = null;

            rndFinAC3 = null;
            tiempoFinAC3 = null;
            proximoFinAC3 = null;

            rndFinAC4 = null;
            tiempoFinAC4 = null;
            proximoFinAC4 = null;


            // Fin atención control comida mayores
            rndFinACM = null;
            tiempoFinACM = null;
            proximoFinACM = null;


            // Reanudación servidor
            s = null;

            tiempoDuracionDetencionServidor = null;

            horaReanudacionServidor = null;


            // Bloqueo de llegadas
            ingresoBloqueado = "No";

            colaBloqueoLlegadas = 0;


            // Caja park
            colaPark1 = 0;
            estadoCajaPark1 = "Libre";
            colaPark2 = 0;
            estadoCajaPark2 = "Libre";
            colaPark3 = 0;
            estadoCajaPark3 = "Libre";
            colaPark4 = 0;
            estadoCajaPark4 = "Libre";
            colaPark5 = 0;
            estadoCajaPark5 = "Libre";


            // Caja entrada
            colaEntrada1y2 = 0;
            estadoCajaEntrada1 = "Libre";
            estadoCajaEntrada2 = "Libre";

            colaEntrada3y4 = 0;
            estadoCajaEntrada3 = "Libre";
            estadoCajaEntrada4 = "Libre";

            colaEntrada5y6 = 0;
            estadoCajaEntrada5 = "Libre";
            estadoCajaEntrada6 = "Libre";


            // Control comida
            colaComida1 = 0;
            estadoControlComida1 = "Libre";

            colaComida2 = 0;
            estadoControlComida2 = "Libre";

            colaComida3 = 0;
            estadoControlComida3 = "Libre";

            colaComida4 = 0;
            estadoControlComida4 = "Libre";


            // Control comida mayores
            colaComidaMayores = 0;
            estadoControlComidaMayores = "Libre";


            // Estadísticas
            metrosPromedioNecesariosParaAparcamiento = 0;
            acumuladorTiempoColaParking = 0;
            cantidadPromedioAutosEnColaPark = 0;

            contadorGruposCajaEntrada = 0;
            acumuladorTiempoColaEntrada = 0;
            tiempoPromedioEnColaEntrada = 0;

            contadorPersonasEnControlComida = 0;
            acumuladorTiempoColaComida = 0;
            tiempoPromedioEnColaComida = 0;

            tiempoEnConseguirEntrada = 0;
            cantidadPromedioGenteEnColaEntrada = 0;
            tiempoEntradaDespuesDeEstacionar = 0;



            // --- Manejo de tabla y próximo evento ---

            // Agregar la nueva fila
            dt.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);

            // Guardar la nueva fila en una variable para enviársela al próximo evento
            DataRow filaActual = dt.Rows[0];

            // Si la simulación está dentro de las que quiere visualizar, agregarla a la grilla
            if ((numeroSimulacionActual >= verDesdeSimulacion && numeroSimulacionActual < verHastaSimulacion) || numeroSimulacionActual == cantidadDeSimulaciones)
            {
                grdSimulacion.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);
            }

            // Determinar el próximo evento
            if (cantidadDeSimulaciones >= numeroSimulacionActual)
            {
                string proxEvento = obtenerProximoEvento(horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6,
    proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor);

                if (proxEvento == "LL Auto")
                {
                    llegadaAuto(filaActual);
                }
                else if (proxEvento == "Fin AP")
                {
                    finAtencionParking(filaActual);
                }
                else if (proxEvento == "Fin AE")
                {
                    finAtencionEntrada(filaActual);
                }
                else if (proxEvento == "Fin AC")
                {
                    finAtencionComida(filaActual);
                }
                else if (proxEvento == "Fin ACM")
                {
                    finAtencionComidaMayores(filaActual);
                }
                else if (proxEvento == "Detención")
                {
                    detencion(filaActual);
                }
                else if (proxEvento == "Reanudación Llegadas")
                {
                    reanudacionLlegadas(filaActual);
                }
                else if (proxEvento == "Reanudación Servidor")
                {
                    reanudacionServidor(filaActual);
                }
                else
                {
                    throw new Exception("No se encontró el evento siguiente desde Evento Inicial.");
                }
            }
        }

        public void llegadaAuto(DataRow filaAnterior) // Actualizado
        {
            // Marcar número de simulación 
            numeroSimulacionActual++;


            // Nombres de las variables (una por cada columna)
            string evento;
            decimal? reloj;

            decimal? rndTipoDetencion;
            string tipoDetencion;
            decimal? beta;
            decimal? tiempoDeDetencion;
            decimal? horaDetencion;

            decimal? rndLlegada;
            decimal? tiempoLlegada;
            decimal? proximaLlegada;

            decimal? l;
            decimal? tiempoDuracionDetencionLlegadas;
            decimal? horaReanudacionLlegadas;

            decimal? rndFinAP;
            decimal? tiempoFinAP;
            decimal? proximoFinAP1;
            decimal? proximoFinAP2;
            decimal? proximoFinAP3;
            decimal? proximoFinAP4;
            decimal? proximoFinAP5;

            decimal? rndFinAE;
            decimal? tiempoFinAE;
            decimal? proximoFinAE1;
            decimal? proximoFinAE2;
            decimal? proximoFinAE3;
            decimal? proximoFinAE4;
            decimal? proximoFinAE5;
            decimal? proximoFinAE6;
            decimal? rndCantidadPersonas;
            int? cantidadPersonas;
            decimal? rndCantidadPersonasMayores;
            int? cantidadPersonasMayores;
            int? cantidadPersonasNoMayores;

            decimal? rndFinAC1;
            decimal? tiempoFinAC1;
            decimal? proximoFinAC1;
            decimal? rndFinAC2;
            decimal? tiempoFinAC2;
            decimal? proximoFinAC2;
            decimal? rndFinAC3;
            decimal? tiempoFinAC3;
            decimal? proximoFinAC3;
            decimal? rndFinAC4;
            decimal? tiempoFinAC4;
            decimal? proximoFinAC4;

            decimal? rndFinACM;
            decimal? tiempoFinACM;
            decimal? proximoFinACM;

            decimal? s;
            decimal? tiempoDuracionDetencionServidor;
            decimal? horaReanudacionServidor;

            string ingresoBloqueado;
            int? colaBloqueoLlegadas;

            int? colaPark1;
            string estadoCajaPark1;
            int? colaPark2;
            string estadoCajaPark2;
            int? colaPark3;
            string estadoCajaPark3;
            int? colaPark4;
            string estadoCajaPark4;
            int? colaPark5;
            string estadoCajaPark5;

            int? colaEntrada1y2;
            string estadoCajaEntrada1;
            string estadoCajaEntrada2;
            int? colaEntrada3y4;
            string estadoCajaEntrada3;
            string estadoCajaEntrada4;
            int? colaEntrada5y6;
            string estadoCajaEntrada5;
            string estadoCajaEntrada6;

            int? colaComida1;
            string estadoControlComida1;
            int? colaComida2;
            string estadoControlComida2;
            int? colaComida3;
            string estadoControlComida3;
            int? colaComida4;
            string estadoControlComida4;

            int? colaComidaMayores;
            string estadoControlComidaMayores;

            decimal? metrosPromedioNecesariosParaAparcamiento;
            decimal? acumuladorTiempoColaParking;
            decimal? cantidadPromedioAutosEnColaPark;
            int? contadorGruposCajaEntrada;
            decimal? acumuladorTiempoColaEntrada;
            decimal? tiempoPromedioEnColaEntrada;
            int? contadorPersonasEnControlComida;
            decimal? acumuladorTiempoColaComida;
            decimal? tiempoPromedioEnColaComida;
            decimal? tiempoEnConseguirEntrada;
            decimal? cantidadPromedioGenteEnColaEntrada;
            decimal? tiempoEntradaDespuesDeEstacionar;


            // Evento
            evento = "LL Auto";


            // Reloj
            reloj = obtenerHoraProxEvento(aDecimal(filaAnterior["horaDetencion"]), aDecimal(filaAnterior["proximaLlegada"]), aDecimal(filaAnterior["horaReanudacionLlegadas"]), 
                    aDecimal(filaAnterior["proximoFinAP1"]), aDecimal(filaAnterior["proximoFinAP2"]), aDecimal(filaAnterior["proximoFinAP3"]),
                    aDecimal(filaAnterior["proximoFinAP4"]), aDecimal(filaAnterior["proximoFinAP5"]), aDecimal(filaAnterior["proximoFinAE1"]),
                    aDecimal(filaAnterior["proximoFinAE2"]), aDecimal(filaAnterior["proximoFinAE3"]), aDecimal(filaAnterior["proximoFinAE4"]), 
                    aDecimal(filaAnterior["proximoFinAE5"]), aDecimal(filaAnterior["proximoFinAE6"]), aDecimal(filaAnterior["proximoFinAC1"]), 
                    aDecimal(filaAnterior["proximoFinAC2"]), aDecimal(filaAnterior["proximoFinAC3"]), aDecimal(filaAnterior["proximoFinAC4"]), 
                    aDecimal(filaAnterior["proximoFinACM"]),aDecimal(filaAnterior["horaReanudacionServidor"]));


            // Detención
            rndTipoDetencion = null;
            
            tipoDetencion = Convert.ToString(filaAnterior["tipoDetencion"]);
            
            beta = null;
            
            tiempoDeDetencion = null;
            
            horaDetencion = aDecimal(filaAnterior["horaDetencion"]);


            // Llegada auto
            rndLlegada = generarRandom();

            tiempoLlegada = generarTiempoExponencial(rndLlegada, mediaLlegada);

            proximaLlegada = generarProximo(reloj, tiempoLlegada);


            // Reanudación llegadas
            l = null;
            
            tiempoDuracionDetencionLlegadas = null;
            
            horaReanudacionLlegadas = aDecimal(filaAnterior["horaReanudacionLlegadas"]);


            // Fin atención parking y Caja Park
            ingresoBloqueado = Convert.ToString(filaAnterior["ingresoBloqueado"]);
            colaBloqueoLlegadas = Convert.ToInt32(filaAnterior["colaBloqueoLlegadas"]);

            if (ingresoBloqueado == "No")
            {
                rndFinAP = generarRandom();

                tiempoFinAP = generarTiempoExponencial(rndFinAP, mediaAP);

                proximoFinAP1 = aDecimal(filaAnterior["proximoFinAP1"]);
                proximoFinAP2 = aDecimal(filaAnterior["proximoFinAP2"]);
                proximoFinAP3 = aDecimal(filaAnterior["proximoFinAP3"]);
                proximoFinAP4 = aDecimal(filaAnterior["proximoFinAP4"]);
                proximoFinAP5 = aDecimal(filaAnterior["proximoFinAP5"]);

                colaPark1 = Convert.ToInt32(filaAnterior["colaPark1"]);
                estadoCajaPark1 = filaAnterior["estadoCajaPark1"].ToString();
                colaPark2 = Convert.ToInt32(filaAnterior["colaPark2"]);
                estadoCajaPark2 = filaAnterior["estadoCajaPark2"].ToString();
                colaPark3 = Convert.ToInt32(filaAnterior["colaPark3"]);
                estadoCajaPark3 = filaAnterior["estadoCajaPark3"].ToString();
                colaPark4 = Convert.ToInt32(filaAnterior["colaPark4"]);
                estadoCajaPark4 = filaAnterior["estadoCajaPark4"].ToString();
                colaPark5 = Convert.ToInt32(filaAnterior["colaPark5"]);
                estadoCajaPark5 = filaAnterior["estadoCajaPark5"].ToString();

                if (estadoCajaPark1 == "Libre")
                {
                    proximoFinAP1 = generarProximo(reloj, tiempoFinAP);
                    estadoCajaPark1 = "Ocupado";
                    grdAutos.Rows.Add("SiendoAt1");
                }
                else if (estadoCajaPark2 == "Libre")
                {
                    proximoFinAP2 = generarProximo(reloj, tiempoFinAP);
                    estadoCajaPark2 = "Ocupado";
                    grdAutos.Rows.Add("SiendoAt2");
                }
                else if (estadoCajaPark3 == "Libre")
                {
                    proximoFinAP3 = generarProximo(reloj, tiempoFinAP);
                    estadoCajaPark3 = "Ocupado";
                    grdAutos.Rows.Add("SiendoAt3");
                }
                else if (estadoCajaPark4 == "Libre")
                {
                    proximoFinAP4 = generarProximo(reloj, tiempoFinAP);
                    estadoCajaPark4 = "Ocupado";
                    grdAutos.Rows.Add("SiendoAt4");
                }
                else if (estadoCajaPark5 == "Libre")
                {
                    proximoFinAP5 = generarProximo(reloj, tiempoFinAP);
                    estadoCajaPark5 = "Ocupado";
                    grdAutos.Rows.Add("SiendoAt5");
                }
                else
                {
                    rndFinAP = null;
                    tiempoFinAP = null;

                    int? colaMasChica = obtenerColaMenorParking(colaPark1, colaPark2, colaPark3, colaPark4, colaPark5);

                    if (colaMasChica == colaPark1)
                    {
                        colaPark1++;
                        grdAutos.Rows.Add("EnColaPark1");
                    }
                    else if (colaMasChica == colaPark2)
                    {
                        colaPark2++;
                        grdAutos.Rows.Add("EnColaPark2");
                    }
                    else if (colaMasChica == colaPark3)
                    {
                        colaPark3++;
                        grdAutos.Rows.Add("EnColaPark3");
                    }
                    else if (colaMasChica == colaPark4)
                    {
                        colaPark4++;
                        grdAutos.Rows.Add("EnColaPark4");
                    }
                    if (colaMasChica == colaPark5)
                    {
                        colaPark5++;
                        grdAutos.Rows.Add("EnColaPark5");
                    }
                }
            }
            else
            {
                colaBloqueoLlegadas++;
                
                rndFinAP = null;
                tiempoFinAP = null;

                proximoFinAP1 = aDecimal(filaAnterior["proximoFinAP1"]);
                proximoFinAP2 = aDecimal(filaAnterior["proximoFinAP2"]);
                proximoFinAP3 = aDecimal(filaAnterior["proximoFinAP3"]);
                proximoFinAP4 = aDecimal(filaAnterior["proximoFinAP4"]);
                proximoFinAP5 = aDecimal(filaAnterior["proximoFinAP5"]);

                colaPark1 = Convert.ToInt32(filaAnterior["colaPark1"]);
                estadoCajaPark1 = filaAnterior["estadoCajaPark1"].ToString();
                colaPark2 = Convert.ToInt32(filaAnterior["colaPark2"]);
                estadoCajaPark2 = filaAnterior["estadoCajaPark2"].ToString();
                colaPark3 = Convert.ToInt32(filaAnterior["colaPark3"]);
                estadoCajaPark3 = filaAnterior["estadoCajaPark3"].ToString();
                colaPark4 = Convert.ToInt32(filaAnterior["colaPark4"]);
                estadoCajaPark4 = filaAnterior["estadoCajaPark4"].ToString();
                colaPark5 = Convert.ToInt32(filaAnterior["colaPark5"]);
                estadoCajaPark5 = filaAnterior["estadoCajaPark5"].ToString();

                grdAutos.Rows.Add("EnColaBloqueo");
            }


            // Fin atención entrada
            rndFinAE = null;

            tiempoFinAE = null;

            proximoFinAE1 = aDecimal(filaAnterior["proximoFinAE1"]);
            proximoFinAE2 = aDecimal(filaAnterior["proximoFinAE2"]);
            proximoFinAE3 = aDecimal(filaAnterior["proximoFinAE3"]);
            proximoFinAE4 = aDecimal(filaAnterior["proximoFinAE4"]);
            proximoFinAE5 = aDecimal(filaAnterior["proximoFinAE5"]);
            proximoFinAE6 = aDecimal(filaAnterior["proximoFinAE6"]);

            rndCantidadPersonas = null;

            cantidadPersonas = null;

            rndCantidadPersonasMayores = null;

            cantidadPersonasMayores = null;

            cantidadPersonasNoMayores = null;


            // Fin atención control comida
            rndFinAC1 = null;
            tiempoFinAC1 = null;
            proximoFinAC1 = aDecimal(filaAnterior["proximoFinAC1"]);

            rndFinAC2 = null;
            tiempoFinAC2 = null;
            proximoFinAC2 = aDecimal(filaAnterior["proximoFinAC2"]);

            rndFinAC3 = null;
            tiempoFinAC3 = null;
            proximoFinAC3 = aDecimal(filaAnterior["proximoFinAC3"]);

            rndFinAC4 = null;
            tiempoFinAC4 = null;
            proximoFinAC4 = aDecimal(filaAnterior["proximoFinAC4"]);


            // Fin atención control comida mayores
            rndFinACM = null;
            tiempoFinACM = null;
            proximoFinACM = aDecimal(filaAnterior["proximoFinACM"]);


            // Reanudacion servidor
            s = null;
            
            tiempoDuracionDetencionServidor = null;

            horaReanudacionServidor = aDecimal(filaAnterior["horaReanudacionServidor"]);


            // Caja entrada
            colaEntrada1y2 = Convert.ToInt32(filaAnterior["colaEntrada1y2"]);
            estadoCajaEntrada1 = filaAnterior["estadoCajaEntrada1"].ToString();
            estadoCajaEntrada2 = filaAnterior["estadoCajaEntrada2"].ToString();

            colaEntrada3y4 = Convert.ToInt32(filaAnterior["colaEntrada3y4"]);
            estadoCajaEntrada3 = filaAnterior["estadoCajaEntrada3"].ToString();
            estadoCajaEntrada4 = filaAnterior["estadoCajaEntrada4"].ToString();

            colaEntrada5y6 = Convert.ToInt32(filaAnterior["colaEntrada5y6"]);
            estadoCajaEntrada5 = filaAnterior["estadoCajaEntrada5"].ToString();
            estadoCajaEntrada6 = filaAnterior["estadoCajaEntrada6"].ToString();


            // Control comida
            colaComida1 = Convert.ToInt32(filaAnterior["colaComida1"]);
            estadoControlComida1 = filaAnterior["estadoControlComida1"].ToString();

            colaComida2 = Convert.ToInt32(filaAnterior["colaComida2"]);
            estadoControlComida2 = filaAnterior["estadoControlComida2"].ToString();

            colaComida3 = Convert.ToInt32(filaAnterior["colaComida3"]);
            estadoControlComida3 = filaAnterior["estadoControlComida3"].ToString();

            colaComida4 = Convert.ToInt32(filaAnterior["colaComida4"]);
            estadoControlComida4 = filaAnterior["estadoControlComida4"].ToString();


            // Control comida mayores
            colaComidaMayores = Convert.ToInt32(filaAnterior["colaComidaMayores"]);
            estadoControlComidaMayores = filaAnterior["estadoControlComidaMayores"].ToString();
           

            // Estadísticas
            metrosPromedioNecesariosParaAparcamiento = aDecimal(filaAnterior["cantidadPromedioAutosEnColaPark"]) * 4;
            acumuladorTiempoColaParking = (reloj - aDecimal(filaAnterior["reloj"])) * (colaPark1 + colaPark2 + colaPark3 + colaPark4 + colaPark5) + aDecimal(filaAnterior["acumuladorTiempoColaParking"]);
            cantidadPromedioAutosEnColaPark = acumuladorTiempoColaParking / reloj;

            contadorGruposCajaEntrada = Convert.ToInt32(filaAnterior["contadorGruposCajaEntrada"]);
            acumuladorTiempoColaEntrada = (reloj - aDecimal(filaAnterior["reloj"])) * (colaEntrada1y2 + colaEntrada3y4 + colaEntrada5y6) + aDecimal(filaAnterior["acumuladorTiempoColaEntrada"]);
            if (contadorGruposCajaEntrada == 0)
            {
                tiempoPromedioEnColaEntrada = 0;
            }
            else
            {
                tiempoPromedioEnColaEntrada = acumuladorTiempoColaEntrada / contadorGruposCajaEntrada;
            }

            contadorPersonasEnControlComida = Convert.ToInt32(filaAnterior["contadorPersonasEnControlComida"]);
            acumuladorTiempoColaComida = (reloj - aDecimal(filaAnterior["reloj"])) * (colaComida1 + colaComida2 + colaComida3 + colaComida4 + colaComidaMayores) + aDecimal(filaAnterior["acumuladorTiempoColaComida"]);
            if (contadorPersonasEnControlComida == 0)
            {
                tiempoPromedioEnColaComida = 0;
            }
            else
            {
                tiempoPromedioEnColaComida = acumuladorTiempoColaComida / contadorPersonasEnControlComida;
            }

            tiempoEnConseguirEntrada = 300 + tiempoPromedioEnColaEntrada + 92;
            cantidadPromedioGenteEnColaEntrada = (acumuladorTiempoColaEntrada / reloj) * (decimal)4.16;
            tiempoEntradaDespuesDeEstacionar = tiempoEnConseguirEntrada + tiempoPromedioEnColaComida + 5;



            // --- Manejo de tabla y próximo evento ---

            // Eliminar fila anterior
            dt.Rows.Remove(filaAnterior);

            // Agregar la nueva fila
            dt.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);

            // Guardar la nueva fila en una variable para enviársela al próximo evento
            DataRow filaActual = dt.Rows[0];

            // Si la simulación está dentro de las que quiere visualizar, agregarla a la grilla
            if ((numeroSimulacionActual >= verDesdeSimulacion && numeroSimulacionActual < verHastaSimulacion) || numeroSimulacionActual == cantidadDeSimulaciones)
            {
                grdSimulacion.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);
            }

            // Determinar el próximo evento
            if (cantidadDeSimulaciones >= numeroSimulacionActual)
            {
                string proxEvento = obtenerProximoEvento(horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6,
    proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor);

                if (proxEvento == "LL Auto")
                {
                    llegadaAuto(filaActual);
                }
                else if (proxEvento == "Fin AP")
                {
                    finAtencionParking(filaActual);
                }
                else if (proxEvento == "Fin AE")
                {
                    finAtencionEntrada(filaActual);
                }
                else if (proxEvento == "Fin AC")
                {
                    finAtencionComida(filaActual);
                }
                else if (proxEvento == "Fin ACM")
                {
                    finAtencionComidaMayores(filaActual);
                }
                else if (proxEvento == "Detención")
                {
                    detencion(filaActual);
                }
                else if (proxEvento == "Reanudación Llegadas")
                {
                    reanudacionLlegadas(filaActual);
                }
                else if (proxEvento == "Reanudación Servidor")
                {
                    reanudacionServidor(filaActual);
                }
                else
                {
                    throw new Exception("No se encontró el evento siguiente desde Ll Auto.");
                }
            }
        }

        public void finAtencionParking(DataRow filaAnterior) // Actualizado
        {
            // Marcar número de simulación
            numeroSimulacionActual++;


            // Nombres de las variables (una por cada columna)
            string evento;
            decimal? reloj;

            decimal? rndTipoDetencion;
            string tipoDetencion;
            decimal? beta;
            decimal? tiempoDeDetencion;
            decimal? horaDetencion;

            decimal? rndLlegada;
            decimal? tiempoLlegada;
            decimal? proximaLlegada;

            decimal? l;
            decimal? tiempoDuracionDetencionLlegadas;
            decimal? horaReanudacionLlegadas;

            decimal? rndFinAP;
            decimal? tiempoFinAP;
            decimal? proximoFinAP1;
            decimal? proximoFinAP2;
            decimal? proximoFinAP3;
            decimal? proximoFinAP4;
            decimal? proximoFinAP5;

            decimal? rndFinAE;
            decimal? tiempoFinAE;
            decimal? proximoFinAE1;
            decimal? proximoFinAE2;
            decimal? proximoFinAE3;
            decimal? proximoFinAE4;
            decimal? proximoFinAE5;
            decimal? proximoFinAE6;
            decimal? rndCantidadPersonas;
            int? cantidadPersonas;
            decimal? rndCantidadPersonasMayores;
            int? cantidadPersonasMayores;
            int? cantidadPersonasNoMayores;

            decimal? rndFinAC1;
            decimal? tiempoFinAC1;
            decimal? proximoFinAC1;
            decimal? rndFinAC2;
            decimal? tiempoFinAC2;
            decimal? proximoFinAC2;
            decimal? rndFinAC3;
            decimal? tiempoFinAC3;
            decimal? proximoFinAC3;
            decimal? rndFinAC4;
            decimal? tiempoFinAC4;
            decimal? proximoFinAC4;

            decimal? rndFinACM;
            decimal? tiempoFinACM;
            decimal? proximoFinACM;

            decimal? s;
            decimal? tiempoDuracionDetencionServidor;
            decimal? horaReanudacionServidor;

            string ingresoBloqueado;
            int? colaBloqueoLlegadas;

            int? colaPark1;
            string estadoCajaPark1;
            int? colaPark2;
            string estadoCajaPark2;
            int? colaPark3;
            string estadoCajaPark3;
            int? colaPark4;
            string estadoCajaPark4;
            int? colaPark5;
            string estadoCajaPark5;

            int? colaEntrada1y2;
            string estadoCajaEntrada1;
            string estadoCajaEntrada2;
            int? colaEntrada3y4;
            string estadoCajaEntrada3;
            string estadoCajaEntrada4;
            int? colaEntrada5y6;
            string estadoCajaEntrada5;
            string estadoCajaEntrada6;

            int? colaComida1;
            string estadoControlComida1;
            int? colaComida2;
            string estadoControlComida2;
            int? colaComida3;
            string estadoControlComida3;
            int? colaComida4;
            string estadoControlComida4;

            int? colaComidaMayores;
            string estadoControlComidaMayores;

            decimal? metrosPromedioNecesariosParaAparcamiento;
            decimal? acumuladorTiempoColaParking;
            decimal? cantidadPromedioAutosEnColaPark;
            int? contadorGruposCajaEntrada;
            decimal? acumuladorTiempoColaEntrada;
            decimal? tiempoPromedioEnColaEntrada;
            int? contadorPersonasEnControlComida;
            decimal? acumuladorTiempoColaComida;
            decimal? tiempoPromedioEnColaComida;
            decimal? tiempoEnConseguirEntrada;
            decimal? cantidadPromedioGenteEnColaEntrada;
            decimal? tiempoEntradaDespuesDeEstacionar;


            // Evento
            evento = "Fin AP";


            // Reloj
            reloj = obtenerHoraProxEvento(aDecimal(filaAnterior["horaDetencion"]), aDecimal(filaAnterior["proximaLlegada"]), aDecimal(filaAnterior["horaReanudacionLlegadas"]),
                    aDecimal(filaAnterior["proximoFinAP1"]), aDecimal(filaAnterior["proximoFinAP2"]), aDecimal(filaAnterior["proximoFinAP3"]),
                    aDecimal(filaAnterior["proximoFinAP4"]), aDecimal(filaAnterior["proximoFinAP5"]), aDecimal(filaAnterior["proximoFinAE1"]),
                    aDecimal(filaAnterior["proximoFinAE2"]), aDecimal(filaAnterior["proximoFinAE3"]), aDecimal(filaAnterior["proximoFinAE4"]),
                    aDecimal(filaAnterior["proximoFinAE5"]), aDecimal(filaAnterior["proximoFinAE6"]), aDecimal(filaAnterior["proximoFinAC1"]),
                    aDecimal(filaAnterior["proximoFinAC2"]), aDecimal(filaAnterior["proximoFinAC3"]), aDecimal(filaAnterior["proximoFinAC4"]),
                    aDecimal(filaAnterior["proximoFinACM"]), aDecimal(filaAnterior["horaReanudacionServidor"]));


            // Detención
            rndTipoDetencion = null;

            tipoDetencion = filaAnterior["tipoDetencion"].ToString();

            beta = null;

            tiempoDeDetencion = null;

            horaDetencion = aDecimal(filaAnterior["horaDetencion"]);


            // Llegada auto
            rndLlegada = null;

            tiempoLlegada = null;

            proximaLlegada = aDecimal(filaAnterior["proximaLlegada"]);


            // Reanudación llegadas
            l = null;

            tiempoDuracionDetencionLlegadas = null;

            horaReanudacionLlegadas = aDecimal(filaAnterior["horaReanudacionLlegadas"]);
            

            // Fin atención parking y Caja park
            rndFinAP = generarRandom();
            tiempoFinAP = generarTiempoExponencial(rndFinAP, mediaAP);

            proximoFinAP1 = aDecimal(filaAnterior["proximoFinAP1"]);
            proximoFinAP2 = aDecimal(filaAnterior["proximoFinAP2"]);
            proximoFinAP3 = aDecimal(filaAnterior["proximoFinAP3"]);
            proximoFinAP4 = aDecimal(filaAnterior["proximoFinAP4"]);
            proximoFinAP5 = aDecimal(filaAnterior["proximoFinAP5"]);

            colaPark1 = Convert.ToInt32(filaAnterior["colaPark1"]);
            estadoCajaPark1 = filaAnterior["estadoCajaPark1"].ToString();
            colaPark2 = Convert.ToInt32(filaAnterior["colaPark2"]);
            estadoCajaPark2 = filaAnterior["estadoCajaPark2"].ToString();
            colaPark3 = Convert.ToInt32(filaAnterior["colaPark3"]);
            estadoCajaPark3 = filaAnterior["estadoCajaPark3"].ToString();
            colaPark4 = Convert.ToInt32(filaAnterior["colaPark4"]);
            estadoCajaPark4 = filaAnterior["estadoCajaPark4"].ToString();
            colaPark5 = Convert.ToInt32(filaAnterior["colaPark5"]);
            estadoCajaPark5 = filaAnterior["estadoCajaPark5"].ToString();

            // Este índice se utiliza para encontrar el nuestra list el auto que sale del parking
            int indiceEncontrado = -1;

            if (proximoFinAP1 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaPark1"]) > 0)
                {
                    proximoFinAP1 = generarProximo(reloj, tiempoFinAP);
                    colaPark1--;

                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt1")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    if (indiceEncontrado != -1)
                    {
                        grdAutos.Rows.RemoveAt(indiceEncontrado);
                    }


                    // Recorro y cambio el estado de EnCola a SiendoAt
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaPark1")
                        {
                            fila.Cells[0].Value = "SiendoAt1";
                            break;
                        }
                    }
                }

                else
                {
                    proximoFinAP1 = null;
                    estadoCajaPark1 = "Libre";

                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt1")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdAutos.Rows.RemoveAt(indiceEncontrado);

                }
            }
            else if (proximoFinAP2 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaPark2"]) > 0)
                {
                    proximoFinAP2 = generarProximo(reloj, tiempoFinAP);
                    colaPark2--;

                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt2")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    if (indiceEncontrado != -1)
                    {
                        grdAutos.Rows.RemoveAt(indiceEncontrado);
                    }

                    // Recorro y cambio el estado de EnCola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaPark2")
                        {
                            fila.Cells[0].Value = "SiendoAt2";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAP2 = null;
                    estadoCajaPark2 = "Libre";
                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt2")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdAutos.Rows.RemoveAt(indiceEncontrado);
                }
            }
            else if (proximoFinAP3 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaPark3"]) > 0)
                {
                    proximoFinAP3 = generarProximo(reloj, tiempoFinAP);
                    colaPark3--;
                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt3")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdAutos.Rows.RemoveAt(indiceEncontrado);

                    // Recorro y cambio el estado de EnCola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaPark3")
                        {
                            fila.Cells[0].Value = "SiendoAt3";
                            break;
                        }
                    }

                }
                else
                {
                    proximoFinAP3 = null;
                    estadoCajaPark3 = "Libre";

                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt3")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdAutos.Rows.RemoveAt(indiceEncontrado);


                }
            }
            else if (proximoFinAP4 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaPark4"]) > 0)
                {
                    proximoFinAP4 = generarProximo(reloj, tiempoFinAP);
                    colaPark4--;

                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt4")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdAutos.Rows.RemoveAt(indiceEncontrado);

                    // Recorro y cambio el estado de EnCola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaPark4")
                        {
                            fila.Cells[0].Value = "SiendoAt4";
                            break;
                        }
                    }
                }

                else
                {
                    proximoFinAP4 = null;
                    estadoCajaPark4 = "Libre";

                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt4")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdAutos.Rows.RemoveAt(indiceEncontrado);
                }
            }
            else if (proximoFinAP5 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaPark5"]) > 0)
                {
                    proximoFinAP5 = generarProximo(reloj, tiempoFinAP);
                    colaPark5--;

                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt5")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdAutos.Rows.RemoveAt(indiceEncontrado);

                    // Recorro y cambio el estado de EnCola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaPark5")
                        {
                            fila.Cells[0].Value = "SiendoAt5";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAP5 = null;
                    estadoCajaPark5 = "Libre";

                    // Recorremos la grid para encontrar el índice del auto
                    for (int i = 0; i < grdAutos.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdAutos.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt5")
                        {
                            indiceEncontrado = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdAutos.Rows.RemoveAt(indiceEncontrado);
                }
            }


            // Fin atención entrada
            rndFinAE = generarRandom();

            tiempoFinAE = generarTiempoExponencial(rndFinAE, mediaAE);

            proximoFinAE1 = aDecimal(filaAnterior["proximoFinAE1"]);
            proximoFinAE2 = aDecimal(filaAnterior["proximoFinAE2"]);
            proximoFinAE3 = aDecimal(filaAnterior["proximoFinAE3"]);
            proximoFinAE4 = aDecimal(filaAnterior["proximoFinAE4"]);
            proximoFinAE5 = aDecimal(filaAnterior["proximoFinAE5"]);
            proximoFinAE6 = aDecimal(filaAnterior["proximoFinAE6"]);

            rndCantidadPersonas = null;

            cantidadPersonas = null;

            rndCantidadPersonasMayores = null;

            cantidadPersonasMayores = null;

            cantidadPersonasNoMayores = null;


            // Fin atención control comida
            rndFinAC1 = null;
            tiempoFinAC1 = null;
            proximoFinAC1 = aDecimal(filaAnterior["proximoFinAC1"]);

            rndFinAC2 = null;
            tiempoFinAC2 = null;
            proximoFinAC2 = aDecimal(filaAnterior["proximoFinAC2"]); ;

            rndFinAC3 = null;
            tiempoFinAC3 = null;
            proximoFinAC3 = aDecimal(filaAnterior["proximoFinAC3"]); ;

            rndFinAC4 = null;
            tiempoFinAC4 = null;
            proximoFinAC4 = aDecimal(filaAnterior["proximoFinAC4"]); ;


            // Fin atención control comida mayores
            rndFinACM = null;
            tiempoFinACM = null;
            proximoFinACM = aDecimal(filaAnterior["proximoFinACM"]); ;


            // Reanudación servidor
            s = null;

            tiempoDuracionDetencionServidor = null;

            horaReanudacionServidor = aDecimal(filaAnterior["horaReanudacionServidor"]);


            // Bloqueo de llegadas
            ingresoBloqueado = filaAnterior["ingresoBloqueado"].ToString();

            colaBloqueoLlegadas = Convert.ToInt32(filaAnterior["colaBloqueoLlegadas"]);


            // Caja entrada
            colaEntrada1y2 = Convert.ToInt32(filaAnterior["colaEntrada1y2"]);
            estadoCajaEntrada1 = filaAnterior["estadoCajaEntrada1"].ToString();
            estadoCajaEntrada2 = filaAnterior["estadoCajaEntrada2"].ToString();

            colaEntrada3y4 = Convert.ToInt32(filaAnterior["colaEntrada3y4"]);
            estadoCajaEntrada3 = filaAnterior["estadoCajaEntrada3"].ToString();
            estadoCajaEntrada4 = filaAnterior["estadoCajaEntrada4"].ToString();

            colaEntrada5y6 = Convert.ToInt32(filaAnterior["colaEntrada5y6"]);
            estadoCajaEntrada5 = filaAnterior["estadoCajaEntrada5"].ToString();
            estadoCajaEntrada6 = filaAnterior["estadoCajaEntrada6"].ToString();


            if (estadoCajaEntrada1 == "Libre")
            {
                proximoFinAE1 = generarProximo(reloj, tiempoFinAE);
                grdGrupos.Rows.Add("SiendoAt1");
                estadoCajaEntrada1 = "Ocupado";
            }
            else if (estadoCajaEntrada2 == "Libre")
            {
                proximoFinAE2 = generarProximo(reloj, tiempoFinAE);
                grdGrupos.Rows.Add("SiendoAt2");
                estadoCajaEntrada2 = "Ocupado";
            }
            else if (estadoCajaEntrada3 == "Libre")
            {
                proximoFinAE3 = generarProximo(reloj, tiempoFinAE);
                grdGrupos.Rows.Add("SiendoAt3");
                estadoCajaEntrada3 = "Ocupado";
            }
            else if (estadoCajaEntrada4 == "Libre")
            {
                proximoFinAE4 = generarProximo(reloj, tiempoFinAE);
                grdGrupos.Rows.Add("SiendoAt4");
                estadoCajaEntrada4 = "Ocupado";
            }
            else if (estadoCajaEntrada5 == "Libre")
            {
                proximoFinAE5 = generarProximo(reloj, tiempoFinAE);
                grdGrupos.Rows.Add("SiendoAt5");
                estadoCajaEntrada5 = "Ocupado";
            }
            else if (estadoCajaEntrada6 == "Libre")
            {
                proximoFinAE6 = generarProximo(reloj, tiempoFinAE);
                grdGrupos.Rows.Add("SiendoAt6");
                estadoCajaEntrada6 = "Ocupado";
            }
            else
            {
                int? colaMasChica = obtenerColaMenorEntrada(colaEntrada1y2, colaEntrada3y4, colaEntrada5y6);

                if (colaMasChica == colaEntrada1y2)
                {
                    colaEntrada1y2++;
                    grdGrupos.Rows.Add("EnColaEntrada1y2");
                }
                else if (colaMasChica == colaEntrada3y4)
                {
                    colaEntrada3y4++;
                    grdGrupos.Rows.Add("EnColaEntrada3y4");
                }
                else if (colaMasChica == colaEntrada5y6)
                {
                    colaEntrada5y6++;
                    grdGrupos.Rows.Add("EnColaEntrada5y6");
                }
            }


            // Control comida
            colaComida1 = Convert.ToInt32(filaAnterior["colaComida1"]);
            estadoControlComida1 = filaAnterior["estadoControlComida1"].ToString();

            colaComida2 = Convert.ToInt32(filaAnterior["colaComida2"]);
            estadoControlComida2 = filaAnterior["estadoControlComida2"].ToString();

            colaComida3 = Convert.ToInt32(filaAnterior["colaComida3"]);
            estadoControlComida3 = filaAnterior["estadoControlComida3"].ToString();

            colaComida4 = Convert.ToInt32(filaAnterior["colaComida4"]);
            estadoControlComida4 = filaAnterior["estadoControlComida4"].ToString();


            // Control comida mayores
            colaComidaMayores = Convert.ToInt32(filaAnterior["colaComidaMayores"]);
            estadoControlComidaMayores = filaAnterior["estadoControlComidaMayores"].ToString();


            // Estadísticas
            metrosPromedioNecesariosParaAparcamiento = aDecimal(filaAnterior["cantidadPromedioAutosEnColaPark"]) * 4;
            acumuladorTiempoColaParking = (reloj - aDecimal(filaAnterior["reloj"])) * (colaPark1 + colaPark2 + colaPark3 + colaPark4 + colaPark5) + aDecimal(filaAnterior["acumuladorTiempoColaParking"]);
            cantidadPromedioAutosEnColaPark = acumuladorTiempoColaParking / reloj;

            contadorGruposCajaEntrada = Convert.ToInt32(filaAnterior["contadorGruposCajaEntrada"]);
            acumuladorTiempoColaEntrada = (reloj - aDecimal(filaAnterior["reloj"])) * (colaEntrada1y2 + colaEntrada3y4 + colaEntrada5y6) + aDecimal(filaAnterior["acumuladorTiempoColaEntrada"]);
            if (contadorGruposCajaEntrada == 0)
            {
                tiempoPromedioEnColaEntrada = 0;
            }
            else
            {
                tiempoPromedioEnColaEntrada = acumuladorTiempoColaEntrada / contadorGruposCajaEntrada;
            }

            contadorPersonasEnControlComida = Convert.ToInt32(filaAnterior["contadorPersonasEnControlComida"]);
            acumuladorTiempoColaComida = (reloj - aDecimal(filaAnterior["reloj"])) * (colaComida1 + colaComida2 + colaComida3 + colaComida4 + colaComidaMayores) + aDecimal(filaAnterior["acumuladorTiempoColaComida"]);
            if (contadorPersonasEnControlComida == 0)
            {
                tiempoPromedioEnColaComida = 0;
            }
            else
            {
                tiempoPromedioEnColaComida = acumuladorTiempoColaComida / contadorPersonasEnControlComida;
            }

            tiempoEnConseguirEntrada = 300 + tiempoPromedioEnColaEntrada + 92;
            cantidadPromedioGenteEnColaEntrada = (acumuladorTiempoColaEntrada / reloj) * (decimal)4.16;
            tiempoEntradaDespuesDeEstacionar = tiempoEnConseguirEntrada + tiempoPromedioEnColaComida + 5;



            // --- Manejo de tabla y próximo evento ---

            // Eliminar fila anterior
            dt.Rows.Remove(filaAnterior);

            // Agregar la nueva fila
            dt.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);

            // Guardar la nueva fila en una variable para enviársela al próximo evento
            DataRow filaActual = dt.Rows[0];

            // Si la simulación está dentro de las que quiere visualizar, agregarla a la grilla
            if ((numeroSimulacionActual >= verDesdeSimulacion && numeroSimulacionActual < verHastaSimulacion) || numeroSimulacionActual == cantidadDeSimulaciones)
            {
                grdSimulacion.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);
            }

            // Determinar el próximo evento
            if (cantidadDeSimulaciones >= numeroSimulacionActual)
            {
                string proxEvento = obtenerProximoEvento(horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6,
                proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor);

                if (proxEvento == "LL Auto")
                {
                    llegadaAuto(filaActual);
                }
                else if (proxEvento == "Fin AP")
                {
                    finAtencionParking(filaActual);
                }
                else if (proxEvento == "Fin AE")
                {
                    finAtencionEntrada(filaActual);
                }
                else if (proxEvento == "Fin AC")
                {
                    finAtencionComida(filaActual);
                }
                else if (proxEvento == "Fin ACM")
                {
                    finAtencionComidaMayores(filaActual);
                }
                else if (proxEvento == "Detención")
                {
                    detencion(filaActual);
                }
                else if (proxEvento == "Reanudación Llegadas")
                {
                    reanudacionLlegadas(filaActual);
                }
                else if (proxEvento == "Reanudación Servidor")
                {
                    reanudacionServidor(filaActual);
                }
                else
                {
                    throw new Exception("No se encontró el evento siguiente desde Fin Atención Parking.");
                }
            }
        }

        public void finAtencionEntrada(DataRow filaAnterior) // Actualizado
        {
            // Marcar número de simulación
            numeroSimulacionActual++;


            // Nombres de las variables (una por cada columna)
            string evento;
            decimal? reloj;

            decimal? rndTipoDetencion;
            string tipoDetencion;
            decimal? beta;
            decimal? tiempoDeDetencion;
            decimal? horaDetencion;

            decimal? rndLlegada;
            decimal? tiempoLlegada;
            decimal? proximaLlegada;

            decimal? l;
            decimal? tiempoDuracionDetencionLlegadas;
            decimal? horaReanudacionLlegadas;

            decimal? rndFinAP;
            decimal? tiempoFinAP;
            decimal? proximoFinAP1;
            decimal? proximoFinAP2;
            decimal? proximoFinAP3;
            decimal? proximoFinAP4;
            decimal? proximoFinAP5;

            decimal? rndFinAE;
            decimal? tiempoFinAE;
            decimal? proximoFinAE1;
            decimal? proximoFinAE2;
            decimal? proximoFinAE3;
            decimal? proximoFinAE4;
            decimal? proximoFinAE5;
            decimal? proximoFinAE6;
            decimal? rndCantidadPersonas;
            int? cantidadPersonas;
            decimal? rndCantidadPersonasMayores;
            int? cantidadPersonasMayores;
            int? cantidadPersonasNoMayores;

            decimal? rndFinAC1;
            decimal? tiempoFinAC1;
            decimal? proximoFinAC1;
            decimal? rndFinAC2;
            decimal? tiempoFinAC2;
            decimal? proximoFinAC2;
            decimal? rndFinAC3;
            decimal? tiempoFinAC3;
            decimal? proximoFinAC3;
            decimal? rndFinAC4;
            decimal? tiempoFinAC4;
            decimal? proximoFinAC4;

            decimal? rndFinACM;
            decimal? tiempoFinACM;
            decimal? proximoFinACM;

            decimal? s;
            decimal? tiempoDuracionDetencionServidor;
            decimal? horaReanudacionServidor;

            string ingresoBloqueado;
            int? colaBloqueoLlegadas;

            int? colaPark1;
            string estadoCajaPark1;
            int? colaPark2;
            string estadoCajaPark2;
            int? colaPark3;
            string estadoCajaPark3;
            int? colaPark4;
            string estadoCajaPark4;
            int? colaPark5;
            string estadoCajaPark5;

            int? colaEntrada1y2;
            string estadoCajaEntrada1;
            string estadoCajaEntrada2;
            int? colaEntrada3y4;
            string estadoCajaEntrada3;
            string estadoCajaEntrada4;
            int? colaEntrada5y6;
            string estadoCajaEntrada5;
            string estadoCajaEntrada6;

            int? colaComida1;
            string estadoControlComida1;
            int? colaComida2;
            string estadoControlComida2;
            int? colaComida3;
            string estadoControlComida3;
            int? colaComida4;
            string estadoControlComida4;

            int? colaComidaMayores;
            string estadoControlComidaMayores;

            decimal? metrosPromedioNecesariosParaAparcamiento;
            decimal? acumuladorTiempoColaParking;
            decimal? cantidadPromedioAutosEnColaPark;
            int? contadorGruposCajaEntrada;
            decimal? acumuladorTiempoColaEntrada;
            decimal? tiempoPromedioEnColaEntrada;
            int? contadorPersonasEnControlComida;
            decimal? acumuladorTiempoColaComida;
            decimal? tiempoPromedioEnColaComida;
            decimal? tiempoEnConseguirEntrada;
            decimal? cantidadPromedioGenteEnColaEntrada;
            decimal? tiempoEntradaDespuesDeEstacionar;


            // Evento
            evento = "Fin AE";


            // Reloj
            reloj = obtenerHoraProxEvento(aDecimal(filaAnterior["horaDetencion"]), aDecimal(filaAnterior["proximaLlegada"]), aDecimal(filaAnterior["horaReanudacionLlegadas"]),
                      aDecimal(filaAnterior["proximoFinAP1"]), aDecimal(filaAnterior["proximoFinAP2"]), aDecimal(filaAnterior["proximoFinAP3"]),
                      aDecimal(filaAnterior["proximoFinAP4"]), aDecimal(filaAnterior["proximoFinAP5"]), aDecimal(filaAnterior["proximoFinAE1"]),
                      aDecimal(filaAnterior["proximoFinAE2"]), aDecimal(filaAnterior["proximoFinAE3"]), aDecimal(filaAnterior["proximoFinAE4"]),
                      aDecimal(filaAnterior["proximoFinAE5"]), aDecimal(filaAnterior["proximoFinAE6"]), aDecimal(filaAnterior["proximoFinAC1"]),
                      aDecimal(filaAnterior["proximoFinAC2"]), aDecimal(filaAnterior["proximoFinAC3"]), aDecimal(filaAnterior["proximoFinAC4"]),
                      aDecimal(filaAnterior["proximoFinACM"]), aDecimal(filaAnterior["horaReanudacionServidor"]));


            // Detención
            rndTipoDetencion = null;
            tipoDetencion = filaAnterior["tipoDetencion"].ToString();

            beta = null;
            tiempoDeDetencion = null;

            horaDetencion = aDecimal(filaAnterior["horaDetencion"]);


            // Llegada auto
            rndLlegada = null;

            tiempoLlegada = null;

            proximaLlegada = aDecimal(filaAnterior["proximaLlegada"]);


            // Reanudacion llegadas
            l = null;

            tiempoDuracionDetencionLlegadas = null;

            horaReanudacionLlegadas = aDecimal(filaAnterior["horaReanudacionLlegadas"]);


            // Fin atención parking
            rndFinAP = null;

            tiempoFinAP = null;

            proximoFinAP1 = aDecimal(filaAnterior["proximoFinAP1"]);
            proximoFinAP2 = aDecimal(filaAnterior["proximoFinAP2"]);
            proximoFinAP3 = aDecimal(filaAnterior["proximoFinAP3"]);
            proximoFinAP4 = aDecimal(filaAnterior["proximoFinAP4"]);
            proximoFinAP5 = aDecimal(filaAnterior["proximoFinAP5"]);


            // Fin atención entrada y Caja entrada
            rndFinAE = generarRandom();

            tiempoFinAE = generarTiempoExponencial(rndFinAE, mediaAE);

            proximoFinAE1 = aDecimal(filaAnterior["proximoFinAE1"]);
            proximoFinAE2 = aDecimal(filaAnterior["proximoFinAE2"]);
            proximoFinAE3 = aDecimal(filaAnterior["proximoFinAE3"]);
            proximoFinAE4 = aDecimal(filaAnterior["proximoFinAE4"]);
            proximoFinAE5 = aDecimal(filaAnterior["proximoFinAE5"]);
            proximoFinAE6 = aDecimal(filaAnterior["proximoFinAE6"]);
            
            colaEntrada1y2 = Convert.ToInt32(filaAnterior["colaEntrada1y2"]);
            estadoCajaEntrada1 = filaAnterior["estadoCajaEntrada1"].ToString();
            estadoCajaEntrada2 = filaAnterior["estadoCajaEntrada2"].ToString();

            colaEntrada3y4 = Convert.ToInt32(filaAnterior["colaEntrada3y4"]);
            estadoCajaEntrada3 = filaAnterior["estadoCajaEntrada3"].ToString();
            estadoCajaEntrada4 = filaAnterior["estadoCajaEntrada4"].ToString();

            colaEntrada5y6 = Convert.ToInt32(filaAnterior["colaEntrada5y6"]);
            estadoCajaEntrada5 = filaAnterior["estadoCajaEntrada5"].ToString();
            estadoCajaEntrada6 = filaAnterior["estadoCajaEntrada6"].ToString();

            rndCantidadPersonas = generarRandom();

            cantidadPersonas = generarCantidadPersonas(rndCantidadPersonas);

            rndCantidadPersonasMayores = generarRandom();

            cantidadPersonasMayores = generarCantidadPersonasMayores(rndCantidadPersonasMayores, cantidadPersonas);

            cantidadPersonasNoMayores = cantidadPersonas - cantidadPersonasMayores;

            int indiceFinAE = 0;
            int finAE = 0;

            if (proximoFinAE1 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaEntrada1y2"]) > 0)
                {
                    proximoFinAE1 = generarProximo(reloj, tiempoFinAE);
                    finAE = 1;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int k = 0; k < grdGrupos.Rows.Count;k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt1")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdGrupos.Rows.RemoveAt(indiceFinAE);

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdGrupos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaEntrada1y2")
                        {
                            fila.Cells[0].Value = "SiendoAt1";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAE1 = 0;
                    estadoCajaEntrada1 = "Libre";
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt1")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }
                    grdGrupos.Rows.RemoveAt(indiceFinAE);
                }
            }
            else if (proximoFinAE2 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaEntrada1y2"]) > 0)
                {
                    proximoFinAE2 = generarProximo(reloj, tiempoFinAE);
                    finAE = 2;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt2")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdGrupos.Rows.RemoveAt(indiceFinAE);

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdGrupos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaEntrada1y2")
                        {
                            fila.Cells[0].Value = "SiendoAt2";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAE2 = 0;
                    estadoCajaEntrada2 = "Libre";
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt2")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }
                    grdGrupos.Rows.RemoveAt(indiceFinAE);
                }
            }
            else if (proximoFinAE3 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaEntrada3y4"]) > 0)
                {
                    proximoFinAE3 = generarProximo(reloj, tiempoFinAE);
                    finAE = 3;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt3")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }

                    // Elimino el objeto
                    if (indiceFinAE != -1)
                    {
                        grdGrupos.Rows.RemoveAt(indiceFinAE);
                    }

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdGrupos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaEntrada3y4")
                        {
                            fila.Cells[0].Value = "SiendoAt3";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAE3 = 0;
                    estadoCajaEntrada3 = "Libre";
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt3")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }
                    if (indiceFinAE != -1)
                    {
                        grdGrupos.Rows.RemoveAt(indiceFinAE);
                    }
                }
            }
            else if (proximoFinAE4 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaEntrada3y4"]) > 0)
                {
                    proximoFinAE4 = generarProximo(reloj, tiempoFinAE);
                    finAE = 4;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt4")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdGrupos.Rows.RemoveAt(indiceFinAE);

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdGrupos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaEntrada3y4")
                        {
                            fila.Cells[0].Value = "SiendoAt4";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAE4 = 0;
                    estadoCajaEntrada4 = "Libre";
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt4")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }
                    grdGrupos.Rows.RemoveAt(indiceFinAE);
                }
            }
            else if (proximoFinAE5 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaEntrada5y6"]) > 0)
                {
                    proximoFinAE5 = generarProximo(reloj, tiempoFinAE);
                    finAE = 5;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt5")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdGrupos.Rows.RemoveAt(indiceFinAE);

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdGrupos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaEntrada5y6")
                        {
                            fila.Cells[0].Value = "SiendoAt5";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAE5 = 0;
                    estadoCajaEntrada5 = "Libre";
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt5")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }
                    if (indiceFinAE != -1)
                    {
                        grdGrupos.Rows.RemoveAt(indiceFinAE);
                    }
                }
            }
            else if (proximoFinAE6 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaEntrada5y6"]) > 0)
                {
                    proximoFinAE6 = generarProximo(reloj, tiempoFinAE);
                    finAE = 6;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt6")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }

                    // Elimino el objeto
                    if (indiceFinAE != -1)
                    {
                        grdGrupos.Rows.RemoveAt(indiceFinAE);
                    }

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdGrupos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaEntrada5y6")
                        {
                            fila.Cells[0].Value = "SiendoAt6";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAE6 = 0;
                    estadoCajaEntrada6 = "Libre";
                    for (int k = 0; k < grdGrupos.Rows.Count; k++)
                    {
                        DataGridViewRow fila = grdGrupos.Rows[k];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt6")
                        {
                            indiceFinAE = k;
                            break;
                        }
                    }
                    if (indiceFinAE != -1)
                    {
                        grdGrupos.Rows.RemoveAt(indiceFinAE);
                    }
                }
            }

            if (finAE == 1 || finAE == 2)
            {
                colaEntrada1y2--;
            }
            else if (finAE == 3 || finAE == 4)
            {
                colaEntrada3y4--;
            }
            else if (finAE == 5 || finAE == 6)
            {
                colaEntrada5y6--;
            }


            // Fin atención control comida y Control Comida
            rndFinAC1 = null;
            tiempoFinAC1 = null;
            proximoFinAC1 = aDecimal(filaAnterior["proximoFinAC1"]);
            rndFinAC2 = null;
            tiempoFinAC2 = null;
            proximoFinAC2 = aDecimal(filaAnterior["proximoFinAC2"]);
            rndFinAC3 = null;
            tiempoFinAC3 = null;
            proximoFinAC3 = aDecimal(filaAnterior["proximoFinAC3"]);
            rndFinAC4 = null;
            tiempoFinAC4 = null;
            proximoFinAC4 = aDecimal(filaAnterior["proximoFinAC4"]);

            colaComida1 = Convert.ToInt32(filaAnterior["colaComida1"]);
            estadoControlComida1 = filaAnterior["estadoControlComida1"].ToString();
            colaComida2 = Convert.ToInt32(filaAnterior["colaComida2"]);
            estadoControlComida2 = filaAnterior["estadoControlComida2"].ToString();
            colaComida3 = Convert.ToInt32(filaAnterior["colaComida3"]);
            estadoControlComida3 = filaAnterior["estadoControlComida3"].ToString();
            colaComida4 = Convert.ToInt32(filaAnterior["colaComida4"]);
            estadoControlComida4 = filaAnterior["estadoControlComida4"].ToString();

            int? personasPorAtender = cantidadPersonasNoMayores;

            if ((filaAnterior["estadoControlComida1"].ToString() == "Libre") && personasPorAtender > 0)
            {
                rndFinAC1 = generarRandom();
                tiempoFinAC1 = generarTiempoExponencial(rndFinAC1, mediaAC);
                proximoFinAC1 = generarProximo(reloj, tiempoFinAC1);
                estadoControlComida1 = "Ocupado";
                personasPorAtender--;
                grdPersonas.Rows.Add("SiendoAt1");
            }
            if ((filaAnterior["estadoControlComida2"].ToString() == "Libre") && personasPorAtender > 0)
            {
                rndFinAC2 = generarRandom();
                tiempoFinAC2 = generarTiempoExponencial(rndFinAC2, mediaAC);
                proximoFinAC2 = generarProximo(reloj, tiempoFinAC2);
                estadoControlComida2 = "Ocupado";
                personasPorAtender--;
                grdPersonas.Rows.Add("SiendoAt2");
            }
            if ((filaAnterior["estadoControlComida3"].ToString() == "Libre") && personasPorAtender > 0)
            {
                rndFinAC3 = generarRandom();
                tiempoFinAC3 = generarTiempoExponencial(rndFinAC3, mediaAC);
                proximoFinAC3 = generarProximo(reloj, tiempoFinAC3);
                estadoControlComida3 = "Ocupado";
                personasPorAtender--;
                grdPersonas.Rows.Add("SiendoAt3");
            }
            if ((filaAnterior["estadoControlComida4"].ToString() == "Libre") && personasPorAtender > 0)
            {
                rndFinAC4 = generarRandom();
                tiempoFinAC4 = generarTiempoExponencial(rndFinAC4, mediaAC);
                proximoFinAC4 = generarProximo(reloj, tiempoFinAC4);
                estadoControlComida4 = "Ocupado";
                personasPorAtender--;
                grdPersonas.Rows.Add("SiendoAt4");
            }

            int i = 0;
            while (personasPorAtender > 0)
            {
                int? colaMasChica = obtenerColaMenorControlComida(colaComida1, colaComida2, colaComida3, colaComida4);

                if (colaMasChica == colaComida1)
                {
                    colaComida1++;
                    grdPersonas.Rows.Add("EnColaComida1");
                }
                else if (colaMasChica == colaComida2)
                {
                    colaComida2++;
                    grdPersonas.Rows.Add("EnColaComida2");
                }
                else if (colaMasChica == colaComida3)
                {
                    colaComida3++;
                    grdPersonas.Rows.Add("EnColaComida3");
                }
                else if (colaMasChica == colaComida4)
                {
                    colaComida4++;
                    grdPersonas.Rows.Add("EnColaComida4");
                }
                personasPorAtender--;


                i++;
                if (i == 999999)
                {
                    throw new Exception("Este ciclo nunca corta");
                }
            }
            

            // Fin atención control comida mayores y Control comida mayores
            rndFinACM = null;
            tiempoFinACM = null;
            proximoFinACM = aDecimal(filaAnterior["proximoFinACM"]);

            colaComidaMayores = Convert.ToInt32(filaAnterior["colaComidaMayores"]);
            estadoControlComidaMayores = filaAnterior["estadoControlComidaMayores"].ToString();

            int? mayoresPorAtender = cantidadPersonasMayores;

            if ((filaAnterior["estadoControlComidaMayores"].ToString() == "Libre") && mayoresPorAtender > 0)
            {
                rndFinACM = generarRandom();
                tiempoFinACM = generarTiempoExponencial(rndFinACM, mediaACM);
                proximoFinACM = generarProximo(reloj, tiempoFinACM);
                estadoControlComidaMayores = "Ocupado";
                mayoresPorAtender--;
                grdPersonasMayores.Rows.Add("SiendoAt");
            }

            int j = 0;
            while (mayoresPorAtender > 0)
            {
                colaComidaMayores++;
                mayoresPorAtender--;
                grdPersonasMayores.Rows.Add("EnColaComidaMayores");

                j++;
                if (j == 999999)
                {
                    throw new Exception("Este ciclo nunca corta");
                }
            }


            // Reanudacion Servidor
            s = null;
            tiempoDuracionDetencionServidor = null;
            horaReanudacionServidor = aDecimal(filaAnterior["horaReanudacionServidor"]);


            // Bloqueo de llegadas
            ingresoBloqueado = filaAnterior["ingresoBloqueado"].ToString();

            colaBloqueoLlegadas = Convert.ToInt32(filaAnterior["colaBloqueoLlegadas"]);
            

            // Caja park
            colaPark1 = Convert.ToInt32(filaAnterior["colaPark1"]);
            estadoCajaPark1 = filaAnterior["estadoCajaPark1"].ToString();
            colaPark2 = Convert.ToInt32(filaAnterior["colaPark2"]);
            estadoCajaPark2 = filaAnterior["estadoCajaPark2"].ToString();
            colaPark3 = Convert.ToInt32(filaAnterior["colaPark3"]);
            estadoCajaPark3 = filaAnterior["estadoCajaPark3"].ToString();
            colaPark4 = Convert.ToInt32(filaAnterior["colaPark4"]);
            estadoCajaPark4 = filaAnterior["estadoCajaPark4"].ToString();
            colaPark5 = Convert.ToInt32(filaAnterior["colaPark5"]);
            estadoCajaPark5 = filaAnterior["estadoCajaPark5"].ToString();


            // Estadísticas
            metrosPromedioNecesariosParaAparcamiento = aDecimal(filaAnterior["cantidadPromedioAutosEnColaPark"]) * 4;
            acumuladorTiempoColaParking = (reloj - aDecimal(filaAnterior["reloj"])) * (colaPark1 + colaPark2 + colaPark3 + colaPark4 + colaPark5) + aDecimal(filaAnterior["acumuladorTiempoColaParking"]);
            cantidadPromedioAutosEnColaPark = acumuladorTiempoColaParking / reloj;

            contadorGruposCajaEntrada = Convert.ToInt32(filaAnterior["contadorGruposCajaEntrada"]) + 1;
            acumuladorTiempoColaEntrada = (reloj - aDecimal(filaAnterior["reloj"])) * (colaEntrada1y2 + colaEntrada3y4 + colaEntrada5y6) + aDecimal(filaAnterior["acumuladorTiempoColaEntrada"]);
            if (contadorGruposCajaEntrada == 0)
            {
                tiempoPromedioEnColaEntrada = 0;
            }
            else
            {
                tiempoPromedioEnColaEntrada = acumuladorTiempoColaEntrada / contadorGruposCajaEntrada;
            }

            contadorPersonasEnControlComida = Convert.ToInt32(filaAnterior["contadorPersonasEnControlComida"]);
            acumuladorTiempoColaComida = (reloj - aDecimal(filaAnterior["reloj"])) * (colaComida1 + colaComida2 + colaComida3 + colaComida4 + colaComidaMayores) + aDecimal(filaAnterior["acumuladorTiempoColaComida"]);
            if (contadorPersonasEnControlComida == 0)
            {
                tiempoPromedioEnColaComida = 0;
            }
            else
            {
                tiempoPromedioEnColaComida = acumuladorTiempoColaComida / contadorPersonasEnControlComida;
            }

            tiempoEnConseguirEntrada = 300 + tiempoPromedioEnColaEntrada + 92;
            cantidadPromedioGenteEnColaEntrada = (acumuladorTiempoColaEntrada / reloj) * (decimal)4.16;
            tiempoEntradaDespuesDeEstacionar = tiempoEnConseguirEntrada + tiempoPromedioEnColaComida + 5;



            // --- Manejo de tabla y próximo evento ---

            // Eliminar fila anterior
            dt.Rows.Remove(filaAnterior);

            // Agregar la nueva fila
            dt.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);

            // Guardar la nueva fila en una variable para enviársela al próximo evento
            DataRow filaActual = dt.Rows[0];

            // Si la simulación está dentro de las que quiere visualizar, agregarla a la grilla
            if ((numeroSimulacionActual >= verDesdeSimulacion && numeroSimulacionActual < verHastaSimulacion) || numeroSimulacionActual == cantidadDeSimulaciones)
            {
                grdSimulacion.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);
            }

            // Determinar el próximo evento
            if (cantidadDeSimulaciones >= numeroSimulacionActual)
            {
                string proxEvento = obtenerProximoEvento(horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6,
    proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor);

                if (proxEvento == "LL Auto")
                {
                    llegadaAuto(filaActual);
                }
                else if (proxEvento == "Fin AP")
                {
                    finAtencionParking(filaActual);
                }
                else if (proxEvento == "Fin AE")
                {
                    finAtencionEntrada(filaActual);
                }
                else if (proxEvento == "Fin AC")
                {
                    finAtencionComida(filaActual);
                }
                else if (proxEvento == "Fin ACM")
                {
                    finAtencionComidaMayores(filaActual);
                }
                else if (proxEvento == "Detención")
                {
                    detencion(filaActual);
                }
                else if (proxEvento == "Reanudación Llegadas")
                {
                    reanudacionLlegadas(filaActual);
                }
                else if (proxEvento == "Reanudación Servidor")
                {
                    reanudacionServidor(filaActual);
                }
                else
                {
                    throw new Exception("No se encontró el evento siguiente desde Fin AE.");
                }
            }
        }

        public void finAtencionComida(DataRow filaAnterior) // Actualizado
        {
            // Marcar número de simulación
            numeroSimulacionActual++;


            // Nombres de las variables (una por cada columna)
            string evento;
            decimal? reloj;

            decimal? rndTipoDetencion;
            string tipoDetencion;
            decimal? beta;
            decimal? tiempoDeDetencion;
            decimal? horaDetencion;

            decimal? rndLlegada;
            decimal? tiempoLlegada;
            decimal? proximaLlegada;

            decimal? l;
            decimal? tiempoDuracionDetencionLlegadas;
            decimal? horaReanudacionLlegadas;

            decimal? rndFinAP;
            decimal? tiempoFinAP;
            decimal? proximoFinAP1;
            decimal? proximoFinAP2;
            decimal? proximoFinAP3;
            decimal? proximoFinAP4;
            decimal? proximoFinAP5;

            decimal? rndFinAE;
            decimal? tiempoFinAE;
            decimal? proximoFinAE1;
            decimal? proximoFinAE2;
            decimal? proximoFinAE3;
            decimal? proximoFinAE4;
            decimal? proximoFinAE5;
            decimal? proximoFinAE6;
            decimal? rndCantidadPersonas;
            int? cantidadPersonas;
            decimal? rndCantidadPersonasMayores;
            int? cantidadPersonasMayores;
            int? cantidadPersonasNoMayores;

            decimal? rndFinAC1;
            decimal? tiempoFinAC1;
            decimal? proximoFinAC1;
            decimal? rndFinAC2;
            decimal? tiempoFinAC2;
            decimal? proximoFinAC2;
            decimal? rndFinAC3;
            decimal? tiempoFinAC3;
            decimal? proximoFinAC3;
            decimal? rndFinAC4;
            decimal? tiempoFinAC4;
            decimal? proximoFinAC4;

            decimal? rndFinACM;
            decimal? tiempoFinACM;
            decimal? proximoFinACM;

            decimal? s;
            decimal? tiempoDuracionDetencionServidor;
            decimal? horaReanudacionServidor;

            string ingresoBloqueado;
            int? colaBloqueoLlegadas;

            int? colaPark1;
            string estadoCajaPark1;
            int? colaPark2;
            string estadoCajaPark2;
            int? colaPark3;
            string estadoCajaPark3;
            int? colaPark4;
            string estadoCajaPark4;
            int? colaPark5;
            string estadoCajaPark5;

            int? colaEntrada1y2;
            string estadoCajaEntrada1;
            string estadoCajaEntrada2;
            int? colaEntrada3y4;
            string estadoCajaEntrada3;
            string estadoCajaEntrada4;
            int? colaEntrada5y6;
            string estadoCajaEntrada5;
            string estadoCajaEntrada6;

            int? colaComida1;
            string estadoControlComida1;
            int? colaComida2;
            string estadoControlComida2;
            int? colaComida3;
            string estadoControlComida3;
            int? colaComida4;
            string estadoControlComida4;

            int? colaComidaMayores;
            string estadoControlComidaMayores;

            decimal? metrosPromedioNecesariosParaAparcamiento;
            decimal? acumuladorTiempoColaParking;
            decimal? cantidadPromedioAutosEnColaPark;
            int? contadorGruposCajaEntrada;
            decimal? acumuladorTiempoColaEntrada;
            decimal? tiempoPromedioEnColaEntrada;
            int? contadorPersonasEnControlComida;
            decimal? acumuladorTiempoColaComida;
            decimal? tiempoPromedioEnColaComida;
            decimal? tiempoEnConseguirEntrada;
            decimal? cantidadPromedioGenteEnColaEntrada;
            decimal? tiempoEntradaDespuesDeEstacionar;


            // Evento
            evento = "Fin AC";


            // Reloj
            reloj = obtenerHoraProxEvento(aDecimal(filaAnterior["horaDetencion"]), aDecimal(filaAnterior["proximaLlegada"]), aDecimal(filaAnterior["horaReanudacionLlegadas"]), aDecimal(filaAnterior["proximoFinAP1"]), aDecimal(filaAnterior["proximoFinAP2"]), aDecimal(filaAnterior["proximoFinAP3"]),
                 aDecimal(filaAnterior["proximoFinAP4"]), aDecimal(filaAnterior["proximoFinAP5"]), aDecimal(filaAnterior["proximoFinAE1"]),
                 aDecimal(filaAnterior["proximoFinAE2"]), aDecimal(filaAnterior["proximoFinAE3"]),
                 aDecimal(filaAnterior["proximoFinAE4"]), aDecimal(filaAnterior["proximoFinAE5"]), aDecimal(filaAnterior["proximoFinAE6"]),
                 aDecimal(filaAnterior["proximoFinAC1"]), aDecimal(filaAnterior["proximoFinAC2"]), aDecimal(filaAnterior["proximoFinAC3"]),
                 aDecimal(filaAnterior["proximoFinAC4"]),
                 aDecimal(filaAnterior["proximoFinACM"]), aDecimal(filaAnterior["horaReanudacionServidor"]));


            // Detención 
            rndTipoDetencion = null;

            tipoDetencion = (string)filaAnterior["tipoDetencion"];

            beta = null;

            tiempoDeDetencion = null;

            horaDetencion = aDecimal(filaAnterior["horaDetencion"]);


            // Llegada auto
            rndLlegada = null;

            tiempoLlegada = null;

            proximaLlegada = aDecimal(filaAnterior["proximaLlegada"]);


            // Reanudación llegadas
            l = null;

            tiempoDuracionDetencionLlegadas = null;

            horaReanudacionLlegadas = aDecimal(filaAnterior["horaReanudacionLlegadas"]);


            // Fin atención parking
            rndFinAP = null;

            tiempoFinAP = null;

            proximoFinAP1 = aDecimal(filaAnterior["proximoFinAP1"]);
            proximoFinAP2 = aDecimal(filaAnterior["proximoFinAP2"]);
            proximoFinAP3 = aDecimal(filaAnterior["proximoFinAP3"]);
            proximoFinAP4 = aDecimal(filaAnterior["proximoFinAP4"]);
            proximoFinAP5 = aDecimal(filaAnterior["proximoFinAP5"]);


            // Fin atención entrada
            rndFinAE = null;

            tiempoFinAE = null;

            proximoFinAE1 = aDecimal(filaAnterior["proximoFinAE1"]);
            proximoFinAE2 = aDecimal(filaAnterior["proximoFinAE2"]);
            proximoFinAE3 = aDecimal(filaAnterior["proximoFinAE3"]);
            proximoFinAE4 = aDecimal(filaAnterior["proximoFinAE4"]);
            proximoFinAE5 = aDecimal(filaAnterior["proximoFinAE5"]);
            proximoFinAE6 = aDecimal(filaAnterior["proximoFinAE6"]);

            rndCantidadPersonas = null;

            cantidadPersonas = null;

            rndCantidadPersonasMayores = null;

            cantidadPersonasMayores = null;

            cantidadPersonasNoMayores = null;


            // Control comida
            colaComida1 = Convert.ToInt32(filaAnterior["colaComida1"]);
            estadoControlComida1 = filaAnterior["estadoControlComida1"].ToString();

            colaComida2 = Convert.ToInt32(filaAnterior["colaComida2"]);
            estadoControlComida2 = filaAnterior["estadoControlComida2"].ToString();

            colaComida3 = Convert.ToInt32(filaAnterior["colaComida3"]);
            estadoControlComida3 = filaAnterior["estadoControlComida3"].ToString();

            colaComida4 = Convert.ToInt32(filaAnterior["colaComida4"]);
            estadoControlComida4 = filaAnterior["estadoControlComida4"].ToString();


            // Fin atención control comida
            rndFinAC1 = null;
            tiempoFinAC1 = null;
            proximoFinAC1 = aDecimal(filaAnterior["proximoFinAC1"]);

            rndFinAC2 = null;
            tiempoFinAC2 = null;
            proximoFinAC2 = aDecimal(filaAnterior["proximoFinAC2"]);

            rndFinAC3 = null;
            tiempoFinAC3 = null;
            proximoFinAC3 = aDecimal(filaAnterior["proximoFinAC3"]);

            rndFinAC4 = null;
            tiempoFinAC4 = null;
            proximoFinAC4 = aDecimal(filaAnterior["proximoFinAC4"]);
            
            int finComida = 0;
            int indiceFinAC = -1;

            if (proximoFinAC1 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaComida1"]) > 0)
                {
                    rndFinAC1 = generarRandom();
                    tiempoFinAC1 = generarTiempoExponencial(rndFinAC1, mediaAC);
                    proximoFinAC1 = generarProximo(reloj, tiempoFinAC1);
                    finComida = 1;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int i = 0; i < grdPersonas.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonas.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt1")
                        {
                            indiceFinAC = i;
                            break;
                        } 
                    }

                    // Elimino el objeto
                    grdPersonas.Rows.RemoveAt(indiceFinAC);

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdPersonas.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaComida1")
                        {
                            fila.Cells[0].Value = "SiendoAt1";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAC1 = null;
                    estadoControlComida1 = "Libre";
                    for (int i = 0; i < grdPersonas.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonas.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt1")
                        {
                            indiceFinAC = i;
                            break;
                        }
                    }
                    grdPersonas.Rows.RemoveAt(indiceFinAC);
                }
            }
            if (proximoFinAC2 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaComida2"]) > 0)
                {
                    rndFinAC2 = generarRandom();
                    tiempoFinAC2 = generarTiempoExponencial(rndFinAC2, mediaAC);
                    proximoFinAC2 = generarProximo(reloj, tiempoFinAC2);
                    finComida = 2;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int i = 0; i < grdPersonas.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonas.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt2")
                        {
                            indiceFinAC = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdPersonas.Rows.RemoveAt(indiceFinAC);

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdPersonas.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaComida2")
                        {
                            fila.Cells[0].Value = "SiendoAt2";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAC2 = null;
                    estadoControlComida2 = "Libre";
                    for (int i = 0; i < grdPersonas.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonas.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt2")
                        {
                            indiceFinAC = i;
                            break;
                        }
                    }
                    grdPersonas.Rows.RemoveAt(indiceFinAC);
                }
            }
            if (proximoFinAC3 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaComida3"]) > 0)
                {
                    rndFinAC3 = generarRandom();
                    tiempoFinAC3 = generarTiempoExponencial(rndFinAC3, mediaAC);
                    proximoFinAC3 = generarProximo(reloj, tiempoFinAC3);
                    finComida = 3;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int i = 0; i < grdPersonas.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonas.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt3")
                        {
                            indiceFinAC = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdPersonas.Rows.RemoveAt(indiceFinAC);

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdPersonas.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaComida3")
                        {
                            fila.Cells[0].Value = "SiendoAt3";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAC3 = null;
                    estadoControlComida3 = "Libre";
                    for (int i = 0; i < grdPersonas.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonas.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt3")
                        {
                            indiceFinAC = i;
                            break;
                        }
                    }
                    grdPersonas.Rows.RemoveAt(indiceFinAC);
                }
            }
            if (proximoFinAC4 == reloj)
            {
                if (Convert.ToInt32(filaAnterior["colaComida4"]) > 0)
                {
                    rndFinAC4 = generarRandom();
                    tiempoFinAC4 = generarTiempoExponencial(rndFinAC4, mediaAC);
                    proximoFinAC4 = generarProximo(reloj, tiempoFinAC4);
                    finComida = 4;

                    // Recorremos la grid para encontrar el indice del grupo
                    for (int i = 0; i < grdPersonas.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonas.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt4")
                        {
                            indiceFinAC = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdPersonas.Rows.RemoveAt(indiceFinAC);

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdPersonas.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaComida4")
                        {
                            fila.Cells[0].Value = "SiendoAt4";
                            break;
                        }
                    }
                }
                else
                {
                    proximoFinAC4 = null;
                    estadoControlComida4 = "Libre";
                    for (int i = 0; i < grdPersonas.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonas.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt4")
                        {
                            indiceFinAC = i;
                            break;
                        }
                    }
                    grdPersonas.Rows.RemoveAt(indiceFinAC);
                }
            }

            if (finComida == 1)
            {
                colaComida1--;
            }
            else if (finComida == 2)
            {
                colaComida2--;
            }
            else if (finComida == 3)
            {
                colaComida3--;
            }
            else if (finComida == 4)
            {
                colaComida4--;
            }


            // Fin atención control comida mayores
            rndFinACM = null;
            tiempoFinACM = null;
            proximoFinACM = aDecimal(filaAnterior["proximoFinACM"]);


            // Reanudación servidor
            s = null;

            tiempoDuracionDetencionServidor = null;

            horaReanudacionServidor = aDecimal(filaAnterior["horaReanudacionServidor"]);


            // Bloqueo de llegadas
            ingresoBloqueado = filaAnterior["ingresoBloqueado"].ToString();

            colaBloqueoLlegadas = Convert.ToInt32(filaAnterior["colaBloqueoLlegadas"]);


            // Caja park
            colaPark1 = Convert.ToInt32(filaAnterior["colaPark1"]);
            estadoCajaPark1 = filaAnterior["estadoCajaPark1"].ToString();
            colaPark2 = Convert.ToInt32(filaAnterior["colaPark2"]);
            estadoCajaPark2 = filaAnterior["estadoCajaPark2"].ToString();
            colaPark3 = Convert.ToInt32(filaAnterior["colaPark3"]);
            estadoCajaPark3 = filaAnterior["estadoCajaPark3"].ToString();
            colaPark4 = Convert.ToInt32(filaAnterior["colaPark4"]);
            estadoCajaPark4 = filaAnterior["estadoCajaPark4"].ToString();
            colaPark5 = Convert.ToInt32(filaAnterior["colaPark5"]);
            estadoCajaPark5 = filaAnterior["estadoCajaPark5"].ToString();


            // Caja entrada
            colaEntrada1y2 = Convert.ToInt32(filaAnterior["colaEntrada1y2"]);
            estadoCajaEntrada1 = filaAnterior["estadoCajaEntrada1"].ToString();
            estadoCajaEntrada2 = filaAnterior["estadoCajaEntrada2"].ToString();

            colaEntrada3y4 = Convert.ToInt32(filaAnterior["colaEntrada3y4"]);
            estadoCajaEntrada3 = filaAnterior["estadoCajaEntrada3"].ToString();
            estadoCajaEntrada4 = filaAnterior["estadoCajaEntrada4"].ToString();

            colaEntrada5y6 = Convert.ToInt32(filaAnterior["colaEntrada5y6"]);
            estadoCajaEntrada5 = filaAnterior["estadoCajaEntrada5"].ToString();
            estadoCajaEntrada6 = filaAnterior["estadoCajaEntrada6"].ToString();


            // Control comida mayores
            colaComidaMayores = Convert.ToInt32(filaAnterior["colaComidaMayores"]);
            estadoControlComidaMayores = filaAnterior["estadoControlComidaMayores"].ToString();


            // Estadísticas
            metrosPromedioNecesariosParaAparcamiento = aDecimal(filaAnterior["cantidadPromedioAutosEnColaPark"]) * 4;
            acumuladorTiempoColaParking = (reloj - aDecimal(filaAnterior["reloj"])) * (colaPark1 + colaPark2 + colaPark3 + colaPark4 + colaPark5) + aDecimal(filaAnterior["acumuladorTiempoColaParking"]);
            cantidadPromedioAutosEnColaPark = acumuladorTiempoColaParking / reloj;

            contadorGruposCajaEntrada = Convert.ToInt32(filaAnterior["contadorGruposCajaEntrada"]);
            acumuladorTiempoColaEntrada = (reloj - aDecimal(filaAnterior["reloj"])) * (colaEntrada1y2 + colaEntrada3y4 + colaEntrada5y6) + aDecimal(filaAnterior["acumuladorTiempoColaEntrada"]);
            if (contadorGruposCajaEntrada == 0)
            {
                tiempoPromedioEnColaEntrada = 0;
            }
            else
            {
                tiempoPromedioEnColaEntrada = acumuladorTiempoColaEntrada / contadorGruposCajaEntrada;
            }

            contadorPersonasEnControlComida = Convert.ToInt32(filaAnterior["contadorPersonasEnControlComida"]) + 1;
            acumuladorTiempoColaComida = (reloj - aDecimal(filaAnterior["reloj"])) * (colaComida1 + colaComida2 + colaComida3 + colaComida4 + colaComidaMayores) + aDecimal(filaAnterior["acumuladorTiempoColaComida"]);
            if (contadorPersonasEnControlComida == 0)
            {
                tiempoPromedioEnColaComida = 0;
            }
            else
            {
                tiempoPromedioEnColaComida = acumuladorTiempoColaComida / contadorPersonasEnControlComida;
            }

            tiempoEnConseguirEntrada = 300 + tiempoPromedioEnColaEntrada + 92;
            cantidadPromedioGenteEnColaEntrada = (acumuladorTiempoColaEntrada / reloj) * (decimal)4.16;
            tiempoEntradaDespuesDeEstacionar = tiempoEnConseguirEntrada + tiempoPromedioEnColaComida + 5;



            // --- Manejo de tabla y próximo evento ---

            // Eliminar la fila anterior
            dt.Rows.Remove(filaAnterior);

            // Agregar la nueva fila
            dt.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);

            // Guardar la nueva fila en una variable para enviársela al próximo evento
            DataRow filaActual = dt.Rows[0];

            // Si la simulación está dentro de las que quiere visualizar, agregarla a la grilla
            if ((numeroSimulacionActual >= verDesdeSimulacion && numeroSimulacionActual < verHastaSimulacion) || numeroSimulacionActual == cantidadDeSimulaciones)
            {
                grdSimulacion.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);
            }

            // Determinar el próximo evento
            if (cantidadDeSimulaciones >= numeroSimulacionActual)
            {
                string proxEvento = obtenerProximoEvento(horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6,
                proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor);

                if (proxEvento == "LL Auto")
                {
                    llegadaAuto(filaActual);
                }
                else if (proxEvento == "Fin AP")
                {
                    finAtencionParking(filaActual);
                }
                else if (proxEvento == "Fin AE")
                {
                    finAtencionEntrada(filaActual);
                }
                else if (proxEvento == "Fin AC")
                {
                    finAtencionComida(filaActual);
                }
                else if (proxEvento == "Fin ACM")
                {
                    finAtencionComidaMayores(filaActual);
                }
                else if (proxEvento == "Detención")
                {
                    detencion(filaActual);
                }
                else if (proxEvento == "Reanudación Llegadas")
                {
                    reanudacionLlegadas(filaActual);
                }
                else if (proxEvento == "Reanudación Servidor")
                {
                    reanudacionServidor(filaActual);
                }
                else
                {
                    throw new Exception("No se encontró el evento siguiente desde Fin AC.");
                }
            }

        }

        public void finAtencionComidaMayores(DataRow filaAnterior) // Actualizado
        {
            // Marcar número de simulación
            numeroSimulacionActual++;


            // Nombres de las variables (una por cada columna)
            string evento;
            decimal? reloj;

            decimal? rndTipoDetencion;
            string tipoDetencion;
            decimal? beta;
            decimal? tiempoDeDetencion;
            decimal? horaDetencion;

            decimal? rndLlegada;
            decimal? tiempoLlegada;
            decimal? proximaLlegada;

            decimal? l;
            decimal? tiempoDuracionDetencionLlegadas;
            decimal? horaReanudacionLlegadas;

            decimal? rndFinAP;
            decimal? tiempoFinAP;
            decimal? proximoFinAP1;
            decimal? proximoFinAP2;
            decimal? proximoFinAP3;
            decimal? proximoFinAP4;
            decimal? proximoFinAP5;

            decimal? rndFinAE;
            decimal? tiempoFinAE;
            decimal? proximoFinAE1;
            decimal? proximoFinAE2;
            decimal? proximoFinAE3;
            decimal? proximoFinAE4;
            decimal? proximoFinAE5;
            decimal? proximoFinAE6;
            decimal? rndCantidadPersonas;
            int? cantidadPersonas;
            decimal? rndCantidadPersonasMayores;
            int? cantidadPersonasMayores;
            int? cantidadPersonasNoMayores;

            decimal? rndFinAC1;
            decimal? tiempoFinAC1;
            decimal? proximoFinAC1;
            decimal? rndFinAC2;
            decimal? tiempoFinAC2;
            decimal? proximoFinAC2;
            decimal? rndFinAC3;
            decimal? tiempoFinAC3;
            decimal? proximoFinAC3;
            decimal? rndFinAC4;
            decimal? tiempoFinAC4;
            decimal? proximoFinAC4;

            decimal? rndFinACM;
            decimal? tiempoFinACM;
            decimal? proximoFinACM;

            decimal? s;
            decimal? tiempoDuracionDetencionServidor;
            decimal? horaReanudacionServidor;

            string ingresoBloqueado;
            int? colaBloqueoLlegadas;

            int? colaPark1;
            string estadoCajaPark1;
            int? colaPark2;
            string estadoCajaPark2;
            int? colaPark3;
            string estadoCajaPark3;
            int? colaPark4;
            string estadoCajaPark4;
            int? colaPark5;
            string estadoCajaPark5;

            int? colaEntrada1y2;
            string estadoCajaEntrada1;
            string estadoCajaEntrada2;
            int? colaEntrada3y4;
            string estadoCajaEntrada3;
            string estadoCajaEntrada4;
            int? colaEntrada5y6;
            string estadoCajaEntrada5;
            string estadoCajaEntrada6;

            int? colaComida1;
            string estadoControlComida1;
            int? colaComida2;
            string estadoControlComida2;
            int? colaComida3;
            string estadoControlComida3;
            int? colaComida4;
            string estadoControlComida4;

            int? colaComidaMayores;
            string estadoControlComidaMayores;

            decimal? metrosPromedioNecesariosParaAparcamiento;
            decimal? acumuladorTiempoColaParking;
            decimal? cantidadPromedioAutosEnColaPark;
            int? contadorGruposCajaEntrada;
            decimal? acumuladorTiempoColaEntrada;
            decimal? tiempoPromedioEnColaEntrada;
            int? contadorPersonasEnControlComida;
            decimal? acumuladorTiempoColaComida;
            decimal? tiempoPromedioEnColaComida;
            decimal? tiempoEnConseguirEntrada;
            decimal? cantidadPromedioGenteEnColaEntrada;
            decimal? tiempoEntradaDespuesDeEstacionar;


            // Evento
            evento = "Fin ACM";


            // Reloj
            reloj = obtenerHoraProxEvento(aDecimal(filaAnterior["horaDetencion"]), aDecimal(filaAnterior["proximaLlegada"]), aDecimal(filaAnterior["horaReanudacionLlegadas"]),
                             aDecimal(filaAnterior["proximoFinAP1"]), aDecimal(filaAnterior["proximoFinAP2"]), aDecimal(filaAnterior["proximoFinAP3"]),
                             aDecimal(filaAnterior["proximoFinAP4"]), aDecimal(filaAnterior["proximoFinAP5"]), aDecimal(filaAnterior["proximoFinAE1"]),
                             aDecimal(filaAnterior["proximoFinAE2"]), aDecimal(filaAnterior["proximoFinAE3"]), aDecimal(filaAnterior["proximoFinAE4"]),
                             aDecimal(filaAnterior["proximoFinAE5"]), aDecimal(filaAnterior["proximoFinAE6"]), aDecimal(filaAnterior["proximoFinAC1"]),
                             aDecimal(filaAnterior["proximoFinAC2"]), aDecimal(filaAnterior["proximoFinAC3"]), aDecimal(filaAnterior["proximoFinAC4"]),
                             aDecimal(filaAnterior["proximoFinACM"]), aDecimal(filaAnterior["horaReanudacionServidor"]));


            // Detención
            rndTipoDetencion = null;

            tipoDetencion = filaAnterior["tipoDetencion"].ToString();

            beta = null;

            tiempoDeDetencion = null;

            horaDetencion = aDecimal(filaAnterior["horaDetencion"]);


            // Llegada auto
            rndLlegada = null;

            tiempoLlegada = null;

            proximaLlegada = aDecimal(filaAnterior["proximaLlegada"]);


            // Reanudación llegadas
            l = null;

            tiempoDuracionDetencionLlegadas = null;

            horaReanudacionLlegadas = aDecimal(filaAnterior["horaReanudacionLlegadas"]);

            
            // Fin atención parking
            rndFinAP = null;

            tiempoFinAP = null;

            proximoFinAP1 = aDecimal(filaAnterior["proximoFinAP1"]);
            proximoFinAP2 = aDecimal(filaAnterior["proximoFinAP2"]);
            proximoFinAP3 = aDecimal(filaAnterior["proximoFinAP3"]);
            proximoFinAP4 = aDecimal(filaAnterior["proximoFinAP4"]);
            proximoFinAP5 = aDecimal(filaAnterior["proximoFinAP5"]);


            // Fin atención entrada
            rndFinAE = null;

            tiempoFinAE = null;

            proximoFinAE1 = aDecimal(filaAnterior["proximoFinAE1"]);
            proximoFinAE2 = aDecimal(filaAnterior["proximoFinAE2"]);
            proximoFinAE3 = aDecimal(filaAnterior["proximoFinAE3"]);
            proximoFinAE4 = aDecimal(filaAnterior["proximoFinAE4"]);
            proximoFinAE5 = aDecimal(filaAnterior["proximoFinAE5"]);
            proximoFinAE6 = aDecimal(filaAnterior["proximoFinAE6"]);

            rndCantidadPersonas = null;

            cantidadPersonas = null;

            rndCantidadPersonasMayores = null;

            cantidadPersonasMayores = null;

            cantidadPersonasNoMayores = null;


            // Fin atención control comida
            rndFinAC1 = null;
            tiempoFinAC1 = null;
            proximoFinAC1 = aDecimal(filaAnterior["proximoFinAC1"]);

            rndFinAC2 = null;
            tiempoFinAC2 = null;
            proximoFinAC2 = aDecimal(filaAnterior["proximoFinAC2"]);

            rndFinAC3 = null;
            tiempoFinAC3 = null;
            proximoFinAC3 = aDecimal(filaAnterior["proximoFinAC3"]);

            rndFinAC4 = null;
            tiempoFinAC4 = null;
            proximoFinAC4 = aDecimal(filaAnterior["proximoFinAC4"]);


            // Fin atención control comida mayores
            rndFinACM = null;
            tiempoFinACM = null;
            proximoFinACM = null;
            colaComidaMayores = Convert.ToInt32(filaAnterior["colaComidaMayores"]);
            estadoControlComidaMayores = Convert.ToString(filaAnterior["estadoControlComidaMayores"]);

            if (estadoControlComidaMayores == "Infectado")
            {
                throw new Exception("Se terminó de atender a un hombre mayor y el servidor estaba infectado");
            }
            else
            {
                int indiceFinACM = -1;
                if (Convert.ToInt32(filaAnterior["colaComidaMayores"]) > 0)
                {
                    rndFinACM = generarRandom();
                    tiempoFinACM = generarTiempoExponencial(rndFinACM, mediaACM);
                    proximoFinACM = generarProximo(reloj, tiempoFinACM);

                    // Recorremos la grid para encontrar el indice de la persona mayor
                    for (int i = 0; i < grdPersonasMayores.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonasMayores.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt")
                        {
                            indiceFinACM = i;
                            break;
                        }
                    }

                    // Elimino el objeto
                    grdPersonasMayores.Rows.RemoveAt(indiceFinACM);

                    // Recorro y cambio el estado de Encola a SiendoAtendido
                    foreach (DataGridViewRow fila in grdPersonasMayores.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaComidaMayores")
                        {
                            fila.Cells[0].Value = "SiendoAt";
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < grdPersonasMayores.Rows.Count; i++)
                    {
                        DataGridViewRow fila = grdPersonasMayores.Rows[i];
                        if (fila.Cells[0].Value.ToString() == "SiendoAt")
                        {
                            indiceFinACM = i;
                            break;
                        }
                    }
                    ;
                    grdPersonasMayores.Rows.RemoveAt(indiceFinACM);
                }
            }


            // Reanudación Servidor
            s = null;

            tiempoDuracionDetencionServidor = null;

            horaReanudacionServidor = aDecimal(filaAnterior["horaReanudacionServidor"]);


            // Bloqueo de llegadas
            ingresoBloqueado = Convert.ToString(filaAnterior["ingresoBloqueado"]);

            colaBloqueoLlegadas = Convert.ToInt32(filaAnterior["colaBloqueoLlegadas"]);


            // Caja park
            colaPark1 = Convert.ToInt32(filaAnterior["colaPark1"]);
            estadoCajaPark1 = filaAnterior["estadoCajaPark1"].ToString();
            colaPark2 = Convert.ToInt32(filaAnterior["colaPark2"]);
            estadoCajaPark2 = filaAnterior["estadoCajaPark2"].ToString();
            colaPark3 = Convert.ToInt32(filaAnterior["colaPark3"]);
            estadoCajaPark3 = filaAnterior["estadoCajaPark3"].ToString();
            colaPark4 = Convert.ToInt32(filaAnterior["colaPark4"]);
            estadoCajaPark4 = filaAnterior["estadoCajaPark4"].ToString();
            colaPark5 = Convert.ToInt32(filaAnterior["colaPark5"]);
            estadoCajaPark5 = filaAnterior["estadoCajaPark5"].ToString();


            // Caja entrada
            colaEntrada1y2 = Convert.ToInt32(filaAnterior["colaEntrada1y2"]);
            estadoCajaEntrada1 = filaAnterior["estadoCajaEntrada1"].ToString();
            estadoCajaEntrada2 = filaAnterior["estadoCajaEntrada2"].ToString();

            colaEntrada3y4 = Convert.ToInt32(filaAnterior["colaEntrada3y4"]);
            estadoCajaEntrada3 = filaAnterior["estadoCajaEntrada3"].ToString();
            estadoCajaEntrada4 = filaAnterior["estadoCajaEntrada4"].ToString();

            colaEntrada5y6 = Convert.ToInt32(filaAnterior["colaEntrada5y6"]);
            estadoCajaEntrada5 = filaAnterior["estadoCajaEntrada5"].ToString();
            estadoCajaEntrada6 = filaAnterior["estadoCajaEntrada6"].ToString();


            // Control comida
            colaComida1 = Convert.ToInt32(filaAnterior["colaComida1"]);
            estadoControlComida1 = filaAnterior["estadoControlComida1"].ToString();

            colaComida2 = Convert.ToInt32(filaAnterior["colaComida2"]);
            estadoControlComida2 = filaAnterior["estadoControlComida2"].ToString();

            colaComida3 = Convert.ToInt32(filaAnterior["colaComida3"]);
            estadoControlComida3 = filaAnterior["estadoControlComida3"].ToString();

            colaComida4 = Convert.ToInt32(filaAnterior["colaComida4"]);
            estadoControlComida4 = filaAnterior["estadoControlComida4"].ToString();


            // Control comida mayores
            if (colaComidaMayores > 0 && estadoControlComidaMayores != "Infectado")
            {
                colaComidaMayores--;
            }
            else if (estadoControlComidaMayores != "Infectado")
            {
                estadoControlComidaMayores = "Libre";
            }


            // Estadísticas
            metrosPromedioNecesariosParaAparcamiento = aDecimal(filaAnterior["cantidadPromedioAutosEnColaPark"]) * 4;
            acumuladorTiempoColaParking = (reloj - aDecimal(filaAnterior["reloj"])) * (colaPark1 + colaPark2 + colaPark3 + colaPark4 + colaPark5) + aDecimal(filaAnterior["acumuladorTiempoColaParking"]);
            cantidadPromedioAutosEnColaPark = acumuladorTiempoColaParking / reloj;

            contadorGruposCajaEntrada = Convert.ToInt32(filaAnterior["contadorGruposCajaEntrada"]);
            acumuladorTiempoColaEntrada = (reloj - aDecimal(filaAnterior["reloj"])) * (colaEntrada1y2 + colaEntrada3y4 + colaEntrada5y6) + aDecimal(filaAnterior["acumuladorTiempoColaEntrada"]);
            if (contadorGruposCajaEntrada == 0)
            {
                tiempoPromedioEnColaEntrada = 0;
            }
            else
            {
                tiempoPromedioEnColaEntrada = acumuladorTiempoColaEntrada / contadorGruposCajaEntrada;
            }

            contadorPersonasEnControlComida = Convert.ToInt32(filaAnterior["contadorPersonasEnControlComida"]) + 1;
            acumuladorTiempoColaComida = (reloj - aDecimal(filaAnterior["reloj"])) * (colaComida1 + colaComida2 + colaComida3 + colaComida4 + colaComidaMayores) + aDecimal(filaAnterior["acumuladorTiempoColaComida"]);
            if (contadorPersonasEnControlComida == 0)
            {
                tiempoPromedioEnColaComida = 0;
            }
            else
            {
                tiempoPromedioEnColaComida = acumuladorTiempoColaComida / contadorPersonasEnControlComida;
            }

            tiempoEnConseguirEntrada = 300 + tiempoPromedioEnColaEntrada + 92;
            cantidadPromedioGenteEnColaEntrada = (acumuladorTiempoColaEntrada / reloj) * (decimal)4.16;
            tiempoEntradaDespuesDeEstacionar = tiempoEnConseguirEntrada + tiempoPromedioEnColaComida + 5;


            //// Personas Mayores
            //DataGridViewRow borrar = new DataGridViewRow();
            //foreach (DataGridViewRow row in grdPersonasMayores.Rows)
            //{
            //    if (row.Cells[0].Value.ToString() == "SiendoAt")
            //    {
            //        borrar = row;
            //        break;
            //    }
            //}
            ////grdPersonasMayores.Rows.Remove(borrar); REVISAR
            //if (colaComidaMayores != 0)
            //{
            //    foreach (DataGridViewRow row in grdPersonasMayores.Rows)
            //    {
            //        if (row.Cells[0].Value.ToString() == "EnColaMayores")
            //        {
            //            row.Cells[0].Value = "SiendoAt";
            //            break;
            //        }
            //    }
            //}



            // --- Manejo de tabla y próximo evento ---

            // Eliminar fila anterior
            dt.Rows.Remove(filaAnterior);

            // Agregar la nueva fila
            dt.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);

            // Guardar la nueva fila en una variable para enviársela al próximo evento
            DataRow filaActual = dt.Rows[0];

            // Si la simulación está dentro de las que quiere visualizar, agregarla a la grilla
            if ((numeroSimulacionActual >= verDesdeSimulacion && numeroSimulacionActual < verHastaSimulacion) || numeroSimulacionActual == cantidadDeSimulaciones)
            {
                grdSimulacion.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);
            }

            // Determinar el próximo evento
            if (cantidadDeSimulaciones >= numeroSimulacionActual)
            {
                string proxEvento = obtenerProximoEvento(horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6,
                proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor);

                if (proxEvento == "LL Auto")
                {
                    llegadaAuto(filaActual);
                }
                else if (proxEvento == "Fin AP")
                {
                    finAtencionParking(filaActual);
                }
                else if (proxEvento == "Fin AE")
                {
                    finAtencionEntrada(filaActual);
                }
                else if (proxEvento == "Fin AC")
                {
                    finAtencionComida(filaActual);
                }
                else if (proxEvento == "Fin ACM")
                {
                    finAtencionComidaMayores(filaActual);
                }
                else if (proxEvento == "Detención")
                {
                    detencion(filaActual);
                }
                else if (proxEvento == "Reanudación Llegadas")
                {
                    reanudacionLlegadas(filaActual);
                }
                else if (proxEvento == "Reanudación Servidor")
                {
                    reanudacionServidor(filaActual);
                }
                else
                {
                    throw new Exception("No se encontró el evento siguiente desde Fin ACM.");
                }
            }
        }

        public void detencion(DataRow filaAnterior) // Actualizado
        {
            // Marcar número de simulación
            numeroSimulacionActual++;


            // Nombres de las variables (una por cada columna)
            string evento;
            decimal? reloj;

            decimal? rndTipoDetencion;
            string tipoDetencion;
            decimal? beta;
            decimal? tiempoDeDetencion;
            decimal? horaDetencion;

            decimal? rndLlegada;
            decimal? tiempoLlegada;
            decimal? proximaLlegada;

            decimal? l;
            decimal? tiempoDuracionDetencionLlegadas;
            decimal? horaReanudacionLlegadas;

            decimal? rndFinAP;
            decimal? tiempoFinAP;
            decimal? proximoFinAP1;
            decimal? proximoFinAP2;
            decimal? proximoFinAP3;
            decimal? proximoFinAP4;
            decimal? proximoFinAP5;

            decimal? rndFinAE;
            decimal? tiempoFinAE;
            decimal? proximoFinAE1;
            decimal? proximoFinAE2;
            decimal? proximoFinAE3;
            decimal? proximoFinAE4;
            decimal? proximoFinAE5;
            decimal? proximoFinAE6;
            decimal? rndCantidadPersonas;
            int? cantidadPersonas;
            decimal? rndCantidadPersonasMayores;
            int? cantidadPersonasMayores;
            int? cantidadPersonasNoMayores;

            decimal? rndFinAC1;
            decimal? tiempoFinAC1;
            decimal? proximoFinAC1;
            decimal? rndFinAC2;
            decimal? tiempoFinAC2;
            decimal? proximoFinAC2;
            decimal? rndFinAC3;
            decimal? tiempoFinAC3;
            decimal? proximoFinAC3;
            decimal? rndFinAC4;
            decimal? tiempoFinAC4;
            decimal? proximoFinAC4;

            decimal? rndFinACM;
            decimal? tiempoFinACM;
            decimal? proximoFinACM;

            decimal? s;
            decimal? tiempoDuracionDetencionServidor;
            decimal? horaReanudacionServidor;

            string ingresoBloqueado;
            int? colaBloqueoLlegadas;

            int? colaPark1;
            string estadoCajaPark1;
            int? colaPark2;
            string estadoCajaPark2;
            int? colaPark3;
            string estadoCajaPark3;
            int? colaPark4;
            string estadoCajaPark4;
            int? colaPark5;
            string estadoCajaPark5;

            int? colaEntrada1y2;
            string estadoCajaEntrada1;
            string estadoCajaEntrada2;
            int? colaEntrada3y4;
            string estadoCajaEntrada3;
            string estadoCajaEntrada4;
            int? colaEntrada5y6;
            string estadoCajaEntrada5;
            string estadoCajaEntrada6;

            int? colaComida1;
            string estadoControlComida1;
            int? colaComida2;
            string estadoControlComida2;
            int? colaComida3;
            string estadoControlComida3;
            int? colaComida4;
            string estadoControlComida4;

            int? colaComidaMayores;
            string estadoControlComidaMayores;

            decimal? metrosPromedioNecesariosParaAparcamiento;
            decimal? acumuladorTiempoColaParking;
            decimal? cantidadPromedioAutosEnColaPark;
            int? contadorGruposCajaEntrada;
            decimal? acumuladorTiempoColaEntrada;
            decimal? tiempoPromedioEnColaEntrada;
            int? contadorPersonasEnControlComida;
            decimal? acumuladorTiempoColaComida;
            decimal? tiempoPromedioEnColaComida;
            decimal? tiempoEnConseguirEntrada;
            decimal? cantidadPromedioGenteEnColaEntrada;
            decimal? tiempoEntradaDespuesDeEstacionar;


            // Evento
            evento = "Detención";


            // Reloj
            reloj = obtenerHoraProxEvento(aDecimal(filaAnterior["horaDetencion"]), aDecimal(filaAnterior["proximaLlegada"]), aDecimal(filaAnterior["horaReanudacionLlegadas"]),
                aDecimal(filaAnterior["proximoFinAP1"]), aDecimal(filaAnterior["proximoFinAP2"]), aDecimal(filaAnterior["proximoFinAP3"]),
                aDecimal(filaAnterior["proximoFinAP4"]), aDecimal(filaAnterior["proximoFinAP5"]), aDecimal(filaAnterior["proximoFinAE1"]),
                aDecimal(filaAnterior["proximoFinAE2"]), aDecimal(filaAnterior["proximoFinAE3"]),
                aDecimal(filaAnterior["proximoFinAE4"]), aDecimal(filaAnterior["proximoFinAE5"]), aDecimal(filaAnterior["proximoFinAE6"]),
                aDecimal(filaAnterior["proximoFinAC1"]), aDecimal(filaAnterior["proximoFinAC2"]), aDecimal(filaAnterior["proximoFinAC3"]),
                aDecimal(filaAnterior["proximoFinAC4"]),
                aDecimal(filaAnterior["proximoFinACM"]),
                aDecimal(filaAnterior["horaReanudacionServidor"]));


            // Detención
            rndTipoDetencion = null;

            tipoDetencion = filaAnterior["tipoDetencion"].ToString();

            beta = null;

            tiempoDeDetencion = null;

            horaDetencion = null;


            // Llegada auto
            rndLlegada = null;

            tiempoLlegada = null;

            proximaLlegada = aDecimal(filaAnterior["proximaLlegada"]);


            // Reanudación llegadas
            l = null;
            tiempoDuracionDetencionLlegadas = null;
            horaReanudacionLlegadas = null;
            if (tipoDetencion == "Llegadas")
            {
                l = reloj;
                tiempoDuracionDetencionLlegadas = rk.calcularRKReanudacionLlegadas(l, 0, (decimal)0.01, out DataTable dtRKreanudacionLlegadas);
                grdRKReanudacionLlegadas.DataSource = dtRKreanudacionLlegadas;
                horaReanudacionLlegadas = reloj + tiempoDuracionDetencionLlegadas;
            }


            // Fin atención parking
            rndFinAP = null;

            tiempoFinAP = null;

            proximoFinAP1 = aDecimal(filaAnterior["proximoFinAP1"]);
            proximoFinAP2 = aDecimal(filaAnterior["proximoFinAP2"]);
            proximoFinAP3 = aDecimal(filaAnterior["proximoFinAP3"]);
            proximoFinAP4 = aDecimal(filaAnterior["proximoFinAP4"]);
            proximoFinAP5 = aDecimal(filaAnterior["proximoFinAP5"]);


            // Fin atención entrada
            rndFinAE = null;

            tiempoFinAE = null;

            proximoFinAE1 = aDecimal(filaAnterior["proximoFinAE1"]);
            proximoFinAE2 = aDecimal(filaAnterior["proximoFinAE2"]);
            proximoFinAE3 = aDecimal(filaAnterior["proximoFinAE3"]);
            proximoFinAE4 = aDecimal(filaAnterior["proximoFinAE4"]);
            proximoFinAE5 = aDecimal(filaAnterior["proximoFinAE5"]);
            proximoFinAE6 = aDecimal(filaAnterior["proximoFinAE6"]);

            rndCantidadPersonas = null;

            cantidadPersonas = null;

            rndCantidadPersonasMayores = null;

            cantidadPersonasMayores = null;

            cantidadPersonasNoMayores = null;


            // Fin atención control comida
            rndFinAC1 = null;
            tiempoFinAC1 = null;
            proximoFinAC1 = aDecimal(filaAnterior["proximoFinAC1"]);

            rndFinAC2 = null;
            tiempoFinAC2 = null;
            proximoFinAC2 = aDecimal(filaAnterior["proximoFinAC2"]); ;

            rndFinAC3 = null;
            tiempoFinAC3 = null;
            proximoFinAC3 = aDecimal(filaAnterior["proximoFinAC3"]); ;

            rndFinAC4 = null;
            tiempoFinAC4 = null;
            proximoFinAC4 = aDecimal(filaAnterior["proximoFinAC4"]); ;


            // Fin atención control comida mayores
            rndFinACM = null;
            tiempoFinACM = null;
            proximoFinACM = aDecimal(filaAnterior["proximoFinACM"]); ;


            // Reanudación servidor
            s = null;
            tiempoDuracionDetencionServidor = null;
            horaReanudacionServidor = null;
            if (tipoDetencion == "Servidor")
            {
                s = reloj;
                tiempoDuracionDetencionServidor = rk.calcularRKReanudacionServidor(s, 0, (decimal)0.01, out DataTable dtRKreanudacionServidor);
                grdRKReanudacionServidor.DataSource = dtRKreanudacionServidor;
                horaReanudacionServidor = reloj + tiempoDuracionDetencionServidor;
            }


            // Bloqueo de llegadas
            ingresoBloqueado = "No";
            if (tipoDetencion == "Llegadas")
            {
                ingresoBloqueado = "Sí";
            }

            colaBloqueoLlegadas = Convert.ToInt32(filaAnterior["colaBloqueoLlegadas"]);


            // Caja park
            colaPark1 = Convert.ToInt32(filaAnterior["colaPark1"]);
            estadoCajaPark1 = filaAnterior["estadoCajaPark1"].ToString();
            colaPark2 = Convert.ToInt32(filaAnterior["colaPark2"]);
            estadoCajaPark2 = filaAnterior["estadoCajaPark2"].ToString();
            colaPark3 = Convert.ToInt32(filaAnterior["colaPark3"]);
            estadoCajaPark3 = filaAnterior["estadoCajaPark3"].ToString();
            colaPark4 = Convert.ToInt32(filaAnterior["colaPark4"]);
            estadoCajaPark4 = filaAnterior["estadoCajaPark4"].ToString();
            colaPark5 = Convert.ToInt32(filaAnterior["colaPark5"]);
            estadoCajaPark5 = filaAnterior["estadoCajaPark5"].ToString();


            // Caja entrada
            colaEntrada1y2 = Convert.ToInt32(filaAnterior["colaEntrada1y2"]);
            estadoCajaEntrada1 = filaAnterior["estadoCajaEntrada1"].ToString();
            estadoCajaEntrada2 = filaAnterior["estadoCajaEntrada2"].ToString();

            colaEntrada3y4 = Convert.ToInt32(filaAnterior["colaEntrada3y4"]);
            estadoCajaEntrada3 = filaAnterior["estadoCajaEntrada3"].ToString();
            estadoCajaEntrada4 = filaAnterior["estadoCajaEntrada4"].ToString();

            colaEntrada5y6 = Convert.ToInt32(filaAnterior["colaEntrada5y6"]);
            estadoCajaEntrada5 = filaAnterior["estadoCajaEntrada5"].ToString();
            estadoCajaEntrada6 = filaAnterior["estadoCajaEntrada6"].ToString();


            // Control comida
            colaComida1 = Convert.ToInt32(filaAnterior["colaComida1"]);
            estadoControlComida1 = filaAnterior["estadoControlComida1"].ToString();

            colaComida2 = Convert.ToInt32(filaAnterior["colaComida2"]);
            estadoControlComida2 = filaAnterior["estadoControlComida2"].ToString();

            colaComida3 = Convert.ToInt32(filaAnterior["colaComida3"]);
            estadoControlComida3 = filaAnterior["estadoControlComida3"].ToString();

            colaComida4 = Convert.ToInt32(filaAnterior["colaComida4"]);
            estadoControlComida4 = filaAnterior["estadoControlComida4"].ToString();


            // Control comida mayores
            colaComidaMayores = Convert.ToInt32(filaAnterior["colaComidaMayores"]);
            estadoControlComidaMayores = filaAnterior["estadoControlComidaMayores"].ToString();

            if (tipoDetencion == "Servidor")
            {
                foreach (DataGridViewRow fila in grdPersonasMayores.Rows)
                {
                    // Verificar si hay alguien siendo atendido y devolverlo a la cola
                    if (fila.Cells[0].Value.ToString() == "SiendoAt")
                    {
                        proximoFinACM = 0;
                        fila.Cells[0].Value = "EnColaComidaMayores";
                    }
                    colaComidaMayores++;
                }
            }


            // Estadísticas
            metrosPromedioNecesariosParaAparcamiento = aDecimal(filaAnterior["cantidadPromedioAutosEnColaPark"]) * 4;
            acumuladorTiempoColaParking = (reloj - aDecimal(filaAnterior["reloj"])) * (colaPark1 + colaPark2 + colaPark3 + colaPark4 + colaPark5) + aDecimal(filaAnterior["acumuladorTiempoColaParking"]);
            cantidadPromedioAutosEnColaPark = acumuladorTiempoColaParking / reloj;

            contadorGruposCajaEntrada = Convert.ToInt32(filaAnterior["contadorGruposCajaEntrada"]);
            acumuladorTiempoColaEntrada = (reloj - aDecimal(filaAnterior["reloj"])) * (colaEntrada1y2 + colaEntrada3y4 + colaEntrada5y6) + aDecimal(filaAnterior["acumuladorTiempoColaEntrada"]);
            if (contadorGruposCajaEntrada == 0)
            {
                tiempoPromedioEnColaEntrada = 0;
            }
            else
            {
                tiempoPromedioEnColaEntrada = acumuladorTiempoColaEntrada / contadorGruposCajaEntrada;
            }

            contadorPersonasEnControlComida = Convert.ToInt32(filaAnterior["contadorPersonasEnControlComida"]);
            acumuladorTiempoColaComida = (reloj - aDecimal(filaAnterior["reloj"])) * (colaComida1 + colaComida2 + colaComida3 + colaComida4 + colaComidaMayores) + aDecimal(filaAnterior["acumuladorTiempoColaComida"]);
            if (contadorPersonasEnControlComida == 0)
            {
                tiempoPromedioEnColaComida = 0;
            }
            else
            {
                tiempoPromedioEnColaComida = acumuladorTiempoColaComida / contadorPersonasEnControlComida;
            }

            tiempoEnConseguirEntrada = 300 + tiempoPromedioEnColaEntrada + 92;
            cantidadPromedioGenteEnColaEntrada = (acumuladorTiempoColaEntrada / reloj) * (decimal)4.16;
            tiempoEntradaDespuesDeEstacionar = tiempoEnConseguirEntrada + tiempoPromedioEnColaComida + 5;



            // --- Manejo de tabla y próximo evento ---

            // Eliminar fila anterior
            dt.Rows.Remove(filaAnterior);

            // Agregar la nueva fila
            dt.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);

            // Guardar la nueva fila en una variable para enviársela al próximo evento
            DataRow filaActual = dt.Rows[0];

            // Si la simulación está dentro de las que quiere visualizar, agregarla a la grilla
            if ((numeroSimulacionActual >= verDesdeSimulacion && numeroSimulacionActual < verHastaSimulacion) || numeroSimulacionActual == cantidadDeSimulaciones)
            {
                grdSimulacion.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);
            }

            // Determinar el próximo evento
            if (cantidadDeSimulaciones >= numeroSimulacionActual)
            {
                string proxEvento = obtenerProximoEvento(horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6,
    proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor);

                if (proxEvento == "LL Auto")
                {
                    llegadaAuto(filaActual);
                }
                else if (proxEvento == "Fin AP")
                {
                    finAtencionParking(filaActual);
                }
                else if (proxEvento == "Fin AE")
                {
                    finAtencionEntrada(filaActual);
                }
                else if (proxEvento == "Fin AC")
                {
                    finAtencionComida(filaActual);
                }
                else if (proxEvento == "Fin ACM")
                {
                    finAtencionComidaMayores(filaActual);
                }
                else if (proxEvento == "Detención")
                {
                    detencion(filaActual);
                }
                else if (proxEvento == "Reanudación Llegadas")
                {
                    reanudacionLlegadas(filaActual);
                }
                else if (proxEvento == "Reanudación Servidor")
                {
                    reanudacionServidor(filaActual);
                }
                else
                {
                    throw new Exception("No se encontró el evento siguiente desde Detención");
                }
            }
        }

        public void reanudacionLlegadas(DataRow filaAnterior) // Actualizado
        {
            // Marcar número de simulación
            numeroSimulacionActual++;


            // Nombres de las variables (una por cada columna)
            string evento;
            decimal? reloj;

            decimal? rndTipoDetencion;
            string tipoDetencion;
            decimal? beta;
            decimal? tiempoDeDetencion;
            decimal? horaDetencion;

            decimal? rndLlegada;
            decimal? tiempoLlegada;
            decimal? proximaLlegada;

            decimal? l;
            decimal? tiempoDuracionDetencionLlegadas;
            decimal? horaReanudacionLlegadas;

            decimal? rndFinAP;
            decimal? tiempoFinAP;
            decimal? proximoFinAP1;
            decimal? proximoFinAP2;
            decimal? proximoFinAP3;
            decimal? proximoFinAP4;
            decimal? proximoFinAP5;

            decimal? rndFinAE;
            decimal? tiempoFinAE;
            decimal? proximoFinAE1;
            decimal? proximoFinAE2;
            decimal? proximoFinAE3;
            decimal? proximoFinAE4;
            decimal? proximoFinAE5;
            decimal? proximoFinAE6;
            decimal? rndCantidadPersonas;
            int? cantidadPersonas;
            decimal? rndCantidadPersonasMayores;
            int? cantidadPersonasMayores;
            int? cantidadPersonasNoMayores;

            decimal? rndFinAC1;
            decimal? tiempoFinAC1;
            decimal? proximoFinAC1;
            decimal? rndFinAC2;
            decimal? tiempoFinAC2;
            decimal? proximoFinAC2;
            decimal? rndFinAC3;
            decimal? tiempoFinAC3;
            decimal? proximoFinAC3;
            decimal? rndFinAC4;
            decimal? tiempoFinAC4;
            decimal? proximoFinAC4;

            decimal? rndFinACM;
            decimal? tiempoFinACM;
            decimal? proximoFinACM;

            decimal? s;
            decimal? tiempoDuracionDetencionServidor;
            decimal? horaReanudacionServidor;

            string ingresoBloqueado;
            int? colaBloqueoLlegadas;

            int? colaPark1;
            string estadoCajaPark1;
            int? colaPark2;
            string estadoCajaPark2;
            int? colaPark3;
            string estadoCajaPark3;
            int? colaPark4;
            string estadoCajaPark4;
            int? colaPark5;
            string estadoCajaPark5;

            int? colaEntrada1y2;
            string estadoCajaEntrada1;
            string estadoCajaEntrada2;
            int? colaEntrada3y4;
            string estadoCajaEntrada3;
            string estadoCajaEntrada4;
            int? colaEntrada5y6;
            string estadoCajaEntrada5;
            string estadoCajaEntrada6;

            int? colaComida1;
            string estadoControlComida1;
            int? colaComida2;
            string estadoControlComida2;
            int? colaComida3;
            string estadoControlComida3;
            int? colaComida4;
            string estadoControlComida4;

            int? colaComidaMayores;
            string estadoControlComidaMayores;

            decimal? metrosPromedioNecesariosParaAparcamiento;
            decimal? acumuladorTiempoColaParking;
            decimal? cantidadPromedioAutosEnColaPark;
            int? contadorGruposCajaEntrada;
            decimal? acumuladorTiempoColaEntrada;
            decimal? tiempoPromedioEnColaEntrada;
            int? contadorPersonasEnControlComida;
            decimal? acumuladorTiempoColaComida;
            decimal? tiempoPromedioEnColaComida;
            decimal? tiempoEnConseguirEntrada;
            decimal? cantidadPromedioGenteEnColaEntrada;
            decimal? tiempoEntradaDespuesDeEstacionar;


            // Evento
            evento = "Reanudación Llegadas";


            // Reloj
            reloj = obtenerHoraProxEvento(aDecimal(filaAnterior["horaDetencion"]), aDecimal(filaAnterior["proximaLlegada"]), aDecimal(filaAnterior["horaReanudacionLlegadas"]),
                aDecimal(filaAnterior["proximoFinAP1"]), aDecimal(filaAnterior["proximoFinAP2"]), aDecimal(filaAnterior["proximoFinAP3"]),
                aDecimal(filaAnterior["proximoFinAP4"]), aDecimal(filaAnterior["proximoFinAP5"]), aDecimal(filaAnterior["proximoFinAE1"]),
                aDecimal(filaAnterior["proximoFinAE2"]), aDecimal(filaAnterior["proximoFinAE3"]),
                aDecimal(filaAnterior["proximoFinAE4"]), aDecimal(filaAnterior["proximoFinAE5"]), aDecimal(filaAnterior["proximoFinAE6"]),
                aDecimal(filaAnterior["proximoFinAC1"]), aDecimal(filaAnterior["proximoFinAC2"]), aDecimal(filaAnterior["proximoFinAC3"]),
                aDecimal(filaAnterior["proximoFinAC4"]),
                aDecimal(filaAnterior["proximoFinACM"]),
                aDecimal(filaAnterior["horaReanudacionServidor"]));


            // Detención
            rndTipoDetencion = generarRandom();

            if (rndTipoDetencion < aDecimal(0.35))
            {
                tipoDetencion = "Llegadas";
            }
            else
            {
                tipoDetencion = "Servidor";
            }

            beta = generarRandom();

            tiempoDeDetencion = rk.calcularRKDetencion(tipoDetencion, (decimal)111.9339, beta, (decimal)0.01, 0, out DataTable dtRKdetencion);
            grdRKDetencion.DataSource = dtRKdetencion;

            horaDetencion = tiempoDeDetencion + reloj;


            // Llegada auto
            rndLlegada = null;

            tiempoLlegada = null;

            proximaLlegada = aDecimal(filaAnterior["proximaLlegada"]);


            // Reanudación Llegadas
            l = null;
            tiempoDuracionDetencionLlegadas = null;
            horaReanudacionLlegadas = null;


            // Fin atención parking
            rndFinAP = null;

            tiempoFinAP = null;

            proximoFinAP1 = aDecimal(filaAnterior["proximoFinAP1"]);
            proximoFinAP2 = aDecimal(filaAnterior["proximoFinAP2"]);
            proximoFinAP3 = aDecimal(filaAnterior["proximoFinAP3"]);
            proximoFinAP4 = aDecimal(filaAnterior["proximoFinAP4"]);
            proximoFinAP5 = aDecimal(filaAnterior["proximoFinAP5"]);


            // Fin atención entrada
            rndFinAE = null;

            tiempoFinAE = null;

            proximoFinAE1 = aDecimal(filaAnterior["proximoFinAE1"]);
            proximoFinAE2 = aDecimal(filaAnterior["proximoFinAE2"]);
            proximoFinAE3 = aDecimal(filaAnterior["proximoFinAE3"]);
            proximoFinAE4 = aDecimal(filaAnterior["proximoFinAE4"]);
            proximoFinAE5 = aDecimal(filaAnterior["proximoFinAE5"]);
            proximoFinAE6 = aDecimal(filaAnterior["proximoFinAE6"]);

            rndCantidadPersonas = null;

            cantidadPersonas = null;

            rndCantidadPersonasMayores = null;

            cantidadPersonasMayores = null;

            cantidadPersonasNoMayores = null;


            // Fin atención control comida
            rndFinAC1 = null;
            tiempoFinAC1 = null;
            proximoFinAC1 = aDecimal(filaAnterior["proximoFinAC1"]);

            rndFinAC2 = null;
            tiempoFinAC2 = null;
            proximoFinAC2 = aDecimal(filaAnterior["proximoFinAC2"]); ;

            rndFinAC3 = null;
            tiempoFinAC3 = null;
            proximoFinAC3 = aDecimal(filaAnterior["proximoFinAC3"]); ;

            rndFinAC4 = null;
            tiempoFinAC4 = null;
            proximoFinAC4 = aDecimal(filaAnterior["proximoFinAC4"]); ;


            // Fin atención control comida mayores
            rndFinACM = null;
            tiempoFinACM = null;
            proximoFinACM = aDecimal(filaAnterior["proximoFinACM"]); ;


            // Reanudación servidor
            s = null;
            tiempoDuracionDetencionServidor = null;
            horaReanudacionServidor = null;


            // Bloqueo de llegadas
            ingresoBloqueado = "No";

            colaBloqueoLlegadas = Convert.ToInt32(filaAnterior["colaBloqueoLlegadas"]);


            // Caja park
            colaPark1 = Convert.ToInt32(filaAnterior["colaPark1"]);
            estadoCajaPark1 = filaAnterior["estadoCajaPark1"].ToString();
            colaPark2 = Convert.ToInt32(filaAnterior["colaPark2"]);
            estadoCajaPark2 = filaAnterior["estadoCajaPark2"].ToString();
            colaPark3 = Convert.ToInt32(filaAnterior["colaPark3"]);
            estadoCajaPark3 = filaAnterior["estadoCajaPark3"].ToString();
            colaPark4 = Convert.ToInt32(filaAnterior["colaPark4"]);
            estadoCajaPark4 = filaAnterior["estadoCajaPark4"].ToString();
            colaPark5 = Convert.ToInt32(filaAnterior["colaPark5"]);
            estadoCajaPark5 = filaAnterior["estadoCajaPark5"].ToString();

            // Acá va a haber que repartir a todos los autos trabados por el POLO OBRERO entre todas las cajas y colas
            int i = 0;
            while (colaBloqueoLlegadas > 0)
            {
                // Preguntar si hay algún servidor libre
                if (estadoCajaPark1 == "Libre")
                {
                    decimal? rnd = generarRandom();
                    decimal? tiempo = generarTiempoExponencial(rnd, mediaAP);
                    proximoFinAP1 = generarProximo(reloj, tiempo);

                    estadoCajaPark1 = "Ocupado";
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                        {
                            fila.Cells[0].Value = "SiendoAt1";
                            break;
                        }
                    }
                    colaBloqueoLlegadas--;
                }
                else if (estadoCajaPark2 == "Libre")
                {
                    decimal? rnd = generarRandom();
                    decimal? tiempo = generarTiempoExponencial(rnd, mediaAP);
                    proximoFinAP2 = generarProximo(reloj, tiempo);

                    estadoCajaPark2 = "Ocupado";
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                        {
                            fila.Cells[0].Value = "SiendoAt2";
                            break;
                        }
                    }
                    colaBloqueoLlegadas--;
                }
                else if (estadoCajaPark3 == "Libre")
                {
                    decimal? rnd = generarRandom();
                    decimal? tiempo = generarTiempoExponencial(rnd, mediaAP);
                    proximoFinAP3 = generarProximo(reloj, tiempo);

                    estadoCajaPark3 = "Ocupado";
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                        {
                            fila.Cells[0].Value = "SiendoAt3";
                            break;
                        }
                    }
                    colaBloqueoLlegadas--;
                }
                else if (estadoCajaPark4 == "Libre")
                {
                    decimal? rnd = generarRandom();
                    decimal? tiempo = generarTiempoExponencial(rnd, mediaAP);
                    proximoFinAP4 = generarProximo(reloj, tiempo);

                    estadoCajaPark4 = "Ocupado";
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                        {
                            fila.Cells[0].Value = "SiendoAt4";
                            break;
                        }
                    }
                    colaBloqueoLlegadas--;
                }
                else if (estadoCajaPark2 == "Libre")
                {
                    decimal? rnd = generarRandom();
                    decimal? tiempo = generarTiempoExponencial(rnd, mediaAP);
                    proximoFinAP5 = generarProximo(reloj, tiempo);

                    estadoCajaPark5 = "Ocupado";
                    foreach (DataGridViewRow fila in grdAutos.Rows)
                    {
                        if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                        {
                            fila.Cells[0].Value = "SiendoAt5";
                            break;
                        }
                    }
                    colaBloqueoLlegadas--;
                }
                else // Si todas las Cajas park están ocupadas, entrar a la cola más chica
                {
                    int? colaMasChica = obtenerColaMenorParking(colaPark1, colaPark2, colaPark3, colaPark4, colaPark5);

                    if (colaMasChica == colaPark1)
                    {
                        colaPark1++;
                        foreach (DataGridViewRow fila in grdAutos.Rows)
                        {
                            if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                            {
                                fila.Cells[0].Value = "EnColaPark1";
                                break;
                            }
                        }
                        colaBloqueoLlegadas--;
                    }
                    else if (colaMasChica == colaPark2)
                    {
                        colaPark2++;
                        foreach (DataGridViewRow fila in grdAutos.Rows)
                        {
                            if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                            {
                                fila.Cells[0].Value = "EnColaPark2";
                                break;
                            }
                        }
                        colaBloqueoLlegadas--;
                    }
                    else if (colaMasChica == colaPark3)
                    {
                        colaPark3++;
                        foreach (DataGridViewRow fila in grdAutos.Rows)
                        {
                            if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                            {
                                fila.Cells[0].Value = "EnColaPark3";
                                break;
                            }
                        }
                        colaBloqueoLlegadas--;
                    }
                    else if (colaMasChica == colaPark4)
                    {
                        colaPark4++;
                        foreach (DataGridViewRow fila in grdAutos.Rows)
                        {
                            if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                            {
                                fila.Cells[0].Value = "EnColaPark4";
                                break;
                            }
                        }
                        colaBloqueoLlegadas--;
                    }
                    else if (colaMasChica == colaPark5)
                    {
                        colaPark5++;
                        foreach (DataGridViewRow fila in grdAutos.Rows)
                        {
                            if (fila.Cells[0].Value.ToString() == "EnColaBloqueo")
                            {
                                fila.Cells[0].Value = "EnColaPark2";
                                break;
                            }
                        }
                        colaBloqueoLlegadas--;
                    }
                }

                i++;
                if (i == 999999)
                {
                    throw new Exception("Este ciclo nunca corta");
                }
            }


            // Caja entrada
            colaEntrada1y2 = Convert.ToInt32(filaAnterior["colaEntrada1y2"]);
            estadoCajaEntrada1 = filaAnterior["estadoCajaEntrada1"].ToString();
            estadoCajaEntrada2 = filaAnterior["estadoCajaEntrada2"].ToString();

            colaEntrada3y4 = Convert.ToInt32(filaAnterior["colaEntrada3y4"]);
            estadoCajaEntrada3 = filaAnterior["estadoCajaEntrada3"].ToString();
            estadoCajaEntrada4 = filaAnterior["estadoCajaEntrada4"].ToString();

            colaEntrada5y6 = Convert.ToInt32(filaAnterior["colaEntrada5y6"]);
            estadoCajaEntrada5 = filaAnterior["estadoCajaEntrada5"].ToString();
            estadoCajaEntrada6 = filaAnterior["estadoCajaEntrada6"].ToString();


            // Control comida
            colaComida1 = Convert.ToInt32(filaAnterior["colaComida1"]);
            estadoControlComida1 = filaAnterior["estadoControlComida1"].ToString();

            colaComida2 = Convert.ToInt32(filaAnterior["colaComida2"]);
            estadoControlComida2 = filaAnterior["estadoControlComida2"].ToString();

            colaComida3 = Convert.ToInt32(filaAnterior["colaComida3"]);
            estadoControlComida3 = filaAnterior["estadoControlComida3"].ToString();

            colaComida4 = Convert.ToInt32(filaAnterior["colaComida4"]);
            estadoControlComida4 = filaAnterior["estadoControlComida4"].ToString();


            // Control comida mayores
            colaComidaMayores = Convert.ToInt32(filaAnterior["colaComidaMayores"]);
            estadoControlComidaMayores = filaAnterior["estadoControlComidaMayores"].ToString();


            // Estadísticas
            metrosPromedioNecesariosParaAparcamiento = aDecimal(filaAnterior["cantidadPromedioAutosEnColaPark"]) * 4;
            acumuladorTiempoColaParking = (reloj - aDecimal(filaAnterior["reloj"])) * (colaPark1 + colaPark2 + colaPark3 + colaPark4 + colaPark5) + aDecimal(filaAnterior["acumuladorTiempoColaParking"]);
            cantidadPromedioAutosEnColaPark = acumuladorTiempoColaParking / reloj;

            contadorGruposCajaEntrada = Convert.ToInt32(filaAnterior["contadorGruposCajaEntrada"]);
            acumuladorTiempoColaEntrada = (reloj - aDecimal(filaAnterior["reloj"])) * (colaEntrada1y2 + colaEntrada3y4 + colaEntrada5y6) + aDecimal(filaAnterior["acumuladorTiempoColaEntrada"]);
            if (contadorGruposCajaEntrada == 0)
            {
                tiempoPromedioEnColaEntrada = 0;
            }
            else
            {
                tiempoPromedioEnColaEntrada = acumuladorTiempoColaEntrada / contadorGruposCajaEntrada;
            }

            contadorPersonasEnControlComida = Convert.ToInt32(filaAnterior["contadorPersonasEnControlComida"]);
            acumuladorTiempoColaComida = (reloj - aDecimal(filaAnterior["reloj"])) * (colaComida1 + colaComida2 + colaComida3 + colaComida4 + colaComidaMayores) + aDecimal(filaAnterior["acumuladorTiempoColaComida"]);
            if (contadorPersonasEnControlComida == 0)
            {
                tiempoPromedioEnColaComida = 0;
            }
            else
            {
                tiempoPromedioEnColaComida = acumuladorTiempoColaComida / contadorPersonasEnControlComida;
            }

            tiempoEnConseguirEntrada = 300 + tiempoPromedioEnColaEntrada + 92;
            cantidadPromedioGenteEnColaEntrada = (acumuladorTiempoColaEntrada / reloj) * (decimal)4.16;
            tiempoEntradaDespuesDeEstacionar = tiempoEnConseguirEntrada + tiempoPromedioEnColaComida + 5;



            // --- Manejo de tabla y próximo evento ---

            // Eliminar fila anterior
            dt.Rows.Remove(filaAnterior);

            // Agregar la nueva fila
            dt.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);

            // Guardar la nueva fila en una variable para enviársela al próximo evento
            DataRow filaActual = dt.Rows[0];

            // Si la simulación está dentro de las que quiere visualizar, agregarla a la grilla
            if ((numeroSimulacionActual >= verDesdeSimulacion && numeroSimulacionActual < verHastaSimulacion) || numeroSimulacionActual == cantidadDeSimulaciones)
            {
                grdSimulacion.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);
            }

            // Determinar el próximo evento
            if (cantidadDeSimulaciones >= numeroSimulacionActual)
            {
                string proxEvento = obtenerProximoEvento(horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6,
    proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor);

                if (proxEvento == "LL Auto")
                {
                    llegadaAuto(filaActual);
                }
                else if (proxEvento == "Fin AP")
                {
                    finAtencionParking(filaActual);
                }
                else if (proxEvento == "Fin AE")
                {
                    finAtencionEntrada(filaActual);
                }
                else if (proxEvento == "Fin AC")
                {
                    finAtencionComida(filaActual);
                }
                else if (proxEvento == "Fin ACM")
                {
                    finAtencionComidaMayores(filaActual);
                }
                else if (proxEvento == "Detención")
                {
                    detencion(filaActual);
                }
                else if (proxEvento == "Reanudación Llegadas")
                {
                    reanudacionLlegadas(filaActual);
                }
                else if (proxEvento == "Reanudación Servidor")
                {
                    reanudacionServidor(filaActual);
                }
                else
                {
                    throw new Exception("No se encontró el evento siguiente desde Detención.");
                }
            }
        }

        public void reanudacionServidor(DataRow filaAnterior) // Actualizado
        {
            // Marcar número de simulación
            numeroSimulacionActual++;


            // Nombres de las variables (una por cada columna)
            string evento;
            decimal? reloj;

            decimal? rndTipoDetencion;
            string tipoDetencion;
            decimal? beta;
            decimal? tiempoDeDetencion;
            decimal? horaDetencion;

            decimal? rndLlegada;
            decimal? tiempoLlegada;
            decimal? proximaLlegada;

            decimal? l;
            decimal? tiempoDuracionDetencionLlegadas;
            decimal? horaReanudacionLlegadas;

            decimal? rndFinAP;
            decimal? tiempoFinAP;
            decimal? proximoFinAP1;
            decimal? proximoFinAP2;
            decimal? proximoFinAP3;
            decimal? proximoFinAP4;
            decimal? proximoFinAP5;

            decimal? rndFinAE;
            decimal? tiempoFinAE;
            decimal? proximoFinAE1;
            decimal? proximoFinAE2;
            decimal? proximoFinAE3;
            decimal? proximoFinAE4;
            decimal? proximoFinAE5;
            decimal? proximoFinAE6;
            decimal? rndCantidadPersonas;
            int? cantidadPersonas;
            decimal? rndCantidadPersonasMayores;
            int? cantidadPersonasMayores;
            int? cantidadPersonasNoMayores;

            decimal? rndFinAC1;
            decimal? tiempoFinAC1;
            decimal? proximoFinAC1;
            decimal? rndFinAC2;
            decimal? tiempoFinAC2;
            decimal? proximoFinAC2;
            decimal? rndFinAC3;
            decimal? tiempoFinAC3;
            decimal? proximoFinAC3;
            decimal? rndFinAC4;
            decimal? tiempoFinAC4;
            decimal? proximoFinAC4;

            decimal? rndFinACM;
            decimal? tiempoFinACM;
            decimal? proximoFinACM;

            decimal? s;
            decimal? tiempoDuracionDetencionServidor;
            decimal? horaReanudacionServidor;

            string ingresoBloqueado;
            int? colaBloqueoLlegadas;

            int? colaPark1;
            string estadoCajaPark1;
            int? colaPark2;
            string estadoCajaPark2;
            int? colaPark3;
            string estadoCajaPark3;
            int? colaPark4;
            string estadoCajaPark4;
            int? colaPark5;
            string estadoCajaPark5;

            int? colaEntrada1y2;
            string estadoCajaEntrada1;
            string estadoCajaEntrada2;
            int? colaEntrada3y4;
            string estadoCajaEntrada3;
            string estadoCajaEntrada4;
            int? colaEntrada5y6;
            string estadoCajaEntrada5;
            string estadoCajaEntrada6;

            int? colaComida1;
            string estadoControlComida1;
            int? colaComida2;
            string estadoControlComida2;
            int? colaComida3;
            string estadoControlComida3;
            int? colaComida4;
            string estadoControlComida4;

            int? colaComidaMayores;
            string estadoControlComidaMayores;

            decimal? metrosPromedioNecesariosParaAparcamiento;
            decimal? acumuladorTiempoColaParking;
            decimal? cantidadPromedioAutosEnColaPark;
            int? contadorGruposCajaEntrada;
            decimal? acumuladorTiempoColaEntrada;
            decimal? tiempoPromedioEnColaEntrada;
            int? contadorPersonasEnControlComida;
            decimal? acumuladorTiempoColaComida;
            decimal? tiempoPromedioEnColaComida;
            decimal? tiempoEnConseguirEntrada;
            decimal? cantidadPromedioGenteEnColaEntrada;
            decimal? tiempoEntradaDespuesDeEstacionar;


            // Evento
            evento = "Reanudación Servidor";


            // Reloj
            reloj = obtenerHoraProxEvento(aDecimal(filaAnterior["horaDetencion"]), aDecimal(filaAnterior["proximaLlegada"]), aDecimal(filaAnterior["horaReanudacionLlegadas"]),
                aDecimal(filaAnterior["proximoFinAP1"]), aDecimal(filaAnterior["proximoFinAP2"]), aDecimal(filaAnterior["proximoFinAP3"]),
                aDecimal(filaAnterior["proximoFinAP4"]), aDecimal(filaAnterior["proximoFinAP5"]), aDecimal(filaAnterior["proximoFinAE1"]),
                aDecimal(filaAnterior["proximoFinAE2"]), aDecimal(filaAnterior["proximoFinAE3"]),
                aDecimal(filaAnterior["proximoFinAE4"]), aDecimal(filaAnterior["proximoFinAE5"]), aDecimal(filaAnterior["proximoFinAE6"]),
                aDecimal(filaAnterior["proximoFinAC1"]), aDecimal(filaAnterior["proximoFinAC2"]), aDecimal(filaAnterior["proximoFinAC3"]),
                aDecimal(filaAnterior["proximoFinAC4"]),
                aDecimal(filaAnterior["proximoFinACM"]),
                aDecimal(filaAnterior["horaReanudacionServidor"]));


            // Detención
            rndTipoDetencion = generarRandom();

            if (rndTipoDetencion < aDecimal(0.35))
            {
                tipoDetencion = "Llegadas";
            }
            else
            {
                tipoDetencion = "Servidor";
            }

            beta = generarRandom();

            tiempoDeDetencion = rk.calcularRKDetencion(tipoDetencion, (decimal)111.9339, beta, (decimal)0.01, 0, out DataTable dtRKdetencion);
            grdRKDetencion.DataSource = dtRKdetencion;

            horaDetencion = tiempoDeDetencion + reloj;


            // Llegada auto
            rndLlegada = null;

            tiempoLlegada = null;

            proximaLlegada = aDecimal(filaAnterior["proximaLlegada"]);


            // Reanudación llegadas
            l = null;

            tiempoDuracionDetencionLlegadas = null;

            horaReanudacionLlegadas = null;


            // Fin atención parking
            rndFinAP = null;

            tiempoFinAP = null;

            proximoFinAP1 = aDecimal(filaAnterior["proximoFinAP1"]);
            proximoFinAP2 = aDecimal(filaAnterior["proximoFinAP2"]);
            proximoFinAP3 = aDecimal(filaAnterior["proximoFinAP3"]);
            proximoFinAP4 = aDecimal(filaAnterior["proximoFinAP4"]);
            proximoFinAP5 = aDecimal(filaAnterior["proximoFinAP5"]);


            // Fin atención entrada
            rndFinAE = null;

            tiempoFinAE = null;

            proximoFinAE1 = aDecimal(filaAnterior["proximoFinAE1"]);
            proximoFinAE2 = aDecimal(filaAnterior["proximoFinAE2"]);
            proximoFinAE3 = aDecimal(filaAnterior["proximoFinAE3"]);
            proximoFinAE4 = aDecimal(filaAnterior["proximoFinAE4"]);
            proximoFinAE5 = aDecimal(filaAnterior["proximoFinAE5"]);
            proximoFinAE6 = aDecimal(filaAnterior["proximoFinAE6"]);

            rndCantidadPersonas = null;

            cantidadPersonas = null;

            rndCantidadPersonasMayores = null;

            cantidadPersonasMayores = null;

            cantidadPersonasNoMayores = null;


            // Fin atención control comida
            rndFinAC1 = null;
            tiempoFinAC1 = null;
            proximoFinAC1 = aDecimal(filaAnterior["proximoFinAC1"]);

            rndFinAC2 = null;
            tiempoFinAC2 = null;
            proximoFinAC2 = aDecimal(filaAnterior["proximoFinAC2"]); ;

            rndFinAC3 = null;
            tiempoFinAC3 = null;
            proximoFinAC3 = aDecimal(filaAnterior["proximoFinAC3"]); ;

            rndFinAC4 = null;
            tiempoFinAC4 = null;
            proximoFinAC4 = aDecimal(filaAnterior["proximoFinAC4"]); ;


            // Fin atención control comida mayores
            rndFinACM = null;
            tiempoFinACM = null;
            proximoFinACM = null;

            if (Convert.ToInt32(filaAnterior["colaComidaMayores"]) > 0)
            {
                rndFinACM = generarRandom();
                tiempoFinACM = generarTiempoExponencial(rndFinACM, mediaACM);
                proximoFinACM = generarProximo(reloj, tiempoFinACM);

                // Recorro y cambio el estado de EnCola a SiendoAt
                foreach (DataGridViewRow fila in grdPersonasMayores.Rows)
                {
                    if (fila.Cells[0].Value.ToString() == "EnColaComidaMayores")
                    {
                        fila.Cells[0].Value = "SiendoAt";
                        break;
                    }
                }
            }


            // Reanudación servidor
            s = null;
            tiempoDuracionDetencionServidor = null;
            horaReanudacionServidor = null;


            // Bloqueo de llegadas
            ingresoBloqueado = Convert.ToString(filaAnterior["ingresoBloqueado"]);
            colaBloqueoLlegadas = Convert.ToInt32(filaAnterior["colaBloqueoLlegadas"]);


            // Caja park
            colaPark1 = Convert.ToInt32(filaAnterior["colaPark1"]);
            estadoCajaPark1 = filaAnterior["estadoCajaPark1"].ToString();
            colaPark2 = Convert.ToInt32(filaAnterior["colaPark2"]);
            estadoCajaPark2 = filaAnterior["estadoCajaPark2"].ToString();
            colaPark3 = Convert.ToInt32(filaAnterior["colaPark3"]);
            estadoCajaPark3 = filaAnterior["estadoCajaPark3"].ToString();
            colaPark4 = Convert.ToInt32(filaAnterior["colaPark4"]);
            estadoCajaPark4 = filaAnterior["estadoCajaPark4"].ToString();
            colaPark5 = Convert.ToInt32(filaAnterior["colaPark5"]);
            estadoCajaPark5 = filaAnterior["estadoCajaPark5"].ToString();


            // Caja entrada
            colaEntrada1y2 = Convert.ToInt32(filaAnterior["colaEntrada1y2"]);
            estadoCajaEntrada1 = filaAnterior["estadoCajaEntrada1"].ToString();
            estadoCajaEntrada2 = filaAnterior["estadoCajaEntrada2"].ToString();

            colaEntrada3y4 = Convert.ToInt32(filaAnterior["colaEntrada3y4"]);
            estadoCajaEntrada3 = filaAnterior["estadoCajaEntrada3"].ToString();
            estadoCajaEntrada4 = filaAnterior["estadoCajaEntrada4"].ToString();

            colaEntrada5y6 = Convert.ToInt32(filaAnterior["colaEntrada5y6"]);
            estadoCajaEntrada5 = filaAnterior["estadoCajaEntrada5"].ToString();
            estadoCajaEntrada6 = filaAnterior["estadoCajaEntrada6"].ToString();


            // Control comida
            colaComida1 = Convert.ToInt32(filaAnterior["colaComida1"]);
            estadoControlComida1 = filaAnterior["estadoControlComida1"].ToString();

            colaComida2 = Convert.ToInt32(filaAnterior["colaComida2"]);
            estadoControlComida2 = filaAnterior["estadoControlComida2"].ToString();

            colaComida3 = Convert.ToInt32(filaAnterior["colaComida3"]);
            estadoControlComida3 = filaAnterior["estadoControlComida3"].ToString();

            colaComida4 = Convert.ToInt32(filaAnterior["colaComida4"]);
            estadoControlComida4 = filaAnterior["estadoControlComida4"].ToString();


            // Control comida mayores
            colaComidaMayores = Convert.ToInt32(filaAnterior["colaComidaMayores"]);
            if (colaComidaMayores > 0)
            {
                colaComidaMayores--;
                estadoControlComidaMayores = "Ocupado";
            }
            else
            {
                estadoControlComidaMayores = "Libre";
            }


            // Estadísticas
            metrosPromedioNecesariosParaAparcamiento = aDecimal(filaAnterior["cantidadPromedioAutosEnColaPark"]) * 4;
            acumuladorTiempoColaParking = (reloj - aDecimal(filaAnterior["reloj"])) * (colaPark1 + colaPark2 + colaPark3 + colaPark4 + colaPark5) + aDecimal(filaAnterior["acumuladorTiempoColaParking"]);
            cantidadPromedioAutosEnColaPark = acumuladorTiempoColaParking / reloj;

            contadorGruposCajaEntrada = Convert.ToInt32(filaAnterior["contadorGruposCajaEntrada"]);
            acumuladorTiempoColaEntrada = (reloj - aDecimal(filaAnterior["reloj"])) * (colaEntrada1y2 + colaEntrada3y4 + colaEntrada5y6) + aDecimal(filaAnterior["acumuladorTiempoColaEntrada"]);
            if (contadorGruposCajaEntrada == 0)
            {
                tiempoPromedioEnColaEntrada = 0;
            }
            else
            {
                tiempoPromedioEnColaEntrada = acumuladorTiempoColaEntrada / contadorGruposCajaEntrada;
            }

            contadorPersonasEnControlComida = Convert.ToInt32(filaAnterior["contadorPersonasEnControlComida"]);
            acumuladorTiempoColaComida = (reloj - aDecimal(filaAnterior["reloj"])) * (colaComida1 + colaComida2 + colaComida3 + colaComida4 + colaComidaMayores) + aDecimal(filaAnterior["acumuladorTiempoColaComida"]);
            if (contadorPersonasEnControlComida == 0)
            {
                tiempoPromedioEnColaComida = 0;
            }
            else
            {
                tiempoPromedioEnColaComida = acumuladorTiempoColaComida / contadorPersonasEnControlComida;
            }

            tiempoEnConseguirEntrada = 300 + tiempoPromedioEnColaEntrada + 92;
            cantidadPromedioGenteEnColaEntrada = (acumuladorTiempoColaEntrada / reloj) * (decimal)4.16;
            tiempoEntradaDespuesDeEstacionar = tiempoEnConseguirEntrada + tiempoPromedioEnColaComida + 5;



            // --- Manejo de tabla y próximo evento ---

            // Eliminar fila anterior
            dt.Rows.Remove(filaAnterior);

            // Agregar la nueva fila
            dt.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);

            // Guardar la nueva fila en una variable para enviársela al próximo evento
            DataRow filaActual = dt.Rows[0];

            // Si la simulación está dentro de las que quiere visualizar, agregarla a la grilla
            if ((numeroSimulacionActual >= verDesdeSimulacion && numeroSimulacionActual < verHastaSimulacion) || numeroSimulacionActual == cantidadDeSimulaciones)
            {
                grdSimulacion.Rows.Add(evento, reloj, rndTipoDetencion, tipoDetencion, beta, tiempoDeDetencion, horaDetencion, rndLlegada, tiempoLlegada, proximaLlegada, l, tiempoDuracionDetencionLlegadas, horaReanudacionLlegadas, rndFinAP, tiempoFinAP, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5,
                rndFinAE, tiempoFinAE, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6, rndCantidadPersonas, cantidadPersonas, rndCantidadPersonasMayores,
                cantidadPersonasMayores, cantidadPersonasNoMayores, rndFinAC1, tiempoFinAC1, proximoFinAC1, rndFinAC2, tiempoFinAC2, proximoFinAC2, rndFinAC3, tiempoFinAC3, proximoFinAC3,
                rndFinAC4, tiempoFinAC4, proximoFinAC4, rndFinACM, tiempoFinACM, proximoFinACM, s, tiempoDuracionDetencionServidor, horaReanudacionServidor, ingresoBloqueado, colaBloqueoLlegadas, colaPark1, estadoCajaPark1, colaPark2, estadoCajaPark2, colaPark3, estadoCajaPark3, colaPark4, estadoCajaPark4,
                colaPark5, estadoCajaPark5, colaEntrada1y2, estadoCajaEntrada1, estadoCajaEntrada2, colaEntrada3y4, estadoCajaEntrada3, estadoCajaEntrada4, colaEntrada5y6, estadoCajaEntrada5, estadoCajaEntrada6,
                colaComida1, estadoControlComida1, colaComida2, estadoControlComida2, colaComida3, estadoControlComida3, colaComida4, estadoControlComida4, colaComidaMayores, estadoControlComidaMayores,
                metrosPromedioNecesariosParaAparcamiento, acumuladorTiempoColaParking, cantidadPromedioAutosEnColaPark, contadorGruposCajaEntrada, acumuladorTiempoColaEntrada, tiempoPromedioEnColaEntrada,
                contadorPersonasEnControlComida, acumuladorTiempoColaComida, tiempoPromedioEnColaComida, tiempoEnConseguirEntrada, cantidadPromedioGenteEnColaEntrada, tiempoEntradaDespuesDeEstacionar);
            }

            // Determinar el próximo evento
            if (cantidadDeSimulaciones >= numeroSimulacionActual)
            {
                string proxEvento = obtenerProximoEvento(horaDetencion, proximaLlegada, horaReanudacionLlegadas, proximoFinAP1, proximoFinAP2, proximoFinAP3, proximoFinAP4, proximoFinAP5, proximoFinAE1, proximoFinAE2, proximoFinAE3, proximoFinAE4, proximoFinAE5, proximoFinAE6,
    proximoFinAC1, proximoFinAC2, proximoFinAC3, proximoFinAC4, proximoFinACM, horaReanudacionServidor);

                if (proxEvento == "LL Auto")
                {
                    llegadaAuto(filaActual);
                }
                else if (proxEvento == "Fin AP")
                {
                    finAtencionParking(filaActual);
                }
                else if (proxEvento == "Fin AE")
                {
                    finAtencionEntrada(filaActual);
                }
                else if (proxEvento == "Fin AC")
                {
                    finAtencionComida(filaActual);
                }
                else if (proxEvento == "Fin ACM")
                {
                    finAtencionComidaMayores(filaActual);
                }
                else if (proxEvento == "Detención")
                {
                    detencion(filaActual);
                }
                else if (proxEvento == "Reanudación Llegadas")
                {
                    reanudacionLlegadas(filaActual);
                }
                else if (proxEvento == "Reanudación Servidor")
                {
                    reanudacionServidor(filaActual);
                }
                else
                {
                    throw new Exception("No se encontró el evento siguiente desde Detención.");
                }
            }

        }


        // Interacciones
        private void picX_Click(object sender, EventArgs e)
        {
            menu.Close();
        }

        private void picArrow_Click(object sender, EventArgs e)
        {
            menu.Show();
            Close();
        }

        private void btnInformación_Click(object sender, EventArgs e)
        {
            DataGridViewRow ultimaFila = grdSimulacion.Rows[grdSimulacion.Rows.Count - 1];
            Metricas metricas = new Metricas(this, ultimaFila);
            metricas.ShowDialog();
        }   
    }
}
