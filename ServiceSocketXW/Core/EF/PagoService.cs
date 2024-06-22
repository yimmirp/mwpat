using ServiceSocketXW.Core.EF.Interfaces;
using ServiceSocketXW.Data;

namespace ServiceSocketXW.Core.EF
{
    public class PagoService :IPagoService
    {
        private readonly DbPagosContext _dbPagosContext;
        private readonly ILogger<PagoService> _logger;

        public PagoService(DbPagosContext dbPagosContext, ILogger<PagoService> logger)
        {
            _dbPagosContext = dbPagosContext;
            _logger = logger;
        }

        public string ConsultarTBL_PAGO(Guid IDTransaction, string T, string Terminal)
        {

            var TIPO_PAGO = T.Substring(50, 6);
            var FECHA_DESDE = T.Substring(56, 8);
            var FECHA_HASTA = T.Substring(64, 8);

            try
            {
                string TramaSended = "";

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


                if (result == null)
                    throw new Exception($"No se encontro el codigo de pago {TIPO_PAGO}.");

                string todayDate_T = DateTime.Now.ToString("ddMMMyy", System.Globalization.CultureInfo.InvariantCulture).ToUpper();
                string date_T = DateTime.Now.ToString("yyMM").ToUpper();
                TramaSended = $"000090000000{Terminal.PadRight(10, ' ').Substring(0, 10)}00100PTXMWT000000{todayDate_T}{date_T}{ValorFormatoTrama(result.ValorTotal)}{TransaccionesFormatoTrama(result.Transacciones)}0";

                return TramaSended;

            }
            catch (Exception ex)
            {
                string todayDate_T = DateTime.Now.ToString("ddMMMyy", System.Globalization.CultureInfo.InvariantCulture).ToUpper();
                string date_T = DateTime.Now.ToString("yyMM").ToUpper();
                string TramaSended = $"000090000000{Terminal.PadRight(10, ' ').Substring(0, 10)}00100PTXMWT000000{todayDate_T}{date_T}000000000000000000000001";
                _logger.LogInformation($"[SERVER][TransactionID: {IDTransaction}] - Error: {ex.Message}.");
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
