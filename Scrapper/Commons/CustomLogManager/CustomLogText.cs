using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Commons.CustomLogManager
{
    public class CustomLogText
    {
        const int MAXMANTENER = 40;

        string   _archivo    = string.Empty;
        string   _nombre     = string.Empty;
        string   _carpeta    = string.Empty;
        Encoding _encoding   = new UTF8Encoding(false);
        object   _lock       = new object();
        int      _size       = 1 * 1024 * 1024;
        int      _maxbackups = 100;
        int      _contador   = 0;


        public CustomLogText (string archivo, Encoding encoding, int maxSize, int maxBackups)
        {
            _archivo    = archivo;
            _nombre     = Path.GetFileName(archivo);
            _carpeta    = Path.GetDirectoryName(archivo);
            _encoding   = encoding;
            _maxbackups = maxBackups;
            _size       = maxSize;
        }


        public void loguear (string texto)
        {
            try
            {
                lock (_lock)
                {
                    if (!Directory.Exists(_carpeta))
                        Directory.CreateDirectory(_carpeta);

                    if (_contador >= MAXMANTENER)
                        mantener();

                    using (FileStream fs = File.Open(_archivo, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        byte[] bytes = formatear(texto);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                    }

                    _contador++;
                }
            }
            catch
            {
                //
            }
        }


        void mantener ()
        {
            try
            {
                FileInfo fi = new FileInfo(_archivo);

                if (fi.Length > _size)
                {
                    DateTime fecha = DateTime.Now;

                    string destino = Path.Combine(
                        _carpeta, 
                        string.Format("{0}_Backup_{1:0000}_{2:00}_{3:00}_{4:00}_{5:00}_{6:00}_{7}.txt", _nombre,
                            fecha.Year, fecha.Month, fecha.Day, fecha.Hour, fecha.Minute, fecha.Second, Guid.NewGuid().ToString("N"))
                    );

                    File.Move (_archivo, destino);

                    string[] archivos = Directory.GetFiles(_carpeta);

                    if (archivos.Length > _maxbackups)
                    {
                        archivos = archivos.OrderBy(f => f).ToArray();

                        for (int k = 0; k < archivos.Length - _maxbackups; k++)
                            File.Delete(archivos[k]);
                    }
                }
            }
            catch 
            {
                //
            }

            _contador = 0;
        }

        byte[] formatear (string texto)
        {
            return _encoding.GetBytes(
                string.Format(
                    "[{0:yyyy/MM/dd HH:mm:ss.fff}] {1}\r\n",
                    DateTime.Now,
                    texto
                )
            );
        }
    }
}
