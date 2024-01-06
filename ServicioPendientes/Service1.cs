using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ServicioPendientes
{
    public partial class PendientesGSC : ServiceBase
    {
        public Logger logger;
        public Mailer mailer = new Mailer();

        public PendientesGSC()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.Log(Logger.LogType.WIN_EVENT,
                "Inicio de Servicio de Pendientes GSC - logs en: [" + SettingsPendings.Default.logPath + "]",
                Logger.LogLevel.INFO);

            logger.Log("Inicio de Servicio de Pendientes GSC ", Logger.LogLevel.INFO);
            mailer.SendMail("SE SUBE SERVICIO DE PENDIENTES GSC", "Se levanta servicio de pendientes de GSC - logs en: [" + SettingsPendings.Default.logPath + "]");

            Pendientes pendientes = new Pendientes();

         }

        protected override void OnStop()
        {
            logger.Log(Logger.LogType.WIN_EVENT,
                "Finaliza Servicio de Pendientes GSC - logs en: [" + SettingsPendings.Default.logPath + "]",
                Logger.LogLevel.INFO);
            
            logger.Log("Finaliza Servicio de Pendientes GSC", Logger.LogLevel.INFO);
            mailer.SendMail("BAJA SERVICIO DE PENDIENTES GSC", "Se bajó servicio de pendientes de GSC - logs en: [" + SettingsPendings.Default.logPath + "]");
        }

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string Porcentaje = (SystemInformation.PowerStatus.BatteryLifePercent * 100).ToString() + "%";
            logger.Log("Porcentaje actual de la bateria: " + Porcentaje, Logger.LogLevel.INFO);
        }
    }
}
