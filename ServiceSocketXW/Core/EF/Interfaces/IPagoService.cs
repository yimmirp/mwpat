namespace ServiceSocketXW.Core.EF.Interfaces
{
    public interface IPagoService
    {
        public string ConsultarTBL_PAGO(Guid IDTransaction, string TRAMA, string Terminal);
    }
}
