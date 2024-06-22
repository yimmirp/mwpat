using SiteManage.Data;
using SiteManage.Services.Handler;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SiteManage.Services.EF
{
    public class PagosService : IPagoService
    {
        private readonly DbPagosContext _dbPagosContext;
        private readonly ILogger<PagosService> _logger;

        public PagosService(DbPagosContext dbPagosContext, ILogger<PagosService> logger)
        {

            _dbPagosContext = dbPagosContext;
            _logger = logger;
        }

        public string ConsultarTBL_PAGO(Guid IDTransaction, string T, string Terminal)
        {

            var TIPO_PAGO = T.Substring(50, 6);
            var FECHA_DESDE = T.Substring(56, 8);
            var FECHA_HASTA = T.Substring(64, 8);
            string CODIGO_RESPUESTA = "0";

            try
            {
                string TramaSended = "";

                // Comprobar si el codigo de pago existe
                bool ExistePago = _dbPagosContext.TblPagos.Any(p => p.TipoPago == Convert.ToInt32(TIPO_PAGO));

                if (!ExistePago)
                {
                    CODIGO_RESPUESTA = "1";
                    throw new Exception($"EL CODIGO '{TIPO_PAGO}' NO EXISTE");
                }

                // Tu lógica de servicio aquí utilizando dbContext
                Pago? result = _dbPagosContext.TblPagos
                .Where(p => p.TipoPago == Convert.ToInt32(TIPO_PAGO) &&
                           p.FechaIngreso >= new DateTime(int.Parse(FECHA_DESDE.Substring(4, 4)), int.Parse(FECHA_DESDE.Substring(2, 2)), int.Parse(FECHA_DESDE.Substring(0, 2)), 0, 0, 0, 000) &&
                           p.FechaIngreso <= new DateTime(int.Parse(FECHA_HASTA.Substring(4, 4)), int.Parse(FECHA_HASTA.Substring(2, 2)), int.Parse(FECHA_HASTA.Substring(0, 2)), 23, 59, 59, 999))
                .GroupBy(p => 1)
                .Select(g => new Pago
                {
                    Transacciones = g.Count().ToString(),
                    ValorTotal = g.Sum(p => p.Efectivo).ToString(),
                }).FirstOrDefault();


                if (result == null) {
                    CODIGO_RESPUESTA = "2";
                    throw new Exception($"NO SE ENCONTRARON PAGOS PARA EL CODIGO '{TIPO_PAGO}' DESDE {FECHA_DESDE} HASTA {FECHA_HASTA}");
                }

                string todayDate_T = DateTime.Now.ToString("ddMMMyy", System.Globalization.CultureInfo.InvariantCulture).ToUpper();
                string date_T = DateTime.Now.ToString("yyMM").ToUpper();
                TramaSended = $"000090000000{Terminal.PadRight(10, ' ').Substring(0, 10)}00100PTXMWT000000{todayDate_T}{date_T}{ValorFormatoTrama(result.ValorTotal)}{TransaccionesFormatoTrama(result.Transacciones)}0";

                return TramaSended;

            }
            catch (Exception ex)
            {
                string todayDate_T = DateTime.Now.ToString("ddMMMyy", System.Globalization.CultureInfo.InvariantCulture).ToUpper();
                string date_T = DateTime.Now.ToString("yyMM").ToUpper();
                string TramaSended = $"000090000000{Terminal.PadRight(10, ' ').Substring(0, 10)}00100PTXMWT000000{todayDate_T}{date_T}00000000000000000000000{CODIGO_RESPUESTA}";
                _logger.LogInformation($"[SERVER][TransactionID: {IDTransaction}] - ERROR: {ex.Message}");
                return TramaSended;

            }

        }

        static string ValorFormatoTrama(string numero)
        {

            // Lo convierto a decimal y si tiene mas de dos decimales de los quita y lo vuelve string
            string numeroRecido = Convert.ToDecimal(numero).ToString("F2");

            // Eliminar el punto decimal y asegurarse de que la cadena es numérica
            string numerosSinDecimal = new string(numero.Where(char.IsDigit).ToArray());

            // Rellenar con ceros a la izquierda para obtener un total de 15 dígitos
            string numeroRellenado = numerosSinDecimal.PadLeft(15, '0');

            return numeroRellenado;
        }

        public string TransaccionesFormatoTrama(string transacciones)
        {

            // Rellenar con ceros a la izquierda para obtener un total de 15 dígitos
            return transacciones.PadLeft(8, '0');

        }


    }

    public class Pago
    {
        public string Transacciones { get; set; }
        public string ValorTotal { get; set; }
    }
}
