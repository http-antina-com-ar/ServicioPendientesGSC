using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicioPendientes
{
    internal class Pendientes
    {
        static int filesPerBatch = SettingsPendings.Default.filesPerBatch; //cantidad maxima de archivos a cargar por vez
        static string pendingsFolder = SettingsPendings.Default.pendingsFolder;
        static string loadedFolder = SettingsPendings.Default.loadedFolder;
        static Logger logger = new Logger();

        public Pendientes()
        {
            Inicio();
        }

        private async void Inicio()
        {
            while (true)
            {
                try
                {
                    await PendingsQueueing();
                    PendingsScheduling();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        static async Task PendingsQueueing()
        {
            /* CARGA ARCHIVOS DE PENDIENTES Y ENCOLADO */
            //Traigo archivos del directorio

            try
            {
                string[] files = Directory.GetFiles(pendingsFolder); // Obtener archivos

                if (files.Count() > filesPerBatch)
                {
                    /* Loguear que hay más archivos del maximo a subir */
                    logger.Log($"[CARGA DE ARCHIVOS] Hay {files.Count()} archivos en la carpeta de pendientes, sólo se subirán {filesPerBatch} de ellos", Logger.LogLevel.WARNING);
                    await LoadFiles(files);
                    
                } 
                else if (files.Count() < 1)
                {
                    await PendingsCleaning();
                }
                else
                {
                    await LoadFiles(files);
                }
            }
            catch (Exception ex)
            {
                /* Loguear error al acceder al archivo o copiarlo */
                string msg = $"ERROR EN EL ACCESO A ARCHIVOS: {ex.Message}";
                logger.Log(Logger.LogType.BOTH, msg, Logger.LogLevel.FATAL);
                //MANDAR MAIL
                throw;
             }
        }

        static async Task LoadFiles(string[] files) 
        {

                int filesCounter = 0;
                List<Task> filesTask = new List<Task>();

                foreach (string file in files)
                {
                    filesTask.Add(LoadFile(file));

                    filesCounter++;
                    if (filesCounter == filesPerBatch) break; //sale si llegó al tope
                }
                await Task.WhenAll(filesTask);
        }

        static async Task LoadFile(string file)
        {

            await new Task(() =>
            {

                logger.Log(Logger.LogType.LOG_FILE, $"CARGA ARCHIVO [{file}]", Logger.LogLevel.INFO);
                
                DAO db = new DAO();
                db.ExecuteQueueing(file);

                string destinationFile = file.Substring(pendingsFolder.Length);
                destinationFile = loadedFolder + destinationFile;

                try
                {
                    /* Mover archivo a carpeta de subidos */
                    File.Move(file, destinationFile);
                }
                catch (Exception ex)
                {
                    /* Loguear error al acceder al archivo o copiarlo */
                    string msg = $"ERROR AL MOVER ARCHIVO A {destinationFile}: {ex.Message}";
                    logger.Log(Logger.LogType.BOTH, msg, Logger.LogLevel.FATAL);
                    //MANDAR MAIL
                    throw ex;
                }
            });

        }

        static void PendingsScheduling()
        {
            /* SE DESENCOLAN PENDIENTES Y SE CARGAN A GSC PARA AGENDAR */
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            DAO db = new DAO();
            db.ExecuteScheduling();
        }

        static async Task PendingsCleaning()
        {
            await new Task(() =>
            {
                /* SE BORRAN TAREAS ENCOLADAS QUE YA FUERON AGENDADAS */
                DAO db = new DAO();
                db.ExecuteCleanning();
            }
            );
        }
    }
}
