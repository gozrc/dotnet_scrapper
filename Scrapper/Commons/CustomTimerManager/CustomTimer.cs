using System;
using System.Timers;

namespace Commons.CustomTimerManager
{
    public delegate void CustomTimerFunction ();
    public delegate void ServicioIniciado    ();
    public delegate void ServicioDetenido    ();
    public delegate void EventoIniciado      ();
    public delegate void EventoTerminado     ();

    public class CustomTimer
    {
        enum Estado { Detenido, Esperando, Ejecutando }

        Timer               _timer      = null;
        CustomTimerFunction _function   = null;
        long                _msLatency  = 0;
        object              _lock       = new object();
        Estado              _estado     = Estado.Detenido;
        
        public event ServicioIniciado onServicioIniciado;
        public event ServicioDetenido onServicioDetenido;
        public event EventoIniciado   onEventoIniciado;
        public event EventoTerminado  onEventoTerminado;


        public CustomTimer (CustomTimerFunction function, long msLatency)
        {
            _function  = function;
            _msLatency = msLatency;

            _timer = new Timer();

            _timer.Elapsed += timerElapsed;
            _timer.Interval = msLatency;

            _estado = Estado.Detenido;
        }


        public bool iniciar (ref string error)
        {
            if (servicioDetenido)
            {
                setearEsperando();

                if (null != onServicioIniciado)
                    onServicioIniciado();
            }
            else
            {
                error = "El servicio ya se encuentra iniciado";
            }

            return (0 == error.Length);
        }

        public void detener ()
        {
            while (servicioEjecutando)
                System.Threading.Thread.Sleep(10);

            setearDetenido();

            if (null != onServicioDetenido)
                onServicioDetenido();
        }



        void timerElapsed (object sender, ElapsedEventArgs e)
        {
            setearEjecutando();

            if (null != onEventoIniciado)
                onEventoIniciado();

            _function();

            setearEsperando();

            if (null != onEventoTerminado)
                onEventoTerminado();
        }


        bool servicioDetenido
        {
            get
            {
                lock (_lock)
                    return (_estado == Estado.Detenido);
            }
        }

        bool servicioEsperando
        {
            get
            {
                lock (_lock)
                    return (_estado == Estado.Esperando);
            }
        }

        bool servicioEjecutando
        {
            get
            {
                lock (_lock)
                    return (_estado == Estado.Ejecutando);
            }
        }


        void setearDetenido ()
        {
            lock (_lock)
            {
                _estado = Estado.Detenido;
                _timer.Enabled = false;
            }
        }

        void setearEsperando ()
        {
            lock (_lock)
            {
                _estado = Estado.Esperando;
                _timer.Enabled = true;
            }
        }

        void setearEjecutando ()
        {
            lock (_lock)
            {
                _estado = Estado.Ejecutando;
                _timer.Enabled = false;
            }
        }
    }
}
